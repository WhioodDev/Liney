using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action OnTileSizeChange;
    
    private Vector2 _currentTileSize;

    private void Start()
    {
        _currentTileSize = GetTileSize();
    }

    public Vector2 GetTileSize()
    {
        Camera cam = Camera.main;
        return new Vector2(cam.orthographicSize * 2f * cam.aspect / 32, cam.orthographicSize * 2f / 18);
    }

    private void Update()
    {
        if (_currentTileSize != GetTileSize())
        {
            _currentTileSize = GetTileSize();
            OnTileSizeChange?.Invoke();
        }
    }

    public void Finish()
    {
        
    }
}