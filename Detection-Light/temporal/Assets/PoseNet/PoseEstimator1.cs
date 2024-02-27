using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Unity.Barracuda;
using RenderHeads.Media.AVProLiveCamera;
public class PoseEstimator1 : MonoBehaviour
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
    public PoseEstimator script3;
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
    public Vector3[] pos1 = new Vector3[12];
    public Vector3[] pos2 = new Vector3[12];
    public Vector3[] pos3 = new Vector3[12];
    public float[] score1 = new float[12];
    public float[] score2 = new float[12];
    public float[] score3 = new float[12];
    

    public float pbr;
    public float pcr;
    public float pr;
    public float pp;
    public float ppb;
    public float ppc;
    public float pvt1;
    public float pvt2;
    public float pvt3;
    public Vector3 previousHipPosition;
    public Vector3 previousHipPosition2;
    private Vector2 a = new Vector2(0.5f, 0.5f);
    private Vector4[] posePositionsArray = new Vector4[17];
    private Vector4[] posePositionsArray2 = new Vector4[17];
    private Vector4[] posePositionsArray3 = new Vector4[17];
    private Vector2[] posZ1 = new Vector2[12];
    private Vector2[] posZ2 = new Vector2[12];
    private Vector2[] posZ3 = new Vector2[12];
    private float[] possZ1 = new float[12];
    private float[] possZ2 = new float[12];
    private float[] possZ3 = new float[12];
    public Vector2 pn0;
    public Vector2 pn1;
    public Vector2 pn2;
    public Vector2 pn3;
    public Vector2 pn4;
    public Vector2 spn0;
    public Vector2 spn1;
    public Vector2 spn2;
    public Vector2 spn3;
    public Vector2 spn4;
    public Vector2[] pns = new Vector2[13];
    public float person1;
    public float person2;
    public float person3;
    public float nbperson;
    public float ptest;
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
       /*
        if (Input.GetKeyDown(KeyCode.B))
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
        }
         */
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
        int t1 = 1; int t2 = 2; int t3 = 3;
        float[] prValues = new float[3];
        prValues[0] = script3.pr;
        prValues[1] = script3.pbr;
        prValues[2] = script3.pcr;
        float[] pt = new float[3];
        pt[0] = posePositionsArray[0].x;
        pt[1] = posePositionsArray2[0].x;
        pt[2] = posePositionsArray3[0].x;
        pvt1 = posePositionsArray[0].x;
        pvt2 = posePositionsArray2[0].x;
        pvt3 = posePositionsArray3[0].x;

        if (posePositionsscore2[0] == 0)
        {
            t1 = 1;
            t2 = 2;
            t3 = 3;
        }
        if (posePositionsscore2[0] > 0 && script3.ps2 > 0)
        {
            if (pt[0] < pt[1])
            {
                if (prValues[0] < prValues[1])
                {
                    t1 = 1;
                    t2 = 2;
                }
                if (prValues[0] > prValues[1])
                {
                    t1 = 2;
                    t2 = 1;
                }

            }
            if (pt[0] > pt[1])
            {
                if (prValues[0] < prValues[1])
                {
                    t1 = 2;
                    t2 = 1;
                }
                if (prValues[0] > prValues[1])
                {
                    t1 = 1;
                    t2 = 2;
                }

            }
        }
        if (posePositionsscore3[0] > 0 && script3.ps3 > 0)
        {
            // Compare pt[0], pt[1], and pt[2]
            if (pt[0] < pt[1] && pt[0] < pt[2])
            {
                // Compare prValues[0], prValues[1], and prValues[2]
                if (prValues[0] < prValues[1] && prValues[0] < prValues[2])
                {
                    t1 = 1;
                    if (pt[1] < pt[2])
                    {
                        if (prValues[1] < prValues[2])
                        {
                            t2 = 2;
                            t3 = 3;
                        }
                        if (prValues[1] > prValues[2])
                        {
                            t2 = 3;
                            t3 = 2;
                        }
                    }
                    if (pt[1] > pt[2])
                    {
                        if (prValues[1] < prValues[2])
                        {
                            t2 = 3;
                            t3 = 2;
                        }
                        if (prValues[1] > prValues[2])
                        {
                            t2 = 2;
                            t3 = 3;
                        }

                    }
                }
                if (prValues[1] < prValues[0] && prValues[1] < prValues[2])
                {
                    t1 = 2;
                    if (pt[1] < pt[2])
                    {
                        if (prValues[0] < prValues[2])
                        {
                            t2 = 1;
                            t3 = 3;
                        }
                        if (prValues[0] > prValues[2])
                        {
                            t2 = 3;
                            t3 = 1;
                        }
                    }
                    if (pt[1] > pt[2])
                    {
                        if (prValues[0] < prValues[2])
                        {
                            t2 = 3;
                            t3 = 1;
                        }
                        if (prValues[0] > prValues[2])
                        {
                            t2 = 1;
                            t3 = 3;
                        }

                    }
                }
                if (prValues[2] < prValues[0] && prValues[2] < prValues[1])
                {
                    t1 = 3;
                    if (pt[1] < pt[2])
                    {
                        if (prValues[0] < prValues[1])
                        {
                            t2 = 1;
                            t3 = 2;
                        }
                        if (prValues[0] > prValues[1])
                        {
                            t2 = 2;
                            t3 = 1;
                        }
                    }
                    if (pt[1] > pt[2])
                    {
                        if (prValues[0] < prValues[1])
                        {
                            t2 = 2;
                            t3 = 1;
                        }
                        if (prValues[0] > prValues[1])
                        {
                            t2 = 1;
                            t3 = 2;
                        }

                    }
                }
            }

            if (pt[1] < pt[0] && pt[1] < pt[2])
            {
                if (prValues[0] < prValues[1] && prValues[0] < prValues[2])
                {
                    t2 = 1;
                    if (pt[0] < pt[2])
                    {
                        if (prValues[1] < prValues[2])
                        {
                            t1 = 2;
                            t3 = 3;
                        }
                        if (prValues[1] > prValues[2])
                        {
                            t1 = 3;
                            t3 = 2;
                        }
                    }
                    if (pt[0] > pt[2])
                    {
                        if (prValues[1] < prValues[2])
                        {
                            t1 = 3;
                            t3 = 2;
                        }
                        if (prValues[1] > prValues[2])
                        {
                            t1 = 2;
                            t3 = 3;
                        }

                    }
                }
                if (prValues[1] < prValues[0] && prValues[1] < prValues[2])
                {
                    t2 = 2;
                    if (pt[0] < pt[2])
                    {
                        if (prValues[0] < prValues[2])
                        {
                            t1 = 1;
                            t3 = 3;
                        }
                        if (prValues[0] > prValues[2])
                        {
                            t1 = 3;
                            t3 = 1;
                        }
                    }
                    if (pt[0] > pt[2])
                    {
                        if (prValues[0] < prValues[2])
                        {
                            t1 = 3;
                            t3 = 1;
                        }
                        if (prValues[0] > prValues[2])
                        {
                            t1 = 1;
                            t3 = 3;
                        }
                    }
                }
                if (prValues[2] < prValues[0] && prValues[2] < prValues[1])
                {
                    t2 = 3;
                    if (pt[0] < pt[2])
                    {
                        if (prValues[0] < prValues[1])
                        {
                            t1 = 1;
                            t3 = 2;
                        }
                        if (prValues[0] > prValues[1])
                        {
                            t1 = 2;
                            t3 = 1;
                        }
                    }
                    if (pt[0] > pt[2])
                    {
                        if (prValues[0] < prValues[1])
                        {
                            t1 = 2;
                            t3 = 1;
                        }
                        if (prValues[0] > prValues[1])
                        {
                            t1 = 1;
                            t3 = 2;
                        }

                    }
                }
            }
            if (pt[2] < pt[0] && pt[2] < pt[1])
            {
                if (prValues[0] < prValues[1] && prValues[0] < prValues[2])
                {
                    t3 = 1;
                    if (pt[0] < pt[1])
                    {
                        if (prValues[1] < prValues[2])
                        {
                            t1 = 2;
                            t2 = 3;
                        }
                        if (prValues[1] > prValues[2])
                        {
                            t1 = 3;
                            t2 = 2;
                        }
                    }
                    if (pt[0] > pt[1])
                    {
                        if (prValues[1] < prValues[2])
                        {
                            t1 = 3;
                            t2 = 2;
                        }
                        if (prValues[1] > prValues[2])
                        {
                            t1 = 2;
                            t2 = 3;
                        }

                    }
                }
                if (prValues[1] < prValues[0] && prValues[1] < prValues[2])
                {
                    t3 = 2;
                    if (pt[0] < pt[1])
                    {
                        if (prValues[0] < prValues[2])
                        {
                            t1 = 1;
                            t2 = 3;
                        }
                        if (prValues[0] > prValues[2])
                        {
                            t1 = 3;
                            t2 = 1;
                        }
                    }
                    if (pt[0] > pt[1])
                    {
                        if (prValues[0] < prValues[2])
                        {
                            t1 = 3;
                            t2 = 1;
                        }
                        if (prValues[0] > prValues[2])
                        {
                            t1 = 1;
                            t2 = 3;
                        }
                    }
                }
                if (prValues[2] < prValues[0] && prValues[2] < prValues[1])
                {
                    t3 = 3;
                    if (pt[0] < pt[1])
                    {
                        if (prValues[0] < prValues[1])
                        {
                            t1 = 1;
                            t2 = 2;
                        }
                        if (prValues[0] > prValues[1])
                        {
                            t1 = 2;
                            t2 = 1;
                        }
                    }
                    if (pt[0] > pt[1])
                    {
                        if (prValues[0] < prValues[1])
                        {
                            t1 = 2;
                            t2 = 1;
                        }
                        if (prValues[0] > prValues[1])
                        {
                            t1 = 1;
                            t2 = 2;
                        }

                    }
                }
            }
        }
        material.SetFloat("_t1", t1);
        material.SetFloat("_t2", t2);
        material.SetFloat("_t3", t3);
        if (t1 == 1)
        {
            posZ1 = script3.posZ1;
            possZ1 = script3.possZ1;
        }
        if (t1 == 2)
        {
            posZ1 = script3.posZ2;
            possZ1 = script3.possZ2;
        }
        if (t1 == 3)
        {
            posZ1 = script3.posZ3;
            possZ1 = script3.possZ3;
        }
        if (t2 == 1)
        {
            posZ2 = script3.posZ1;
            possZ2 = script3.possZ1;
        }
        if (t2 == 2)
        {
            posZ2 = script3.posZ2;
            possZ2 = script3.possZ2;
        }
        if (t2 == 3)
        {
            posZ2 = script3.posZ3;
            possZ2 = script3.possZ3;
        }
        if (t3 == 1)
        {
            posZ3 = script3.posZ1;
            possZ2 = script3.possZ1;
        }
        if (t3 == 2)
        {
            posZ3 = script3.posZ2;
            possZ3 = script3.possZ2;
        }
        if (t3 == 3)
        {
            posZ3 = script3.posZ3;
            possZ3 = script3.possZ3;
        }

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
        person1 -= 0.02f;
        person2 -= 0.02f;
        person3 -= 0.02f;
        float v = 0.3f;
        if (pfa*mpa > v && pfb*mpb > v && pfc*mpc > v)
        {
            person3 += 0.05f;
        }
        if (pfa * mpa > v && pfb *mpb > v || pfb * mpb > v && pfc * mpc > v || pfa * mpa > v && pfc * mpc > v)
        {
            person2 += 0.05f;
        }
        if (pfc * mpc > v || pfb * mpb > v || pfa * mpa > v)
        {
            person1 += 0.05f;
        }
        person1 = Mathf.Clamp(person1, 0f, 2f);
        person2 = Mathf.Clamp(person2, 0f, 2f);
        person3 = Mathf.Clamp(person3, 0f, 2f);
        nbperson = poses.Length;
        pn0 = Vector2.SmoothDamp(pn0, new Vector2(posePositionsArray[0].x, posePositionsArray[0].y), ref spn0, 0.3f);
        pn1 = Vector2.SmoothDamp(pn1, new Vector2(posePositionsArray[9].x, posePositionsArray[9].y), ref spn1, 0.3f);
        pn2 = Vector2.SmoothDamp(pn2, new Vector2(posePositionsArray[10].x, posePositionsArray[10].y), ref spn2, 0.3f);
        pn3 = Vector2.SmoothDamp(pn3, new Vector2(posePositionsArray[13].x, posePositionsArray[13].y), ref spn3, 0.3f);
        pn4 = Vector2.SmoothDamp(pn4, new Vector2(posePositionsArray[14].x, posePositionsArray[14].y), ref spn4, 0.3f);
        pns[0] = new Vector2(posePositionsArray[0].x, posePositionsArray[0].y);
        pns[1] = new Vector2(posePositionsArray[5].x, posePositionsArray[5].y);
        pns[2] = new Vector2(posePositionsArray[6].x, posePositionsArray[6].y);
        pns[3] = new Vector2(posePositionsArray[7].x, posePositionsArray[7].y);
        pns[4] = new Vector2(posePositionsArray[8].x, posePositionsArray[8].y);
        pns[5] = new Vector2(posePositionsArray[9].x, posePositionsArray[9].y);
        pns[6] = new Vector2(posePositionsArray[10].x, posePositionsArray[10].y);
        pns[7] = new Vector2(posePositionsArray[11].x, posePositionsArray[11].y);
        pns[8] = new Vector2(posePositionsArray[12].x, posePositionsArray[12].y);
        pns[9] = new Vector2(posePositionsArray[13].x, posePositionsArray[13].y);
        pns[10] = new Vector2(posePositionsArray[14].x, posePositionsArray[14].y);
        pns[11] = new Vector2(posePositionsArray[15].x, posePositionsArray[15].y);
        pns[12] = new Vector2(posePositionsArray[16].x, posePositionsArray[16].y);


        previousHipPosition = Vector3.SmoothDamp(previousHipPosition, new Vector3(posePositionsArray[0].x, posePositionsArray[0].y, pr), ref smoothDampVelocity, smoothingTime);

        previousHipPosition2 = Vector3.SmoothDamp(previousHipPosition2, new Vector3(posePositionsArray[0].x, posePositionsArray[0].y, pbr), ref smoothDampVelocity2, smoothingTime);


        //"nose", "leftShoulder", "rightShoulder", "leftElbow", "rightElbow", "leftWrist", "rightWrist", "leftHip", "rightHip", "leftKnee", "rightKnee", "leftAnkle", "rightAnkle"
        if (pfa * mpa >score)
        {
            material.SetVectorArray("_pos", posePositionsArray);
            pos1[7] = new Vector3((posePositionsArray[11].x + posePositionsArray[12].x) * 0.5f, (posePositionsArray[11].y + posePositionsArray[12].y) * 0.5f, posZ1[7].x);
            Vector2 p72 = new Vector2(pos1[7].x, pos1[7].y);
            pr = 5 * Vector2.Distance(p72, new Vector2((posePositionsArray[5].x + posePositionsArray[6].x), (posePositionsArray[5].y + posePositionsArray[6].y)) * 0.5f);
            Vector2 p0 = ((pn0 - p72) / pr + a);
            Vector2 p1 = ((new Vector2(posePositionsArray[5].x, posePositionsArray[5].y) - p72) / pr + a);
            Vector2 p2 = ((new Vector2(posePositionsArray[6].x, posePositionsArray[6].y) - p72) / pr + a);
            Vector2 p3 = ((new Vector2(posePositionsArray[7].x, posePositionsArray[7].y) - p72) / pr + a);
            Vector2 p4 = ((new Vector2(posePositionsArray[8].x, posePositionsArray[8].y) - p72) / pr + a);
            Vector2 p5 = ((pn1 - p72) / pr + a);
            Vector2 p6 = ((pn2 - p72) / pr + a);
            Vector2 p8 = ((pn3 - p72) / pr + a);
            Vector2 p9 = ((pn4 - p72) / pr + a);
            Vector2 p10 = ((new Vector2(posePositionsArray[15].x, posePositionsArray[15].y) - p72) / pr + a);
            Vector2 p11 = ((new Vector2(posePositionsArray[16].x, posePositionsArray[16].y) - p72) / pr + a);
            pos1[0] = new Vector3(p0.x, p0.y, posZ1[0].x);
            pos1[1] = new Vector3(p1.x, p1.y, posZ1[1].x);
            pos1[2] = new Vector3(p2.x, p2.y, posZ1[2].x);
            pos1[3] = new Vector3(p3.x, p3.y, posZ1[3].x);
            pos1[4] = new Vector3(p4.x, p4.y, posZ1[4].x);
            pos1[5] = new Vector3(p5.x, p5.y, posZ1[5].x);
            pos1[6] = new Vector3(p6.x, p6.y, posZ1[6].x);
            pos1[8] = new Vector3(p8.x, p8.y, posZ1[8].x);
            pos1[9] = new Vector3(p9.x, p9.y, posZ1[9].x);
            pos1[10] = new Vector3(p10.x, p10.y, posZ1[10].x);
            pos1[11] = new Vector3(p11.x, p11.y, posZ1[11].x);
            score1[0] = Mathf.Max(posePositionsscore[0], possZ1[0]);
            score1[1] = Mathf.Max(posePositionsscore[5], possZ1[1]);
            score1[2] = Mathf.Max(posePositionsscore[6], possZ1[2]);
            score1[3] = Mathf.Max(posePositionsscore[7], possZ1[3]);
            score1[4] = Mathf.Max(posePositionsscore[8], possZ1[4]);
            score1[5] = Mathf.Max(posePositionsscore[9], possZ1[5]);
            score1[6] = Mathf.Max(posePositionsscore[10], possZ1[6]);
            score1[8] = Mathf.Max(posePositionsscore[13], possZ1[8]);
            score1[7] = Mathf.Max((posePositionsscore[11] + posePositionsscore[12]) * 0.5f, possZ1[7]);
            score1[9] = Mathf.Max(posePositionsscore[14], possZ1[9]);
            score1[10] = Mathf.Max(posePositionsscore[15], possZ1[10]);
            score1[11] = Mathf.Max(posePositionsscore[16], possZ1[11]);
            pp = Mathf.Min(posePositionsArray[15].y, posePositionsArray[16].y);
        }
        else
        {
            //Debug.Log("true0");
            score1[0] = 0;
            score1[1] = 0;
            score1[2] = 0;
            score1[3] = 0;
            score1[4] = 0;
            score1[5] = 0;
            score1[6] = 0;
            score1[8] = 0;
            score1[7] = 0;
            score1[9] = 0;
            score1[10] = 0;
            score1[11] = 0;
        }
        if (pfb * mpb >score)
        {
            material.SetVectorArray("_pos2", posePositionsArray2);

            pos2[7] = new Vector3((posePositionsArray2[11].x + posePositionsArray2[12].x) * 0.5f, (posePositionsArray2[11].y + posePositionsArray2[12].y) * 0.5f, posZ2[7].x);
            Vector2 p7b2 = new Vector2(pos2[7].x, pos2[7].y);
            pbr = 5 * Vector2.Distance(p7b2, new Vector2((posePositionsArray2[5].x + posePositionsArray2[6].x), (posePositionsArray2[5].y + posePositionsArray2[6].y)) * 0.5f);
            Vector2 pb0 = ((new Vector2(posePositionsArray2[0].x, posePositionsArray2[0].y) - p7b2) / pbr + a);
            Vector2 pb1 = ((new Vector2(posePositionsArray2[5].x, posePositionsArray2[5].y) - p7b2) / pbr + a);
            Vector2 pb2 = ((new Vector2(posePositionsArray2[6].x, posePositionsArray2[6].y) - p7b2) / pbr + a);
            Vector2 pb3 = ((new Vector2(posePositionsArray2[7].x, posePositionsArray2[7].y) - p7b2) / pbr + a);
            Vector2 pb4 = ((new Vector2(posePositionsArray2[8].x, posePositionsArray2[8].y) - p7b2) / pbr + a);
            Vector2 pb5 = ((new Vector2(posePositionsArray2[9].x, posePositionsArray2[9].y) - p7b2) / pbr + a);
            Vector2 pb6 = ((new Vector2(posePositionsArray2[10].x, posePositionsArray2[10].y) - p7b2) / pbr + a);
            Vector2 pb8 = ((new Vector2(posePositionsArray2[13].x, posePositionsArray2[13].y) - p7b2) / pbr + a);
            Vector2 pb9 = ((new Vector2(posePositionsArray2[14].x, posePositionsArray2[14].y) - p7b2) / pbr + a);
            Vector2 pb10 = ((new Vector2(posePositionsArray2[15].x, posePositionsArray2[15].y) - p7b2) / pbr + a);
            Vector2 pb11 = ((new Vector2(posePositionsArray2[16].x, posePositionsArray2[16].y) - p7b2) / pbr + a);
            pos2[0] = new Vector3(pb0.x, pb0.y, posZ2[0].x);
            pos2[1] = new Vector3(pb1.x, pb1.y, posZ2[1].x);
            pos2[2] = new Vector3(pb2.x, pb2.y, posZ2[2].x);
            pos2[3] = new Vector3(pb3.x, pb3.y, posZ2[3].x);
            pos2[4] = new Vector3(pb4.x, pb4.y, posZ2[4].x);
            pos2[5] = new Vector3(pb5.x, pb5.y, posZ2[5].x);
            pos2[6] = new Vector3(pb6.x, pb6.y, posZ2[6].x);
            pos2[8] = new Vector3(pb8.x, pb8.y, posZ2[8].x);
            pos2[9] = new Vector3(pb9.x, pb9.y, posZ2[9].x);
            pos2[10] = new Vector3(pb10.x, pb10.y, posZ2[10].x);
            pos2[11] = new Vector3(pb11.x, pb11.y, posZ2[11].x);
            score2[0] = Mathf.Max(posePositionsscore2[0], possZ2[0]);
            score2[1] = Mathf.Max(posePositionsscore2[5], possZ2[1]);
            score2[2] = Mathf.Max(posePositionsscore2[6], possZ2[2]);
            score2[3] = Mathf.Max(posePositionsscore2[7], possZ2[3]);
            score2[4] = Mathf.Max(posePositionsscore2[8], possZ2[4]);
            score2[5] = Mathf.Max(posePositionsscore2[9], possZ2[5]);
            score2[6] = Mathf.Max(posePositionsscore2[10], possZ2[6]);
            score2[8] = Mathf.Max(posePositionsscore2[13], possZ2[8]);
            score2[7] = Mathf.Max((posePositionsscore2[11] + posePositionsscore2[12]) * 0.5f, possZ2[7]);
            score2[9] = Mathf.Max(posePositionsscore2[14], possZ2[9]);
            score2[10] = Mathf.Max(posePositionsscore2[15], possZ2[10]);
            score2[11] = Mathf.Max(posePositionsscore2[16], possZ2[11]);
            ppb = Mathf.Min(posePositionsArray2[15].y, posePositionsArray2[16].y);
        }
        else
        {
            //Debug.Log("true01");
            score2[0] = 0;
            score2[1] = 0;
            score2[2] = 0;
            score2[3] = 0;
            score2[4] = 0;
            score2[5] = 0;
            score2[6] = 0;
            score2[8] = 0;
            score2[7] = 0;
            score2[9] = 0;
            score2[10] = 0;
            score2[11] = 0;
        }
        if (pfc * mpc >score)
        {
            material.SetVectorArray("_pos3", posePositionsArray3);
            pos3[7] = new Vector4((posePositionsArray3[11].x + posePositionsArray3[12].x) * 0.5f, (posePositionsArray3[11].y + posePositionsArray3[12].y) * 0.5f
                , posZ3[7].x, Mathf.Max((posePositionsscore3[11] + posePositionsscore3[12]) * 0.5f, possZ3[7]));
            Vector2 p7c2 = new Vector2(pos3[7].x, pos3[7].y);
            pcr = 5 * Vector2.Distance(p7c2, new Vector2((posePositionsArray3[5].x + posePositionsArray3[6].x), (posePositionsArray3[5].y + posePositionsArray3[6].y)) * 0.5f);
            Vector2 pc0 = ((new Vector2(posePositionsArray3[0].x, posePositionsArray3[0].y) - p7c2) / pcr + a);
            Vector2 pc1 = ((new Vector2(posePositionsArray3[5].x, posePositionsArray3[5].y) - p7c2) / pcr + a);
            Vector2 pc2 = ((new Vector2(posePositionsArray3[6].x, posePositionsArray3[6].y) - p7c2) / pcr + a);
            Vector2 pc3 = ((new Vector2(posePositionsArray3[7].x, posePositionsArray3[7].y) - p7c2) / pcr + a);
            Vector2 pc4 = ((new Vector2(posePositionsArray3[8].x, posePositionsArray3[8].y) - p7c2) / pcr + a);
            Vector2 pc5 = ((new Vector2(posePositionsArray3[9].x, posePositionsArray3[9].y) - p7c2) / pcr + a);
            Vector2 pc6 = ((new Vector2(posePositionsArray3[10].x, posePositionsArray3[10].y) - p7c2) / pcr + a);
            Vector2 pc8 = ((new Vector2(posePositionsArray3[13].x, posePositionsArray3[13].y) - p7c2) / pcr + a);
            Vector2 pc9 = ((new Vector2(posePositionsArray3[14].x, posePositionsArray3[14].y) - p7c2) / pcr + a);
            Vector2 pc10 = ((new Vector2(posePositionsArray3[15].x, posePositionsArray3[15].y) - p7c2) / pcr + a);
            Vector2 pc11 = ((new Vector2(posePositionsArray3[16].x, posePositionsArray3[16].y) - p7c2) / pcr + a);
            pos3[0] = new Vector3(pc0.x, pc0.y, posZ3[0].x);
            pos3[1] = new Vector3(pc1.x, pc1.y, posZ3[1].x);
            pos3[2] = new Vector3(pc2.x, pc2.y, posZ3[2].x);
            pos3[3] = new Vector3(pc3.x, pc3.y, posZ3[3].x);
            pos3[4] = new Vector3(pc4.x, pc4.y, posZ3[4].x);
            pos3[5] = new Vector3(pc5.x, pc5.y, posZ3[5].x);
            pos3[6] = new Vector3(pc6.x, pc6.y, posZ3[6].x);
            pos3[8] = new Vector3(pc8.x, pc8.y, posZ3[8].x);
            pos3[9] = new Vector3(pc9.x, pc9.y, posZ3[9].x);
            pos3[10] = new Vector3(pc10.x, pc10.y, posZ3[10].x);
            pos3[11] = new Vector3(pc11.x, pc11.y, posZ3[11].x);
            score3[0] = Mathf.Max(posePositionsscore3[0], possZ3[0]);
            score3[1] = Mathf.Max(posePositionsscore3[5], possZ3[1]);
            score3[2] = Mathf.Max(posePositionsscore3[6], possZ3[2]);
            score3[3] = Mathf.Max(posePositionsscore3[7], possZ3[3]);
            score3[4] = Mathf.Max(posePositionsscore3[8], possZ3[4]);
            score3[5] = Mathf.Max(posePositionsscore3[9], possZ3[5]);
            score3[6] = Mathf.Max(posePositionsscore3[10], possZ3[6]);
            score3[8] = Mathf.Max(posePositionsscore3[13], possZ3[8]);
            score3[7] = Mathf.Max((posePositionsscore3[11] + posePositionsscore3[12]) * 0.5f, possZ3[7]);
            score3[9] = Mathf.Max(posePositionsscore3[14], possZ3[9]);
            score3[10] = Mathf.Max(posePositionsscore3[15], possZ3[10]);
            score3[11] = Mathf.Max(posePositionsscore3[16], possZ3[11]);
            ppc = Mathf.Min(posePositionsArray3[15].y, posePositionsArray3[16].y);
        }
        else
        {
            //Debug.Log("true02");
            score3[0] = 0;
            score3[1] = 0;
            score3[2] = 0;
            score3[3] = 0;
            score3[4] = 0;
            score3[5] = 0;
            score3[6] = 0;
            score3[8] = 0;
            score3[7] = 0;
            score3[9] = 0;
            score3[10] = 0;
            score3[11] = 0;
        }
        material.SetFloat("_pr", pr);
        material.SetFloat("_pp", pp);
        material.SetFloat("_pos1", skeletons[0].keypoints[0].x);
        material.SetFloat("_pos2a", skeletons[1].keypoints[0].x);
        material.SetFloat("_pos3a", skeletons[2].keypoints[0].x);
        material.SetFloatArray("_score", score1);
        material.SetFloatArray("_score1", score2);
        material.SetFloatArray("_score2", score3);
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
