using UnityEngine;
using System.Collections;
using TMPro;
using System;
//using RenderHeads.Media.AVProLiveCamera;

public class computeRenderVid : MonoBehaviour
{
    public ComputeShader compute_shader;    
    public Material preview;
    public Material final;
    RenderTexture A;
    RenderTexture B;
    int handle_main;
    public PoseEstimatorVid1 script;
    public int resx = 128;
    public int resy = 128;
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
    private Vector2 p0;
    private Vector2 p1;
    private Vector2 p2;
    private Vector2 p3;
    private Vector2 p4;
    private Vector2 p5;
    private Vector2 p6;
    private Vector2 p8;
    private Vector2 p9;
    private Vector2 p10;
    private Vector2 p11;
    public float pr;
    private Vector2 a = new Vector2(0.5f, 0.5f);
    public SaveTextureToFile capture;
    public float ti;
    void Start()
    {
        // A = new RenderTexture(resx, resy, 0, RenderTextureFormat.ARGBFloat);
        A = new RenderTexture(resx, resy, 0);
        A.enableRandomWrite = true;
        A.filterMode = FilterMode.Point;
        A.Create();        
        B = new RenderTexture(resx, resy, 0);
        B.enableRandomWrite = true;
        B.filterMode = FilterMode.Point;
        B.Create();
        handle_main = compute_shader.FindKernel("CSMain");
        compute_shader.SetFloat("_resx", resx);
        compute_shader.SetFloat("_resy", resy);
        final.SetFloat("_resx2", resx);
        final.SetFloat("_resy2", resy);
        
    }

    void Update()
    {
        ti = Time.frameCount;
        //if (pos.x == 0) { ti = Time.frameCount; }
        //float tt = Time.frameCount;
        //"nose", "leftShoulder", "rightShoulder", "leftElbow", "rightElbow", "leftWrist", "rightWrist", 
        //"leftHip", "rightHip", "leftKnee", "rightKnee", "leftAnkle", "rightAnkle"
        compute_shader.SetTexture(handle_main, "reader", A);
        /*compute_shader.SetVector("_pos0", script.pos0);
        compute_shader.SetVector("_pos1", script.pos1);
        compute_shader.SetVector("_pos2", script.pos2);
        compute_shader.SetVector("_pos3", script.pos3);
        compute_shader.SetVector("_pos4", script.pos4);
        compute_shader.SetVector("_pos5", script.pos5);
        compute_shader.SetVector("_pos6", script.pos6);
        compute_shader.SetVector("_pos7", script.pos7);
        compute_shader.SetVector("_pos8", script.pos8);
        compute_shader.SetVector("_pos9", script.pos9);
        compute_shader.SetVector("_pos10", script.pos10);
        compute_shader.SetVector("_pos11", script.pos11);  */
        compute_shader.SetFloat("_time", ti);
        compute_shader.SetTexture(handle_main, "writer", B);
        compute_shader.Dispatch(handle_main, B.width / 8, B.height / 8, 1);
        compute_shader.SetTexture(handle_main, "reader", B);
        compute_shader.SetTexture(handle_main, "writer", A);
        compute_shader.Dispatch(handle_main, B.width / 8, B.height / 8, 1);
        capture.frame = (ti + 1) / (resx * Mathf.Floor(resy / 12));
        capture.Tex = B;
        preview.SetTexture("_MainTex", B);
        final.SetTexture("_PosTex", B);
        final.SetFloat("_time", ti);
 
    }

    private void CleanupResources()
    {
        // Destroy the RenderTexture
        if (A != null)
        {
            Destroy(A);
        }
      
    }
    private void OnEnable()
    {

        if (A == null)
        {
            A = new RenderTexture(resx, resy, 0);
            A.enableRandomWrite = true;
            A.Create();
        }
      
    }

    }