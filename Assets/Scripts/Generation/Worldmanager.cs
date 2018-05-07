using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Worldmanager : MonoBehaviour {

    public static Worldmanager instance;

    public float blendMultiplier = 0.001f;
    float blend = 0;
    public Material mat;

    Settingsmanager settings;

    public float renderInterval;
    float renderTimer;

    public Transform target;

    public int tileXY;
    public float tileScale;
    [HideInInspector]
    public int TileSize { get { return (int)(tileXY * tileScale); } }

    public Material groundMaterial;

    Vector2 playerChunkPos = new Vector2(float.MinValue, float.MaxValue);
    [HideInInspector]
    public bool updated = false;
    [HideInInspector]
    public bool generating = false;

    float checkChunksTimer;

    Dictionary<Vector2, GameObject> chunkDictionary = new Dictionary<Vector2, GameObject>();
    List<Chunk> chunksToUpdate = new List<Chunk>();

    Thread generationThread;

    public GameObject loadingUI;
    public UnityEngine.UI.Image progressbar;
    public bool showLoadingScreen = false;

    bool loadingLevel = true;
    int loadedChunks = 0;

    public IManager[] managers;

    public PerlinSettings terrainPerlin;

    public Biome[] biomes;

    public float zOffset = 10;

    private void Awake() {
        //Application.targetFrameRate = 60;

        instance = this;

        loadingUI.SetActive(showLoadingScreen);

        //UpdateChunk();
    }

    void Start() {
        managers = GetComponents<IManager>();
        settings = GetComponent<Settingsmanager>();
    }

    void OnDisable() {
        mat.SetFloat("_Blend", 0);
    }

    void Update () {
        Vector2 newChunkPos = new Vector2(Mathf.FloorToInt(target.position.x / TileSize), Mathf.FloorToInt((target.position.z + zOffset) / TileSize));

        if (mat != null) {
            blend += Time.deltaTime * blendMultiplier;
            if (blend >= 4) {
                blend = 0;
            }
            renderTimer += Time.deltaTime;
            if (renderTimer >= renderInterval) {
                settings.UpdateTreeCamera();
                renderTimer = 0;
            }

            mat.SetFloat("_Blend", blend);
        }

        if (!generating && chunksToUpdate.Count > 0) {
                generationThread = new Thread(chunksToUpdate[0].GenerateData);
                generationThread.Start();
        }

        if (updated) {
            chunksToUpdate[0].FinalizeChunk();
            chunksToUpdate.RemoveAt(0);

            if (loadingLevel) {
                loadedChunks++;
                progressbar.fillAmount = loadedChunks / 16f;
                if (chunksToUpdate.Count == 0) {
                    Destroy(loadingUI);
                    loadingLevel = false;
                }
            }

            generating = false;
            updated = false;
        }

        if (newChunkPos != playerChunkPos) {
            UpdateChunk();
        }

        checkChunksTimer += Time.deltaTime;

        if (checkChunksTimer >= 30) {
            foreach (Transform child in transform) {
                if (!chunkDictionary.ContainsKey(new Vector2(child.position.x / TileSize, child.position.z / TileSize))) {
                    Destroy(child.gameObject);
                    //Debug.Log("This one does not belong here");
                }
            }

            checkChunksTimer = 0;
        }

        playerChunkPos = newChunkPos;
    }

    void UpdateChunk() {
        List<Vector2> lists = new List<Vector2>();

        for (int xOffset = -2; xOffset <= 3; xOffset++) {
            for (int yOffset = 0; yOffset <= 2; yOffset++) {
                if ((xOffset == -2 || xOffset == 3) && yOffset < 2)
                    continue;

                Vector2 viewedChunkPos = new Vector2(Mathf.FloorToInt(target.position.x / TileSize) + xOffset, Mathf.FloorToInt((target.position.z + zOffset) / TileSize) + yOffset);
                lists.Add(viewedChunkPos);

                if (!chunkDictionary.ContainsKey(viewedChunkPos)) {
                    Chunk chunk = new Chunk() {
                        chunkPos = viewedChunkPos
                    };
                    chunksToUpdate.Add(chunk);
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
            foreach(IManager manager in managers) {
                manager.RemoveAtChunk(remove[0]);
            }

            Destroy(chunkDictionary[remove[0]]);
            chunkDictionary.Remove(remove[0]);
            remove.RemoveAt(0);
        }
    }

    public void AddToChunkDictionary(Vector2 v, GameObject g) {
        if(!chunkDictionary.ContainsKey(v))
            chunkDictionary.Add(v, g);
    }

    public void PlaceTree(Vector3 pos, Transform parent, int biomeID, int layerID, Vector2 chunkPos) {
        FoliageItem foliage = biomes[biomeID].types[layerID];
        if (!foliage.isNothing) {
            System.Random prng = new System.Random((int)(pos.x + pos.y * pos.z) + biomeID);
            GameObject go = Instantiate(foliage.itemsToChoose[prng.Next(0, foliage.itemsToChoose.Length)], parent);

            float newx = (int)pos.z % 2 == 0 ? 0 : 0.5f;
            float newy = (int)pos.x % 2 == 0 ? 0 : 0.5f;
            go.transform.localPosition = pos + new Vector3((prng.Next(0, 200) - 100) * 0.001f + newx, 0f, (prng.Next(0, 200) - 100) * 0.0015f + newy);

            go.transform.localEulerAngles = new Vector3(0, (prng.Next(0, 30) - 15), (prng.Next(0, 30) - 15));
        }
    }
}

public class Chunk {
    public Vector2 chunkPos;

    List<Vector3> verts = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<int> tris = new List<int>();

    Dictionary<Vector3, int> foliageMap = new Dictionary<Vector3, int>();
    //List<Vector3> treeLoc = new List<Vector3>();

    System.Random rng;

    public void GenerateData() {
        Worldmanager.instance.generating = true;
        rng = new System.Random();

        int chunkSize = Worldmanager.instance.tileXY;

        float[,] map = Worldmanager.instance.terrainPerlin.GetPerlinMapFloat((int)chunkPos.x * Worldmanager.instance.tileXY, (int)chunkPos.y * Worldmanager.instance.tileXY, chunkSize + 1, 10);
        //float[,] map = Noise.GetHeightMap((int)chunkPos.x * Worldmanager.instance.tileXY, (int)chunkPos.y * Worldmanager.instance.tileXY, chunkSize + 1);

        float scale = Worldmanager.instance.tileScale;

        for (int y = 0; y < chunkSize; y++) {
            for (int x = 0; x < chunkSize; x++) {
                int beginX = (int)(x * scale) - (int)((chunkSize * scale) / 2);
                int beginY = (int)(y * scale) - (int)((chunkSize * scale) / 2);

                if (rng.Next(2) == 0) {
                    verts.Add(new Vector3(beginX, map[x, y], beginY));
                    verts.Add(new Vector3(beginX, map[x, y + 1], beginY + scale));
                    verts.Add(new Vector3(beginX + scale, map[x + 1, y], beginY));
                    verts.Add(new Vector3(beginX + scale, map[x + 1, y], beginY));
                    verts.Add(new Vector3(beginX, map[x, y + 1], beginY + scale));
                    verts.Add(new Vector3(beginX + scale, map[x + 1, y + 1], beginY + scale));
                } else {
                    verts.Add(new Vector3(beginX, map[x, y], beginY));
                    verts.Add(new Vector3(beginX, map[x, y + 1], beginY + scale));
                    verts.Add(new Vector3(beginX + scale, map[x + 1, y + 1], beginY + scale));
                    verts.Add(new Vector3(beginX, map[x, y], beginY));
                    verts.Add(new Vector3(beginX + scale, map[x + 1, y + 1], beginY + scale));
                    verts.Add(new Vector3(beginX + scale, map[x + 1, y], beginY));
                }

                int fileSize = 32;
                float one = 1 / (float)fileSize;
                int newRandom = 7;

                int random = rng.Next(0, newRandom);
                int rY = Mathf.FloorToInt(random / fileSize);
                int rX = random % fileSize;

                uvs.Add(new Vector2(rX * one, rY * one));
                uvs.Add(new Vector2(rX * one, one + rY * one));
                uvs.Add(new Vector2(one + rX * one, rY * one));

                random = rng.Next(0, newRandom);
                rY = Mathf.FloorToInt(random / fileSize);
                rX = random % fileSize;

                uvs.Add(new Vector2(one + rX * one, rY * one));
                uvs.Add(new Vector2(rX * one, one + rY * one));
                uvs.Add(new Vector2(one + rX * one, one + rY * one));

                int triCount = tris.Count;

                tris.Add(triCount + 0);
                tris.Add(triCount + 1);
                tris.Add(triCount + 2);
                tris.Add(triCount + 3);
                tris.Add(triCount + 4);
                tris.Add(triCount + 5);

            }
        }
        
        chunkSize = (int)(Worldmanager.instance.tileXY * Worldmanager.instance.tileScale);

        int[,] foliage = Worldmanager.instance.biomes[0].settings.GetPerlinMapInt((int)chunkPos.x * Worldmanager.instance.tileXY, (int)chunkPos.y * Worldmanager.instance.tileXY, chunkSize, Worldmanager.instance.biomes[0].types.Length, Worldmanager.instance.biomes[0].curve);//Noise.GetFoliageMap((int)chunkPos.x * Worldmanager.instance.tileXY, (int)chunkPos.y * Worldmanager.instance.tileXY, chunkSize, Worldmanager.instance.biomes[0].types.Length, Worldmanager.instance.biomes[0].layerCurve,
            //Worldmanager.instance.perlin1, Worldmanager.instance.perlin2, Worldmanager.instance.perlin3, Worldmanager.instance.perlin4,
            //Worldmanager.instance.perlin1Multiplier, Worldmanager.instance.perlin2Multiplier, Worldmanager.instance.perlin3Multiplier, Worldmanager.instance.perlin4Multiplier);
    
        for (int y = 0; y < chunkSize; y++) {
            for (int x = 0; x < chunkSize; x++) {

                int beginX = x - (int)(chunkSize / 2);
                int beginY = y - (int)(chunkSize / 2);

                int posX = (int)(x / 2);
                int posY = (int)(y / 2);

                foliageMap.Add(new Vector3(beginX, map[posX, posY], beginY), foliage[x, y]);

                //if (treemap[x, y]) {
                //    treeLoc.Add(new Vector3(beginX, map[posX, posY], beginY));
                //}
            }
        }

        Worldmanager.instance.updated = true;
    }

    public void FinalizeChunk() {
        int size = (int)(Worldmanager.instance.tileXY * Worldmanager.instance.tileScale);

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

        verts.Clear();
        uvs.Clear();
        tris.Clear();
        mesh = null;
        meshFilter = null;
        meshRenderer = null;
        meshCollider = null;

        List<Vector3> obstacle = new List<Vector3>();

        foreach (IManager manager in Worldmanager.instance.managers) {
            for (int y = 0; y < 3; y++) {
                for (int x = 0; x < 3; x++) {
                    if(manager.IsSpawn(((int)chunkPos.x * 3) + x, ((int)chunkPos.y * 3) + y)) {
                        Vector3 newLoc = new Vector3((chunkPos.x * size) + ((size * 0.33f) * x), 0, (chunkPos.y * size) + ((size * 0.33f) * y));
                        manager.Spawn(newLoc);
                        if (!manager.SpawnTrees()) {
                            obstacle.Add(newLoc);
                        }
                    }
                }
            }
        }

        foreach (KeyValuePair<Vector3, int> foli in foliageMap) {
            if(!InBounds(obstacle, chunkPos * size, foli.Key, 3.5f))
                Worldmanager.instance.PlaceTree(foli.Key, chunkObject.transform, 0, foli.Value, chunkPos);
        }
        foliageMap.Clear();

        Worldmanager.instance.AddToChunkDictionary(chunkPos, chunkObject);
        Worldmanager.instance.updated = false;
    }

    bool InBounds(List<Vector3> obstacle, Vector2 v2, Vector3 vec, float radius) {
        if (obstacle.Count <= 0)
            return false;

        foreach (Vector3 vec3 in obstacle) {
            if (Vector3.Distance(vec3, new Vector3(v2.x + vec.x, 0, v2.y + vec.z)) < radius) {
                return true;
            }
        }
        return false;
    }
}
