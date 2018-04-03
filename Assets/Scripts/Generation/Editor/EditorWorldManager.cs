using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Worldmanager))]
public class EditorWorldManager : Editor {

    Worldmanager worldmanager;

    int currentlyShowBiome = -1;

    bool debug = false;

    Texture2D tex;

    void OnEnable() {
        worldmanager = (Worldmanager)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (currentlyShowBiome >= worldmanager.biomes.Length)
            currentlyShowBiome = -1;

        GUILayout.BeginVertical();
        {
            GUILayout.Label(new GUIContent("World Slider", "Sliders for the world's generation"), EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Foliage", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
                GUILayout.Label("XL");
                GUILayout.Label("L");
                GUILayout.Label("M");
                GUILayout.Label("S");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();

                GUILayout.Label("Perlin", GUILayout.MaxWidth(100));
                float f1 = GUILayout.HorizontalSlider(worldmanager.perlin1, 50f, 100f);
                float f2 = GUILayout.HorizontalSlider(worldmanager.perlin2, 25f, 75f);
                float f3 = GUILayout.HorizontalSlider(worldmanager.perlin3, 10f, 50f);
                float f4 = GUILayout.HorizontalSlider(worldmanager.perlin4, 2f, 10f);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(worldmanager, "Change Perlin");

                    worldmanager.perlin1 = f1;
                    worldmanager.perlin2 = f2;
                    worldmanager.perlin3 = f3;
                    worldmanager.perlin4 = f4;

                    if (debug)
                        DisplayDebugInfo(debug);
                }
            }
            GUILayout.EndHorizontal();

            if (debug) {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(100);
                    GUILayout.Label(worldmanager.perlin1.ToString("F4"));
                    GUILayout.Label(worldmanager.perlin2.ToString("F4"));
                    GUILayout.Label(worldmanager.perlin3.ToString("F4"));
                    GUILayout.Label(worldmanager.perlin4.ToString("F4"));
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();

                GUILayout.Label("Multiplier", GUILayout.MaxWidth(100));
                float f1 = GUILayout.HorizontalSlider(worldmanager.perlin1Multiplier, 1f, 10f);
                float f2 = GUILayout.HorizontalSlider(worldmanager.perlin2Multiplier, 1f, 10f);
                float f3 = GUILayout.HorizontalSlider(worldmanager.perlin3Multiplier, 1f, 10f);
                float f4 = GUILayout.HorizontalSlider(worldmanager.perlin4Multiplier, 1f, 10f);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(worldmanager, "Change Multiplier");

                    worldmanager.perlin1Multiplier = f1;
                    worldmanager.perlin2Multiplier = f2;
                    worldmanager.perlin3Multiplier = f3;
                    worldmanager.perlin4Multiplier = f4;

                    if (debug)
                        DisplayDebugInfo(debug);
                }
            }
            GUILayout.EndHorizontal();

