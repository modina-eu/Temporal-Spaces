using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activation : MonoBehaviour

{
    public ComputeShader compute_shader;
    RenderTexture A;
    RenderTexture B;
    RenderTexture C;
    ComputeBuffer t3Buffer2;
    public float[] floatArray1 = new float[3];
    int handle_main;
    int handle_main2;
    int handle_t3;
    public Material mat;
    public Texture tex;
    public Material mat2;
    public Texture tex2;
    public Material mat3;
    public Texture tex3;

    void Start()
    {
        A = new RenderTexture(64, 64, 0);
        A.enableRandomWrite = true;
        A.Create();
        B = new RenderTexture(64, 64, 0);
        B.enableRandomWrite = true;
        B.Create();
        C = new RenderTexture(64, 64, 0);
        C.enableRandomWrite = true;
        C.Create();

        t3Buffer2 = new ComputeBuffer(3, sizeof(float));
        handle_main = compute_shader.FindKernel("CSMain");
        handle_t3 = compute_shader.FindKernel("CSMain3");
        handle_main2 = compute_shader.FindKernel("CSMain2");
    }

    void Update()
    {

        tex = mat.GetTexture("_MainTex");
        tex2 = mat2.GetTexture("_MainTex");
        tex3 = mat3.GetTexture("_MainTex");

        compute_shader.SetTexture(handle_main2, "reader2", tex);
        compute_shader.SetTexture(handle_main2, "reader3", tex2);
        compute_shader.SetTexture(handle_main2, "reader4", tex3);
        compute_shader.SetTexture(handle_main2, "writer", C);
        compute_shader.Dispatch(handle_main2, C.width / 8, C.height / 8, 1);    


        compute_shader.SetTexture(handle_main, "reader", A);
        compute_shader.SetTexture(handle_main, "reader2", C);
        compute_shader.SetFloat("_resx", 64);
        compute_shader.SetFloat("_resy", 64);
        compute_shader.SetTexture(handle_main, "writer", B);
        compute_shader.Dispatch(handle_main, B.width / 8, B.height / 8, 1);
        compute_shader.SetTexture(handle_main, "reader", B);
        compute_shader.SetTexture(handle_main, "writer", A);
        compute_shader.Dispatch(handle_main, B.width / 8, B.height / 8, 1);


        compute_shader.SetTexture(handle_t3, "reader", B);
        compute_shader.SetBuffer(handle_t3, "t3Buffer2", t3Buffer2);
        compute_shader.Dispatch(handle_t3, 4, 1, 1);

        float[] t3Data2 = new float[3]; ;
        t3Buffer2.GetData(t3Data2, 0, 0, 3);
        floatArray1 = t3Data2;
        /*material.SetFloat("_float1", floatArray1[0]);
        material.SetFloat("_float2", floatArray1[1]);
        material.SetFloat("_float3", floatArray1[2]);  */


      
    }
}