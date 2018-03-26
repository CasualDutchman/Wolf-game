using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestingManager : MonoBehaviour, IManager {

    public static RestingManager instance;

    List<Transform> restingAreas = new List<Transform>();
    public GameObject restingPrefab;

    void Awake() {
        instance = this;
    }

    public int GetBitID() {
        return 2;
    }

    public void Spawn(Vector3 pos) {
        GameObject go = Instantiate(restingPrefab);
        go.transform.position = pos + Vector3.up;

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
