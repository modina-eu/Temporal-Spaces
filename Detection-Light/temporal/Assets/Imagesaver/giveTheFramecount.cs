using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class giveTheFramecount : MonoBehaviour
{
    public Material mat;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetFloat("_time", Time.frameCount);
    }
}
