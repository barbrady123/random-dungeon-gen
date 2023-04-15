using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    private DungeonManager dungeonManager;

    private void Awake()
    {
        dungeonManager = FindObjectOfType<DungeonManager>();
        DungeonManager.CreatePrefab(PrefabType.Floor, transform.position, dungeonManager);
    }

    void Start()
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                var testPos = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z);
                if (!Global.TestTileCollision(testPos, LayerMaskType.WallFloor))
                {
                    DungeonManager.CreatePrefab(PrefabType.Wall, testPos, dungeonManager);
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.8f);
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}
