using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class PieChartDrawer
{
    private static float chartSize = 140f;
    private static float zoom = 1f;
    private static int legendFontSize = 11;

    private static Dictionary<string, Color> colorCache = new Dictionary<string, Color>();

    public static string Draw(Dictionary<string, float> data, string currentSelection)
    {
        if (data == null || data.Count == 0)
            return currentSelection;

        // CONTROLS
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("Text", GUILayout.Width(35));
        legendFontSize = (int)GUILayout.HorizontalSlider(legendFontSize, 9, 18);
        GUILayout.Label(legendFontSize.ToString(), GUILayout.Width(30));
        
        GUILayout.Space(10);

        GUILayout.Label("Zoom", GUILayout.Width(40));
        zoom = GUILayout.HorizontalSlider(zoom, 0.5f, 2f);
        GUILayout.Label(zoom.ToString("F1") + "x", GUILayout.Width(40));

        GUILayout.EndHorizontal();

        float effectiveSize = chartSize * zoom;

        GUILayout.BeginHorizontal();

        // LEGEND
        GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

        GUILayout.Label("Asset Distribution", EditorStyles.boldLabel);

        float total = 0;
        foreach (var v in data.Values) total += v;

        foreach (var kvp in data)
        {
            Color color = GetColor(kvp.Key);

            GUILayout.BeginHorizontal();

            int boxSize = Mathf.Clamp(legendFontSize, 8, 16);

            // Color box
            GUI.color = color;
            GUILayout.Box("", GUILayout.Width(boxSize), GUILayout.Height(boxSize));
            GUI.color = Color.white;

            GUILayout.Space(4);

            float percent = (kvp.Value / total) * 100f;

            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.fontSize = legendFontSize;

            // COLOR TEXT
            style.normal.textColor = color;

            if (currentSelection == kvp.Key)
                style.fontStyle = FontStyle.Bold;

            if (GUILayout.Button($"{kvp.Key}: {percent:F1}%", style))
            {
                currentSelection = kvp.Key;
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        // PIE
        Rect rect = GUILayoutUtility.GetRect(
            effectiveSize,
            effectiveSize,
            GUILayout.Width(effectiveSize),
            GUILayout.Height(effectiveSize)
        );

        Vector2 center = rect.center;

        float startAngle = 0;
        string clicked = null;

        foreach (var kvp in data)
        {
            float angle = (kvp.Value / total) * 360f;
            Color color = GetColor(kvp.Key);

            Handles.color = color;
            Handles.DrawSolidArc(
                center,
                Vector3.forward,
                Quaternion.Euler(0, 0, startAngle) * Vector3.right,
                angle,
                effectiveSize / 2
            );

            if (Event.current.type == EventType.MouseDown)
            {
                Vector2 mouse = Event.current.mousePosition;

                if (Vector2.Distance(mouse, center) < effectiveSize / 2)
                {
                    float mouseAngle = Mathf.Atan2(
                        mouse.y - center.y,
                        mouse.x - center.x
                    ) * Mathf.Rad2Deg;

                    if (mouseAngle < 0) mouseAngle += 360;

                    if (mouseAngle >= startAngle && mouseAngle <= startAngle + angle)
                    {
                        clicked = kvp.Key;
                        Event.current.Use();
                    }
                }
            }

            startAngle += angle;
        }

        GUILayout.EndHorizontal();

        return string.IsNullOrEmpty(clicked) ? currentSelection : clicked;
    }

    private static Color GetColor(string key)
    {
        if (!colorCache.ContainsKey(key))
        {
            Random.InitState(key.GetHashCode());

            colorCache[key] = Random.ColorHSV(
                0f, 1f,
                0.7f, 1f,
                0.7f, 1f
            );
        }

        return colorCache[key];
    }
}