using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class AssetTypeAnalyzer
{
    private Dictionary<string, float> data = new Dictionary<string, float>();
    private Dictionary<string, List<string>> assetsByType = new Dictionary<string, List<string>>();

    private Vector2 scrollPos;

    public void Scan()
    {
        data.Clear();
        assetsByType.Clear();

        var paths = AssetDatabase.GetAllAssetPaths()
            .Where(p => p.StartsWith("Assets") && File.Exists(p));

        foreach (var path in paths)
        {
            if (path.EndsWith(".meta")) continue;

            string type = GetAssetType(path);
            if (string.IsNullOrEmpty(type)) type = "Unknown";

            float size = GetRealMemory(path);

            if (!data.ContainsKey(type)) data[type] = 0;
            if (!assetsByType.ContainsKey(type)) assetsByType[type] = new List<string>();

            data[type] += size;
            assetsByType[type].Add(path);
        }
    }

    private float GetRealMemory(string path)
    {
        string ext = Path.GetExtension(path).ToLower();

        try
        {
            if (ext == ".cs" || ext == ".shader" || ext == ".mat")
                return new FileInfo(path).Length;

            var obj = AssetDatabase.LoadMainAssetAtPath(path);
            if (obj == null) return 0;

            if (obj is Texture tex)
                return tex.width * tex.height * 4;

            if (obj is Mesh mesh)
                return mesh.vertexCount * 32;

            if (obj is AudioClip clip)
                return clip.samples * clip.channels * 2;
        }
        catch
        {
            return 0;
        }

        return new FileInfo(path).Length;
    }

    private string GetAssetType(string path)
    {
        string ext = Path.GetExtension(path).ToLower();
        
        if (ext == ".prefab")
        {
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (obj != null && obj.GetComponentInChildren<ParticleSystem>() != null)
                return "VFX";

            return "Prefab";
        }
        if (ext == ".png" || ext == ".jpg") return "Texture";
        if (ext == ".mat") return "Material";
        if (ext == ".fbx" || ext == ".obj") return "Mesh";
        if (ext == ".anim") return "Animation";
        if (ext == ".cs") return "Script";
        if (ext == ".prefab") return "Prefab";
        if (ext == ".wav" || ext == ".mp3") return "Audio";
        if (ext == ".shader") return "Shader";

        return "Other";
    }

    public Dictionary<string, float> GetData() => data;

    public void DrawAssetsOfType(string type)
    {
        if (!assetsByType.ContainsKey(type)) return;

        GUILayout.Label($"Assets ({assetsByType[type].Count})", EditorStyles.miniBoldLabel);

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));

        foreach (var asset in assetsByType[type])
        {
            float size = GetRealMemory(asset);
            string formatted = FormatSize(size);

            string extra = "";

            // Mesh polygon count
            if (type == "Mesh")
            {
                var obj = AssetDatabase.LoadAssetAtPath<Object>(asset);

                Mesh mesh = null;
                
                if (obj is Mesh m)
                {
                    mesh = m;
                }
                // Case 2: FBX / Prefab
                else if (obj is GameObject go)
                {
                    var mf = go.GetComponentInChildren<MeshFilter>();
                    if (mf != null)
                        mesh = mf.sharedMesh;
                }

                if (mesh != null)
                {
                    int polyCount = mesh.triangles.Length / 3;
                    extra = $" | {polyCount} tris";
                }
            }

            if (GUILayout.Button($"{Path.GetFileName(asset)} - {formatted}{extra}", EditorStyles.miniButton))
            {
                var obj = AssetDatabase.LoadAssetAtPath<Object>(asset);
                Selection.activeObject = obj;
            }
        }

        GUILayout.EndScrollView();
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