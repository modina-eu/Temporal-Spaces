using UnityEngine;

public class SoundRangeController : MonoBehaviour
{
    public Material mat; // Reference to the material that uses the SoundRangeShader
    public float[] volumeLevels; // Array of volume levels for each sound source
    public float[] volumeAccumulations; // Array of volume accumulations for each sound source

    void Update()
    {
        // Update volume levels and accumulations in shader
        for (int i = 0; i < volumeLevels.Length; i++)
        {
            string volumeLevelPropertyName = string.Format("ti{0}", i + 1);
            string volumeAccumulationPropertyName = string.Format("_ta{0}", i + 1);
            mat.SetFloat(volumeLevelPropertyName, volumeLevels[i]);
            mat.SetFloat(volumeAccumulationPropertyName, volumeAccumulations[i]);
        }
    }

    // Mute the sound by setting volume levels and accumulations to zero
    public void MuteSound()
    {
        for (int i = 0; i < volumeLevels.Length; i++)
        {
            volumeLevels[i] = 0;
            volumeAccumulations[i] = 0;
        }
    }
}