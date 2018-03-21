using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;

public enum SpawnType { Procedural, Panels }

public class Worldmanager : MonoBehaviour {

    public static Worldmanager instance;

    public GameObject[] treePrefab;

    public Transform target;

    public SpawnType spawnType;

    public int tileXY;
    public bool useThread;

    public Material groundMaterial;

    Vector2 playerChunkPos;
    [HideInInspector]
    public bool updated = false;
    [HideInInspector]
    public bool generating = false;

    Dictionary<Vector2, GameObject> chunkDictionary = new Dictionary<Vector2, GameObject>();
    List<Chunk> chunksToUpdate = new List<Chunk>();
    List<Vector3> panelsToAdd = new List<Vector3>();

    Thread generationThread;

    public UnityEngine.UI.Text settingsText;

    public Vector3[] trianglePoints;

    public bool SpawnTiles = true;

    [Header("Panel Settings")]
    public GameObject[] panels;

    [Header("Procedural Settings")]
    public Vector3 offset;
    public bool flatShading = false;

    System.Random rng;
    int seed = 10;

    private void Awake() {
        instance = this;
        rng = new System.Random(seed);
    }

    void Start () {
        if (PlayerPrefs.HasKey("Thr")) {
            //useThread = PlayerPrefs.GetInt("Thr") == 0;
            //flatShading = PlayerPrefs.GetInt("Fla") == 0;
            //spawnType = PlayerPrefs.GetInt("Pro") == 0 ? SpawnType.Procedural : SpawnType.Panels;
        }

        spawnType = SpawnType.Procedural;
        useThread = true;
        flatShading = true;

        settingsText.text = spawnType.ToString() + (spawnType == SpawnType.Procedural ? " / " + (useThread ? "Threading" : "Main Thread") + " / " + (flatShading ? "Flat" : "Smooth") : "");

        //if(SpawnTiles)
            //UpdateChunk();
    }

    public void ToggleProcedural(bool b) {
        if (b) {
            PlayerPrefs.SetInt("Pro", 0);
        } else {
            PlayerPrefs.SetInt("Pro", 1);
        }
    }

    public void ToggleThreading(bool b) {
        PlayerPrefs.SetInt("Thr", b ? 0 : 1);
    }

    public void ToggleFlatShading(bool b) {
        PlayerPrefs.SetInt("Fla", b ? 0 : 1);
    }

    public void ReloadScene() {
        PlayerPrefs.Save();
        SceneManager.LoadScene(0);
    }

    float checkChunksTimer;

    void Update () {
        Vector2 newChunkPos = new Vector2(Mathf.FloorToInt(target.position.x / (tileXY * 0.5f)), Mathf.FloorToInt(target.position.z / (tileXY * 0.5f)));

        if (SpawnTiles) {
            if (!generating && (chunksToUpdate.Count > 0 || panelsToAdd.Count > 0)) {
                if (spawnType == SpawnType.Procedural) {
                    if (useThread) {
                        generationThread = new Thread(chunksToUpdate[0].GenerateData);
                        generationThread.Start();

                        //OcclusionThread = new Thread(TreeChecker);
                        //OcclusionThread.Start();
                    } else {
                        chunksToUpdate[0].GenerateData();
                    }
                } else {
                    SpawnTile(panelsToAdd[0]);
                    generating = false;
                }
            }

            if (updated) {
                chunksToUpdate[0].FinalizeChunk();
                chunksToUpdate.RemoveAt(0);

                generating = false;
                updated = false;
            }

            if (newChunkPos != playerChunkPos) {
                UpdateChunk();
            }

            checkChunksTimer += Time.deltaTime;

            if (checkChunksTimer >= 30) {

                List<GameObject> toRemove = new List<GameObject>();
                foreach (Transform child in transform) {
                    if(!chunkDictionary.ContainsKey(new Vector2(child.position.x / tileXY, child.position.z / tileXY))) {
                        toRemove.Add(child.gameObject);
                        Debug.Log("This one does not belong here");
                    }
                }

                for (int i = 0; i < toRemove.Count; i++) {
                    Destroy(toRemove[i]);
                }

                toRemove.Clear();

                checkChunksTimer = 0;
            }
        }

        playerChunkPos = newChunkPos;
    }

