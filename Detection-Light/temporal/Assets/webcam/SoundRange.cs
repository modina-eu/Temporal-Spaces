using UnityEngine;
using UnityEngine.Audio;

public class SoundRange : MonoBehaviour
{
    public AudioMixerGroup outputMixerGroup = null;
    public float sensitivity = 1.0f;
    public int microphoneIndex = 0;
    public float[] volumeLevels = new float[7]; // Array to store the 7 volume levels
    public float[] volumeAccumulations = new float[7]; // Array to store the 7 volume accumulations
    private AudioSource _audioSource;
    private float[] _spectrumData;

    public Material mat;

    void Start()
    {
        string[] microphones = Microphone.devices;
        if (microphones.Length > 0 && microphoneIndex < microphones.Length)
        {
            string microphoneDevice = microphones[microphoneIndex];
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.outputAudioMixerGroup = outputMixerGroup;
            _audioSource.loop = true;
            _audioSource.clip = Microphone.Start(microphoneDevice, true, 1, AudioSettings.outputSampleRate);
            //_audioSource.mute = true; // Mute the audio playback
            _audioSource.volume = 0.001f;
            _audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No microphone found or microphone index out of range.");
        }
        _spectrumData = new float[1024]; // Set the size of the spectrum data buffer
    }

    void Update()
    {
        float[] samples = new float[_audioSource.clip.samples];
        _audioSource.clip.GetData(samples, 0);
        float sum = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += Mathf.Abs(samples[i]);
        }
        float average = sum / samples.Length;
        float normalizedVolume = Mathf.Clamp01(average * sensitivity);
        _audioSource.GetSpectrumData(_spectrumData, 0, FFTWindow.BlackmanHarris);

        // Calculate volume levels and accumulations for each range
        for (int i = 0; i < 7; i++)
        {
            int rangeStart = Mathf.FloorToInt(_spectrumData.Length * (i / 7f));
            int rangeEnd = Mathf.FloorToInt(_spectrumData.Length * ((i + 1) / 7f));
            float rangeSum = 0f;
            for (int j = rangeStart; j < rangeEnd; j++) // Updated loop condition
            {
                if (j >= 0 && j < _spectrumData.Length) // Added index boundary check
                {
                    rangeSum += _spectrumData[j];
                }
            }
            volumeLevels[i] = rangeSum * sensitivity*100;
            volumeAccumulations[i] += volumeLevels[i] ;
        }

        // Set material properties
        for (int i = 0; i < 7; i++)
        {
            int t = i*15 + 1;
            string volumeLevelPropertyName = string.Format("ti{0}", i + 1);
            string volumeAccumulationPropertyName = string.Format("_c{0}", i + 1);
            mat.SetFloat(volumeLevelPropertyName, volumeLevels[i]);
            mat.SetFloat(volumeAccumulationPropertyName, volumeAccumulations[i]*t);
        }
    }
}