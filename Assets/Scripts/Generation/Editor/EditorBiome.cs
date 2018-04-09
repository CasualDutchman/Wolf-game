using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Biome))]
public class EditorBiome : Editor {

    Biome biome;

    Texture2D debugTex;
    bool editPerlinSettings = false;

    void OnEnable() {
        biome = (Biome)target;
        UpdateDebugInfo();
    }

    void OnDisable() {
        debugTex = null;
    }

    public override void OnInspectorGUI() {
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(biome.name, EditorStyles.boldLabel);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Curve", GUILayout.MaxWidth(90));
                EditorGUI.BeginChangeCheck();
                AnimationCurve tempc = EditorGUILayout.CurveField(biome.curve);
                if (EditorGUI.EndChangeCheck()) {
                    biome.curve = tempc;
                    UpdateDebugInfo();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                if (GUILayout.Button(new GUIContent("+", "Add layer to biome: " + biome.name), EditorStyles.miniButton)) {
                    Undo.RecordObject(biome, "Add layer to biome: " + biome.name);
                    AddType();
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            /*
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(new GUIContent("Layer Curve", "This curve controls the spawning of the foliage in this biome. The curve gets devided into an equal amount of vertical parts, using the amount of layers provided. Looking at the curve X, it provides a Y value. This value is connected to the layers"));

                EditorGUI.BeginChangeCheck();
                AnimationCurve tempcurve = EditorGUI.CurveField(EditorGUILayout.GetControlRect(), biome.layerCurve);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(biome, "change biome Curve");
                    biome.layerCurve = tempcurve;
                    UpdateDebugInfo();
                }
            }
            GUILayout.EndHorizontal();
            */

            for (int i1 = 0; i1 < biome.types.Length; i1++) {
                BiomeTypeLabel(i1);
            }
        

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            PerlinSettings temp = (PerlinSettings)EditorGUILayout.ObjectField(biome.settings, typeof(PerlinSettings), false);
            if (EditorGUI.EndChangeCheck()) {
                biome.settings = temp;
                UpdateDebugInfo();
            }
            if (debugTex != null && biome.settings != null) {

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(debugTex);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                EditorGUI.BeginChangeCheck();
                bool b = GUILayout.Toggle(editPerlinSettings, "Edit Perlin Settings");
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(biome, "Toggle biome layer");
                    editPerlinSettings = b;
                }

                if (editPerlinSettings) {
                    EditorGUI.BeginChangeCheck();

                    PerlinSett();

                    if (EditorGUI.EndChangeCheck()) {
                        Undo.RecordObject(biome, "Toggle biome layer");
                        UpdateDebugInfo();
                    }
                }
            }
        }
        GUILayout.EndVertical();
    }

