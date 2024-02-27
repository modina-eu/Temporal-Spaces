using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using System.IO;

public class TextureComparator : MonoBehaviour
{
    public NNModel modelFile;
    private Model model;
    private IWorker worker;
    private List<float[]> textureFeatures;
    public float result;
    public float score;
    public RenderTexture B;
    private string[] imageFiles; // Array to store image file paths

    void OnEnable()
    {
        model = ModelLoader.Load(modelFile);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, model);
        textureFeatures = new List<float[]>();
        LoadTexturesFromStreamingAssets();
        FindClosestMatch();
    }

    void OnDisable()
    {
        worker.Dispose();
    }

    private void LoadTexturesFromStreamingAssets()
    {
        string streamingAssetsPath = Application.streamingAssetsPath;
        string captureFolderPath = Path.Combine(streamingAssetsPath, "Capture");
        if (Directory.Exists(captureFolderPath))
        {
            // Find all PNG files in the Capture folder
            imageFiles = Directory.GetFiles(captureFolderPath, "*.png");

            // Load textures and extract features
            foreach (string imageFile in imageFiles)
            {
                Texture2D texture = LoadTextureFromFile(imageFile);
                if (texture != null)
                {
                    float[] features = ExtractFeatures(texture);
                    textureFeatures.Add(features);
                }
            }
        }
        else
        {
            Debug.LogError("Capture folder not found in StreamingAssets folder: " + captureFolderPath);
        }
    }

    private Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2); // Adjust size as needed
        if (texture.LoadImage(fileData))
        {
            return texture;
        }
        return null;
    }

    private float[] ExtractFeatures(Texture2D texture)
    {
        Tensor inputTensor = new Tensor(texture, channels: 3);
        worker.Execute(inputTensor);
        Tensor outputTensor = worker.PeekOutput();
        float[] features = outputTensor.ToReadOnlyArray();
        inputTensor.Dispose();
        outputTensor.Dispose();
        return features;
    }

    private void FindClosestMatch()
    {
        // Convert the input render texture to a Texture2D
        Texture2D inputTexture = RenderTextureToTexture2D(B);
        // Extract features from the input texture
        float[] inputFeatures = ExtractFeatures(inputTexture);
        // Initialize variables to track the closest texture number and similarity score
        float maxSimilarity = -1f;
        int closestTextureNumber = 0;

        // Loop through the list of texture features
        for (int i = 0; i < textureFeatures.Count-1; i++)
        {
            // Extract the texture number from the filename
            string filename = Path.GetFileNameWithoutExtension(imageFiles[i]);
            string numberPart = filename.Substring("capture".Length);
            int textureNumber;
            // Try parsing the extracted number part
            if (int.TryParse(numberPart, out textureNumber))
            {
                // Calculate the similarity between the input texture features and the current texture features
                float similarity = CosineSimilarity(inputFeatures, textureFeatures[i]);
                // Update the closest texture number and maximum similarity if the similarity is higher
                if (similarity > maxSimilarity)
                {
                    maxSimilarity = similarity;
                    closestTextureNumber = textureNumber;
                }
            }
        }

        // Log the closest texture match and its similarity score
        Debug.Log("Closest texture match: " + closestTextureNumber);
        Debug.Log("Similarity score: " + maxSimilarity);

        // Assign the closest texture number and similarity score to public variables
        result = closestTextureNumber;
        score = maxSimilarity;

        // Disable the script to prevent further comparisons until re-enabled
        enabled = false;
    }

    private float CosineSimilarity(float[] embedding1, float[] embedding2)
    {
        float dotProduct = 0f;
        float norm1 = 0f;
        float norm2 = 0f;
        for (int i = 0; i < embedding1.Length; i++)
        {
            dotProduct += embedding1[i] * embedding2[i];
            norm1 += Mathf.Pow(embedding1[i], 2);
            norm2 += Mathf.Pow(embedding2[i], 2);
        }
        norm1 = Mathf.Sqrt(norm1);
        norm2 = Mathf.Sqrt(norm2);
        float similarity = dotProduct / (norm1 * norm2);
        return similarity;
    }

    private Texture2D RenderTextureToTexture2D(RenderTexture renderTexture)
    {
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        return texture;
    }
}
