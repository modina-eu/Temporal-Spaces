using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class webcam : MonoBehaviour
{
    public Material mat;
    public ComputeShader compute_shader;
    RenderTexture A;
    int handle_main;
   /* private WebCamTexture webcamTexture1;
    private WebCamTexture webcamTexture2;
    private WebCamTexture webcamTexture3; */
    public int desiredWidth = 1920; 
    public int desiredHeight = 1080;
    public Material mat1;
    public Texture tex1;
    public Material mat2;
    public Texture tex2;
    public Material mat3;
    public Texture tex3;
    void Start()
    {

        WebCamDevice[] devices = WebCamTexture.devices;

        // for debugging purposes, prints available devices to the console
       /* for (int i = 0; i < devices.Length; i++)
        {
            print("Webcam available: " + devices[i].name);
        }       */

       /* webcamTexture1 = new WebCamTexture("USB2.0 PC CAMERA"); 
        webcamTexture1.Play(); 
       // mat.mainTexture = webcamTexture1;    

        webcamTexture2 = new WebCamTexture("HD Pro Webcam C920");
        webcamTexture2.requestedWidth = desiredWidth; 
        webcamTexture2.requestedHeight = desiredHeight;
        webcamTexture2.Play(); 
     
       // mat.SetTexture("_MainTex2", webcamTexture2);

        webcamTexture3 = new WebCamTexture("Trust Webcam"); 
        webcamTexture3.requestedWidth = desiredWidth; 
        webcamTexture3.requestedHeight = desiredHeight;
        webcamTexture3.Play(); 
        //mat.SetTexture("_MainTex3", webcamTexture3);    */

        A = new RenderTexture(desiredWidth, desiredHeight, 0);
        A.enableRandomWrite = true;
        A.Create();
        //A.filterMode = FilterMode.Point;

        handle_main = compute_shader.FindKernel("CSMain");

        compute_shader.SetFloat("_resx", desiredWidth);
        compute_shader.SetFloat("_resy", desiredHeight);
        mat.SetTexture("_MainTex4", A);
        tex1 = mat1.GetTexture("_MainTex");
        tex2 = mat2.GetTexture("_MainTex");
        tex3 = mat3.GetTexture("_MainTex");
    }
    void Update()
    {

        
        compute_shader.SetTexture(handle_main, "reader1", tex2);
        compute_shader.SetTexture(handle_main, "reader2", tex1);
        compute_shader.SetTexture(handle_main, "reader3", tex3);
        compute_shader.SetTexture(handle_main, "writer", A);
        compute_shader.Dispatch(handle_main, A.width / 8, A.height / 8, 1);

     
    }
   /* private void OnDisable()
    {

        CleanupResources();
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
            A = new RenderTexture(1920, 1080, 0);
            A.enableRandomWrite = true;
            A.Create();
        }
    }  */
    }
