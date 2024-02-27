using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

public class TextureComparator2 : MonoBehaviour
{
    public NNModel modelFile; // Reference to the model file in Unity
    private Model model;
    private IWorker worker;

    public Texture2D[] textures; // Array of textures to compare
    private List<float[]> textureFeatures; // List to store feature vectors for each texture

    public Texture2D inputTexture; // The input texture to compare against others

    void Start()
    {
        // Load the model
        model = ModelLoader.Load(modelFile);

        // Create a worker to execute the model
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, model);

        // Initialize list to store texture features
        textureFeatures = new List<float[]>();

        // Extract features from each texture
        foreach (Texture2D texture in textures)
        {
            // Preprocess texture and extract features
            float[] features = ExtractFeatures(texture);

            // Store feature vector
            textureFeatures.Add(features);
        }

        // Compare input texture with others and find the closest match
        FindClosestMatch();
    }

    void OnDestroy()
    {
        // Dispose of the worker when it's no longer needed
        worker.Dispose();
    }

    private float[] ExtractFeatures(Texture2D texture)
    {
        // Convert texture to tensor
        Tensor inputTensor = new Tensor(texture, channels: 3); // Assuming RGB input

        // Execute the model with the input tensor
        worker.Execute(inputTensor);

        // Get the output tensor without specifying the output name
        Tensor outputTensor = worker.PeekOutput();

        // Convert tensor data to float array
        float[] features = outputTensor.ToReadOnlyArray();

        // Dispose of tensors
        inputTensor.Dispose();
        outputTensor.Dispose();

        return features;
    }

    private void FindClosestMatch()
    {
        // Convert input texture to feature vector
        float[] inputFeatures = ExtractFeatures(inputTexture);

        // Compare input texture features with others and find closest match
        float maxSimilarity = -1f;
        int closestTextureIndex = -1;

        for (int i = 0; i < textureFeatures.Count; i++)
        {
            // Compute similarity (Cosine similarity)
            float similarity = CosineSimilarity(inputFeatures, textureFeatures[i]);

            // Update closest match
            if (similarity > maxSimilarity)
            {
                maxSimilarity = similarity;
                closestTextureIndex = i;
            }
        }

        // Output closest match
        Debug.Log("Closest texture match: " + closestTextureIndex);
        Debug.Log("Similarity score: " + maxSimilarity);
    }

    private float CosineSimilarity(float[] embedding1, float[] embedding2)
    {
        // Compute cosine similarity between two embeddings
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

}
