using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderActivation : MonoBehaviour
{
    public GameObject render0;
    public GameObject render1;
    public GameObject render2;
    private bool isActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isActive = !isActive; // Toggle the isActive flag
            render0.SetActive(isActive);
            render1.SetActive(isActive);
            render2.SetActive(isActive);
        }
    }
}
