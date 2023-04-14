using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public GameObject SpawnerPrefab;

    public int MaxTileCount;

    private HashSet<Vector3> floorList = null;

    // Do we need these public?
    private float MinX { get; set; } = float.MaxValue;
    private float MaxX { get; set; } = float.MaxValue;
    private float MinY { get; set; } = float.MinValue;
    private float MaxY { get; set; } = float.MinValue;

    private void Start()
    {
        RandomWalker();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void RandomWalker()
    {
        var currentPos = Vector3.zero;
        floorList = new HashSet<Vector3> { currentPos };

        while (floorList.Count < this.MaxTileCount)
        {
            currentPos += GetRandomDirectionVector();
            floorList.Add(currentPos);
        }

        foreach (var floor in floorList)
        {
            TileSpawner.CreateTile(this.SpawnerPrefab, floor, this);
        }
    }

    public GameObject GetTilePrefab(PrefabType type)
    {
        return type switch
        {
            PrefabType.Floor => this.FloorPrefab,
            PrefabType.Wall => this.WallPrefab,
            PrefabType.Spawner => this.SpawnerPrefab,
            _ => throw new Exception($"Invalid prefab requested '{type}'")
        };
    }

    public void SetMinMaxDimensions(Vector3 location)
    {
        this.MinX = this.MinX <= location.x ? this.MinX : location.x;
        this.MinY = this.MinY <= location.y ? this.MinY : location.y;
        this.MaxX = this.MaxX >= location.x ? this.MaxX : location.x;
        this.MaxY = this.MaxY >= location.y ? this.MaxY : location.y;
    }

    private Vector3 GetRandomDirectionVector()
    {
        return UnityEngine.Random.Range(0, 4) switch
        {
            0 => Vector3.up,
            1 => Vector3.right,
            2 => Vector3.down,
            3 => Vector3.left,
            _ => Vector3.zero   // Impossible
        };
    }
}
