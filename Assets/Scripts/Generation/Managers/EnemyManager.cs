using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IManager {

    public static EnemyManager instance;

    public Noise enemyNoise;

    public Transform cameraRig;

    List<Enemy> enemies = new List<Enemy>();
    public GameObject[] enemyPrefab;

	void Awake () {
        instance = this;
	}
	
    public bool IsSpawn(int x, int y) {
        return enemyNoise.IsOn(x, y);
    }

    public bool SpawnTrees() {
        return true;
    }

    public string Info() {
        return "E";
    }

    public void Spawn(Vector3 pos) {
        GameObject go = Instantiate(enemyPrefab[Random.Range(0, enemyPrefab.Length)]);
        go.transform.position = pos;
        go.GetComponent<Enemy>().cameraRig = cameraRig;

        enemies.Add(go.GetComponent<Enemy>());
    }

    public Object[] FromPosition(Vector3 pos, float radius) {
        List<Enemy> list = new List<Enemy>();
        foreach (Enemy enemy in enemies) {

            bool betweenX = Mathf.Abs(enemy.transform.position.x - pos.x) < radius;
            bool betweenZ = Mathf.Abs(enemy.transform.position.z - pos.z) < radius;

            if (betweenX && betweenZ) {
                list.Add(enemy);
            }
        }

        return list.ToArray();
    }

    public void RemoveAtChunk(Vector2 chunkPos) {
        Enemy[] enemys = (Enemy[])FromPosition(new Vector3(chunkPos.x * Worldmanager.instance.TileSize, 0, chunkPos.y * Worldmanager.instance.TileSize), Worldmanager.instance.TileSize);
        foreach (Enemy item in enemys) {
            GameObject go = item.gameObject;
            enemies.Remove(item);
            Destroy(go);
        }
    }

    public void Remove(Object obj) {
        enemies.Remove((Enemy)obj);
    }
}
