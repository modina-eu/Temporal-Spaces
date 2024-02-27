using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramedWebcam : MonoBehaviour
{
    public Material mat;
    public Texture tex;
    public Material mat2;
    public Texture tex2;
    public Material mat3;
    public Texture tex3;
    public Material matFinal;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tex = mat.GetTexture("_MainTex");
        tex2 = mat2.GetTexture("_MainTex");
        tex3 = mat3.GetTexture("_MainTex");
        matFinal.SetTexture("_MainTex", tex);
        matFinal.SetTexture("_MainTex2", tex2);
        matFinal.SetTexture("_MainTex3", tex3);
    }
}
