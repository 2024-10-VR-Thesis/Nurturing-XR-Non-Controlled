using System;
using System.IO;
using UnityEngine;

public static class WavUtility
{
    public static int GetChannelCount(byte[] wavFile)
    {
        return BitConverter.ToInt16(wavFile, 22); // El número de canales está en el byte 22 del encabezado WAV
    }

    public static AudioClip ToAudioClip(byte[] wavFile, string name)
    {
        int headerOffset = 44; // Offset for WAV header
        int frequency = BitConverter.ToInt32(wavFile, 24);
        int channels = GetChannelCount(wavFile);

        if (channels > 2)
        {
            Debug.LogWarning($"Reducing number of channels from {channels} to 2 (stereo)");
            channels = 2; // Limit to stereo
        }

        int byteLength = wavFile.Length - headerOffset;
        float[] samples = new float[byteLength / 2];

        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = BitConverter.ToInt16(wavFile, headerOffset + i * 2) / 32768.0f;
        }

        AudioClip audioClip = AudioClip.Create(name, samples.Length / channels, channels, frequency, false);
        audioClip.SetData(samples, 0);

        return audioClip;
    }
}
