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

    public float redLine = 0.9f;

	void Start () {
        float[,] map = GetNoise(0, 0, size);

        tex = new Texture2D(size, size);
        tex.name = "Pu";

        colors = new Color[size * size];

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
                float col = perc;
                Color color = new Color(col, col, col);
                colors[(y * size) + x] = RedAt(color, perc);
                
            }
        }

        tex.SetPixels(colors);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.transform.eulerAngles = new Vector3(90, 0, 0);
        go.transform.position = new Vector3(0, 1, 0);
        go.transform.localScale = Vector3.one * (size / 5);
        go.GetComponent<MeshRenderer>().material.mainTexture = tex;
    }
	
	void Update () {
        float[,] map = GetNoise(0, 0, size);

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
                float col = perc;
                Color color = new Color(col, col, col);
                colors[(y * size) + x] = RedAt(color, perc);

            }
        }

        tex.SetPixels(colors);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
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
