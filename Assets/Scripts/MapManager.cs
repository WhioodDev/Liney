using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public enum EBlocks
{
    Air,
    Wall,
    Finish
}

public class MapManager : MonoBehaviour
{
    public static event Action<Vector2, float> OnMapChange;

    [SerializeField] private List<Level> levels;
    [SerializeField] private List<Blocks> blocks;
    [SerializeField] private GameManager gameManager;
    
    private Vector2 _playerStartPosition;
    private GameObject _squarePrefab;
    private int _currentLevel;
    private Vector2 _tileSize;
    private int[,] _map;

    private void Awake()
    {
        GameManager.OnTileSizeChange += GenerateMap;
    }

    private void Start()
    {
        // Initialize square prefab
        _squarePrefab = Resources.Load<GameObject>("Square");

        GenerateMap();
    }

    private void OnDisable()
    {
        GameManager.OnTileSizeChange -= GenerateMap;
    }

    private void DeleteAllBlocks()
    {
        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void GenerateMap()
    {
        // Calculate the tile size
        _tileSize = gameManager.GetTileSize();
        
        DeleteAllBlocks();
        _map = levels[_currentLevel].To2DArray();
        for (int y = 0; y < _map.GetLength(0); y++)
        {
            for (int x = 0; x < _map.GetLength(1); x++)
            {
                switch (_map[y, x])
                {
                    case 0:
                        PlaceBlock(x, y, EBlocks.Air);
                        break;
                    case 1:
                        PlaceBlock(x, y, EBlocks.Wall);
                        break;
                    case 2:
                        PlaceBlock(x, y, EBlocks.Air);
                        SetPlayerStartPosition(x, y);
                        break;
                    case 3:
                        PlaceBlock(x, y, EBlocks.Finish);
                        break;
                }
            }
        }
        OnMapChange?.Invoke(_playerStartPosition, levels[_currentLevel].tickTimer);
    }

    private void SetPlayerStartPosition(int x, int y)
    {
        int flippedY = _map.GetLength(0) - 1 - y;
        _playerStartPosition = new Vector3(_tileSize.x * x, _tileSize.y * flippedY, 0);
        _playerStartPosition.y -= Camera.main.orthographicSize - _tileSize.y / 2;
        _playerStartPosition.x -= (Camera.main.orthographicSize * Camera.main.aspect) - _tileSize.x / 2;
    }
    
    private void PlaceBlock(int x, int y, EBlocks block)
    {
        GameObject newSquare = Instantiate(_squarePrefab, transform);
        newSquare.transform.localScale = new Vector3(_tileSize.x, _tileSize.y);
        newSquare.GetComponent<SpriteRenderer>().color = blocks[(int)block].blockColor;
        int flippedY = _map.GetLength(0) - 1 - y;
        Vector3 newSquarePosition = new Vector3(_tileSize.x * x, _tileSize.y * flippedY, 0);
        newSquarePosition.y -= Camera.main.orthographicSize - _tileSize.y / 2;
        newSquarePosition.x -= (Camera.main.orthographicSize * Camera.main.aspect) - _tileSize.x / 2;
        newSquare.transform.position = newSquarePosition;
        newSquare.AddComponent<BoxCollider2D>().isTrigger = true;
        if (blocks[(int)block].blockLogic != null)
            newSquare.AddComponent(blocks[(int)block].blockLogic.GetClass());
        newSquare.name = blocks[(int)block].name;
    }
}
