// WavUtility.cs — drop into Assets/Voice
using UnityEngine;
using System;
using System.IO;

public static class WavUtility
{
    const int HEADER_SIZE = 44;

    public static void FromAudioClip(AudioClip clip, string filePath, bool trim = false)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            byte[] wav = ConvertAudioClipToWav(clip, trim);
            fileStream.Write(wav, 0, wav.Length);
        }
    }

    public static byte[] ConvertAudioClipToWav(AudioClip clip, bool trim)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        if (trim)
        {
            samples = TrimSilence(samples);
        }

        byte[] bytesData = ConvertTo16Bit(samples);
        byte[] header = GetHeader(clip, bytesData.Length);

        byte[] wav = new byte[header.Length + bytesData.Length];
        Buffer.BlockCopy(header, 0, wav, 0, header.Length);
        Buffer.BlockCopy(bytesData, 0, wav, header.Length, bytesData.Length);

        return wav;
    }

    private static byte[] GetHeader(AudioClip clip, int dataLength)
    {
        int hz = clip.frequency;
        int channels = clip.channels;
        int samples = dataLength / 2;

        MemoryStream stream = new MemoryStream(HEADER_SIZE);

        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
        writer.Write(dataLength + HEADER_SIZE - 8);
        writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
        writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((ushort)1);
        writer.Write((ushort)channels);
        writer.Write(hz);
        writer.Write(hz * channels * 2);
        writer.Write((ushort)(channels * 2));
        writer.Write((ushort)16);
        writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
        writer.Write(dataLength);

        return stream.ToArray();
    }

    private static byte[] ConvertTo16Bit(float[] samples)
    {
        short[] intData = new short[samples.Length];
        byte[] bytesData = new byte[samples.Length * 2];

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * short.MaxValue);
        }

        Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);
        return bytesData;
    }

    private static float[] TrimSilence(float[] samples, float threshold = 0.02f)
    {
        int start = 0;
        while (start < samples.Length && Mathf.Abs(samples[start]) < threshold) start++;

        int end = samples.Length - 1;
        while (end > start && Mathf.Abs(samples[end]) < threshold) end--;

        int trimmedLength = end - start + 1;
        float[] trimmed = new float[trimmedLength];
        Array.Copy(samples, start, trimmed, 0, trimmedLength);
        return trimmed;
    }
}
