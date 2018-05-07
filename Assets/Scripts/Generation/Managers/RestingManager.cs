using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestingManager : MonoBehaviour, IManager {

    public static RestingManager instance;

    public Noise restingNoise;

    List<Transform> restingAreas = new List<Transform>();
    public GameObject restingPrefab;

    void Awake() {
        instance = this;
    }

    public bool IsSpawn(int x, int y) {
        return restingNoise.IsOn(x, y);
    }

    public bool SpawnTrees() {
        return false;
    }

    public string Info() {
        return "R";
    }

    public void Spawn(Vector3 pos) {
        Ray ray = new Ray(pos + Vector3.up * 10, Vector3.down);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 15, LayerMask.GetMask("Water"));
        GameObject go = Instantiate(restingPrefab);
        go.transform.position = hit.point;

        restingAreas.Add(go.transform);
    }

    public Object FromPosition(Vector3 pos, float radius) {
        foreach (Transform t in restingAreas) {
            bool betweenX = Mathf.Abs(t.transform.position.x - pos.x) < radius;
            bool betweenZ = Mathf.Abs(t.transform.position.z - pos.z) < radius;

            if (betweenX && betweenZ) {
                return t;
            }
        }

        return null;
    }

    public void RemoveAtChunk(Vector2 chunkPos) {
        Transform t = (Transform)FromPosition(new Vector3(chunkPos.x * Worldmanager.instance.TileSize, 0, chunkPos.y * Worldmanager.instance.TileSize), Worldmanager.instance.TileSize);
        if (t != null) {
            GameObject go = t.gameObject;
            restingAreas.Remove(t);
            Destroy(go);
        }
    }

    public void Remove(Object obj) {
        restingAreas.Remove((Transform)obj);
    }
}
