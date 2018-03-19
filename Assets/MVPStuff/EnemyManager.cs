using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public static EnemyManager instance;

    public GameObject enemyPrefab;

    public Transform[] spawnLocations;

    public List<Enemy> enemies = new List<Enemy>();

    public float timeBetweenSpawns;
    public int maxEnemies = 15;
    int enemyCount;

    float timer;

	void Awake () {
        instance = this;
	}
	
	void Update () {
        if (enemyCount < maxEnemies) {
            timer += Time.deltaTime;
            if (timer >= timeBetweenSpawns) {
                timer -= timeBetweenSpawns;

                GameObject go = Instantiate(enemyPrefab, spawnLocations[Random.Range(0, spawnLocations.Length)].position, Quaternion.identity);
                enemies.Add(go.GetComponent<Enemy>());
                enemyCount++;
            }
        }
	}

    public void RemoveEnemy(Enemy enemy) {
        enemies.Remove(enemy);
    }

    public Enemy EnemyAtPosition(Vector3 pos, float radius) {
        foreach (Enemy enemy in enemies) {
            bool betweenX = Mathf.Abs(enemy.transform.position.x - pos.x) < radius;
            bool betweenZ = Mathf.Abs(enemy.transform.position.z - pos.z) < radius;

            if (betweenX && betweenZ) {
                return enemy;
            }
        }

        return null;
    }

    void OnDrawGizmos() {
        foreach (Transform t in spawnLocations) {
            Gizmos.DrawCube(t.position, Vector3.one);
        }
    }
}
