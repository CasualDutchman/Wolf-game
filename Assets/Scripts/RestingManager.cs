using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestingManager : MonoBehaviour {

    public static RestingManager instance;

    List<Transform> restingAreas = new List<Transform>();
    public GameObject restingPrefab;

    void Awake() {
        instance = this;
    }

    public void SpawnResting(Vector3 pos) {
        GameObject go = Instantiate(restingPrefab);
        go.transform.position = pos + Vector3.up;

        restingAreas.Add(go.transform);
    }

    public Transform RestingAreaAtPosition(Vector3 pos, float radius) {
        foreach (Transform t in restingAreas) {
            bool betweenX = Mathf.Abs(t.transform.position.x - pos.x) < radius;
            bool betweenZ = Mathf.Abs(t.transform.position.z - pos.z) < radius;

            if (betweenX && betweenZ) {
                return t;
            }
        }

        return null;
    }

    public void RemoveRestingAtChunk(Vector2 chunkPos) {
        Transform t = RestingAreaAtPosition(new Vector3(chunkPos.x * Worldmanager.instance.tileXY, 0, chunkPos.y * Worldmanager.instance.tileXY), Worldmanager.instance.tileXY);
        if (t != null) {
            GameObject go = t.gameObject;
            restingAreas.Remove(t);
            Destroy(go);
        }
    }
}
