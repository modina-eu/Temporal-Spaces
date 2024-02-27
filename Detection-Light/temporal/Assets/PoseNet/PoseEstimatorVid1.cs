using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Video;
using Unity.Barracuda;
using RenderHeads.Media.AVProLiveCamera;
public class PoseEstimatorVid1 : MonoBehaviour
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
    public Material material2;
    public PoseEstimatorVid script2;
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
    public Vector4 previousHipPosition;
    public Vector4 previousHipPosition2;
    public Vector2 previousHipPositiona;
    public Vector2 previousHipPositiona2;
    public Vector2 previousHipPositionb;
    public Vector2 previousHipPositionb2;
    private Vector2 smoothDampVelocity;
    private Vector2 smoothDampVelocity2;
    private Vector2 smoothDampVelocityb;
    private Vector2 smoothDampVelocityb2;
    public int closestSkeletonIndex = 0;
    public int closestSkeletonIndex2 = 0;
    public int closestSkeletonIndex3 = 0;
    public int closestSkeletonIndex2Candidate = -1;
    public float pt0;

    public Vector4 pos0;
    public Vector4 pos1;
    public Vector4 pos2;
    public Vector4 pos3;
    public Vector4 pos4;
    public Vector4 pos5;
    public Vector4 pos6;
    public Vector4 pos7;
    public Vector4 pos8;
    public Vector4 pos9;
    public Vector4 pos10;
    public Vector4 pos11;

    public Vector4 posb0;
    public Vector4 posb1;
    public Vector4 posb2;
    public Vector4 posb3;
    public Vector4 posb4;
    public Vector4 posb5;
    public Vector4 posb6;
    public Vector4 posb7;
    public Vector4 posb8;
    public Vector4 posb9;
    public Vector4 posb10;
    public Vector4 posb11;

    public Vector4 posc0;
    public Vector4 posc1;
    public Vector4 posc2;
    public Vector4 posc3;
    public Vector4 posc4;
    public Vector4 posc5;
    public Vector4 posc6;
    public Vector4 posc7;
    public Vector4 posc8;
    public Vector4 posc9;
    public Vector4 posc10;
    public Vector4 posc11;

    public float pbr;
    public float pcr;
    public float pr;
    public float pp;
    public float pvt1;
    public float pvt2;
    public float pvt3;
    
    private Vector2 a = new Vector2(0.5f, 0.5f);
    private Vector4[] posePositionsArray = new Vector4[17];
    private Vector4[] posePositionsArray2 = new Vector4[17];
    private Vector4[] posePositionsArray3 = new Vector4[17];
    private float[] posZ1 = new float[14];
    private float[] posZ2 = new float[14];
    private float[] posZ3 = new float[14];
    public float person1;
    public float person2;
    public float person3;
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
   /* void CompareAndAssign(int i, int j, ref int ti, ref int tj, float[] values)
    {
        if (values[i] < values[j])
        {
            ti = i + 1;
            tj = j + 1;
        }
        else
        {
            ti = j + 1;
            tj = i + 1;
        }
    }      */
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
               
                    float distanceToHip = Vector4.Distance(new Vector4(skeletons[i].keypoints[0].x,
                        skeletons[i].keypoints[0].y, posZ1[12], posZ1[13]), previousHipPosition);

                    float distanceToHip2 = Vector4.Distance(new Vector4(skeletons[i].keypoints[0].x,
                       skeletons[i].keypoints[0].y, posZ2[12], posZ2[13]), previousHipPosition2);

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
        prValues[0] = script2.pr;
        prValues[1] = script2.pbr;
        prValues[2] = script2.pcr;
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
        if (posePositionsscore2[0] > 0 && script2.ps2 > 0)
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
        if (posePositionsscore3[0] > 0 && script2.ps3 > 0)
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
        material2.SetFloat("_t1", t1);
        material2.SetFloat("_t2", t2);
        material2.SetFloat("_t3", t3);
         if(t1 == 1)
        {
            posZ1 = script2.posZ1;
        }
        if (t1 == 2)
        {
            posZ1 = script2.posZ2;
        }
        if (t1 == 3)
        {
            posZ1 = script2.posZ3;
        }
        if (t2 == 1)
        {
            posZ2 = script2.posZ1;
        }
        if (t2 == 2)
        {
            posZ2 = script2.posZ2;
        }
        if (t2 == 3)
        {
            posZ2 = script2.posZ3;
        }
        if (t3 == 1)
        {
            posZ3 = script2.posZ1;
        }
        if (t3 == 2)
        {
            posZ3 = script2.posZ2;
        }
        if (t3 == 3)
        {
            posZ3 = script2.posZ3;
        }
        previousHipPositiona = Vector2.SmoothDamp(previousHipPositiona, new Vector2(skeletons[closestSkeletonIndex].keypoints[0].x, skeletons[closestSkeletonIndex].keypoints[0].y), ref smoothDampVelocity, smoothingTime);
        previousHipPositionb = Vector2.SmoothDamp(previousHipPositionb, new Vector2(posZ1[12], posZ1[13]), ref smoothDampVelocityb, smoothingTime);
        previousHipPosition = new Vector4(previousHipPositiona.x, previousHipPositiona.y, previousHipPositionb.x, previousHipPositionb.y);
        previousHipPositiona2 = Vector2.SmoothDamp(previousHipPositiona2, new Vector2(skeletons[closestSkeletonIndex2].keypoints[0].x, skeletons[closestSkeletonIndex2].keypoints[0].y), ref smoothDampVelocity2, smoothingTime);
        previousHipPositionb2 = Vector2.SmoothDamp(previousHipPositionb2, new Vector2(posZ2[12], posZ2[13]), ref smoothDampVelocityb2, smoothingTime);
        previousHipPosition2 = new Vector4(previousHipPositiona2.x, previousHipPositiona2.y, previousHipPositionb2.x, previousHipPositionb2.y);
        person1 = 0;
        person2 = 0;
        person3 = 0;
        
        //"nose", "leftShoulder", "rightShoulder", "leftElbow", "rightElbow", "leftWrist", "rightWrist", "leftHip", "rightHip", "leftKnee", "rightKnee", "leftAnkle", "rightAnkle"
        if (posePositionsscore[0] > 0) {
            
            material.SetVectorArray("_pos", posePositionsArray);
            pos7 = new Vector4((posePositionsArray[11].x + posePositionsArray[12].x) * 0.5f, (posePositionsArray[11].y + posePositionsArray[12].y) * 0.5f, posZ1[6], (posePositionsscore[11] + posePositionsscore[12]) * 0.5f);
            if (pos7.w > 0.3f) { person1 = 1; }
            Vector2 p72 = new Vector2(pos7.x, pos7.y);
            pr = 5 * Vector2.Distance(p72, new Vector2((posePositionsArray[5].x + posePositionsArray[6].x), (posePositionsArray[5].y + posePositionsArray[6].y)) * 0.5f);
            Vector2 p0 = ((new Vector2(posePositionsArray[0].x, posePositionsArray[0].y) - p72) / pr + a);
            Vector2 p1 = ((new Vector2(posePositionsArray[5].x, posePositionsArray[5].y) - p72) / pr + a);
            Vector2 p2 = ((new Vector2(posePositionsArray[6].x, posePositionsArray[6].y) - p72) / pr + a);
            Vector2 p3 = ((new Vector2(posePositionsArray[7].x, posePositionsArray[7].y) - p72) / pr + a);
            Vector2 p4 = ((new Vector2(posePositionsArray[8].x, posePositionsArray[8].y) - p72) / pr + a);
            Vector2 p5 = ((new Vector2(posePositionsArray[9].x, posePositionsArray[9].y) - p72) / pr + a);
            Vector2 p6 = ((new Vector2(posePositionsArray[10].x, posePositionsArray[10].y) - p72) / pr + a);
            Vector2 p8 = ((new Vector2(posePositionsArray[13].x, posePositionsArray[13].y) - p72) / pr + a);
            Vector2 p9 = ((new Vector2(posePositionsArray[14].x, posePositionsArray[14].y) - p72) / pr + a);
            Vector2 p10 = ((new Vector2(posePositionsArray[15].x, posePositionsArray[15].y) - p72) / pr + a);
            Vector2 p11 = ((new Vector2(posePositionsArray[16].x, posePositionsArray[16].y) - p72) / pr + a);
            pos0 = new Vector4(p0.x, p0.y, posZ1[0], posePositionsscore[0]);
            pos1 = new Vector4(p1.x, p1.y, posZ1[1], posePositionsscore[5]);
            pos2 = new Vector4(p2.x, p2.y, posZ1[2], posePositionsscore[6]);
            pos3 = new Vector4(p3.x, p3.y, posZ1[3], posePositionsscore[7]);
            pos4 = new Vector4(p4.x, p4.y, posZ1[3], posePositionsscore[8]);
            pos5 = new Vector4(p5.x, p5.y, posZ1[4], posePositionsscore[9]);
            pos6 = new Vector4(p6.x, p6.y, posZ1[5], posePositionsscore[10]);
            pos8 = new Vector4(p8.x, p8.y, posZ1[7], posePositionsscore[13]);
            pos9 = new Vector4(p9.x, p9.y, posZ1[8], posePositionsscore[14]);
            pos10 = new Vector4(p10.x, p10.y, posZ1[9], posePositionsscore[15]);
            pos11 = new Vector4(p11.x, p11.y, posZ1[10], posePositionsscore[16]);
            pp = (posePositionsArray[15].y + posePositionsArray[16].y) / 2;
            pt0 = posePositionsArray[0].x;
        }
        if (posePositionsscore2[0] > 0)
        {          
            material.SetVectorArray("_pos2", posePositionsArray2);
            posb7 = new Vector4((posePositionsArray2[11].x + posePositionsArray2[12].x) * 0.5f, (posePositionsArray2[11].y + posePositionsArray2[12].y) * 0.5f, posZ2[6], (posePositionsscore2[11] + posePositionsscore2[12]) * 0.5f);
            if (posb7.w > 0.3f) { person2 = 1; }
            Vector2 p7b2 = new Vector2(posb7.x, posb7.y);
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
            posb0 = new Vector4(pb0.x, pb0.y, posePositionsscore2[0], t2);
            posb1 = new Vector4(pb1.x, pb1.y, posePositionsscore2[5], 0);
            posb2 = new Vector4(pb2.x, pb2.y, posePositionsscore2[6], 0);
            posb3 = new Vector4(pb3.x, pb3.y, posePositionsscore2[7], 0);
            posb4 = new Vector4(pb4.x, pb4.y, posePositionsscore2[8], 0);
            posb5 = new Vector4(pb5.x, pb5.y, posePositionsscore2[9], 0);
            posb6 = new Vector4(pb6.x, pb6.y, posePositionsscore2[10], 0);
            posb8 = new Vector4(pb8.x, pb8.y, posePositionsscore2[13], 0);
            posb9 = new Vector4(pb9.x, pb9.y, posePositionsscore2[14], 0);
            posb10 = new Vector4(pb10.x, pb10.y, posePositionsscore2[15], 0);
            posb11 = new Vector4(pb11.x, pb11.y, posePositionsscore2[16], 0);
        }
        if (posePositionsscore3[0] > 0)
        {            
            material.SetVectorArray("_pos3", posePositionsArray3);
            posc7 = new Vector4((posePositionsArray3[11].x + posePositionsArray3[12].x) * 0.5f, (posePositionsArray3[11].y + posePositionsArray3[12].y) * 0.5f, posZ3[6], (posePositionsscore3[11] + posePositionsscore3[12]) * 0.5f);
            if (posc7.w > 0.5f) { person3 = 1; }
            Vector2 p7c2 = new Vector2(posc7.x, posc7.y);
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
            posc0 = new Vector4(pc0.x, pc0.y, posePositionsscore3[0], t3);
            posc1 = new Vector4(pc1.x, pc1.y, posePositionsscore3[5], 0);
            posc2 = new Vector4(pc2.x, pc2.y, posePositionsscore3[6], 0);
            posc3 = new Vector4(pc3.x, pc3.y, posePositionsscore3[7], 0);
            posc4 = new Vector4(pc4.x, pc4.y, posePositionsscore3[8], 0);
            posc5 = new Vector4(pc5.x, pc5.y, posePositionsscore3[9], 0);
            posc6 = new Vector4(pc6.x, pc6.y, posePositionsscore3[10], 0);
            posc8 = new Vector4(pc8.x, pc8.y, posePositionsscore3[13], 0);
            posc9 = new Vector4(pc9.x, pc9.y, posePositionsscore3[14], 0);
            posc10 = new Vector4(pc10.x, pc10.y, posePositionsscore3[15], 0);
            posc11 = new Vector4(pc11.x, pc11.y, posePositionsscore3[16], 0);
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
