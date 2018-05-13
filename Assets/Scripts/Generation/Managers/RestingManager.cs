using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//cougar, reindeer, fox, moose

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
        GameObject go = Instantiate(restingPrefab);

        Ray ray = new Ray(pos + Vector3.up * 10, Vector3.down);
        RaycastHit hit = new RaycastHit();
        Physics.Raycast(ray, out hit, 15, LayerMask.GetMask("Water"));
        go.transform.position = hit.point;

        foreach (Transform child in go.transform) {
            ray = new Ray(child.transform.position + Vector3.up * 10, Vector3.down);
            hit = new RaycastHit();
            Physics.Raycast(ray, out hit, 15, LayerMask.GetMask("Water"));
            child.position = hit.point;
        }

        //Ray ray = new Ray(pos + Vector3.up * 10, Vector3.down);
        //RaycastHit hit;
        //Physics.Raycast(ray, out hit, 15, LayerMask.GetMask("Water"));
        //GameObject go = Instantiate(restingPrefab);
        //go.transform.position = hit.point;

        restingAreas.Add(go.transform);
    }

    public Object[] FromPosition(Vector3 pos, float radius) {
        List<Transform> list = new List<Transform>();
        foreach (Transform t in restingAreas) {
            bool betweenX = Mathf.Abs(t.transform.position.x - pos.x) < radius;
            bool betweenZ = Mathf.Abs(t.transform.position.z - pos.z) < radius;

            if (betweenX && betweenZ) {
                list.Add(t);
            }
        }

        return list.ToArray();
    }

    public void RemoveAtChunk(Vector2 chunkPos) {
        Transform[] t = (Transform[])FromPosition(new Vector3(chunkPos.x * Worldmanager.instance.TileSize, 0, chunkPos.y * Worldmanager.instance.TileSize), Worldmanager.instance.TileSize);
        foreach (Transform item in t) {
            GameObject go = item.gameObject;
            restingAreas.Remove(item);
            Destroy(go);
        }
    }

    public void Remove(Object obj) {
        restingAreas.Remove((Transform)obj);
    }
}
