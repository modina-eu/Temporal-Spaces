
using UnityEngine;
using UnityEngine.UI;
using Klak.TestTools;
using YoloV4Tiny;
using Unity.Mathematics;
using System.Collections.Generic;
using RenderHeads.Media.AVProLiveCamera;
sealed class Visualizer : MonoBehaviour
{
  //  [SerializeField] ImageSource _source = null;
    [SerializeField, Range(0, 1)] float _threshold = 0.5f;
    [SerializeField] ResourceSet _resources = null;
    //[SerializeField] RawImage _preview = null;
    public Material mat;

    private ObjectDetector _detector;
    private List<Vector4> markers = new List<Vector4>();

    public float smoothTime = 0.3f;
    float Velocit1 = 0.0f;
    float Velocit2 = 0.0f;
    float Velocit3 = 0.0f;
    float Velocit4 = 0.0f;
    public float a;
    public float b;
    public float c;
    public float dd;
    public AVProLiveCamera script2;
    public computeParsing script1;
    private RenderTexture reducedTexture;
    void Start()
    {
        _detector = new ObjectDetector(_resources);
        reducedTexture = new RenderTexture(1280, 720, 0);
        reducedTexture.enableRandomWrite = true;
        reducedTexture.Create();  
        mat.SetTexture("_MainTex2", script2.OutputTexture);
        
    }

    void OnDisable()
    {
        _detector.Dispose();
      
    }

    void Update()
    {


        Graphics.Blit(script2.OutputTexture, reducedTexture);
        _detector.ProcessImage(reducedTexture, _threshold);

        foreach (var d in _detector.Detections)
        {
            if (d.classIndex == 14 && d.score > _threshold)
            {
                markers.Add(new Vector4(d.x, d.y, d.w, d.h));
            }
        }

        if (markers.Count > 0)
        {
            float da = 10; 
            float closestX = 0, closestY = 0, closestZ = 0, closestW = 0;
             
            for (int i = 0; i < markers.Count; i++)
            {
                if (da > Mathf.Abs(a - markers[i].x))
                {
                    closestX = markers[i].x;
                    closestY = markers[i].y;
                    closestZ = markers[i].z;
                    closestW = markers[i].w;
                    da = Mathf.Abs(a - markers[i].x);
                   
                }
            }
            a = Mathf.SmoothDamp(a, closestX, ref Velocit1, smoothTime);
            b = Mathf.SmoothDamp(b, closestY, ref Velocit2, smoothTime);
            c = Mathf.SmoothDamp(c, closestZ, ref Velocit3, smoothTime);
            dd = Mathf.SmoothDamp(dd, closestW, ref Velocit4, smoothTime);

            mat.SetFloat("_a0", a);
            mat.SetFloat("_b0", 1-b);
            mat.SetFloat("_c0", c);
            mat.SetFloat("_d0", dd);
            script1.a = a;
            script1.b = b;
            script1.c = c;
            script1.d = dd;
        }

        markers.Clear();
    }

       
    
}