    void UpdateChunk() {
        List<Vector2> lists = new List<Vector2>();

        for (int xOffset = -2; xOffset <= 3; xOffset++) {
            for (int yOffset = 0; yOffset <= 2; yOffset++) {
                if ((xOffset == -2 || xOffset == 3) && yOffset <= 0) {
                    continue;
                }

                Vector2 viewedChunkPos = new Vector2(Mathf.FloorToInt(target.position.x / tileXY) + xOffset, Mathf.FloorToInt(target.position.z / tileXY) + yOffset);
                lists.Add(viewedChunkPos);

                if (!chunkDictionary.ContainsKey(viewedChunkPos)) {
                    if (spawnType == SpawnType.Procedural) {
                        Chunk chunk = new Chunk() {
                            chunkPos = viewedChunkPos
                        };
                        chunksToUpdate.Add(chunk);
                    } else {
                        panelsToAdd.Add(viewedChunkPos);
                    }
                }
            }
        }

        List<Vector2> remove = new List<Vector2>();

        foreach (KeyValuePair<Vector2, GameObject> item in chunkDictionary) {
            if (!lists.Contains(item.Key)) {
                remove.Add(item.Key);
            }
        }

        while (remove.Count > 0) {
            EnemyManagerr.instance.RemoveEnemyAtChunk(remove[0]);
            Destroy(chunkDictionary[remove[0]]);
            chunkDictionary.Remove(remove[0]);
            remove.RemoveAt(0);
        }

        if (spawnType == SpawnType.Procedural) {
            
        } else {

        }
    }

    GameObject SpawnTile(Vector2 v2) {
        float perlin = Mathf.PerlinNoise(v2.x * 0.2f, v2.y * 0.2f);
        int index = Mathf.FloorToInt(perlin * panels.Length);
        //print(index);
        //print(perlin * panels.Length);
        //GameObject go = Instantiate(panels[Random.Range(0, panels.Length)], transform);
        GameObject go = Instantiate(panels[index], transform);
        go.transform.position = new Vector3(v2.x * tileXY, 0, v2.y * tileXY);

        panelsToAdd.RemoveAt(0);

        if(!chunkDictionary.ContainsKey(v2))
            chunkDictionary.Add(v2, go);

        return go;
    }

    public void AddToChunkDictionary(Vector2 v, GameObject g) {
        if(!chunkDictionary.ContainsKey(v))
            chunkDictionary.Add(v, g);
    }

    int amount;

    public void PlaceTree(Vector3 pos, Transform parent) {
        System.Random prng = new System.Random((int)(pos.x + pos.y + pos.z));
        GameObject go = Instantiate(treePrefab[prng.Next(treePrefab.Length)], parent);

        go.transform.localPosition = pos + new Vector3((prng.Next(0, 200) - 100) * 0.006f, 0f, (prng.Next(0, 200) - 100) * 0.008f);

        //if (amount < 10) {
        //    GameObject l = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    l.transform.position = go.transform.position;
        //    amount++;
        //}
    }
}

public class Chunk {
    public Vector2 chunkPos;

    List<Vector3> verts = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<int> tris = new List<int>();

    List<Vector3> treeLoc = new List<Vector3>();

    System.Random rng;

