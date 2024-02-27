using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscSimpl.Examples;

public class CaptureActivation : MonoBehaviour
{
    public GameObject sc;
    public GettingStartedReceiving script;
    private bool isComputeRenderEnabled = false;
    public float stop;
    private bool startRWasPositive = false;
    private float stopTimerDuration = 1.0f; // Set the duration for which 'stop' remains at 1
    private float stopTimer;

    void Start()
    {

    }

    void Update()
    {
        if (script.startR > 0 && !isComputeRenderEnabled)
        {
            EnableComputeRender();
        }
        else if (script.startR == 0 && isComputeRenderEnabled)
        {
            DisableComputeRender();
        }

        // Reset the flag for the next frame
        startRWasPositive = script.startR > 0;

        // Check if 'stop' is temporarily set to 1 and the timer has exceeded the duration
        if (stop == 1 && Time.time > stopTimer)
        {
            // Reset 'stop' to 0
            stop = 0;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            //Debug.Log("C key pressed");
            sc.GetComponent<computeRender>().enabled = true;
          //  sc.GetComponent<computeRender>().startTime = Time.frameCount;
         //   sc.GetComponent<computeRender>().startTime2 = Time.time;
        }
    }
    void EnableComputeRender()
    {
        sc.GetComponent<computeRender>().enabled = true;
        isComputeRenderEnabled = true;
     //   sc.GetComponent<computeRender>().startTime = Time.frameCount;
     //   sc.GetComponent<computeRender>().startTime2 = Time.time;
    }

    void DisableComputeRender()
    {
        sc.GetComponent<computeRender>().enabled = false;
        isComputeRenderEnabled = false;

        // Check if 'startR' transitioned from positive to 0 in the current frame
        if (startRWasPositive)
        {
            // Set 'stop' to 1 for one frame
            stop = 1;

            // Start the timer
            stopTimer = Time.time + stopTimerDuration;
        }
    }
}
