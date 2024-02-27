using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.TestTools;
public class webcamtransfer : MonoBehaviour
{
    public ImageSource webcam;
    public Material mat;
    void Start()
    {
        mat.SetTexture("_MainTex", webcam.Texture);
    }

    
    void Update()
    {
        
    }
}
