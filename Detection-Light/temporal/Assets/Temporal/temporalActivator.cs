using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProLiveCamera;
using UnityEngine.SceneManagement;
public class temporalActivator : MonoBehaviour
{
    public float scriptTime;
    public float OscTime;
   
    public temporalSetUp script;
    public GameObject OSC;
    public AVProLiveCamera cam1;
    public AVProLiveCamera cam2;
    //public AVProLiveCamera cam3;
    public float restartTime;
    public GameObject object1;
    public GameObject object2;
    void Start()
    {
        restartTime = Time.time;
    }
           

    void Update()
    {
        if (Time.time - restartTime > scriptTime)
        {
            script.enabled = true;
        }
        if (Time.time - restartTime > OscTime)
        {
            OSC.SetActive(true);
            script.enabled = false;
            cam1._updateSettings = false;
            cam2._updateSettings = false;
            //cam3._updateSettings = false;

        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleObjects();
        }
    }
    void ToggleObjects()
    {
        // Toggle the active state of object1 and object2
        object1.SetActive(!object1.activeSelf);
        object2.SetActive(!object2.activeSelf);
    }
}