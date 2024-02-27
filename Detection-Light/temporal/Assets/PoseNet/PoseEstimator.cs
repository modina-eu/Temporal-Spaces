using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Unity.Barracuda;
using RenderHeads.Media.AVProLiveCamera;
public class PoseEstimator : MonoBehaviour
{
    public enum ModelType
    {
        MobileNet
    }
    public enum EstimationType
    {
        MultiPose,
        SinglePose
    }
    public Material material;
    public AVProLiveCamera script2;
    [Tooltip("The ComputeShader that will perform the model-specific preprocessing")]
    public ComputeShader posenetShader;
    [Tooltip("Use GPU for preprocessing")]
    public bool useGPU = true;
    [Tooltip("The dimensions of the image being fed to the model")]
    public Vector2Int imageDims = new Vector2Int(256, 256);
    [Tooltip("The MobileNet model asset file to use when performing inference")]
    public NNModel mobileNetModelAsset;
    [Tooltip("The backend to use when performing inference")]
    public WorkerFactory.Type workerType = WorkerFactory.Type.Auto;
    [Tooltip("The type of pose estimation to be performed")]
    public EstimationType estimationType = EstimationType.SinglePose;
    [Tooltip("The maximum number of posees to estimate")]
    [Range(1, 20)]
    public int maxPoses = 20;
    [Tooltip("The score threshold for multipose estimation")]
    [Range(0, 1.0f)]
    public float scoreThreshold = 0.25f;
    [Tooltip("Non-maximum suppression part distance")]
    public int nmsRadius = 100;
    public float smoothingTime = 0.2f;
    public float score;
    [Tooltip("The minimum confidence level required to display the key point")]
    // The texture used to create input tensor
    private RenderTexture rTex;
    // The preprocessing function for the current model type
    private System.Action<float[]> preProcessFunction;
    // Stores the input data for the model
    private Tensor input;

    private struct Engine
    {
        public WorkerFactory.Type workerType;
        public IWorker worker;
        public ModelType modelType;
        public Engine(WorkerFactory.Type workerType, Model model, ModelType modelType)
        {
            this.workerType = workerType;
            worker = WorkerFactory.CreateWorker(workerType, model);
            this.modelType = modelType;
        }
    }
    // The interface used to execute the neural network
    private Engine engine;
    // The name for the heatmap layer in the model asset
    private string heatmapLayer;
    // The name for the offsets layer in the model asset
    private string offsetsLayer;
    // The name for the forwards displacement layer in the model asset
    private string displacementFWDLayer;
    // The name for the backwards displacement layer in the model asset
    private string displacementBWDLayer;
    // The name for the Sigmoid layer that returns the heatmap predictions
    private string predictionLayer = "heatmap_predictions";
    // Stores the current estimated 2D keypoint locations in videoTexture
    private Utils.Keypoint[][] poses;
    // Array of pose skeletons
    private PoseSkeleton[] skeletons;
   // public Vector3[] posePositions;
    private PoseSkeleton[] skeletons2;
    //public Vector3[] posePositions2;
    private Vector3 smoothDampVelocity;
    private Vector3 smoothDampVelocity2;
    public int closestSkeletonIndex = 0;
    public int closestSkeletonIndex2 = 0;
    public int closestSkeletonIndex3 = 0;
    public int closestSkeletonIndex2Candidate = -1;


    public float pbr;
    public float pr;
    public float pp;
    public float ps2;
    public float ps3;

    public Vector4 posc7;
    public float pcr;
    public Vector3 previousHipPosition;
    public Vector3 previousHipPosition2;
    private Vector2 a = new Vector2(0.5f, 0.5f);
    private Vector4[] posePositionsArray = new Vector4[17];
    private Vector4[] posePositionsArray2 = new Vector4[17];
    private Vector4[] posePositionsArray3 = new Vector4[17];
    public Vector2[] posZ1 = new Vector2[13];
    public Vector2[] posZ2 = new Vector2[13];
    public Vector2[] posZ3 = new Vector2[13];
    public float[] possZ1 = new float[12];
    public float[] possZ2 = new float[12];
    public float[] possZ3 = new float[12];
    public float pa;
    public float pb;
    public float pc;
    public float mpa;
    public float mpb;
    public float mpc;
    public int framesConditionTrueA = 0; // Counter to keep track of frames the condition is true for mpa
    public int framesConditionTrueB = 0; // Counter to keep track of frames the condition is true for mpb
    public int framesConditionTrueC = 0; // Counter to keep track of frames the condition is true for mpc
    private bool isImageModified;
    //public Vector3[] pos;
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="mirrorScreen"></param>

