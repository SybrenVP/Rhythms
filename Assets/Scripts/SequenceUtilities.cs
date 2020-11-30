using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SequenceUtilities
{
    public static Texture2D GetWaveformTextureFromAudioClip(AudioClip audio, float saturation, int width, int height, Color color)
    {
        Texture2D waveformTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        float[] samples = new float[audio.samples * audio.channels];
        float[] waveform = new float[width];

        audio.GetData(samples, 0);

        int packSize = (samples.Length / width) + 1;
        int waveIndex = 0;
        for(int i = 0; i < samples.Length - packSize; i += packSize)
        {
            float average = 0f;
            for(int j = i; j < i + packSize; j++)
            {
                average += Mathf.Abs(samples[j]);
            }
            average /= packSize;

            waveform[waveIndex] = Mathf.Abs(average);
            waveIndex++;
        }

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                waveformTex.SetPixel(x, y, Color.black);
            }
        }

        for(int x = 0; x < waveform.Length; x++)
        {
            for(int y = 0; y <= waveform[x] * ((float)height); y++)
            {
                waveformTex.SetPixel(x, (height / 2) + y, color);
                waveformTex.SetPixel(x, (height / 2) - y, color);
            }
        }

        waveformTex.Apply();

        return waveformTex;
    }

    public static Texture2D GetTexture(int width, int height, Color color)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(x, y, color);
            }
        }
        tex.Apply();

        return tex;
    }

    public static void DrawGUILine(int height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, height);
        rect.height = height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}
