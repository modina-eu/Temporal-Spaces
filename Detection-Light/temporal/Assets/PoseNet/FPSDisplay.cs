using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;
   
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(10, 10, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);


        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        string fpsText = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        //string ti2Text = "Capture Time: " + script.ti2.ToString();
        string labelText = fpsText + "\n" ;

        GUI.Label(rect, labelText, style);

    }
}