    private void InitializeBarracuda()
    {
        Model m_RunTimeModel;
     
            preProcessFunction = Utils.PreprocessMobileNet;
            m_RunTimeModel = ModelLoader.Load(mobileNetModelAsset);
            displacementFWDLayer = m_RunTimeModel.outputs[2];
            displacementBWDLayer = m_RunTimeModel.outputs[3];
        heatmapLayer = m_RunTimeModel.outputs[0];
        offsetsLayer = m_RunTimeModel.outputs[1];
        ModelBuilder modelBuilder = new ModelBuilder(m_RunTimeModel);
        modelBuilder.Sigmoid(predictionLayer, heatmapLayer);
        workerType = WorkerFactory.ValidateType(workerType);
        engine = new Engine(workerType, modelBuilder.model, ModelType.MobileNet);
    }
    private void InitializeSkeletons()
    {
        skeletons = new PoseSkeleton[maxPoses];
        for (int i = 0; i < maxPoses; i++) skeletons[i] = new PoseSkeleton();
    }
    void Start()
    {     
        rTex = new RenderTexture(imageDims.x, imageDims.y, 24, RenderTextureFormat.ARGBHalf);
        InitializeBarracuda();
        InitializeSkeletons();
        material.SetFloat("_resx", imageDims.x);
        material.SetFloat("_resy", imageDims.y);     
    }
    /// <param name="image"></param>
    /// <param name="functionName"></param>
    /// <returns></returns>
    private void ProcessImageGPU(RenderTexture image, string functionName)
    {
        int numthreads = 8;
        int kernelHandle = posenetShader.FindKernel(functionName);
        // Define a temporary HDR RenderTexture
        RenderTexture result = RenderTexture.GetTemporary(image.width, image.height, 24, RenderTextureFormat.ARGBHalf);
        result.enableRandomWrite = true;
        result.Create();
        posenetShader.SetTexture(kernelHandle, "Result", result);
        posenetShader.SetTexture(kernelHandle, "InputImage", image);
        posenetShader.Dispatch(kernelHandle, result.width / numthreads, result.height / numthreads, 1);
        Graphics.Blit(result, image);
        RenderTexture.ReleaseTemporary(result);
    }

    /// <param name="image"></param>
    private void ProcessImage(RenderTexture image)
    {
        if (useGPU)
        {
            // Apply preprocessing steps
            ProcessImageGPU(image, preProcessFunction.Method.Name);
            // Create a Tensor of shape [1, image.height, image.width, 3]
            input = new Tensor(image, channels: 3);
        }
        else
        {
            // Create a Tensor of shape [1, image.height, image.width, 3]
            input = new Tensor(image, channels: 3);
            // Download the tensor data to an array
            float[] tensor_array = input.data.Download(input.shape);
            // Apply preprocessing steps
            preProcessFunction(tensor_array);
            // Update input tensor with new color data
            input = new Tensor(input.shape.batch,
                               input.shape.height,
                               input.shape.width,
                               input.shape.channels,
                               tensor_array);
        }
    }

