using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static event Action<Vector2> OnMapChange;

    [SerializeField] private List<Level> levels;
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
                        break;
                    case 1:
                        PlaceWall(x, y);
                        break;
                    case 2:
                        SetPlayerStartPosition(x, y);
                        break;
                    case 3:
                        PlaceFinish(x, y);
                        break;
                }
            }
        }
        OnMapChange?.Invoke(_playerStartPosition);
    }

    private void SetPlayerStartPosition(int x, int y)
    {
        int flippedY = _map.GetLength(0) - 1 - y;
        _playerStartPosition = new Vector3(_tileSize.x * x, _tileSize.y * flippedY, 0);
        _playerStartPosition.y -= Camera.main.orthographicSize - _tileSize.y / 2;
        _playerStartPosition.x -= (Camera.main.orthographicSize * Camera.main.aspect) - _tileSize.x / 2;
    }

    private void PlaceFinish(int x, int y)
    {
        GameObject newSquare = Instantiate(_squarePrefab, transform);
        newSquare.transform.localScale = new Vector3(_tileSize.x, _tileSize.y);
        newSquare.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        int flippedY = _map.GetLength(0) - 1 - y;
        Vector3 newSquarePosition = new Vector3(_tileSize.x * x, _tileSize.y * flippedY, 0);
        newSquarePosition.y -= Camera.main.orthographicSize - _tileSize.y / 2;
        newSquarePosition.x -= (Camera.main.orthographicSize * Camera.main.aspect) - _tileSize.x / 2;
        newSquare.transform.position = newSquarePosition;
        newSquare.AddComponent<BoxCollider2D>().isTrigger = true;
        newSquare.AddComponent<Finish>();
        newSquare.name = "Finish";
    }
    
    private void PlaceWall(int x, int y)
    {
        GameObject newSquare = Instantiate(_squarePrefab, transform);
        newSquare.transform.localScale = new Vector3(_tileSize.x, _tileSize.y);
        newSquare.GetComponent<SpriteRenderer>().color = Color.black;
        int flippedY = _map.GetLength(0) - 1 - y;
        Vector3 newSquarePosition = new Vector3(_tileSize.x * x, _tileSize.y * flippedY, 0);
        newSquarePosition.y -= Camera.main.orthographicSize - _tileSize.y / 2;
        newSquarePosition.x -= (Camera.main.orthographicSize * Camera.main.aspect) - _tileSize.x / 2;
        newSquare.transform.position = newSquarePosition;
        newSquare.AddComponent<BoxCollider2D>().isTrigger = true;
        newSquare.AddComponent<Wall>();
        newSquare.name =  "Wall";
    }
}
