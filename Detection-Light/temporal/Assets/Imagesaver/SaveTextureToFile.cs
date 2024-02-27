using UnityEngine;
using UnityEditor;
//using Unity.Barracuda;
using OscSimpl.Examples;
using System.IO;
//using RenderHeads.Media.AVProLiveCamera;
public class SaveTextureToFile : MonoBehaviour
{
    public RenderTexture Tex;
    public GettingStartedReceiving osc;
    public computeRender compute;
    public TextureComparator resnet;
    public int captureCounter = 0;
    public float frame;
    private float previousFrame = 0.0f;
    private float previousPhase2 = 0.0f;
    private float previousPhaseR = 1.0f;
    private bool textureSavedThisFrame ;
    void Start()
    {

    }
    private void Update()
    {
        float phase2 = osc.Phase2;
        
        if (phase2 == 0 && previousPhase2 == 1)
        {
            compute.enabled = true;
            compute.startTime2 = Time.time;
        }
        previousPhase2 = phase2;
        if (phase2 == 1 && previousPhaseR == 0)
        {
            resnet.enabled = true;
        }
        
        if (frame > previousFrame && Mathf.Floor(frame) > Mathf.Floor(previousFrame))
        {
            SaveRTToFile(Tex,(int)osc.NbrR);
            compute.enabled = false;
            captureCounter++;
        }
        else if (phase2 == 1 && previousPhaseR == 0 && frame < previousFrame && Mathf.Floor(frame) < Mathf.Floor(previousFrame))
        {
            SaveRTToFile(Tex, (int)osc.NbrR);
            compute.enabled = false;
            captureCounter++;
        }
        previousFrame = frame;
        previousPhaseR = phase2;
 
    }
    public static void SaveRTToFile(RenderTexture rt,int captureCounter)
    {

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;

        byte[] bytes;
        bytes = tex.EncodeToPNG();
        Object.Destroy(tex);
        string counterString = captureCounter.ToString("0000");
        // string path = "//MSI/Index/64Img" + "/capture" +  captureCounter + ".png";
        string path = "D:/GIT/TemporalSpace/temporal/Assets/StreamingAssets/Capture" + "/capture"+ counterString + ".png";
        File.WriteAllBytes(path, bytes);
    }

}