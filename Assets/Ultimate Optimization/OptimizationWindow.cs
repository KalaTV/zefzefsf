using UnityEngine;
using UnityEditor;

public class OptimizationWindow : EditorWindow
{
    private AssetTypeAnalyzer assetAnalyzer = new AssetTypeAnalyzer();
    private SceneAssetAnalyzer sceneAnalyzer = new SceneAssetAnalyzer();
    private DuplicateAnalyzer duplicateAnalyzer = new DuplicateAnalyzer();

    private string selectedType = null;
    private bool sceneMode = false;

    [MenuItem("Tools/Ultimate Optimization Tool")]
    public static void ShowWindow()
    {
        GetWindow<OptimizationWindow>("Ultimate Optimization Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Ultimate Optimization Tool", EditorStyles.boldLabel);

        // MODE SWITCH
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Project"))
            sceneMode = false;

        if (GUILayout.Button("Scene"))
            sceneMode = true;

        GUILayout.EndHorizontal();

        // SCAN BUTTON
        if (GUILayout.Button("Scan"))
        {
            if (sceneMode)
                sceneAnalyzer.Scan();
            else
            {
                assetAnalyzer.Scan();
                duplicateAnalyzer.Scan();
            }

            Repaint();
        }
        

        var data = sceneMode ? sceneAnalyzer.GetData() : assetAnalyzer.GetData();

        selectedType = PieChartDrawer.Draw(data, selectedType);
        
        if (sceneMode)
        {
            GUILayout.Space(10);
            GUILayout.Label("Scene Stats", EditorStyles.boldLabel);

            GUILayout.Label($"Lights: {sceneAnalyzer.GetLightCount()}");
            GUILayout.Label($"Estimated Build Size: {FormatSize(sceneAnalyzer.GetBuildSize())}");
        }

        if (!string.IsNullOrEmpty(selectedType))
        {
            GUILayout.Space(10);
            GUILayout.Label("Top Assets (" + selectedType + ")", EditorStyles.boldLabel);

            if (sceneMode)
                sceneAnalyzer.DrawAssetsOfType(selectedType);
            else
                assetAnalyzer.DrawAssetsOfType(selectedType);
        }

        if (!sceneMode)
        {
            GUILayout.Space(10);
            duplicateAnalyzer.Draw();
        }
    }
    
    private string FormatSize(float bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;

        while (bytes >= 1024 && order < sizes.Length - 1)
        {
            order++;
            bytes /= 1024;
        }

        return $"{bytes:0.##} {sizes[order]}";
    }
}