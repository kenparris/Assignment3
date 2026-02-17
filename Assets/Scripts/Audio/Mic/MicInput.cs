using UnityEngine;
using System.Linq;

public class MicInput : MonoBehaviour
{
    public string selectedDevice;
    private AudioClip microphoneClip;
    private const int sampleSize = 1024;
    private float[] audioSamples = new float[sampleSize];

    public static float Amplitude { get; private set; }

    void Start()
    {
        // Check your audio device and replace it in the ***string.
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
        StartMicrophone();
    }

    void Update()
    {
        Amplitude = GetAmplitude() * 50f;
        Debug.Log(Amplitude);
    }

    void StartMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            selectedDevice = Microphone.devices[0]; // Use the first available microphone
            microphoneClip = Microphone.Start(selectedDevice, true, 1, AudioSettings.outputSampleRate);
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    float GetAmplitude()
    {
        if (microphoneClip == null) return 0f;

        int micPosition = Microphone.GetPosition(selectedDevice) - sampleSize;
        if (micPosition < 0) return 0f;

        microphoneClip.GetData(audioSamples, micPosition);
        return audioSamples.Select(Mathf.Abs).Average(); // Get average absolute amplitude
    }
}
