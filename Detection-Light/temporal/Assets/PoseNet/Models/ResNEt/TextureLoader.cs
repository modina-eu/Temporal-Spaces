using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextureLoader : MonoBehaviour
{
    // Path to the StreamingAssets folder
    private string streamingAssetsPath;

    // Start is called before the first frame update
    void Start()
    {
        // Get the path to the StreamingAssets folder for the current platform
        streamingAssetsPath = Application.streamingAssetsPath;

        // Load textures from the StreamingAssets folder
        LoadTextures();
    }

    // Load textures from the StreamingAssets folder
    void LoadTextures()
    {
        // Get the file paths of all PNG files in the StreamingAssets/Capture folder
        string captureFolderPath = Path.Combine(streamingAssetsPath, "Capture");
        string[] imageFiles = Directory.GetFiles(captureFolderPath, "*.png");

        // Iterate through each image file and load its texture
        foreach (string filePath in imageFiles)
        {
            // Read the bytes from the file
            byte[] fileData = File.ReadAllBytes(filePath);

            // Create a new texture and load the image data into it
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            // Ensure that the texture is readable
            texture.Apply();

            // Access pixel data from the texture if it's readable
            Color[] pixels = texture.GetPixels();

            // Do something with the pixel data if needed
            Debug.Log("Loaded texture: " + Path.GetFileName(filePath) + ", Pixels count: " + pixels.Length);
        }
    }
}
