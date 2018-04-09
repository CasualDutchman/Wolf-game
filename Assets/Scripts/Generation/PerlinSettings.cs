using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Perlin Settings", menuName = "Generation/Perlin Settings", order = 1)]
public class PerlinSettings : ScriptableObject {

    [Header("Perlin")]
    [Range(20.0f, 100.0f)]
    public float perlin1 = 70.0f;
    [Range(10.0f, 60.0f)]
    public float perlin2 = 40.0f;
    [Range(1.0f, 40.0f)]
    public float perlin3 = 30.0f;
    [Range(1.1f, 10.0f)]
    public float perlin4 = 2.0f;

    [Header("Multipliers")]
    [Range(0.1f, 10.0f)]
    public float multiplier1 = 1.0f;
    [Range(0.1f, 10.0f)]
    public float multiplier2 = 1.0f;
    [Range(0.1f, 10.0f)]
    public float multiplier3 = 1.0f;
    [Range(0.1f, 10.0f)]
    public float multiplier4 = 1.0f;

    int defaultX = 0;
    int defaultY = 0;
    int defaultSize = 32;
    int defaultLength = 5;
    int defaultBelow = 3;

    public float[,] GetPerlinMapFloat() {
        return GetPerlinMapFloat(defaultX, defaultY, defaultSize, defaultLength);
    }

    public float[,] GetPerlinMapFloat(int beginX, int beginY, int size, int length) {
        return GetPerlinMapFloat(beginX, beginY, size, length, null);
    }

    public float[,] GetPerlinMapFloat(int beginX, int beginY, int size, int length, AnimationCurve curve) {
        float[,] noiseMap = new float[size, size];

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float f = Mathf.PerlinNoise((beginX + x) / perlin1, (beginY + y) / perlin1);
                float f2 = Mathf.PerlinNoise((beginX + x) / perlin2, (beginY + y) / perlin2);
                float f3 = Mathf.PerlinNoise((beginX + x) / perlin3, (beginY + y) / perlin3);
                float f4 = Mathf.PerlinNoise((beginX + x) / perlin4, (beginY + y) / perlin4);
                f = (f * multiplier1) + (f2 * multiplier2) + (f3 * multiplier3) + (f4 * multiplier4);
                f /= (multiplier1 + multiplier2 + multiplier3 + multiplier4);

                if (f < 0)
                    f = 0.001f;

                if (f >= 1)
                    f = 0.999f;

                float value = f;
                if (curve != null) {
                    value = curve.Evaluate(f);

                    if (value < 0)
                        value = 0.001f;

                    if (value >= 1)
                        value = 0.999f;
                }

                noiseMap[x, y] = length * value;
            }
        }

        return noiseMap;
    }

    public int[,] GetPerlinMapInt() {
        return GetPerlinMapInt(defaultX, defaultY, defaultSize, defaultLength);
    }

    public int[,] GetPerlinMapInt(int beginX, int beginY, int size, int length) {
        return GetPerlinMapInt(beginX, beginY, size, length, null);
    }

    public int[,] GetPerlinMapInt(int beginX, int beginY, int size, int length, AnimationCurve curve) {
        int[,] noiseMap = new int[size, size];
        float[,] perlinMap = GetPerlinMapFloat(beginX, beginY, size, 1, curve);

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                noiseMap[x, y] = Mathf.FloorToInt(length * perlinMap[x, y]);
            }
        }

        return noiseMap;
    }

    public bool[,] GetPerlinMapBool() {
        return GetPerlinMapBool(defaultX, defaultY, defaultSize, defaultLength, defaultBelow);
    }

    public bool[,] GetPerlinMapBool(int beginX, int beginY, int size, int length, int below) {
        return GetPerlinMapBool(beginX, beginY, size, length, below, null);
    }

    public bool[,] GetPerlinMapBool(int beginX, int beginY, int size, int length, int below, AnimationCurve curve) {
        bool[,] noiseMap = new bool[size, size];
        float[,] perlinMap = GetPerlinMapFloat(beginX, beginY, size, length, curve);

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                noiseMap[x, y] = perlinMap[x, y] < below;
            }
        }

        return noiseMap;
    }

}
