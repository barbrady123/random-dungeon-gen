using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public GameObject SpawnerPrefab;
    public GameObject ExitPrefab;

    [Range(20, 1000)] public int MaxTileCount;
    [Range(0, 100)] public int RandomItemChance;

    public GameObject[] RandomItems;

    private HashSet<Vector3> _floorList = null;

    private int MinX { get; set; } = Int32.MaxValue;
    private int MaxX { get; set; } = Int32.MaxValue;
    private int MinY { get; set; } = Int32.MinValue;
    private int MaxY { get; set; } = Int32.MinValue;

    private void Start()
    {
        RandomWalker();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GenerateDungeon();
        }
    }

    public void GenerateDungeon()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void RandomWalker()
    {
        var currentPos = Vector3.zero;
        _floorList = new HashSet<Vector3> { currentPos };

        while (_floorList.Count < this.MaxTileCount)
        {
            currentPos += GetRandomDirectionVector();
            _floorList.Add(currentPos);
        }

        foreach (var floor in _floorList)
        {
            TileSpawner.Create(GetTilePrefab(PrefabType.Spawner), floor, this);
        }

        StartCoroutine(AfterSpawnersDestroyed(_floorList));
    }

    private IEnumerator AfterSpawnersDestroyed(IEnumerable<Vector3> floorList)
    {
        while (FindFirstObjectByType<TileSpawner>() != null)
        {
            yield return null;
        }

        TileSpawner.Create(GetTilePrefab(PrefabType.Exit), floorList.Last(), this);

        foreach (var floor in floorList.Skip(1).SkipLast(1))
        {
            if (UnityEngine.Random.Range(0, 100) < this.RandomItemChance)
            {
                AddRandomItem(floor);
            }
        }
    }

    private void AddRandomItem(Vector3 position)
    {
        Instantiate(RandomItems.ChooseRandomElement(), position, Quaternion.identity);
    }

    public GameObject GetTilePrefab(PrefabType type)
    {
        return type switch
        {
            PrefabType.Floor => this.FloorPrefab,
            PrefabType.Wall => this.WallPrefab,
            PrefabType.Spawner => this.SpawnerPrefab,
            PrefabType.Exit => this.ExitPrefab,
            _ => throw new Exception($"Invalid prefab requested '{type}'")
        };
    }

    public void SetMinMaxDimensions(Vector3 location)
    {
        this.MinX = this.MinX <= location.x ? this.MinX : (int)location.x;
        this.MinY = this.MinY <= location.y ? this.MinY : (int)location.y;
        this.MaxX = this.MaxX >= location.x ? this.MaxX : (int)location.x;
        this.MaxY = this.MaxY >= location.y ? this.MaxY : (int)location.y;
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
