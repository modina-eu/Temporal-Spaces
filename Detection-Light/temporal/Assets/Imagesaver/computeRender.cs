using UnityEngine;
using System.Collections;
using TMPro;
using System;
public class computeRender : MonoBehaviour
{
    public ComputeShader compute_shader;    
    public Material preview;
    RenderTexture A;
    RenderTexture B;
    int handle_main;
    public PoseEstimator1 script;
    public int resx = 128;
    public int resy = 128;
    public SaveTextureToFile capture;
    public TextureComparator Resnet;
    public float ti2;
    public float startTime2;
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
    }
    void Update()
    {

        ti2 = Time.time - startTime2;
        float ti3 = Mathf.Floor(ti2 * 30);
        compute_shader.SetTexture(handle_main, "reader", A);
        compute_shader.SetVector("_pos0", new Vector3(script.pos1[0].x, script.pos1[0].y, script.score1[0]));
        compute_shader.SetVector("_pos1", new Vector3(script.pos1[1].x, script.pos1[1].y, script.score1[1]));
        compute_shader.SetVector("_pos2", new Vector3(script.pos1[2].x, script.pos1[2].y, script.score1[2]));
        compute_shader.SetVector("_pos3", new Vector3(script.pos1[3].x, script.pos1[3].y, script.score1[3]));
        compute_shader.SetVector("_pos4", new Vector3(script.pos1[4].x, script.pos1[4].y, script.score1[4]));
        compute_shader.SetVector("_pos5", new Vector3(script.pos1[5].x, script.pos1[5].y, script.score1[5]));
        compute_shader.SetVector("_pos6", new Vector3(script.pos1[6].x, script.pos1[6].y, script.score1[6]));
        compute_shader.SetVector("_pos7", new Vector3(script.pos1[7].x, script.pos1[7].y, script.score1[7]));
        compute_shader.SetVector("_pos8", new Vector3(script.pos1[8].x, script.pos1[8].y, script.score1[8]));
        compute_shader.SetVector("_pos9", new Vector3(script.pos1[9].x, script.pos1[9].y, script.score1[9]));
        compute_shader.SetVector("_pos10", new Vector3(script.pos1[10].x, script.pos1[10].y, script.score1[10]));
        compute_shader.SetVector("_pos11", new Vector3(script.pos1[11].x, script.pos1[11].y, script.score1[11])); 
        compute_shader.SetFloat("_time", ti3);
       compute_shader.SetTexture(handle_main, "writer", B);
       compute_shader.Dispatch(handle_main, B.width / 8, B.height / 8, 1);
       compute_shader.SetTexture(handle_main, "reader", B);
       compute_shader.SetTexture(handle_main, "writer", A);
       compute_shader.Dispatch(handle_main, B.width / 8, B.height / 8, 1);
       capture.frame = (ti3 + 1) / (resx * Mathf.Floor(resy / 12));
       capture.Tex = B;
        Resnet.B = B;
       preview.SetTexture("_MainTex", B);

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
     