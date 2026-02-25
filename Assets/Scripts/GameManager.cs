using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float tileSizeWidth;
    public float tileSizeHeight;

    [SerializeField] private MapManager mapManager;

    private void Awake()
    {
        if (Camera.main == null) return;

        Camera cam = Camera.main;

        // World size of camera
        float worldHeight = cam.orthographicSize * 2f;
        float worldWidth = worldHeight * cam.aspect;

        // Divide by grid size
        tileSizeWidth = worldWidth / 32;
        tileSizeHeight = worldHeight / 18;
    }
}