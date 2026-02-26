using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action OnTileSizeChange;
    public static event Action OnTick;
    
    private float _tickTime;
    private Vector2 _currentTileSize;
    private float _tickTimer;

    private void Awake()
    {
        MapManager.OnMapChange += OnMapChange;
    }

    private void Start()
    {
        _currentTileSize = GetTileSize();
    }

    public Vector2 GetTileSize()
    {
        Camera cam = Camera.main;
        return new Vector2(cam.orthographicSize * 2f * cam.aspect / 32, cam.orthographicSize * 2f / 18);
    }

    private void OnDisable()
    {
        MapManager.OnMapChange -= OnMapChange;
    }

    private void Update()
    {
        _tickTimer += Time.deltaTime;
        if (_currentTileSize != GetTileSize())
        {
            _currentTileSize = GetTileSize();
            OnTileSizeChange?.Invoke();
        }

        if (_tickTimer >= _tickTime)
        {
            _tickTimer = 0;
            OnTick?.Invoke();
        }
    }

    private void OnMapChange(Vector2 mapSize, float tickTime)
    {
        _tickTime = tickTime;
    }

    public void Finish()
    {
        print(1);
    }
}