    public void GenerateData() {
        Worldmanager.instance.generating = true;
        rng = new System.Random();

        int chunkSize = Worldmanager.instance.tileXY;

        //float[,] map = GetNoise((int)chunkPos.x * Worldmanager.instance.tileXY, (int)chunkPos.y * Worldmanager.instance.tileXY, chunkSize + 1);
        //float[,] map = Noise.GenerateMap(chunkPos.x * Worldmanager.instance.tileXY, chunkPos.y * Worldmanager.instance.tileXY, chunkSize + 1);


        float[,] map = Noise.GetHeightMap((int)chunkPos.x * Worldmanager.instance.tileXY, (int)chunkPos.y * Worldmanager.instance.tileXY, chunkSize + 1);
        bool[,] treemap = Noise.GetTreeMap((int)chunkPos.x * Worldmanager.instance.tileXY, (int)chunkPos.y * Worldmanager.instance.tileXY, chunkSize);

        if (Worldmanager.instance.flatShading) {
            for (int y = 0; y < chunkSize; y++) {
                for (int x = 0; x < chunkSize; x++) {
                    int beginX = x - (int)(chunkSize / 2);
                    int beginY = y - (int)(chunkSize / 2);

                    if (rng.Next(2) == 0) {
                        verts.Add(new Vector3(beginX, map[x, y], beginY));
                        verts.Add(new Vector3(beginX, map[x, y + 1], beginY + 1));
                        verts.Add(new Vector3(beginX + 1, map[x + 1, y], beginY));
                        verts.Add(new Vector3(beginX + 1, map[x + 1, y], beginY));
                        verts.Add(new Vector3(beginX, map[x, y + 1], beginY + 1));
                        verts.Add(new Vector3(beginX + 1, map[x + 1, y + 1], beginY + 1));
                    } else {
                        verts.Add(new Vector3(beginX, map[x, y], beginY));
                        verts.Add(new Vector3(beginX, map[x, y + 1], beginY + 1));
                        verts.Add(new Vector3(beginX + 1, map[x + 1, y + 1], beginY + 1));
                        verts.Add(new Vector3(beginX, map[x, y], beginY));
                        verts.Add(new Vector3(beginX + 1, map[x + 1, y + 1], beginY + 1));
                        verts.Add(new Vector3(beginX + 1, map[x + 1, y], beginY));
                    }

                    int random = rng.Next(16);
                    int rY = Mathf.FloorToInt(random / 4);
                    int rX = random % 4;

                    uvs.Add(new Vector2(rX * 0.25f, rY * 0.25f));
                    uvs.Add(new Vector2(rX * 0.25f, 0.25f + rY * 0.25f));
                    uvs.Add(new Vector2(0.25f + rX * 0.25f, rY * 0.25f));

                    random = rng.Next(16);
                    rY = Mathf.FloorToInt(random / 4);
                    rX = random % 4;

                    uvs.Add(new Vector2(0.25f + rX * 0.25f, rY * 0.25f));
                    uvs.Add(new Vector2(rX * 0.25f, 0.25f + rY * 0.25f));
                    uvs.Add(new Vector2(0.25f + rX * 0.25f, 0.25f + rY * 0.25f));

                    int triCount = tris.Count;

                    tris.Add(triCount + 0);
                    tris.Add(triCount + 1);
                    tris.Add(triCount + 2);
                    tris.Add(triCount + 3);
                    tris.Add(triCount + 4);
                    tris.Add(triCount + 5);

                    if (treemap[x, y]) {
                        treeLoc.Add(new Vector3(beginX, map[x, y], beginY));
                    }
                }
            }
        } 
        else {
            for (int y = 0; y < chunkSize + 1; y++) {
                for (int x = 0; x < chunkSize + 1; x++) {
                    verts.Add(new Vector3(x, map[x, y], y));

                    uvs.Add(new Vector2(0.25f * x, 0.25f * y));
                }
            }

            for (int y = 0; y < chunkSize; y++) {
                for (int x = 0; x < chunkSize; x++) {

                    int start = (y * (chunkSize + 1)) + x;
                    tris.Add(start);
                    tris.Add(start + chunkSize + 1);
                    tris.Add(start + 1);
                    tris.Add(start + 1);
                    tris.Add(start + chunkSize + 1);
                    tris.Add(start + chunkSize + 1 + 1);
                }
            }
        }

        Worldmanager.instance.updated = true;
    }

    float[,] GetNoise(int chunkX, int chunkY, int size) {
        float[,] noisemap = new float[size, size];

        float multiplier = 2;
        //float other = 1.2f;

        for (int i = 0; i < 8; i++) {
            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    float height = 0;
                    height += Mathf.PerlinNoise((chunkX + x + (i * 10)) * 0.1f, (chunkY + y - (i * 5)) * 0.1f);
                    height *= multiplier;
                    noisemap[x, y] += height * 0.13f;
                }
            }

            multiplier += 0.4f;
            //other += 0.05f;
        }

        return noisemap;
    }

    public void FinalizeChunk() {
        int size = Worldmanager.instance.tileXY;

        Vector3 location = new Vector3(chunkPos.x * size, 0, chunkPos.y * size);

        GameObject chunkObject = new GameObject();
        chunkObject.name = location.ToString();
        chunkObject.transform.position = location;
        chunkObject.transform.SetParent(Worldmanager.instance.transform);
        chunkObject.layer = LayerMask.NameToLayer("Water");

        MeshFilter meshFilter = chunkObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = chunkObject.AddComponent<MeshCollider>();

        Mesh mesh = new Mesh {
            name = chunkObject.name,
            vertices = verts.ToArray(),
            uv = uvs.ToArray(),
            triangles = tris.ToArray()
        };

        mesh.RecalculateNormals();

        meshFilter.mesh = null;
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;

        meshRenderer.material = Worldmanager.instance.groundMaterial;

        foreach (Vector3 loc in treeLoc) {
            Worldmanager.instance.PlaceTree(loc, chunkObject.transform);
        }

        //Debug.Log("Chunk Done Loading");

        verts.Clear();
        uvs.Clear();
        tris.Clear();
        mesh = null;
        meshFilter = null;
        meshRenderer = null;
        meshCollider = null;

        treeLoc.Clear();

        if (Noise.IsEnemyChunk((int)chunkPos.x, (int)chunkPos.y)) {
            chunkObject.name = "Enemy Chunk";

            EnemyManagerr.instance.SpawnEnemy(location);
        }

        Worldmanager.instance.AddToChunkDictionary(chunkPos, chunkObject);
        Worldmanager.instance.updated = false;
    }
}
