using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DungeonManager : MonoBehaviour
{
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public GameObject SpawnerPrefab;
    public GameObject ExitPrefab;

    [Range(200, 10000)] public int MaxTileCount;
    
    [Range(0, 100)] public int RandomItemChance;
    [Range(0, 100)] public int RandomEnemyChance;

    public GameObject[] RandomItems;
    public GameObject[] RandomEnemies;
    public GameObject[] WallTrims;

    public DungeonType DungeonType;

    private int MinX { get; set; }
    private int MaxX { get; set; }
    private int MinY { get; set; }
    private int MaxY { get; set; }

    private Dictionary<Vector3, PrefabType> _tiles = null;

    private void Start()
    {
        switch (this.DungeonType)
        {
            case DungeonType.Caverns :  RandomWalker(); break;
            case DungeonType.Rooms :    RoomWalker();   break;
        }
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

    private void RoomWalker()
    {
        var currentPos = Vector3.zero;
        _tiles = new Dictionary<Vector3, PrefabType>();

        var prevDir = Vector3.zero;
        var dir = Vector3.zero;

        while (_tiles.Count < this.MaxTileCount)
        {
            do
            {
                dir = GetRandomDirectionVector();
            }
            while (prevDir + dir == Vector3.zero);
            
            prevDir = dir;

            int walkLen = Random.Range(9, 18);

            for (int x = 0; x < walkLen; x++)
            {
                if (_tiles.TryAdd(currentPos, PrefabType.Floor))
                {
                    DungeonManager.CreatePrefab(PrefabType.Spawner, currentPos, this);
                    SetMinMaxDimensions(currentPos);
                }

                currentPos += dir;
            }

            GenerateRoom(currentPos);
        }
        
        PadMinMaxDimensions(1);

        StartCoroutine(AfterSpawnersDestroyed());
    }

    private void GenerateRoom(Vector3 center)
    {
        int roomWidth = Random.Range(1, 5);
        int roomHeight = Random.Range(1, 5);

        for (int y = -roomHeight; y <= roomHeight; y++)
        {
            for (int x = -roomWidth; x <= roomWidth; x++)
            {
                var roomPos = new Vector3(center.x + x, center.y + y, center.z);

                if (_tiles.TryAdd(roomPos, PrefabType.Floor))
                {
                    DungeonManager.CreatePrefab(PrefabType.Spawner, roomPos, this);
                    SetMinMaxDimensions(roomPos);
                }
            }
        }
    }

    private void RandomWalker()
    {
        var currentPos = Vector3.zero;
        _tiles = new Dictionary<Vector3, PrefabType>();

        while (_tiles.Count < this.MaxTileCount)
        {           
            if (_tiles.TryAdd(currentPos, PrefabType.Floor))
            {
                DungeonManager.CreatePrefab(PrefabType.Spawner, currentPos, this);
                SetMinMaxDimensions(currentPos);
            }

            currentPos += GetRandomDirectionVector();
        }
        
        PadMinMaxDimensions(1);

        StartCoroutine(AfterSpawnersDestroyed());
    }

    private IEnumerator AfterSpawnersDestroyed()
    {
        // Wait for walls to finish...
        while (FindFirstObjectByType<TileSpawner>() != null)
        {
            yield return null;
        }

        FindWallTiles();

        // Exit...
        var exitCoords = _tiles.Where(x => (x.Value == PrefabType.Floor) && (x.Key != Vector3.zero)).ChooseRandomElement().Key;
        DungeonManager.CreatePrefab(PrefabType.Exit, exitCoords, this);

        var floorTiles = _tiles.Where(x => x.Value == PrefabType.Floor).Select(x => x.Key).ToArray();
        var wallTiles = _tiles.Where(x => x.Value == PrefabType.Wall).Select(x => x.Key).ToArray();

        // Wall Trim...
        foreach (var wall in wallTiles)
        {
            int trimIndex = 0;
            
            /*
            trimIndex += floorTiles.Contains(new Vector3(wall.x, wall.y + 1, wall.z)) ? 1 : 0;
            trimIndex += floorTiles.Contains(new Vector3(wall.x + 1, wall.y, wall.z)) ? 2 : 0;
            trimIndex += floorTiles.Contains(new Vector3(wall.x, wall.y - 1, wall.z)) ? 4 : 0;
            trimIndex += floorTiles.Contains(new Vector3(wall.x - 1, wall.y, wall.z)) ? 8 : 0;
            */

            trimIndex += !wallTiles.Contains(new Vector3(wall.x, wall.y + 1, wall.z)) ? 1 : 0;
            trimIndex += !wallTiles.Contains(new Vector3(wall.x + 1, wall.y, wall.z)) ? 2 : 0;
            trimIndex += !wallTiles.Contains(new Vector3(wall.x, wall.y - 1, wall.z)) ? 4 : 0;
            trimIndex += !wallTiles.Contains(new Vector3(wall.x - 1, wall.y, wall.z)) ? 8 : 0;

            CreateWallTrim(trimIndex, wall);
        }

        // Items...
        var itemLocs = new List<Vector3> { new Vector3(0f, 0f, transform.position.z) };

        foreach (var floor in floorTiles.Except(new[] { Vector3.zero, exitCoords }))
        {            
            if (Global.Random.Success(this.RandomItemChance))
            {
                itemLocs.Add(AddRandomItem(floor).transform.position);
            }
        }

        // Enemies...
        var openFloorList = floorTiles.Except(itemLocs).Except(new[] { Vector3.zero, exitCoords }).ToArray();

        foreach (var openFloor in openFloorList)
        {
            if (Global.Random.Success(this.RandomEnemyChance))
            {
                AddRandomEnemy(openFloor);
            }
        }
    }

    private void FindWallTiles()
    {
        for (int y = MinY; y <= MaxY; y++)
        {
            for (int x = MinX; x <= MaxX; x++)
            {
                var testPos = new Vector3(x, y, transform.position.z);
                if (Global.TestTileCollision(testPos, LayerMaskType.Wall))
                {
                    if (!_tiles.TryAdd(testPos, PrefabType.Wall))
                    {
                        throw new Exception($"Duplicate tile position found while scanning for walls ({x}, {y})");
                    }
                }
            }
        }
    }

    private GameObject AddRandomItem(Vector3 position) => Instantiate(RandomItems.ChooseRandomElement(), position, Quaternion.identity);

    private GameObject AddRandomEnemy(Vector3 position) => Instantiate(RandomEnemies.ChooseRandomElement(), position, Quaternion.identity);

    public GameObject GetPrefab(PrefabType type)
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

    private void SetMinMaxDimensions(Vector3 location)
    {
        this.MinX = this.MinX <= location.x ? this.MinX : (int)location.x;
        this.MinY = this.MinY <= location.y ? this.MinY : (int)location.y;
        this.MaxX = this.MaxX >= location.x ? this.MaxX : (int)location.x;
        this.MaxY = this.MaxY >= location.y ? this.MaxY : (int)location.y;
    }

    private void PadMinMaxDimensions(int padding)
    {
        this.MinX -= padding;
        this.MinY -= padding;
        this.MaxX += padding;
        this.MaxY += padding;
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

    private GameObject CreateWallTrim(int trimIndex, Vector3 position)
    {
        if (trimIndex <= 0)
            return null;

        var prefab = this.WallTrims[trimIndex];
        var obj = Instantiate(prefab, position, Quaternion.identity);
        obj.transform.SetParent(transform);
        return obj;
    }

    public static GameObject CreatePrefab(PrefabType type, Vector3 position, DungeonManager dungeonManager)
    {
        var prefab = dungeonManager.GetPrefab(type);
        var obj = Instantiate(prefab, position, Quaternion.identity);
        obj.transform.SetParent(dungeonManager.transform);
        return obj;
    }
}
