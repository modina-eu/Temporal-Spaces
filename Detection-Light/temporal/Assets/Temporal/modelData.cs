using UnityEngine;
using UnityEngine.UI;
using Klak.TestTools;
using BodyPix;
using Unity.Mathematics;
using RenderHeads.Media.AVProLiveCamera;
public sealed class modelData : MonoBehaviour
{
    // [SerializeField] ImageSource _source = null;
    [SerializeField] ResourceSet _resources = null;

    public float3 handRight;
    public float3 handLeft;
    public float3 headc;
    public float3 shoulderRight;
    public float3 shoulderLeft;
    public float3 elbowRight;
    public float3 elbowLeft;
    public float3 hipRight;
    public float3 hipLeft;
    public float3 kneeRight;
    public float3 kneeLeft;
    public float3 ankleRight;
    public float3 ankleLeft;

    public RenderTexture tex;
    public Material mat;
    BodyDetector _detector;
    //BodyDetector _detector2;
    private Vector3 velocity = Vector3.zero;
    private Vector3 velocity2 = Vector3.zero;
    private Vector3 velocity3 = Vector3.zero;
    private Vector3 velocity4 = Vector3.zero;
    private Vector3 velocity5 = Vector3.zero;
    private Vector3 velocity6 = Vector3.zero;
    private Vector3 velocity7 = Vector3.zero;
    private Vector3 velocity8 = Vector3.zero;
    private Vector3 velocity9 = Vector3.zero;
    private Vector3 velocity10 = Vector3.zero;
    private Vector3 velocity11 = Vector3.zero;
    private Vector3 velocity12 = Vector3.zero;
    private Vector3 velocity13 = Vector3.zero;

    public float smoothTime = 0.0f;
    public float maximum = 0.8f;
    public int res = 128;
    public AVProLiveCamera script2;
    //public InfraredSourceCompute script;
    

    OneEuroFilter2 _filter =
      new OneEuroFilter2() { Beta = 0.1f, MinCutoff = 0.5f };

    void Start()
    {
        Application.runInBackground = true;
        _detector = new BodyDetector(_resources, res , res);
       // _detector2 = new BodyDetector(_resources, res, res);
    }
    void OnDestroy()
    {
        _detector?.Dispose();
        _detector = null;
       // _detector2?.Dispose();
      // _detector2 = null;
    }

    void Update()
    {
       

        _detector.ProcessImage(tex);
       // _detector2.ProcessImage(tex);
        var hand2 = _detector.Keypoints[(int)Body.KeypointID.RightWrist];
        var hand = _detector.Keypoints[(int)Body.KeypointID.LeftWrist];
        var head = _detector.Keypoints[(int)Body.KeypointID.Nose];
        var shoulder = _detector.Keypoints[(int)Body.KeypointID.LeftShoulder];
        var shoulder2 = _detector.Keypoints[(int)Body.KeypointID.RightShoulder];
        var elbow = _detector.Keypoints[(int)Body.KeypointID.LeftElbow];
        var elbow2 = _detector.Keypoints[(int)Body.KeypointID.RightElbow];
        var hip = _detector.Keypoints[(int)Body.KeypointID.LeftHip];
        var hip2 = _detector.Keypoints[(int)Body.KeypointID.RightHip];
        var knee = _detector.Keypoints[(int)Body.KeypointID.LeftKnee];
        var knee2 = _detector.Keypoints[(int)Body.KeypointID.RightKnee];
        var ankle = _detector.Keypoints[(int)Body.KeypointID.LeftAnkle];
        var ankle2 = _detector.Keypoints[(int)Body.KeypointID.RightAnkle];



        float handScore = (hand.Score >= maximum) ? 1.0f : 0.0f;
        handRight = Vector3.SmoothDamp(handRight, math.float3(hand.Position, handScore), ref velocity, smoothTime);  
        mat.SetVector("_handRight", math.float4(handRight, 0));


        float hand2Score = (hand2.Score >= maximum) ? 1.0f : 0.0f;
        handLeft = Vector3.SmoothDamp(handLeft, math.float3(hand2.Position, hand2Score), ref velocity2, smoothTime);
        mat.SetVector("_handLeft", math.float4(handLeft, 0));

        float headScore = (head.Score >= maximum) ? 1.0f : 0.0f;
        headc = Vector3.SmoothDamp(headc, math.float3(head.Position, headScore), ref velocity3, smoothTime);
        mat.SetVector("_head", math.float4(headc, 0));

        float shoulderScore = (shoulder.Score >= maximum) ? 1.0f : 0.0f;
        shoulderRight = Vector3.SmoothDamp(shoulderRight, math.float3(shoulder.Position, shoulderScore), ref velocity4, smoothTime);
        mat.SetVector("_shoulderRight", math.float4(shoulderRight, 0));

        float shoulder2Score = (shoulder2.Score >= maximum) ? 1.0f : 0.0f;
        shoulderLeft = Vector3.SmoothDamp(shoulderLeft, math.float3(shoulder2.Position, shoulder2Score), ref velocity5, smoothTime);
        mat.SetVector("_shoulderLeft", math.float4(shoulderLeft, 0));

        float elbowScore = (elbow.Score >= maximum) ? 1.0f : 0.0f;
        elbowRight = Vector3.SmoothDamp(elbowRight, math.float3(elbow.Position, elbowScore), ref velocity6, smoothTime);
        mat.SetVector("_elbowRight", math.float4(elbowRight, 0));

        float elbow2Score = (elbow2.Score >= maximum) ? 1.0f : 0.0f;
        elbowLeft = Vector3.SmoothDamp(elbowLeft, math.float3(elbow2.Position, elbow2Score), ref velocity7, smoothTime);
        mat.SetVector("_elbowLeft", math.float4(elbowLeft, 0));

        float hipScore = (hip.Score >= maximum) ? 1.0f : 0.0f;
        hipRight = Vector3.SmoothDamp(hipRight, math.float3(hip.Position, hipScore), ref velocity8, smoothTime);
        mat.SetVector("_hipRight", math.float4(hipRight, 0));

        float hip2Score = (hip2.Score >= maximum) ? 1.0f : 0.0f;
        hipLeft = Vector3.SmoothDamp(hipLeft, math.float3(hip2.Position, hip2Score), ref velocity9, smoothTime);
        mat.SetVector("_hipLeft", math.float4(hipLeft, 0));

        float kneeScore = (knee.Score >= maximum) ? 1.0f : 0.0f;
        kneeRight = Vector3.SmoothDamp(kneeRight, math.float3(knee.Position, kneeScore), ref velocity10, smoothTime);
        mat.SetVector("_kneeRight", math.float4(kneeRight, 0));

        float knee2Score = (knee2.Score >= maximum) ? 1.0f : 0.0f;
        kneeLeft = Vector3.SmoothDamp(kneeLeft, math.float3(knee2.Position, knee2Score), ref velocity11, smoothTime);
        mat.SetVector("_kneeLeft", math.float4(kneeLeft, 0));

        float ankleScore = (ankle.Score >= maximum) ? 1.0f : 0.0f;
        ankleRight = Vector3.SmoothDamp(ankleRight, math.float3(ankle.Position, ankleScore), ref velocity12, smoothTime);
        mat.SetVector("_ankleRight", math.float4(ankleRight, 0));

        float ankle2Score = (ankle2.Score >= maximum) ? 1.0f : 0.0f;
        ankleLeft = Vector3.SmoothDamp(ankleLeft, math.float3(ankle2.Position, ankle2Score), ref velocity13, smoothTime);
        mat.SetVector("_ankleLeft", math.float4(ankleLeft, 0));
      
    }
}
