using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PerlinSettings))]
public class EditorPerlinSettings : Editor {

    PerlinSettings settings;

    Texture2D debugTex;

    void OnEnable() {
        settings = (PerlinSettings)target;

        UpdateDebugInfo();
    }

    void OnDisable() {
        debugTex = null;
    }

    public override void OnInspectorGUI() {
        //DrawDefaultInspector();

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

        float[,] iArr = settings.GetPerlinMapFloat(0, 0, size, 1);

        Color[] colorss = new Color[(size * scale) * (size * scale)];
        for (int y = 0; y < size * scale; y++) {
            for (int x = 0; x < size * scale; x++) {
                float id = iArr[Mathf.FloorToInt(x / (float)scale), Mathf.FloorToInt(y / (float)scale)];
                Color col = new Color(id, 0, 0);
                colorss[(y * (size * scale)) + x] = col;
            }
        }

        debugTex.SetPixels(colorss);
        debugTex.Apply();
    }
}
