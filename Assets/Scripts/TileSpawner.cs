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
        TileSpawner.CreateTile(dungeonManager.GetTilePrefab(PrefabType.Floor), transform.position, dungeonManager);

        dungeonManager.SetMinMaxDimensions(transform.position);
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
                    TileSpawner.CreateTile(dungeonManager.GetTilePrefab(PrefabType.Wall), testPos, dungeonManager);
                }
            }
        }

        Destroy(gameObject);
    }

    public static GameObject CreateTile(GameObject tilePrefab, Vector3 position, DungeonManager dungeonManager)
    {
        var tile = Instantiate(tilePrefab, position, Quaternion.identity);
        tile.name = tilePrefab.name;
        tile.transform.SetParent(dungeonManager.transform);
        return tile;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.8f);
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}
