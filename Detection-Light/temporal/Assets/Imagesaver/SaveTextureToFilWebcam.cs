using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//using Unity.Barracuda;
using OscSimpl.Examples;
using System.IO;
using RenderHeads.Media.AVProLiveCamera;
public class SaveTextureToFileWebcam : MonoBehaviour
{
    public RenderTexture Tex;
    public int captureCounter = 1;
    public float frame;
    private float previousFrame = 0.0f;
    public GettingStartedReceiving script;
    public AVProLiveCamera script2;
    void Start()
    {
        Tex = new RenderTexture(1920, 1080, 0);
    }
    private void Update()
    {
        
            SaveRTToFile(Tex, captureCounter, script2);
            captureCounter++;
          
    }
    public static void SaveRTToFile(RenderTexture rt,int captureCounter, AVProLiveCamera script2)
    {
        Graphics.Blit(script2.OutputTexture, rt);
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;

        byte[] bytes;
        bytes = tex.EncodeToPNG();
        Object.Destroy(tex);

        string path = "D:/GIT/TemporalSpace/Captures" + "/capture" +  captureCounter + ".png";
        File.WriteAllBytes(path, bytes);
    }

}