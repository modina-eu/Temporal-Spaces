using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Unity.Barracuda;
using RenderHeads.Media.AVProLiveCamera;
public class PoseEstimatorVid : MonoBehaviour
{
    public RenderTexture Tex;
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
    public PoseEstimatorVid1 script2;
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
    public float[] posZ1 = new float[14];
    public float[] posZ2 = new float[14];
    public float[] posZ3 = new float[14];
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
        for (int j = 0; j < 17; j++)
        {
            posePositionsArray3[j] = new Vector4(0, 0, 0, 0);
            posePositionsArray2[j] = new Vector4(0, 0, 0, 0);
            posePositionsArray[j] = new Vector4(0, 0, 0, 0);

        }
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
        float[] posePositionsscore = new float[17];
        float[] posePositionsscore2 = new float[17];
        float[] posePositionsscore3 = new float[17];
        Graphics.Blit(Tex, rTex);
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

            float hipX = skeletons[0].keypoints[0].x;

            float distanceToHip = Vector3.Distance(new Vector3(skeletons[i].keypoints[0].x,
                skeletons[i].keypoints[0].y, script2.pt0), previousHipPosition);

            float distanceToHip2 = Vector3.Distance(new Vector3(skeletons[i].keypoints[0].x,
               skeletons[i].keypoints[0].y, 0), previousHipPosition2);

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
        previousHipPosition = Vector3.SmoothDamp(previousHipPosition, new Vector3(skeletons[closestSkeletonIndex].keypoints[0].x, skeletons[closestSkeletonIndex].keypoints[0].y, script2.pt0), ref smoothDampVelocity, smoothingTime);
        previousHipPosition2 = Vector3.SmoothDamp(previousHipPosition2, new Vector3(skeletons[closestSkeletonIndex2].keypoints[0].x, skeletons[closestSkeletonIndex2].keypoints[0].y, 0), ref smoothDampVelocity2, smoothingTime);

