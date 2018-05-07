﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Noise", menuName = "Generation/Noise", order = 1)]
public class Noise : ScriptableObject{

    public AnimationCurve curve;

    [Range(2, 20)]
    public float bigFloat;
    [Range(1, 10)]
    public float normalFloat;
    [Range(0.1f, 5)]
    public float smallFloat;

    [Range(0.0001f, 1f)]
    public float threshold;

    public bool IsOn(int x, int y) {
        float f = Mathf.PerlinNoise(x / bigFloat, y / bigFloat);
        float f2 = Mathf.PerlinNoise(x / normalFloat, y / normalFloat);
        float f3 = Mathf.PerlinNoise(x / smallFloat, y / smallFloat);
        //f = (f * multiplier1) + (f2 * multiplier2) + (f3 * multiplier3) + (f4 * multiplier4);
        //f /= (multiplier1 + multiplier2 + multiplier3 + multiplier4);
        f = f + f2 + f3;
        f /= 3;

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

        return value < threshold;
    }


    /*
    public enum NormalizeMode { Local, Global };

    public static int GetChunkInfo(int chunkX, int chunkY) {
        float f = Mathf.PerlinNoise(chunkX / 1.86f, chunkY / 1.86f) * 0.5f;
        bool b1 = f > 0.4f || f < 0.1f;

        float f1 = Mathf.PerlinNoise(chunkX / 1.12f, chunkY / 1.12f) * 0.7f;
        bool b2 = f1 > 0.6f || f1 < 0.1f;

        int i = 0;
        i |= b1 ? (1 << 0) : 0;//change first bit in the int
        i |= b2 ? (1 << 1) : 0;//change second bit in the int
        //i += b1 ? 1 : 0;
        //i += b2 ? 2 : 0;
        return i;

        //float f = Mathf.PerlinNoise(chunkX / 1.86f, chunkX / 1.86f) * 0.5f;
        //return f > 0.4f || f < 0.1f;
    }

    public static int[,] GetFoliageMap(int chunkX, int chunkY, int size, int length, AnimationCurve curve) {
        int[,] noiseMap = new int[size, size];

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float f = Mathf.PerlinNoise((chunkX + x) / 70.0f, (chunkY + y) / 70.0f);
                float f2 = Mathf.PerlinNoise((chunkX + x) / 22.54f, (chunkY + y) / 22.54f);
                float f3 = Mathf.PerlinNoise((chunkX + x) / 7.11f, (chunkY + y) / 7.11f);
                float f4 = Mathf.PerlinNoise((chunkX + x) / 2.11f, (chunkY + y) / 2.11f);
                f = (f) + (f2) + (f3 * 2.13f) + (f4);
                f /= (1f + 1f + 1f + 2.13f);

                if (f < 0)
                    f = 0;

                if (f >= 1)
                    f = 0.999f;

                float value = curve.Evaluate(f);

                noiseMap[x, y] = Mathf.FloorToInt(length * value);
            }
        }

        return noiseMap;
    }

    public static int[,] GetFoliageMap(int chunkX, int chunkY, int size, int length, AnimationCurve curve, float r1, float r2, float r3, float r4, float p1, float p2, float p3, float p4) {
        int[,] noiseMap = new int[size, size];

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float f = Mathf.PerlinNoise((chunkX + x) / r1, (chunkY + y) / r1);
                float f2 = Mathf.PerlinNoise((chunkX + x) / r2, (chunkY + y) / r2);
                float f3 = Mathf.PerlinNoise((chunkX + x) / r3, (chunkY + y) / r3);
                float f4 = Mathf.PerlinNoise((chunkX + x) / r4, (chunkY + y) / r4);
                f = (f * p1) + (f2 * p2) + (f3 * p3) + (f4 * p4);
                f /= (p1 + p2 + p3 + p4);

                if (f < 0)
                    f = 0.001f;

                if (f >= 1)
                    f = 0.999f;

                float value = curve.Evaluate(f);

                if (value < 0)
                    value = 0.001f;

                if (value >= 1)
                    value = 0.999f;

                noiseMap[x, y] = Mathf.FloorToInt(length * value);
            }
        }

        return noiseMap;
    }

    public static float[,] GetHeightMap(int chunkX, int chunkY, int size) {
        float[,] noiseMap = new float[size, size];

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float f = Mathf.PerlinNoise((chunkX + x) / 32.0f, (chunkY + y) / 32.0f);
                float f2 = Mathf.PerlinNoise((chunkX + x) / 8.5f, (chunkY + y) / 8.5f);
                f = f + f2;
                //f *= 2f;
                noiseMap[x, y] = f;
            }
        }

        return noiseMap;
    }

    public static float[,] GenerateMap(float x, float y, int size) {
        NoiseSettings setting = new NoiseSettings() {
            scale = 10,
            octaves = 3,
            persistance = 0.5f,
            lacunarity = 3,
            //offset = new Vector2(x, y)
        };
        return GenerateNoiseMap(size, size, setting, Vector3.zero, new Vector2(x, y));
    }

    public static int[,] SimplePerlin(float chunkX, float chunkY, int size, float scale, float scale2, float scale3) {
        int[,] noiseMap = new int[size, size];

        

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {

                int useX = x - (size / 2);
                int useY = y - (size / 2);

                //float k = Mathf.PerlinNoise(x / scale, y / scale) * 10.0f;
                //bool bb = Mathf.RoundToInt(Mathf.PerlinNoise((x + x) / scale, (y + y) / scale)) == 0;

                //System.Random rng = new System.Random(x + y);
                //bool p = rng.Next(x + y) == 0;

                float prefix = 0.5f;
                float f = Mathf.PerlinNoise(useX / scale, useY / scale) * prefix;
                bool b1 = f > prefix - 0.1f || f < 0.1f;

                float prefix2 = 0.7f;
                float f1 = Mathf.PerlinNoise(useX / scale2, useY / scale2) * prefix2;
                bool b2 = f1 > prefix2 - 0.1f || f1 < 0.1f;


                int i = 0;
                i += b1 ? 1 : 0;
                i += b2 ? 2 : 0;
                //i += b3 ? 4 : 0;

                //bool b2 = f > p * 0.5f && f < (p + 1) * 0.05f;
                //float f2 = Mathf.PerlinNoise((chunkX + x) / scale2, (chunkY + y) / scale2);
                //float f3 = Mathf.PerlinNoise((chunkX + x) / scale3, (chunkY + y) / scale3);
                //f = f + f2 + f3;
                //f /= 3f;
                noiseMap[x, y] = i;//b1 ? 1 : (b2 ? 2 : 0);//f > 0.5f ? 1 : 0;

            }
        }

        return noiseMap;
    }

    static float ClampToNearest(float value, float increment) {
        float div = value / increment;
        if (div > increment) {
            return Mathf.Ceil(div) * increment;
        } else {
            return Mathf.Floor(div) * increment;
        }
    }

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCentre, Vector2 offset) {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
            float offsetY = prng.Next(-100000, 100000) - settings.offset.y - sampleCentre.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < settings.octaves; i++) {
                    float sampleX = ((x + offset.x) - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = ((y + offset.y) - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight) {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;

                if (settings.normalizeMode == NormalizeMode.Global) {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        if (settings.normalizeMode == NormalizeMode.Local) {
            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
            }
        }

        return noiseMap;
    }

}

[System.Serializable]
public class NoiseSettings {
    public Noise.NormalizeMode normalizeMode;

    public float scale = 50;

    public int octaves = 6;
    [Range(0, 1)]
    public float persistance = .6f;
    public float lacunarity = 2;

    public int seed;
    public Vector2 offset;

    public void ValidateValues() {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
    */
}
