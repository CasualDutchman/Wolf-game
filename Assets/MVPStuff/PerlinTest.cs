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

    public bool spawnFoliage = false;
    public GameObject Tree1, Tree2;
    public GameObject Bush1;
    public GameObject Rock1;
    public GameObject Grass;
    public float treeThreshold, bushThreshold, rockThreshold, grassThreshold;

    void Start() {
        if (!spawnFoliage) {
            tex = new Texture2D(size, size) {
                name = "Pu"
            };

            colors = new Color[size * size];

            tex.SetPixels(colors);
            tex.filterMode = FilterMode.Point;
            tex.Apply();

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.transform.eulerAngles = new Vector3(90, 0, 0);
            go.transform.position = new Vector3((size * 0.5f) * 0.5f, 0, (size * 0.5f) * 0.5f);
            go.transform.localScale = Vector3.one * (size * 0.5f);
            go.GetComponent<MeshRenderer>().material.mainTexture = tex;

            go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.transform.eulerAngles = new Vector3(90, 0, 0);
            go.transform.position = new Vector3(-(size * 0.5f) * 0.5f, 0, (size * 0.5f) * 0.5f);
            go.transform.localScale = Vector3.one * (size * 0.5f);

            //SpawnFoliage();
        } 
        else {
            SpawnFoliage();
        }
    }

    public void SpawnFoliage() {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        //go.transform.eulerAngles = new Vector3(90, 0, 0);
        //go.transform.localScale = Vector3.one * size;
        //go.transform.position = new Vector3(size * 0.5f, 0, size * 0.5f);

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float f = Mathf.PerlinNoise((x) / 35.0f, (y) / 35.0f);
                float f2 = Mathf.PerlinNoise((x) / 2.54f, (y) / 2.54f);
                float f3 = Mathf.PerlinNoise((x) / 0.11f, (y) / 0.11f);
                f = f + f2 + f3;
                f /= 3f;
                if (f > treeThreshold) {
                    if (Random.Range(0, 2) == 0) {
                        go = Instantiate(Tree1);
                    } else {
                        go = Instantiate(Tree2);
                    }
                }
                else if (f > bushThreshold) {
                    //go = Instantiate(Bush1);
                    if(Random.Range(0, 20) > 1) {
                        go = Instantiate(Bush1);
                    } else {
                        go = Instantiate(Rock1);
                    }
                } 
                //else if (f > rockThreshold) {
                //    go = Instantiate(Rock1);
                //} 
                else if (f > grassThreshold) {
                    go = Instantiate(Grass);
                }

                go.transform.position = new Vector3(x, 0, y) + new Vector3(0.5f, 0, 0.5f);//new Vector3(Random.Range(-0.6f, 0.6f), 0, Random.Range(-0.6f, 0.6f));
                //go.transform.localScale = new Vector3(go.transform.localScale.x * Random.Range(0.8f, 1.4f),
                //                                    go.transform.localScale.y * Random.Range(0.8f, 1.4f),
                //                                    go.transform.localScale.z * Random.Range(0.8f, 1.4f));
            }
        }  
    }

    public NoiseSettings settings;

    float timer2;

    [Range(2, 50)]
    public int go = 2;

    [Range(1, 100)]
    public float slider100;
    [Range(1, 50)]
    public float slider50;
    [Range(1, 20)]
    public float slider20;
    [Range(1, 10)]
    public float slider10;

    [Range(1, 10)]
    public float priority1;
    [Range(1, 10)]
    public float priority2;
    [Range(1, 10)]
    public float priority3;
    [Range(1, 10)]
    public float priority4;

    [Range(1, 100)]
    public float devision;

    float[,] map;

    void Update() {
        if (!spawnFoliage) {
            //float[,] map = GetNoise(0, 0, size);
            //float[,] map = GetTreeNoise(0, 0, size, go);
            //int[,] map = Noise.SimplePerlin(0, 0, size, otherBegin, multiplierIncrease, otherIncrease);

            map = new float[size, size];

            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    float f1 = Mathf.PerlinNoise(x / slider100, y / slider100);
                    float f2 = Mathf.PerlinNoise(x / slider50, y / slider50);
                    float f3 = Mathf.PerlinNoise(x / slider20, y / slider20);
                    float f4 = Mathf.PerlinNoise(x / slider10, y / slider10);
                    float f = (f1 * priority1) + (f2 * priority2) + (f3 * priority3) + (f4 * priority4);
                    //devision = priority1 + priority2 + priority3 + priority4;
                    f /= devision;
                    map[x, y] = curve.Evaluate(f);
                }
            }

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
                    //float perc = Mathf.InverseLerp(lowest, highest, map[x, y]);
                    //float r = map[x, y] == 1 || map[x, y] == 3 || map[x, y] == 5 || map[x, y] == 7 ? 1 : 0;
                    //float g = map[x, y] == 2 || map[x, y] == 3 || map[x, y] == 6 || map[x, y] == 7 ? 1 : 0;
                    //float b = map[x, y] == 4 || map[x, y] == 5 || map[x, y] == 6 || map[x, y] == 7 ? 1 : 0;
                    //Color color = new Color(r, g, b);
                    //colors[(y * size) + x] = RedAt(color, perc);
                    //colors[(y * size) + x] = GetColor(perc);
                    /*
                    float f = map[x, y];
                    if (f > treeThreshold) {
                        colors[(y * size) + x] = Color.green;
                    } else if (f > bushThreshold) {
                        colors[(y * size) + x] = Color.yellow;
                    } //else if (f > rockThreshold) {
                        //colors[(y * size) + x] = new Color(1, 0.5f, 0);
                    //} 
                else if (f > grassThreshold) {
                        colors[(y * size) + x] = Color.red;
                    }
                    else {
                        colors[(y * size) + x] = Color.white;
                    }
                    */
                    //colors[(y * size) + x] = new Color(map[x, y], map[x, y], map[x, y]);
                    colors[(y * size) + x] = GetColor(map[x, y]);
                }
            }

            tex.SetPixels(colors);
            tex.filterMode = FilterMode.Point;
            tex.Apply();
        }
    }

    List<Transform> spawns = new List<Transform>();

    [ContextMenu("Spawn")]
    public void Spawn() {
        if (spawns.Count > 0) {
            foreach (var item in spawns) {
                Destroy(item.gameObject);
            }
        }

        spawns.Clear();

        for (int y = 0; y < size / 2; y++) {
            for (int x = 0; x < size / 2; x++) {
                GameObject g = GetGameObject(GetValue(x, y));
                if (g != null) {
                    GameObject go = Instantiate(g);

                    go.transform.position = new Vector3(-(size * 0.5f) + x, 0, y) + new Vector3(0.5f, 0, 0.5f);
                    spawns.Add(go.transform);
                }
            }
        }
    }

    float GetValue(int x, int y) {
        //int posX = x * 2;
        //posX += Random.Range(0)

        float value = map[x * 2, y * 2];

        return value;
    }

    GameObject GetGameObject(float value) {
        GameObject[] gos = new GameObject[] { null, Grass, Rock1, Bush1, Tree1 };

        float one = 1f / (float)gos.Length;

        int i = 0;
        for (int k = 0; k < gos.Length - 1; k++) {

            if (value > one * k && value < one * (k + 1)) {
                break;
            }

            i++;
        }

        return gos[i];
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