    /// <param name="engine"></param>
    private void ProcessOutput(IWorker engine)
    {
        // Get the model output
        Tensor heatmaps = engine.PeekOutput(predictionLayer);
        Tensor offsets = engine.PeekOutput(offsetsLayer);
        Tensor displacementFWD = engine.PeekOutput(displacementFWDLayer);
        Tensor displacementBWD = engine.PeekOutput(displacementBWDLayer);
        // Calculate the stride used to scale down the inputImage
        int stride = (imageDims.y - 1) / (heatmaps.shape.height - 1);
        stride -= (stride % 8);
        if (estimationType == EstimationType.SinglePose)
        {
            // Initialize the array of Keypoint arrays
            poses = new Utils.Keypoint[1][];
            // Determine the key point locations
            poses[0] = Utils.DecodeSinglePose(heatmaps, offsets, stride);
        }
        else
        {
            // Determine the key point locations
            poses = Utils.DecodeMultiplePoses(
                heatmaps, offsets,
                displacementFWD, displacementBWD,
                stride: stride, maxPoseDetections: maxPoses,
                scoreThreshold: scoreThreshold,
                nmsRadius: nmsRadius);
        }
        // Release the resources allocated for the output Tensors
        heatmaps.Dispose();
        offsets.Dispose();
        displacementFWD.Dispose();
        displacementBWD.Dispose();
    }
    void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.B))
        {
            if (isImageModified)
            {
                imageDims = new Vector2Int(960, 540);
                isImageModified = false;
            }
            else
            {
                imageDims = new Vector2Int(480, 270);
                isImageModified = true;
            }
        }   */
        float[] posePositionsscore = new float[17];
        float[] posePositionsscore2 = new float[17];
        float[] posePositionsscore3 = new float[17];
        Graphics.Blit(script2.OutputTexture, rTex);
        ProcessImage(rTex);
        engine.worker.Execute(input);
        input.Dispose();
        ProcessOutput(engine.worker);
        float closestDistance = 2000.0f;
        float closestDistance2 = 3000.0f;

        for (int i = 0; i < poses.Length; i++)
        {

            skeletons[i].UpdateKeyPointPositions(poses[i], imageDims);
            //Vector3[] keyPoints = skeletons[i].keypoints;

            float psr = 5 * Vector2.Distance(new Vector2((skeletons[i].keypoints[11].x + skeletons[i].keypoints[12].x) * 0.5f, (skeletons[i].keypoints[11].y + skeletons[i].keypoints[12].y) * 0.5f)
               , new Vector2((posePositionsArray[5].x + posePositionsArray[6].x), (posePositionsArray[5].y + posePositionsArray[6].y)) * 0.5f);

            float distanceToHip = Vector3.Distance(new Vector3(skeletons[i].keypoints[0].x,
                skeletons[i].keypoints[0].y, psr), previousHipPosition);

            float distanceToHip2 = Vector3.Distance(new Vector3(skeletons[i].keypoints[0].x,
               skeletons[i].keypoints[0].y, psr), previousHipPosition2);

            if (distanceToHip < closestDistance)
            {
                closestDistance = distanceToHip;
                closestSkeletonIndex = i;
            }
            if (distanceToHip2 < closestDistance2)
            {
                closestDistance2 = distanceToHip2;
                closestSkeletonIndex2Candidate = i;
            }


        }
        if (closestSkeletonIndex2Candidate != closestSkeletonIndex)
        {
            closestSkeletonIndex2 = closestSkeletonIndex2Candidate;
        }
        if (closestSkeletonIndex == 0)
        {
            Vector3[] keyPoints0 = skeletons[0].keypoints;
            if (closestSkeletonIndex2 == 1)
            {
                Vector3[] keyPoints1 = skeletons[1].keypoints;
                Vector3[] keyPoints2 = skeletons[2].keypoints;
                for (int j = 0; j < 17; j++)
                {
                    int index = j;
                    posePositionsArray2[index] = new Vector4(keyPoints1[j].x, keyPoints1[j].y, 0, 1);
                    posePositionsscore2[index] = keyPoints1[j].z;
                }
                for (int j = 0; j < 17; j++)
                {
                    int index = j;
                    posePositionsArray3[index] = new Vector4(keyPoints2[j].x, keyPoints2[j].y, 0, 1);
                    posePositionsscore3[index] = keyPoints2[j].z;
                }
            }
            if (closestSkeletonIndex2 == 2)
            {
                Vector3[] keyPoints1 = skeletons[2].keypoints;
                Vector3[] keyPoints2 = skeletons[1].keypoints;
                for (int j = 0; j < 17; j++)
                {
                    int index = j;
                    posePositionsArray2[index] = new Vector4(keyPoints1[j].x, keyPoints1[j].y, 0, 1);
                    posePositionsscore2[index] = keyPoints1[j].z;
                }
                for (int j = 0; j < 17; j++)
                {
                    int index = j;
                    posePositionsArray3[index] = new Vector4(keyPoints2[j].x, keyPoints2[j].y, 0, 1);
                    posePositionsscore3[index] = keyPoints2[j].z;
                }
            }
            for (int j = 0; j < 17; j++)
            {
                int index = j;
                posePositionsArray[index] = new Vector4(keyPoints0[j].x, keyPoints0[j].y, 0, 1);
                posePositionsscore[index] = keyPoints0[j].z;
            }
        }
        if (closestSkeletonIndex == 1)
        {
            Vector3[] keyPoints0 = skeletons[1].keypoints;
            if (closestSkeletonIndex2 == 0)
            {
                Vector3[] keyPoints1 = skeletons[0].keypoints;
                Vector3[] keyPoints2 = skeletons[2].keypoints;
                for (int j = 0; j < 17; j++)
                {
                    int index = j;
                    posePositionsArray2[index] = new Vector4(keyPoints1[j].x, keyPoints1[j].y, 0, 1);
                    posePositionsscore2[index] = keyPoints1[j].z;
                }
                for (int j = 0; j < 17; j++)
                {
                    int index = j;
                    posePositionsArray3[index] = new Vector4(keyPoints2[j].x, keyPoints2[j].y, 0, 1);
                    posePositionsscore3[index] = keyPoints2[j].z;
                }
            }
            if (closestSkeletonIndex2 == 2)
            {
                Vector3[] keyPoints1 = skeletons[2].keypoints;
                Vector3[] keyPoints2 = skeletons[0].keypoints;
                for (int j = 0; j < 17; j++)
                {
                    int index = j;
                    posePositionsArray2[index] = new Vector4(keyPoints1[j].x, keyPoints1[j].y, 0, 1);
                    posePositionsscore2[index] = keyPoints1[j].z;
                }
                for (int j = 0; j < 17; j++)
                {
                    int index = j;
                    posePositionsArray3[index] = new Vector4(keyPoints2[j].x, keyPoints2[j].y, 0, 1);
                    posePositionsscore3[index] = keyPoints2[j].z;
                }
            }
            for (int j = 0; j < 17; j++)
            {
                int index = j;
                posePositionsArray[index] = new Vector4(keyPoints0[j].x, keyPoints0[j].y, 0, 1);
                posePositionsscore[index] = keyPoints0[j].z;
            }
        }
        if (closestSkeletonIndex == 2)
        {
            Vector3[] keyPoints0 = skeletons[2].keypoints;
            if (closestSkeletonIndex2 == 1)
            {
                Vector3[] keyPoints1 = skeletons[1].keypoints;
                Vector3[] keyPoints2 = skeletons[0].keypoints;
                for (int j = 0; j < 17; j++)
                {
                    int index = j;
                    posePositionsArray2[index] = new Vector4(keyPoints1[j].x, keyPoints1[j].y, 0, 1);
                    posePositionsscore2[index] = keyPoints1[j].z;
                }
                for (int j = 0; j < 17; j++)
                {
                    int index = j;
                    posePositionsArray3[index] = new Vector4(keyPoints2[j].x, keyPoints2[j].y, 0, 1);
                    posePositionsscore3[index] = keyPoints2[j].z;
                }
            }
            if (closestSkeletonIndex2 == 0)
            {
                Vector3[] keyPoints1 = skeletons[0].keypoints;
                Vector3[] keyPoints2 = skeletons[1].keypoints;
                for (int j = 0; j < 17; j++)
                {
                    int index = j;
                    posePositionsArray2[index] = new Vector4(keyPoints1[j].x, keyPoints1[j].y, 0, 1);
                    posePositionsscore2[index] = keyPoints1[j].z;
                }
                for (int j = 0; j < 17; j++)
                {
                    int index = j;
                    posePositionsArray3[index] = new Vector4(keyPoints2[j].x, keyPoints2[j].y, 0, 1);
                    posePositionsscore3[index] = keyPoints2[j].z;
                }
            }
            for (int j = 0; j < 17; j++)
            {
                int index = j;
                posePositionsArray[index] = new Vector4(keyPoints0[j].x, keyPoints0[j].y, 0, 1);
                posePositionsscore[index] = keyPoints0[j].z;
            }
        }
        Vector2 p72 = new Vector2((posePositionsArray[11].x + posePositionsArray[12].x) * 0.5f, (posePositionsArray[11].y + posePositionsArray[12].y) * 0.5f);
        pr = 5 * Vector2.Distance(p72, new Vector2((posePositionsArray[5].x + posePositionsArray[6].x), (posePositionsArray[5].y + posePositionsArray[6].y)) * 0.5f);
        previousHipPosition = Vector3.SmoothDamp(previousHipPosition, new Vector3(posePositionsArray[0].x, posePositionsArray[0].y, pr), ref smoothDampVelocity, smoothingTime);
        material.SetVectorArray("_pos", posePositionsArray);
        pp = (posePositionsArray[15].y + posePositionsArray[16].y) / 2;

        Vector2 p7b2 = new Vector2((posePositionsArray2[11].x + posePositionsArray2[12].x) * 0.5f, (posePositionsArray2[11].y + posePositionsArray2[12].y) * 0.5f);
        pbr = 5 * Vector2.Distance(p7b2, new Vector2((posePositionsArray2[5].x + posePositionsArray2[6].x), (posePositionsArray2[5].y + posePositionsArray2[6].y)) * 0.5f);

        previousHipPosition2 = Vector3.SmoothDamp(previousHipPosition2, new Vector3(posePositionsArray2[0].x, posePositionsArray2[0].y, pbr), ref smoothDampVelocity2, smoothingTime);

        //"nose", "leftShoulder", "rightShoulder", "leftElbow", "rightElbow", "leftWrist", "rightWrist", "leftHip", "rightHip", "leftKnee", "rightKnee", "leftAnkle", "rightAnkle"

        float pfa = Mathf.Max(posePositionsscore[11], posePositionsscore[0]);
        if (pfa == pa)
        {
            framesConditionTrueA++;
            if (framesConditionTrueA >= 10)
            {
                mpa = 0;
            }
        }
        else
        {
            mpa = 1;
            framesConditionTrueA = 0;
        }
        float pfb = Mathf.Max(posePositionsscore2[11], posePositionsscore2[0]);
        if (pfb == pb)
        {
            framesConditionTrueB++;
            if (framesConditionTrueB >= 10)
            {
                mpb = 0;
            }
        }
        else
        {
            mpb = 1;
            framesConditionTrueB = 0;
        }
        float pfc = Mathf.Max(posePositionsscore3[11], posePositionsscore3[0]);
        if (pfc == pc)
        {
            framesConditionTrueC++;
            if (framesConditionTrueC >= 10)
            {
                mpc = 0;
            }
        }
        else
        {
            mpc = 1;
            framesConditionTrueC = 0;
        }

        if (pfa * mpa > score)
        {
            
            posZ1[0] = ((new Vector2(posePositionsArray[0].x, posePositionsArray[0].y) - p72) / pr + a);
            posZ1[1] = ((new Vector2(posePositionsArray[5].x, posePositionsArray[5].y) - p72) / pr + a);
            posZ1[2] = ((new Vector2(posePositionsArray[6].x, posePositionsArray[6].y) - p72) / pr + a);
            posZ1[3] = ((new Vector2(posePositionsArray[7].x, posePositionsArray[7].y) - p72) / pr + a);
            posZ1[4] = ((new Vector2(posePositionsArray[8].x, posePositionsArray[8].y) - p72) / pr + a);
            posZ1[5] = ((new Vector2(posePositionsArray[9].x, posePositionsArray[9].y) - p72) / pr + a);
            posZ1[6] = ((new Vector2(posePositionsArray[10].x, posePositionsArray[10].y) - p72) / pr + a);
            posZ1[7] = new Vector2(p72.x, p72.y);
            posZ1[8] = ((new Vector2(posePositionsArray[13].x, posePositionsArray[13].y) - p72) / pr + a);
            posZ1[9] = ((new Vector2(posePositionsArray[14].x, posePositionsArray[14].y) - p72) / pr + a);
            posZ1[10] = ((new Vector2(posePositionsArray[15].x, posePositionsArray[15].y) - p72) / pr + a);
            posZ1[11] = ((new Vector2(posePositionsArray[16].x, posePositionsArray[16].y) - p72) / pr + a);
            possZ1[0] = posePositionsscore[0];
            possZ1[1] = posePositionsscore[5];
            possZ1[2] = posePositionsscore[6];
            possZ1[3] = posePositionsscore[7];
            possZ1[4] = posePositionsscore[8];
            possZ1[5] = posePositionsscore[9];
            possZ1[6] = posePositionsscore[10];
            possZ1[7] = (posePositionsscore[11] + posePositionsscore[12]) * 0.5f;
            possZ1[8] = posePositionsscore[13];
            possZ1[9] = posePositionsscore[14];
            possZ1[10] = posePositionsscore[15];
            possZ1[11] = posePositionsscore[16];
        }
        else
        {
            possZ1[0] = 0;
            possZ1[1] = 0;
            possZ1[2] = 0;
            possZ1[3] = 0;
            possZ1[4] = 0;
            possZ1[5] = 0;
            possZ1[6] = 0;
            possZ1[7] = 0;
            possZ1[8] = 0;
            possZ1[9] = 0;
            possZ1[10] = 0;
            possZ1[11] = 0;
            pp = 10;
        }
        if (pfb * mpb > score)
        {           
            material.SetVectorArray("_pos2", posePositionsArray2);
            posZ2[0] = ((new Vector2(posePositionsArray2[0].x, posePositionsArray2[0].y) - p7b2) / pbr + a);
            posZ2[1] = ((new Vector2(posePositionsArray2[5].x, posePositionsArray2[5].y) - p7b2) / pbr + a);
            posZ2[2] = ((new Vector2(posePositionsArray2[6].x, posePositionsArray2[6].y) - p7b2) / pbr + a);
            posZ2[3] = ((new Vector2(posePositionsArray2[7].x, posePositionsArray2[7].y) - p7b2) / pbr + a);
            posZ2[4] = ((new Vector2(posePositionsArray2[8].x, posePositionsArray2[8].y) - p7b2) / pbr + a);
            posZ2[5] = ((new Vector2(posePositionsArray2[9].x, posePositionsArray2[9].y) - p7b2) / pbr + a);
            posZ2[6] = ((new Vector2(posePositionsArray2[10].x, posePositionsArray2[10].y) - p7b2) / pbr + a);
            posZ2[7] = new Vector2(p7b2.x, p7b2.y);
            posZ2[8] = ((new Vector2(posePositionsArray2[13].x, posePositionsArray2[13].y) - p7b2) / pbr + a);
            posZ2[9] = ((new Vector2(posePositionsArray2[14].x, posePositionsArray2[14].y) - p7b2) / pbr + a);
            posZ2[10] = ((new Vector2(posePositionsArray2[15].x, posePositionsArray2[15].y) - p7b2) / pbr + a);
            posZ2[11] = ((new Vector2(posePositionsArray2[16].x, posePositionsArray2[16].y) - p7b2) / pbr + a);
            possZ2[0] = posePositionsscore2[0];
            possZ2[1] = posePositionsscore2[5];
            possZ2[2] = posePositionsscore2[6];
            possZ2[3] = posePositionsscore2[7];
            possZ2[4] = posePositionsscore2[8];
            possZ2[5] = posePositionsscore2[9];
            possZ2[6] = posePositionsscore2[10];
            possZ2[7] = (posePositionsscore2[11] + posePositionsscore2[12]) * 0.5f;
            possZ2[8] = posePositionsscore2[13];
            possZ2[9] = posePositionsscore2[14];
            possZ2[10] = posePositionsscore2[15];
            possZ2[11] = posePositionsscore2[16];
        }
        else
        {
            possZ2[0] = 0;
            possZ2[1] = 0;
            possZ2[2] = 0;
            possZ2[3] = 0;
            possZ2[4] = 0;
            possZ2[5] = 0;
            possZ2[6] = 0;
            possZ2[7] = 0;
            possZ2[8] = 0;
            possZ2[9] = 0;
            possZ2[10] = 0;
            possZ2[11] = 0;
            pbr = 10;
        }
        if (pfc * mpc > score)
        {
            material.SetVectorArray("_pos3", posePositionsArray3);
            Vector2 p7c2 = new Vector2((posePositionsArray3[11].x + posePositionsArray3[12].x) * 0.5f, (posePositionsArray3[11].y + posePositionsArray3[12].y) * 0.5f);
            pcr = 5 * Vector2.Distance(p7c2, new Vector2((posePositionsArray3[5].x + posePositionsArray3[6].x), (posePositionsArray3[5].y + posePositionsArray3[6].y)) * 0.5f);
            ps3 = posePositionsscore3[0];
            posZ3[0] = ((new Vector2(posePositionsArray3[0].x, posePositionsArray3[0].y) - p7c2) / pcr + a);
            posZ3[1] = ((new Vector2(posePositionsArray3[5].x, posePositionsArray3[5].y) - p7c2) / pcr + a);
            posZ3[2] = ((new Vector2(posePositionsArray3[6].x, posePositionsArray3[6].y) - p7c2) / pcr + a);
            posZ3[3] = ((new Vector2(posePositionsArray3[7].x, posePositionsArray3[7].y) - p7c2) / pcr + a);
            posZ3[4] = ((new Vector2(posePositionsArray3[8].x, posePositionsArray3[8].y) - p7c2) / pcr + a);
            posZ3[5] = ((new Vector2(posePositionsArray3[9].x, posePositionsArray3[9].y) - p7c2) / pcr + a);
            posZ3[6] = ((new Vector2(posePositionsArray3[10].x, posePositionsArray3[10].y) - p7c2) / pcr + a);
            posZ3[7] = new Vector2(p7c2.x, p7c2.y);
            posZ3[8] = ((new Vector2(posePositionsArray3[13].x, posePositionsArray3[13].y) - p7c2) / pcr + a);
            posZ3[9] = ((new Vector2(posePositionsArray3[14].x, posePositionsArray3[14].y) - p7c2) / pcr + a);
            posZ3[10] = ((new Vector2(posePositionsArray3[15].x, posePositionsArray3[15].y) - p7c2) / pcr + a);
            posZ3[11] = ((new Vector2(posePositionsArray3[16].x, posePositionsArray3[16].y) - p7c2) / pcr + a);
            possZ3[0] = posePositionsscore3[0];
            possZ3[1] = posePositionsscore3[5];
            possZ3[2] = posePositionsscore3[6];
            possZ3[3] = posePositionsscore3[7];
            possZ3[4] = posePositionsscore3[8];
            possZ3[5] = posePositionsscore3[9];
            possZ3[6] = posePositionsscore3[10];
            possZ3[7] = (posePositionsscore3[11] + posePositionsscore3[12]) * 0.5f;
            possZ3[8] = posePositionsscore3[13];
            possZ3[9] = posePositionsscore3[14];
            possZ3[10] = posePositionsscore3[15];
            possZ3[11] = posePositionsscore3[16];
        }
        else
        {
            possZ3[0] = 0;
            possZ3[1] = 0;
            possZ3[2] = 0;
            possZ3[3] = 0;
            possZ3[4] = 0;
            possZ3[5] = 0;
            possZ3[6] = 0;
            possZ3[7] = 0;
            possZ3[8] = 0;
            possZ3[9] = 0;
            possZ3[10] = 0;
            possZ3[11] = 0;
            pp = 10;
        }
        if (poses.Length > 2)
        {
            ps3 = 1;
        }
        else
        {
            ps3 = 0;
            pcr = 10;
        }
        if (poses.Length > 1)
        {
            ps2 = 1;
        }
        else
        {
            ps2 = 0;
            pbr = 10;
        }

        material.SetFloat("_pr", pr);
        material.SetFloat("_pp", pp);
        material.SetFloat("_pos1", skeletons[0].keypoints[0].x);
        material.SetFloat("_pos2a", skeletons[1].keypoints[0].x);
        material.SetFloat("_pos3a", skeletons[2].keypoints[0].x);
        material.SetFloatArray("_score", possZ1);
        material.SetFloatArray("_score1", possZ2);
        material.SetFloatArray("_score2", possZ3);
        material.SetFloat("_ttsocre", score);
        pa = Mathf.Max(posePositionsscore[11], posePositionsscore[0]);
        pb = Mathf.Max(posePositionsscore2[11], posePositionsscore2[0]);
        pc = Mathf.Max(posePositionsscore3[11], posePositionsscore3[0]);
    }
    private void OnDisable()
    {
        engine.worker.Dispose();
    }
}
