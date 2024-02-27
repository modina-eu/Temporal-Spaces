using UnityEngine;
using System.Collections;
using TMPro;
using System;
using RenderHeads.Media.AVProLiveCamera;

public class computeParsing : MonoBehaviour
{
    public ComputeShader compute_shader;
    RenderTexture A;
    public Material material;   
    int handle_main;
    public float a;
    public float b;
    public float c;
    public float d;
    public modelData script1;
    public AVProLiveCamera script2;
    public int res = 128;
    void Start()
    {
        A = new RenderTexture(res, res, 0);
        A.enableRandomWrite = true;
        A.Create();      
        handle_main = compute_shader.FindKernel("CSMain");
        compute_shader.SetFloat("_resx", res);
        compute_shader.SetFloat("_resy", res);
    }

    void Update()
    {

        //compute_shader.SetTexture(handle_main, "reader", A);
        compute_shader.SetTexture(handle_main, "reader", script2.OutputTexture);
        compute_shader.SetTexture(handle_main, "writer", A);
        compute_shader.Dispatch(handle_main, A.width / 8, A.height / 8, 1);
        compute_shader.SetFloat("_a", a);
        compute_shader.SetFloat("_b", b);
        compute_shader.SetFloat("_c", c);
        compute_shader.SetFloat("_d", d);
        material.SetTexture("_MainTex", script2.OutputTexture);
        material.SetFloat("_a0", a);
        material.SetFloat("_b0", b);
        material.SetFloat("_c0", c);
        material.SetFloat("_d0", d);     
        material.SetTexture("_MainTex", A);
        script1.tex = A;
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
            A = new RenderTexture(64, 64, 0);
            A.enableRandomWrite = true;
            A.Create();
        }
      
    }

    }