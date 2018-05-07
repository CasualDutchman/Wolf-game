using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Noise))]
public class EditorNoise : Editor {

    Noise noise;

    Texture2D debugTex;

    void OnEnable() {
        noise = (Noise)target;

        UpdateDebugInfo();
    }

    void OnDisable() {
        debugTex = null;
    }

    public override void OnInspectorGUI() { 

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck()) {
            UpdateDebugInfo();
        }

        if (debugTex != null) {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(debugTex);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }
    }

    void UpdateDebugInfo() {
        int size = 48;
        int scale = 4;

        if (debugTex == null) {
            debugTex = new Texture2D(size * scale, size * scale) {
                filterMode = FilterMode.Point
            };
        }

        bool[,] on = new bool[size, size];

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                on[x, y] = noise.IsOn(x, y);
            }
        }

        Color[] colorss = new Color[(size * scale) * (size * scale)];
        for (int y = 0; y < size * scale; y++) {
            for (int x = 0; x < size * scale; x++) {
                bool id = on[Mathf.FloorToInt(x / (float)scale), Mathf.FloorToInt(y / (float)scale)];
                Color col = new Color(id ? 1 : 0, 0, 0);
                colorss[(y * (size * scale)) + x] = col;
            }
        }

        debugTex.SetPixels(colorss);
        debugTex.Apply();
    }
}
