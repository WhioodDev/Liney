using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    const int Max20 = 26;
    const int Min20 = 14;

    int paintValue = 1;
    bool showNumbers = true;

    public override void OnInspectorGUI()
    {
        var level = (Level)target;

        // Default fields for width/height
        EditorGUI.BeginChangeCheck();
        level.width = EditorGUILayout.IntField("Width", level.width);
        level.height = EditorGUILayout.IntField("Height", level.height);
        level.width = Mathf.Max(1, level.width);
        level.height = Mathf.Max(1, level.height);

        // Simple paint controls
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Paint", EditorStyles.boldLabel);
        paintValue = EditorGUILayout.IntField("Paint Value", paintValue);
        showNumbers = EditorGUILayout.Toggle("Show Numbers", showNumbers);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear (0)")) { Undo.RecordObject(level, "Clear Level"); level.Clear(0); EditorUtility.SetDirty(level); }
        if (GUILayout.Button("Fill (Paint Value)")) { Undo.RecordObject(level, "Fill Level"); Fill(level, paintValue); EditorUtility.SetDirty(level); }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);

        // Draw grid
        DrawGrid(level);

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(level);
        }
    }

    void DrawGrid(Level level)
    {
        // Input handling: click/drag to paint
        var e = Event.current;

        // Calculate total grid rect
        float gridW = level.width * 20;
        float gridH = level.height * 20;
        var gridRect = GUILayoutUtility.GetRect(gridW, gridH, GUILayout.ExpandWidth(false));

        // Background
        EditorGUI.DrawRect(gridRect, new Color(0.12f, 0.12f, 0.12f, 1f));

        bool isPainting = (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0;
        bool isErasing = (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 1;

        // Paint if mouse is inside
        if ((isPainting || isErasing) && gridRect.Contains(e.mousePosition))
        {
            int x = Mathf.FloorToInt((e.mousePosition.x - gridRect.x) / 20);
            int y = Mathf.FloorToInt((e.mousePosition.y - gridRect.y) / 20);

            Undo.RecordObject(level, "Paint Cell");
            level.Set(x, y, isErasing ? 0 : paintValue);
            EditorUtility.SetDirty(level);
            e.Use();
        }

        // Draw cells
        for (int y = 0; y < level.height; y++)
        {
            for (int x = 0; x < level.width; x++)
            {
                int v = level.Get(x, y);
                var r = new Rect(
                    gridRect.x + x * 20,
                    gridRect.y + y * 20,
                    20 - 1,
                    20 - 1
                );

                // Color by value (simple + readable)
                var col = ValueToColor(v);
                EditorGUI.DrawRect(r, col);

                if (showNumbers && 20 >= 18)
                {
                    var style = new GUIStyle(EditorStyles.miniLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        normal = { textColor = (col.grayscale > 0.5f) ? Color.black : Color.white }
                    };
                    GUI.Label(r, v.ToString(), style);
                }
            }
        }

        // Hint
        EditorGUILayout.HelpBox("Left-click/drag to paint. Right-click/drag to erase (set to 0).", MessageType.Info);
    }

    void Fill(Level level, int value)
    {
        for (int y = 0; y < level.height; y++)
            for (int x = 0; x < level.width; x++)
                level.Set(x, y, value);
    }

    Color ValueToColor(int v)
    {
        // 0 = dark
        if (v == 0) return new Color(0.20f, 0.20f, 0.20f, 1f);

        // A few distinct steps for quick visual parsing
        return v switch
        {
            1 => new Color(0.25f, 0.65f, 0.25f, 1f),
            2 => new Color(0.25f, 0.45f, 0.85f, 1f),
            3 => new Color(0.85f, 0.55f, 0.20f, 1f),
            4 => new Color(0.75f, 0.25f, 0.25f, 1f),
            _ => new Color(0.75f, 0.75f, 0.25f, 1f),
        };
    }
}