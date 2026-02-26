using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level", order = 1)]
public class Level : ScriptableObject
{
    [Min(1)] public int width = 32;
    [Min(1)] public int height = 18;

    // Flattened: index = x + y * width
    [SerializeField] private List<int> cells = new();

    public float tickTimer;

    public int Get(int x, int y)
    {
        if (!InBounds(x, y)) return 0;
        EnsureSize();
        return cells[x + y * width];
    }

    public void Set(int x, int y, int value)
    {
        if (!InBounds(x, y)) return;
        EnsureSize();
        cells[x + y * width] = value;
    }

    public int[,] To2DArray()
    {
        EnsureSize();
        var map = new int[height, width];
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            map[y, x] = cells[x + y * width];
        return map;
    }

    public void Clear(int value = 0)
    {
        EnsureSize();
        for (int i = 0; i < cells.Count; i++) cells[i] = value;
    }

    bool InBounds(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;

    void EnsureSize()
    {
        int target = width * height;
        if (cells == null) cells = new List<int>(target);

        if (cells.Count < target)
        {
            while (cells.Count < target) cells.Add(0);
        }
        else if (cells.Count > target)
        {
            cells.RemoveRange(target, cells.Count - target);
        }
    }

    // Optional: useful for other scripts that want raw cells
    public IReadOnlyList<int> RawCells
    {
        get { EnsureSize(); return cells; }
    }
}