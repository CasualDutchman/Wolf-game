using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Worldmanager))]
public class EditorWorldManager : Editor {

    Worldmanager worldmanager;

    int currentlyShowBiome = -1;

    void OnEnable() {
        worldmanager = (Worldmanager)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label(new GUIContent("Biomes", "Biome contain all the information for spawning foliage"), EditorStyles.boldLabel);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button(new GUIContent("+", "Add Biome"), EditorStyles.miniButtonLeft)) {
                Undo.RecordObject(worldmanager, "Add Biome");
                AddBiome();
            }

            if (GUILayout.Button(new GUIContent("-", "Delete last Biome"), EditorStyles.miniButtonRight)) {
                if (worldmanager.biomes.Length > 1) {
                    Undo.RecordObject(worldmanager, "Delete last Biome");
                    DeleteBiome();
                }
            }
        }
        GUILayout.EndHorizontal();

        for (int i1 = 0; i1 < worldmanager.biomes.Length; i1++) {
            BiomeLabel(i1);
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
                }

                if (currentlyShowBiome == i) {
                    if (GUILayout.Button(new GUIContent("+", "Add layer to biome: " + worldmanager.biomes[i].name), EditorStyles.miniButtonMid)) {
                        Undo.RecordObject(worldmanager, "Add layer to biome: " + worldmanager.biomes[i].name);
                        AddType(i);
                    }

                    if (GUILayout.Button(new GUIContent("-", "Delete last layer from biome: " + worldmanager.biomes[i].name), EditorStyles.miniButtonRight)) {
                        if (worldmanager.biomes[i].types.Length > 1) {
                            Undo.RecordObject(worldmanager, "Delete last layer from biome: " + worldmanager.biomes[i].name);
                            DeleteType(i);
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (currentlyShowBiome == i) {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(new GUIContent("Layer Curve", "This curve controls the spawning of the foliage in this biome. The curve gets devided into an equal amount of vertical parts, using the amount of layers provided. Looking at the curve X, it provides a Y value. This value is connected to the layers"));
                    worldmanager.biomes[i].layerCurve = EditorGUI.CurveField(EditorGUILayout.GetControlRect(), worldmanager.biomes[i].layerCurve);
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

                    if (GUILayout.Button(new GUIContent("-", "Delete last object from layer " + i), EditorStyles.miniButtonRight)) {
                        if (worldmanager.biomes[biomeID].types[i].itemsToChoose.Length > 1) {
                            Undo.RecordObject(worldmanager, "Delete last object from layer " + i);
                            DeleteFoliage(biomeID, i);
                        }
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
    }

    void DeleteType(int ID) {
        FoliageItem[] types = worldmanager.biomes[ID].types;
        FoliageItem[] types2 = new FoliageItem[types.Length - 1];
        for (int i = 0; i < types2.Length; i++) {
            types2[i] = types[i];
        }
        worldmanager.biomes[ID].types = types2;
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
}
