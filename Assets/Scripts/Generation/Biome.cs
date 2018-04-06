using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "Generation/Biome", order = 1)]
public class Biome : ScriptableObject {
    public PerlinSettings settings;
    public AnimationCurve layerCurve = new AnimationCurve();
    public FoliageItem[] types = new FoliageItem[] { new FoliageItem() };
}

[System.Serializable]
public class FoliageItem {
    public bool isNothing = false;
    public GameObject[] itemsToChoose = new GameObject[1];
}
