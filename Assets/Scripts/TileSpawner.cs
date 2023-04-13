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
        CreateTile(dungeonManager.GetTilePrefab(TilePrefabType.Floor), transform.position);

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
                    CreateTile(dungeonManager.GetTilePrefab(TilePrefabType.Wall), testPos);
                }
            }
        }



        Destroy(gameObject);
    }

    private void CreateTile(GameObject tilePrefab, Vector3 position)
    {
        var tile = Instantiate(tilePrefab, position, Quaternion.identity);
        tile.name = tilePrefab.name;
        tile.transform.SetParent(dungeonManager.transform);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.8f);
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}
