using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManager {

    bool IsSpawn(int x, int y);

    bool SpawnTrees();

    string Info();

    void Spawn(Vector3 pos);

    Object FromPosition(Vector3 pos, float radius);

    void RemoveAtChunk(Vector2 chunkPos);

    void Remove(Object obj);
}
