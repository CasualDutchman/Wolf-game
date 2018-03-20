using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinTest : MonoBehaviour {

    public int size = 100;

    [Range(1, 10)]
    public int octaves = 3;

    public Vector3 offsetAdder;
    public Vector3 offset;

    GameObject[,] gos;

    float lowest = float.MaxValue, highest = float.MinValue;

    public Texture2D tex;
    Color[] colors;

    public Color[] colorPallete;

    public float redLine = 0.9f;

    float timer;

    public AnimationCurve curve;

    void Start() {
        tex = new Texture2D(size, size) {
            name = "Pu"
        };

        colors = new Color[size * size];

        tex.SetPixels(colors);
        tex.filterMode = FilterMode.Bilinear;
        tex.Apply();

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.transform.eulerAngles = new Vector3(90, 0, 0);
        go.transform.position = new Vector3(0, 1, 0);
        go.transform.localScale = Vector3.one * (size / 5);
        go.GetComponent<MeshRenderer>().material.mainTexture = tex;
    }

    public NoiseSettings settings;

    float timer2;

    [Range(2, 50)]
    public int go = 2;

    void Update() {
        //float[,] map = GetNoise(0, 0, size);
        float[,] map = GetTreeNoise(0, 0, size, go);


        //timer += Time.deltaTime * 5f;
        //timer2 += Time.deltaTime * 0.1f;

        //Vector2 v2 = new Vector2(timer, -timer);
        //
        //settings.offset = v2;
        //settings.persistance = 0.1f + curve.Evaluate(timer2) * 0.5f;

        //if (timer2 >= 1) {
        //    timer2 -= 1;
        //}

        //float[,] map = Noise.GenerateNoiseMap(size, size, settings, Vector2.zero, Vector2.zero);

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                if (map[x, y] < lowest) {
                    lowest = map[x, y];
                }

                if (map[x, y] > highest) {
                    highest = map[x, y];
                }
            }
        }

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float perc = Mathf.InverseLerp(lowest, highest, map[x, y]);
                float col = map[x, y];
                Color color = new Color(col, col, col);
                //colors[(y * size) + x] = RedAt(color, perc);
                //colors[(y * size) + x] = GetColor(perc);
                colors[(y * size) + x] =color;
            }
        }

        tex.SetPixels(colors);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
    }

    int amount;
    bool b;

    Color GetColor(float value) {
        float one = 1f / (float)(colorPallete.Length);
        int i = 0;
        for (int k = 0; k < colorPallete.Length - 1; k++) {

            if (value > one * k && value < one * (k + 1)) {
                break;
            }

            i++;
        }
        if (amount < 20) {
            print(i);
            //print(current);
            print(one);
            b = true;
            amount++;
        }

        return colorPallete[i];
    }

    Color RedAt(Color col, float above) {
        if (above > redLine) {
            return Color.red;
        }
        return col;
    }

    public float multiplierBegin = 2;
    public float otherBegin = 1.2f;
    public float multiplierIncrease = 0.6f;
    public float otherIncrease = 0.05f;

    float[,] GetTreeNoise(int chunkX, int chunkY, int size, int passageLength){

        float[,] noisemap = new float[size, size];

        float multiplier = multiplierBegin;
        float other = otherBegin;

        Vector3 offsetmultiplier = new Vector3();

        for (int i = 0; i < octaves; i++) {
            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    float height = 0;
                    height += Mathf.PerlinNoise(offset.x + (offsetmultiplier.x + (chunkX + x + (i * 10)) * 0.1f * other), offset.z + (offsetmultiplier.z + (chunkY + y - (i * 5)) * 0.1f * other));
                    height *= multiplier;
                    noisemap[x, y] += height;
                }
            }

            multiplier += multiplierIncrease;
            other += otherIncrease;
            offsetmultiplier += offsetAdder;
        }

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                noisemap[x, y] = ClampToNearest(noisemap[x, y], 1f / (float)(passageLength - 1));
            }
        }

        return noisemap;

        /*
        float[,] noisemap = new float[size, size];

        for (int i = 0; i < octaves; i++) {
            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    float height = Mathf.PerlinNoise(offset.x + (chunkX + x + (i * 10)) * 0.1f, offset.z + (chunkY + y + (i * 10)) * 0.1f);

                    noisemap[x, y] = ClampToNearest(height, 1f / (float)(passageLength - 1));
                }
            }
        }
                

        return noisemap;
        */
    }

    float ClampToNearest(float value, float increment) {
        float div = value / increment;
        if (div > increment) {
            return Mathf.Ceil(div) * increment;
        } else {
            return Mathf.Floor(div) * increment;
        }
    }

    float[,] GetNoise(int chunkX, int chunkY, int size) {
        float[,] noisemap = new float[size, size];

        float multiplier = multiplierBegin;
        float other = otherBegin;

        Vector3 offsetmultiplier = new Vector3();

        for (int i = 0; i < octaves; i++) {
            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    float height = 0;
                    height += Mathf.PerlinNoise(offset.x + (offsetmultiplier.x + (chunkX + x + (i * 10)) * 0.1f * other), offset.z + (offsetmultiplier.z + (chunkY + y - (i * 5)) * 0.1f * other));
                    height *= multiplier;
                    noisemap[x, y] += height;
                    
                }
            }

            multiplier += multiplierIncrease;
            other += otherIncrease;
            offsetmultiplier += offsetAdder;
        }

        return noisemap;
    }
}


public static class Noise {

    public enum NormalizeMode { Local, Global };

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
}