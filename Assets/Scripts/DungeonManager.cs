using System;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public GameObject FloorPrefab;
    public GameObject WallPrefab;

    // Do we need these public?
    private float MinX { get; set; } = float.MaxValue;
    private float MaxX { get; set; } = float.MaxValue;
    private float MinY { get; set; } = float.MinValue;
    private float MaxY { get; set; } = float.MinValue;

    public GameObject GetTilePrefab(TilePrefabType type)
    {
        return type switch
        {
            TilePrefabType.Floor => this.FloorPrefab,
            TilePrefabType.Wall => this.WallPrefab,
            _ => throw new Exception($"Invalid tile prefab requested '{type}'")
        };
    }

    public void SetMinMaxDimensions(Vector3 location)
    {
        this.MinX = this.MinX <= location.x ? this.MinX : location.x;
        this.MinY = this.MinY <= location.y ? this.MinY : location.y;
        this.MaxX = this.MaxX >= location.x ? this.MaxX : location.x;
        this.MaxY = this.MaxY >= location.y ? this.MaxY : location.y;
    }
}
