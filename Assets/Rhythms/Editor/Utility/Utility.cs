using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System;

namespace RhythmEditor
{
    public struct Inset
    {
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;

        public Inset(float left, float right, float top, float bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
    }

    public static class Utility
    {
        public static Texture2D GetWaveformTextureFromAudioClip(AudioClip audio, int width, int height, Color waveformColor, Color backgroundColor)
        {
            width = Mathf.Min(SystemInfo.maxTextureSize, width);
            height = Mathf.Min(SystemInfo.maxTextureSize, height);

            Texture2D waveformTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float[] samples = new float[audio.samples * audio.channels];
            float[] waveform = new float[width];

            audio.GetData(samples, 0);

            int packSize = (samples.Length / width) + 1;
            int waveIndex = 0;

            Debug.Log("Texture packsize: " + packSize);

            for (int i = 0; i < samples.Length - packSize; i += packSize)
            {
                float average = 0f;
                for (int j = i; j < i + packSize; j++)
                {
                    average += Mathf.Abs(samples[j]);
                }
                average /= packSize;

                waveform[waveIndex] = Mathf.Abs(average);
                waveIndex++;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    waveformTex.SetPixel(x, y, backgroundColor);
                }
            }

            for (int x = 0; x < waveform.Length; x++)
            {
                for (int y = 0; y <= waveform[x] * ((float)height); y++)
                {
                    waveformTex.SetPixel(x, (height / 2) + y, waveformColor);
                    waveformTex.SetPixel(x, (height / 2) - y, waveformColor);
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

        public static void DrawShadowRect(Rect rect, Inset inset, int pixelWidth, Color shadowColor)
        {
            Rect nextRect = rect;
            Color newColor = shadowColor;

            //UPPER EDGE
            for (int i = pixelWidth; i > 0; i--)
            {
                nextRect.y = rect.y + (pixelWidth - i) + inset.Top;
                nextRect.height = 1f;
                nextRect.width = rect.width - inset.Left - inset.Right;
                nextRect.x = rect.x + inset.Left;

                newColor.a = ((float)(i) / (float)pixelWidth) * shadowColor.a;

                EditorGUI.DrawRect(nextRect, newColor);
            }

            nextRect = rect;
            newColor = shadowColor;

            //LOWER EDGE
            for (int i = pixelWidth; i > 0; i--)
            {
                nextRect.y = rect.y + rect.height + (i - pixelWidth) - inset.Bottom;
                nextRect.height = 1f;
                nextRect.width = rect.width - inset.Left - inset.Right;
                nextRect.x = rect.x + inset.Left;

                newColor.a = ((float)(i) / (float)pixelWidth) * shadowColor.a;

                EditorGUI.DrawRect(nextRect, newColor);
            }

            nextRect = rect;
            newColor = shadowColor;

            //LEFT EDGE
            for (int i = pixelWidth; i > 0; i--)
            {
                nextRect.x = rect.x + pixelWidth - i + inset.Left;
                nextRect.width = 1f;
                nextRect.height = rect.height - inset.Top - inset.Bottom;
                nextRect.y = rect.y + inset.Top;

                newColor.a = ((float)(i) / (float)pixelWidth) * shadowColor.a;

                EditorGUI.DrawRect(nextRect, newColor);
            }

            nextRect = rect;
            newColor = shadowColor;

            //RIGHT EDGE
            for (int i = pixelWidth; i > 0; i--)
            {
                nextRect.x = rect.x + rect.width + (i - pixelWidth) - inset.Right;
                nextRect.width = 1f;
                nextRect.height = rect.height - inset.Top - inset.Bottom;
                nextRect.y = rect.y + inset.Top;

                newColor.a = ((float)(i) / (float)pixelWidth) * shadowColor.a;

                EditorGUI.DrawRect(nextRect, newColor);
            }
        }

        public static List<System.Type> GetAllSubclassesOf(System.Type baseType)
        {
            return Assembly.GetAssembly(baseType).GetTypes().Where(type => type.IsSubclassOf(baseType)).ToList();
        }

        public static void DrawGUILine(Color color, int height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, color);
        }

        public static void DrawRectOutline(Rect rect, int width, Color color)
        {
            //UPPER
            Rect changes = rect;

            changes.height = width;

            EditorGUI.DrawRect(changes, color);

            changes.y = rect.y + rect.height;

            EditorGUI.DrawRect(changes, color);

            changes = rect;

            changes.width = width;

            EditorGUI.DrawRect(changes, color);

            changes.x = rect.width;
            EditorGUI.DrawRect(changes, color);
        }

        public static List<string> GetVariableList()
        {
            System.Array enumArray = System.Enum.GetValues(typeof(Rhythm.VariableType));
            List<string> result = new List<string>();
            foreach (var variableType in enumArray)
            {
                result.Add(variableType.ToString()); 
            }

            return result;
        }

        public static TAttribute GetAttributes<TAttribute>(this SerializedProperty prop) where TAttribute : Attribute
        {
            if (prop == null)
                return null;

            Type propType = prop.serializedObject.targetObject.GetType();
            if (propType == null)
                return null;

            foreach (string pathSegment in prop.propertyPath.Split('.'))
            {
                FieldInfo fieldInfo = propType.GetField(pathSegment, (BindingFlags)(-1));
                if (fieldInfo != null)
                    return fieldInfo.GetCustomAttribute<TAttribute>(false);

                PropertyInfo propInfo = propType.GetProperty(pathSegment, (BindingFlags)(-1));
                if (propInfo != null)
                    return propInfo.GetCustomAttribute<TAttribute>(false);
            }

            return null;
        }
    }
}