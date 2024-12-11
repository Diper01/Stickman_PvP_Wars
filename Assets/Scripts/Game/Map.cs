using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Map : MonoBehaviour {

    public static Map Instance = null;

    public byte[,] ByteGrid {
        get {
            return byteGrid;
        }
    }
    public List<Tile> MoveNodes {
        get { return moveNodes; }
    }
    public List<Tile> AmunitionNodes {
        get { return amunitionNodes; }
    }
    public List<Tile> SpawnPositions {
        get { return spawnPositions; }
    }
    public List<Tile> RedSpawnPositions {
        get { return redSpawnPositions; }
    }
    public List<Tile> BlueSpawnPosition {
        get { return blueSpawnPosition; }
    }

    [SerializeField] Color gridColor;
    [SerializeField] int gridHeight;
    [SerializeField] int gridWidth;
    [SerializeField] List<Tile> redSpawnPositions;
    [SerializeField] List<Tile> blueSpawnPosition;

    private byte[,] byteGrid;
    private List<Tile> moveNodes;
    private List<Tile> amunitionNodes;
    private List<Tile> spawnPositions;

    private void OnDrawGizmos()
    {
        for (int x = 0; x <= gridWidth; x++)
        {
            Gizmos.color = gridColor;
            Gizmos.DrawLine(new Vector3(x, 0, 0), new Vector3(x, gridHeight, 0));
        }
        for (int y = 0; y <= gridHeight; y++)
        {
            Gizmos.color = gridColor;
            Gizmos.DrawLine(new Vector3(0, y, 0), new Vector3(gridWidth, y, 0));
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            Init();
        }
    }

    private void Init()
    {
        moveNodes = new List<Tile>();
        amunitionNodes = new List<Tile>();
        spawnPositions = new List<Tile>();
        byteGrid = new byte[Mathf.NextPowerOfTwo(gridWidth), Mathf.NextPowerOfTwo(gridHeight)];
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                byteGrid[x, y] = 1;
            }
        }

        Tile[] tiles = FindObjectsOfType<Tile>();

        foreach (var tile in tiles)
        {
            if (tile.X >= gridWidth || tile.X < 0 || tile.Y >= gridHeight || tile.Y < 0)
                continue;

            if (tile.Type == TileType.Block)
                byteGrid[tile.X, tile.Y] = 0;
            else if (tile.Type == TileType.MoveNode)
                moveNodes.Add(tile);
            else if (tile.Type == TileType.AmunitionNode)
                amunitionNodes.Add(tile);
            else if (tile.Type == TileType.SpawnPosition)
                spawnPositions.Add(tile);
        }
    }

    public bool IsTileOnGround(int x, int y) {
        if (byteGrid[x, y - 1] == 0)        
            return true;        
        else
            return false;        
    }
}
