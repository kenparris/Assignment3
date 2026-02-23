// Unity Audio Spectrum data analysis
// IMDM Course Material 
// Author: Myungin Lee
using UnityEngine;

[RequireComponent (typeof(AudioSource))]

public class Spectrum : MonoBehaviour
{
    AudioSource source;
    public static int FFTSIZE = 4096; // https://en.wikipedia.org/wiki/Fast_Fourier_transform
    public static float[] samples = new float[FFTSIZE];
    public static float audioAmp = 0f;
    public static float bassAmp = 0f;
    public int displayBins = 100;
    public static int[] mappedMelBins;
    float nyquist, hzPerBin;
    void Start()
    {
        source = GetComponent<AudioSource>();      
        nyquist = AudioSettings.outputSampleRate * 0.5f;
        hzPerBin = nyquist / FFTSIZE;
        //displayBins = Reactive.numObjects;
        displayBins = Mathf.Clamp(displayBins, 16, FFTSIZE);
        mappedMelBins = new int[displayBins];
        
        float melMax = 2595f * Mathf.Log10(1f + nyquist / 700f);

        for (int i = 0; i < displayBins; i++)
        {
            float t = i / (float)(displayBins - 1); 
            float hz = 0f;

            if (t > 0f)
            {
                // Interpolate in mel space, then convert mel -> Hz.
                float melValue = Mathf.Lerp(0f, melMax, t);
                hz = 700f * (Mathf.Pow(10f, melValue / 2595f) - 1f);
            }

            mappedMelBins[i] = Mathf.Clamp(Mathf.RoundToInt(hz / hzPerBin), 0, FFTSIZE - 1);
        }
    }
    void Update()
    {
        // The source (time domain) transforms into samples in frequency domain 
        GetComponent<AudioSource>().GetSpectrumData(samples, 0, FFTWindow.Hanning);
        // Empty first, and pull down the value.
        audioAmp = 0f;
        for (int i = 0; i < FFTSIZE; i++)
        {
            audioAmp += samples[i];
        }

        bassAmp = 0f;
        for (int i = 0; i < FFTSIZE / 4; i++)
        {
            bassAmp += samples[i];
        }
    }

}
