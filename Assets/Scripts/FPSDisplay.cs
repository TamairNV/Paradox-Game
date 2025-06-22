using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;

    void Update()
    {
        // Calculate the time between frames
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int width = Screen.width, height = Screen.height;

        // Set up the style for the text
        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(400, 0, width, height * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = height * 2 / 50;
        style.normal.textColor = Color.white;

        // Calculate FPS
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        // Display the FPS and frame time
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}