        //"nose", "leftShoulder", "rightShoulder", "leftElbow", "rightElbow", "leftWrist", "rightWrist", "leftHip", "rightHip", "leftKnee", "rightKnee", "leftAnkle", "rightAnkle"
        if (posePositionsscore[0] > 0) {
            material.SetVectorArray("_pos", posePositionsArray);
            Vector2 p72 = new Vector2((posePositionsArray[11].x + posePositionsArray[12].x) * 0.5f, (posePositionsArray[11].y + posePositionsArray[12].y) * 0.5f);
            pr = 5 * Vector2.Distance(p72, new Vector2((posePositionsArray[5].x + posePositionsArray[6].x), (posePositionsArray[5].y + posePositionsArray[6].y)) * 0.5f);
            pp = (posePositionsArray[15].y + posePositionsArray[16].y) / 2;
            posZ1[0] = (posePositionsArray[0].x - p72.x) / pr + 0.5f;
            posZ1[1] = (posePositionsArray[5].x - p72.x) / pr + 0.5f;
            posZ1[2] = (posePositionsArray[6].x - p72.x) / pr + 0.5f;
            posZ1[3] = (posePositionsArray[7].x - p72.x) / pr + 0.5f;
            posZ1[4] = (posePositionsArray[8].x - p72.x) / pr + 0.5f;
            posZ1[5] = (posePositionsArray[9].x - p72.x) / pr + 0.5f;
            posZ1[6] = (posePositionsArray[10].x - p72.x) / pr + 0.5f;
            posZ1[7] = p72.x;
            posZ1[8] = (posePositionsArray[13].x - p72.x) / pr + 0.5f;
            posZ1[9] = (posePositionsArray[14].x - p72.x) / pr + 0.5f;
            posZ1[10] = (posePositionsArray[15].x - p72.x) / pr + 0.5f;
            posZ1[11] = (posePositionsArray[16].x - p72.x) / pr + 0.5f;
            posZ1[12] = posePositionsArray[0].x;
            posZ1[13] = posePositionsArray[0].y;
        }
        else { pr = 10; }
        if(posePositionsscore2[0] > 0) {
            material.SetVectorArray("_pos2", posePositionsArray2);
        Vector2 p7b2 = new Vector2((posePositionsArray2[11].x + posePositionsArray2[12].x) * 0.5f, (posePositionsArray2[11].y + posePositionsArray2[12].y) * 0.5f);
        pbr = 5 * Vector2.Distance(p7b2, new Vector2((posePositionsArray2[5].x + posePositionsArray2[6].x), (posePositionsArray2[5].y + posePositionsArray2[6].y)) * 0.5f);

        posZ2[0] = (posePositionsArray2[0].x - p7b2.x) / pr + 0.5f;
        posZ2[1] = (posePositionsArray2[5].x - p7b2.x) / pr + 0.5f;
        posZ2[2] = (posePositionsArray2[6].x - p7b2.x) / pr + 0.5f;
        posZ2[3] = (posePositionsArray2[7].x - p7b2.x) / pr + 0.5f;
        posZ2[4] = (posePositionsArray2[8].x - p7b2.x) / pr + 0.5f;
        posZ2[5] = (posePositionsArray2[9].x - p7b2.x) / pr + 0.5f;
        posZ2[6] = (posePositionsArray2[10].x - p7b2.x) / pr + 0.5f;
        posZ2[7] = p7b2.x;
        posZ2[8] = (posePositionsArray2[13].x - p7b2.x) / pr + 0.5f;
        posZ2[9] = (posePositionsArray2[14].x - p7b2.x) / pr + 0.5f;
        posZ2[10] = (posePositionsArray2[15].x - p7b2.x) / pr + 0.5f;
        posZ2[11] = (posePositionsArray2[16].x - p7b2.x) / pr + 0.5f;
        posZ2[12] = posePositionsArray2[0].x;
        posZ2[13] = posePositionsArray2[0].y;
        }
        else { pbr = 10; }
         if (poses.Length > 1)
            {
            ps2 = 1;
            }
        else
        {
            ps2 = 0;
         
        }
        if(posePositionsscore3[0] > 0) {
            material.SetVectorArray("_pos3", posePositionsArray3);
            Vector2 p7c2 = new Vector2((posePositionsArray3[11].x + posePositionsArray3[12].x) * 0.5f, (posePositionsArray3[11].y + posePositionsArray3[12].y) * 0.5f);
            pcr = 5 * Vector2.Distance(p7c2, new Vector2((posePositionsArray3[5].x + posePositionsArray3[6].x), (posePositionsArray3[5].y + posePositionsArray3[6].y)) * 0.5f);
            ps3 = posePositionsscore3[0];
            posZ3[0] = (posePositionsArray3[0].x - p7c2.x) / pr + 0.5f;
            posZ3[1] = (posePositionsArray3[5].x - p7c2.x) / pr + 0.5f;
            posZ3[2] = (posePositionsArray3[6].x - p7c2.x) / pr + 0.5f;
            posZ3[3] = (posePositionsArray3[7].x - p7c2.x) / pr + 0.5f;
            posZ3[4] = (posePositionsArray3[8].x - p7c2.x) / pr + 0.5f;
            posZ3[5] = (posePositionsArray3[9].x - p7c2.x) / pr + 0.5f;
            posZ3[6] = (posePositionsArray3[10].x - p7c2.x) / pr + 0.5f;
            posZ3[7] = p7c2.x;
            posZ3[8] = (posePositionsArray3[13].x - p7c2.x) / pr + 0.5f;
            posZ3[9] = (posePositionsArray3[14].x - p7c2.x) / pr + 0.5f;
            posZ3[10] = (posePositionsArray3[15].x - p7c2.x) / pr + 0.5f;
            posZ3[11] = (posePositionsArray3[16].x - p7c2.x) / pr + 0.5f;
            posZ3[12] = posePositionsArray3[0].x;
            posZ3[13] = posePositionsArray3[0].y;
        }  else { pcr = 10; }
            if (poses.Length > 2)
            {
                ps3 = 1;
            }
            else
            {
                ps3 = 0;
           
            }

        material.SetFloat("_pr", pr);
        material.SetFloat("_pp", pp);
        material.SetFloat("_pos1", skeletons[0].keypoints[0].x);
        material.SetFloat("_pos2a", skeletons[1].keypoints[0].x);
        material.SetFloat("_pos3a", skeletons[2].keypoints[0].x);
        material.SetFloatArray("_score", posePositionsscore);
        material.SetFloatArray("_score1", posePositionsscore2);
        material.SetFloatArray("_score2", posePositionsscore3);
    }
    private void OnDisable()
    {
        engine.worker.Dispose();
    }
}