            if (debug) {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(100);
                    GUILayout.Label(worldmanager.perlin1Multiplier.ToString("F2"));
                    GUILayout.Label(worldmanager.perlin2Multiplier.ToString("F2"));
                    GUILayout.Label(worldmanager.perlin3Multiplier.ToString("F2"));
                    GUILayout.Label(worldmanager.perlin4Multiplier.ToString("F2"));
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label(new GUIContent("Biomes", "Biome contain all the information for spawning foliage"), EditorStyles.boldLabel);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button(new GUIContent("+", "Add Biome"), EditorStyles.miniButton)) {
                Undo.RecordObject(worldmanager, "Add Biome");
                AddBiome();
            }
        }
        GUILayout.EndHorizontal();

        for (int i1 = 0; i1 < worldmanager.biomes.Length; i1++) {
            BiomeLabel(i1);
        }

        GUILayout.BeginVertical();
        {
            EditorGUI.BeginChangeCheck();
            bool tempbool = GUILayout.Toggle(debug, new GUIContent("Debug", "Show extra information to show what the world might look like"));
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(worldmanager, "debug");
                debug = tempbool;
                DisplayDebugInfo(debug);
            }

            if (tex != null && debug) {

                GUILayout.Label(tex);
            }
        }
        GUILayout.EndVertical();
    }

    int size = 48;
    int scale = 4;

    void DisplayDebugInfo(bool show) {
        if (show && currentlyShowBiome >= 0) {
            tex = new Texture2D(size * scale, size * scale) {
                filterMode = FilterMode.Point
            };

            Biome biome = worldmanager.biomes[currentlyShowBiome];

            int max = biome.types.Length;
            int[,] iArr = Noise.GetFoliageMap(0, 0, size * scale, max, biome.layerCurve,
                worldmanager.perlin1, worldmanager.perlin2, worldmanager.perlin3, worldmanager.perlin4,
                worldmanager.perlin1Multiplier, worldmanager.perlin2Multiplier, worldmanager.perlin3Multiplier, worldmanager.perlin4Multiplier);

            Color[] colors = new Color[(size * scale) * (size * scale)];
            for (int y = 0; y < size * scale; y++) {
                for (int x = 0; x < size * scale; x++) {
                    colors[(y * (size * scale)) + x] = new Color((float)(1 / (float)max) * iArr[Mathf.FloorToInt(x / (float)scale), Mathf.FloorToInt(y / (float)scale)], 0, 0);
                }
            }

            tex.SetPixels(colors);
            tex.Apply();
        } 
        else {
            tex = null;
        }
    }

    void BiomeLabel(int i) {
        GUILayout.BeginVertical("", EditorStyles.helpBox);
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                string tempstr = EditorGUILayout.TextField(worldmanager.biomes[i].name, EditorStyles.boldLabel);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(worldmanager, "change biome name");
                    worldmanager.biomes[i].name = tempstr;
                }

                if (GUILayout.Button(new GUIContent(currentlyShowBiome == i ? "Hide" : "Expand", "Expand or hide"), EditorStyles.miniButtonLeft)) {
                    currentlyShowBiome = currentlyShowBiome == i ? -1 : i;
                    if (debug)
                        DisplayDebugInfo(debug);
                }

                if (currentlyShowBiome == i) {
                    if (GUILayout.Button(new GUIContent("+", "Add layer to biome: " + worldmanager.biomes[i].name), EditorStyles.miniButtonMid)) {
                        Undo.RecordObject(worldmanager, "Add layer to biome: " + worldmanager.biomes[i].name);
                        AddType(i);
                    }

                    if (GUILayout.Button(new GUIContent("Delete", "Delete Biome: " + worldmanager.biomes[i].name), EditorStyles.miniButtonRight)) {
                        if (worldmanager.biomes.Length > 1) {
                            Undo.RecordObject(worldmanager, "Delete Biome: " + worldmanager.biomes[i].name);
                            DeleteBiome(i);

                            if (currentlyShowBiome >= worldmanager.biomes.Length)
                                currentlyShowBiome = -1;
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (currentlyShowBiome == i) {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(new GUIContent("Layer Curve", "This curve controls the spawning of the foliage in this biome. The curve gets devided into an equal amount of vertical parts, using the amount of layers provided. Looking at the curve X, it provides a Y value. This value is connected to the layers"));

                    EditorGUI.BeginChangeCheck();
                    AnimationCurve tempcurve = EditorGUI.CurveField(EditorGUILayout.GetControlRect(), worldmanager.biomes[i].layerCurve);
                    if (EditorGUI.EndChangeCheck()) {
                        Undo.RecordObject(worldmanager, "change biome Curve");
                        worldmanager.biomes[i].layerCurve = tempcurve;
                        if (debug) {
                            DisplayDebugInfo(debug);
                        }
                    }
                }
                GUILayout.EndHorizontal();
                /*
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(new GUIContent("+", "Expand or hide the World information"), EditorStyles.miniButton)) {
                        AddType(i);
                    }

                    if (GUILayout.Button(new GUIContent("-", "Expand or hide the World information"), EditorStyles.miniButtonRight)) {
                        if (worldmanager.biomes[i].types.Length > 1)
                            DeleteType(i);
                    }
                }
                GUILayout.EndHorizontal();*/

                for (int i1 = 0; i1 < worldmanager.biomes[i].types.Length; i1++) {
                    BiomeTypeLabel(i, i1);
                }
            }
        }
        GUILayout.EndVertical();
    }

    void BiomeTypeLabel(int biomeID, int i) {
        Biome b = worldmanager.biomes[biomeID];

        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                b.types[i].isNothing = GUILayout.Toggle(b.types[i].isNothing, string.Format(b.types[i].isNothing ? "Layer {0} Is Nothing" : "Layer {0}", i));

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                if (!b.types[i].isNothing) {
                    if (GUILayout.Button(new GUIContent("+", "Add object to layer " + i), EditorStyles.miniButtonLeft)) {
                        Undo.RecordObject(worldmanager, "Add object to layer " + i);
                        AddFoliage(biomeID, i);
                    }

                    if (GUILayout.Button(new GUIContent("-", "Delete last object from layer " + i), EditorStyles.miniButtonMid)) {
                        if (worldmanager.biomes[biomeID].types[i].itemsToChoose.Length > 1) {
                            Undo.RecordObject(worldmanager, "Delete last object from layer " + i);
                            DeleteFoliage(biomeID, i);
                        }
                    }
                }

                if (GUILayout.Button(new GUIContent("Delete", "Delete layer " + i + " from biome: " + worldmanager.biomes[biomeID].name), !b.types[i].isNothing ? EditorStyles.miniButtonRight : EditorStyles.miniButton)) {
                    if (worldmanager.biomes[biomeID].types.Length > 1) {
                        Undo.RecordObject(worldmanager, "Delete layer " + i + " from biome: " + worldmanager.biomes[biomeID].name);
                        DeleteType(i, biomeID);
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (!b.types[i].isNothing) {
                for (int i1 = 0; i1 < b.types[i].itemsToChoose.Length; i1++) {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.Space();
                        b.types[i].itemsToChoose[i1] = (GameObject)EditorGUILayout.ObjectField(b.types[i].itemsToChoose[i1], typeof(GameObject), false);
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }


        GUILayout.EndVertical();
    }

    void AddFoliage(int ID, int ID2) {
        GameObject[] gos = worldmanager.biomes[ID].types[ID2].itemsToChoose;
        GameObject[] gos2 = new GameObject[gos.Length + 1];
        for (int i = 0; i < gos2.Length; i++) {
            if (i == gos2.Length - 1) {
                gos2[i] = null;
            } else {
                gos2[i] = gos[i];
            }
        }
        worldmanager.biomes[ID].types[ID2].itemsToChoose = gos2;
    }

    void DeleteFoliage(int ID, int ID2) {
        GameObject[] gos = worldmanager.biomes[ID].types[ID2].itemsToChoose;
        GameObject[] gos2 = new GameObject[gos.Length - 1];
        for (int i = 0; i < gos2.Length; i++) {
            gos2[i] = gos[i];
        }
        worldmanager.biomes[ID].types[ID2].itemsToChoose = gos2;
    }

    void AddType(int ID) {
        FoliageItem[] types = worldmanager.biomes[ID].types;
        FoliageItem[] types2 = new FoliageItem[types.Length + 1];
        for (int i = 0; i < types2.Length; i++) {
            if (i == types2.Length - 1) {
                types2[i] = new FoliageItem();// types[i - 1].Copy();
            } else {
                types2[i] = types[i];
            }
        }
        worldmanager.biomes[ID].types = types2;

        if (debug) {
            Undo.RecordObject(worldmanager, "Change debug values");
            DisplayDebugInfo(true);
        }
    }

    void DeleteType(int i2, int ID) {
        FoliageItem[] types = worldmanager.biomes[ID].types;
        FoliageItem[] types2 = new FoliageItem[types.Length - 1];
        for (int i = 0; i < types2.Length; i++) {
            types2[i] = types[i >= i2 ? i + 1: i];
        }
        worldmanager.biomes[ID].types = types2;

        if (debug) {
            Undo.RecordObject(worldmanager, "Change debug values");
            DisplayDebugInfo(true);
        }
    }

    void AddBiome() {
        Biome[] biomes = worldmanager.biomes;
        Biome[] biomes2 = new Biome[worldmanager.biomes.Length + 1];
        for (int i = 0; i < biomes2.Length; i++) {
            if (i == biomes2.Length - 1) {
                biomes2[i] = new Biome();// biomes[i - 1].Copy();
            } else {
                biomes2[i] = biomes[i];
            }
        }
        worldmanager.biomes = biomes2;
    }

    void DeleteBiome() {
        Biome[] biomes = worldmanager.biomes;
        Biome[] biomes2 = new Biome[worldmanager.biomes.Length - 1];
        for (int i = 0; i < biomes2.Length; i++) {
            biomes2[i] = biomes[i];
        }
        worldmanager.biomes = biomes2;
    }

    void DeleteBiome(int i2) {
        Biome[] biomes = worldmanager.biomes;
        Biome[] biomes2 = new Biome[worldmanager.biomes.Length - 1];
        for (int i = 0; i < biomes2.Length; i++) {
            biomes2[i] = biomes[i >= i2 ? i + 1 : i];
        }
        worldmanager.biomes = biomes2;
    }
}
