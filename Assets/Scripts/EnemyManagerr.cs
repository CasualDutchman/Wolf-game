using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagerr : MonoBehaviour {

    public static EnemyManagerr instance;

    List<Enemy> enemies = new List<Enemy>();
    public GameObject enemyPrefab;

	void Awake () {
        instance = this;
	}
	
	public void SpawnEnemy(Vector3 pos) {
        GameObject go = Instantiate(enemyPrefab);
        go.transform.position = pos;

        enemies.Add(go.GetComponent<Enemy>());
    }

    public Enemy FromPosition(Vector3 pos, float radius) {
        foreach (Enemy enemy in enemies) {
            bool betweenX = Mathf.Abs(enemy.transform.position.x - pos.x) < radius;
            bool betweenZ = Mathf.Abs(enemy.transform.position.z - pos.z) < radius;

            if (betweenX && betweenZ) {
                return enemy;
            }
        }

        return null;
    }

    public void RemoveEnemyAtChunk(Vector2 chunkPos) {
        Enemy enemy = FromPosition(new Vector3(chunkPos.x * Worldmanager.instance.tileXY, 0, chunkPos.y * Worldmanager.instance.tileXY), Worldmanager.instance.tileXY);
        if (enemy != null) {
            GameObject go = enemy.gameObject;
            enemies.Remove(enemy);
            Destroy(go);
        }
    }

    public void RemoveEnemy(Enemy enemy) {
        enemies.Remove(enemy);
    }
}
