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
            GUILayout.Label("Biomes", EditorStyles.boldLabel);

            if (GUILayout.Button(new GUIContent("+", "Expand or hide the World information"), EditorStyles.miniButtonLeft)) {
                AddBiome();
            }

            if (GUILayout.Button(new GUIContent("-", "Expand or hide the World information"), EditorStyles.miniButtonRight)) {
                if(worldmanager.biomes.Length > 1)
                    DeleteBiome();
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
                    if (GUILayout.Button(new GUIContent("+", "Expand or hide the World information"), EditorStyles.miniButtonMid)) {
                        AddType(i);
                    }

                    if (GUILayout.Button(new GUIContent("-", "Expand or hide the World information"), EditorStyles.miniButtonRight)) {
                        if (worldmanager.biomes[i].types.Length > 1)
                            DeleteType(i);
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (currentlyShowBiome == i) {
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
                    if (GUILayout.Button(new GUIContent("+", "Expand or hide the World information"), EditorStyles.miniButtonLeft)) {
                        AddFoliage(biomeID, i);
                    }

                    if (GUILayout.Button(new GUIContent("-", "Expand or hide the World information"), EditorStyles.miniButtonRight)) {
                        if (worldmanager.biomes[biomeID].types[i].itemsToChoose.Length > 1)
                            DeleteFoliage(biomeID, i);
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
                gos2[i] = gos[i - 1];
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
                types2[i] = types[i - 1].Copy();
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
                biomes2[i] = biomes[i - 1].Copy();
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