    void PerlinSett() {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Perlin 1", GUILayout.MaxWidth(90));
            biome.settings.perlin1 = EditorGUILayout.Slider(biome.settings.perlin1, 1, 100);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Perlin 2", GUILayout.MaxWidth(90));
            biome.settings.perlin2 = EditorGUILayout.Slider(biome.settings.perlin2, 1, 100);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Perlin 3", GUILayout.MaxWidth(90));
            biome.settings.perlin3 = EditorGUILayout.Slider(biome.settings.perlin3, 1, 100);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Perlin 4", GUILayout.MaxWidth(90));
            biome.settings.perlin4 = EditorGUILayout.Slider(biome.settings.perlin4, 1, 100);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Multiplier 1", GUILayout.MaxWidth(90));
            biome.settings.multiplier1 = EditorGUILayout.Slider(biome.settings.multiplier1, 0.1f, 10);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Multiplier 2", GUILayout.MaxWidth(90));
            biome.settings.multiplier2 = EditorGUILayout.Slider(biome.settings.multiplier2, 0.1f, 10);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Multiplier 3", GUILayout.MaxWidth(90));
            biome.settings.multiplier3 = EditorGUILayout.Slider(biome.settings.multiplier3, 0.1f, 10);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Multiplier 4", GUILayout.MaxWidth(90));
            biome.settings.multiplier4 = EditorGUILayout.Slider(biome.settings.multiplier4, 0.1f, 10);
        }
        GUILayout.EndHorizontal();
    }

    Color[] colors = new Color[] { new Color(1, 0, 0), new Color(1, 0.5f, 0), new Color(1, 1, 0), new Color(0.5f, 1, 0), new Color(0, 1, 0), new Color(0, 1, 0.5f),
        new Color(0, 1, 1), new Color(0, 0.5f, 1), new Color(0, 0, 1), new Color(0.5f, 0, 1) };

    void BiomeTypeLabel(int i) {

        GUIStyle style = EditorStyles.helpBox;
        style.normal.background = GetColorTexture(2, 2, colors[i % colors.Length] * 0.75f);

        GUILayout.BeginVertical("", style);
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                bool b = GUILayout.Toggle(biome.types[i].isNothing, string.Format(biome.types[i].isNothing ? "Layer {0} Is Nothing" : "Layer {0}", i));
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(biome, "Toggle biome layer");
                    biome.types[i].isNothing = b;
                    UpdateDebugInfo();
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                if (i > 0) {
                    if (GUILayout.Button(new GUIContent("Delete", "Delete layer " + i + " from biome: " + biome.name), !biome.types[i].isNothing ? EditorStyles.miniButtonLeft : EditorStyles.miniButtonLeft)) {
                        if (biome.types.Length > 1) {
                            Undo.RecordObject(biome, "Delete layer " + i + " from biome: " + biome.name);
                            DeleteType(i);
                        }
                    }
                }

                if (!biome.types[i].isNothing) {
                    if (GUILayout.Button(new GUIContent("+", "Add object to layer " + i), i > 0 ? EditorStyles.miniButtonMid : EditorStyles.miniButtonLeft)) {
                        Undo.RecordObject(biome, "Add object to layer " + i);
                        AddFoliage(i);
                    }

                    if (GUILayout.Button(new GUIContent("-", "Delete last object from layer " + i), EditorStyles.miniButtonRight)) {
                        if (biome.types[i].itemsToChoose.Length > 1) {
                            Undo.RecordObject(biome, "Delete last object from layer " + i);
                            DeleteFoliage(i);
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (i < biome.types.Length && !biome.types[i].isNothing) {
                for (int i1 = 0; i1 < biome.types[i].itemsToChoose.Length; i1++) {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.Space();
                        biome.types[i].itemsToChoose[i1] = (GameObject)EditorGUILayout.ObjectField(biome.types[i].itemsToChoose[i1], typeof(GameObject), false);
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }


        GUILayout.EndVertical();
    }

    void UpdateDebugInfo() {
        EditorUtility.SetDirty(biome);

        if (biome.settings == null)
            return;

        int size = 48;
        int scale = 4;

        if (debugTex == null) {
            debugTex = new Texture2D(size * scale, size * scale) {
                filterMode = FilterMode.Point
            };
        }

        int max = biome.types.Length;
        int[,] iArr = biome.settings.GetPerlinMapInt(0, 0, size, max, biome.curve);

        Color[] colorss = new Color[(size * scale) * (size * scale)];
        for (int y = 0; y < size * scale; y++) {
            for (int x = 0; x < size * scale; x++) {
                int id = iArr[Mathf.FloorToInt(x / (float)scale), Mathf.FloorToInt(y / (float)scale)];
                Color col = biome.types[id].isNothing ? Color.black : colors[id % colors.Length];
                colorss[(y * (size * scale)) + x] = col;
            }
        }

        debugTex.SetPixels(colorss);
        debugTex.Apply();
    }

    void AddFoliage(int ID) {
        GameObject[] gos = biome.types[ID].itemsToChoose;
        GameObject[] gos2 = new GameObject[gos.Length + 1];
        for (int i = 0; i < gos2.Length; i++) {
            if (i == gos2.Length - 1) {
                gos2[i] = null;
            } else {
                gos2[i] = gos[i];
            }
        }
        biome.types[ID].itemsToChoose = gos2;
    }

    void DeleteFoliage(int ID) {
        GameObject[] gos = biome.types[ID].itemsToChoose;
        GameObject[] gos2 = new GameObject[gos.Length - 1];
        for (int i = 0; i < gos2.Length; i++) {
            gos2[i] = gos[i];
        }
        biome.types[ID].itemsToChoose = gos2;
    }

    void AddType() {
        FoliageItem[] types = biome.types;
        FoliageItem[] types2 = new FoliageItem[types.Length + 1];
        for (int i = 0; i < types2.Length; i++) {
            if (i == types2.Length - 1) {
                types2[i] = new FoliageItem();// types[i - 1].Copy();
            } else {
                types2[i] = types[i];
            }
        }
        biome.types = types2;

        UpdateDebugInfo();
    }

    void DeleteType(int i2) {
        FoliageItem[] types = biome.types;
        FoliageItem[] types2 = new FoliageItem[types.Length - 1];
        for (int i = 0; i < types2.Length; i++) {
            types2[i] = types[i >= i2 ? i + 1 : i];
        }
        biome.types = types2;

        UpdateDebugInfo();
    }

    Texture2D GetColorTexture(int width, int height, Color col) {
        Texture2D tex = new Texture2D(width, height);
        Color[] colors = new Color[width * height];
        for (int i = 0; i < colors.Length; i++) {
            colors[i] = col;
        }
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }
}
