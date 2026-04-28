using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SceneAssetAnalyzer
{
    private Dictionary<string, float> data = new Dictionary<string, float>();
    private Dictionary<string, List<(GameObject go, float size)>> objectsByType = new Dictionary<string, List<(GameObject, float)>>();

    private Vector2 scroll;
    private int totalLights = 0;
    private float estimatedBuildSize = 0f;
    public int GetLightCount() => totalLights;
    public float GetBuildSize() => estimatedBuildSize;


    public void Scan()
    {
        data.Clear();
        objectsByType.Clear();

        var meshes = Object.FindObjectsOfType<MeshFilter>();

        foreach (var mf in meshes)
        {
            if (mf.sharedMesh == null) continue;

            Add("Mesh", mf.sharedMesh.vertexCount * 32, mf.gameObject);
        }

        var renderers = Object.FindObjectsOfType<Renderer>();

        foreach (var r in renderers)
        {
            foreach (var mat in r.sharedMaterials)
            {
                if (mat == null) continue;

                Add("Material", 1024, r.gameObject);

                if (mat.mainTexture is Texture tex)
                {
                    Add("Texture", tex.width * tex.height * 4, r.gameObject);
                }
            }
        }

        var audios = Object.FindObjectsOfType<AudioSource>();

        foreach (var a in audios)
        {
            if (a.clip == null) continue;

            Add("Audio", a.clip.samples * a.clip.channels * 2, a.gameObject);
        }
        
        var scripts = Object.FindObjectsOfType<MonoBehaviour>();
        foreach (var s in scripts)
            Add("Script", 256, s.gameObject);
        
        var vfx = Object.FindObjectsOfType<ParticleSystem>();

        foreach (var ps in vfx)
        {
            var main = ps.main;
            Add("VFX", main.maxParticles * 16, ps.gameObject);
        }
        
        var lights = Object.FindObjectsOfType<Light>();
        totalLights = lights.Length;
        
        estimatedBuildSize = 0f;

        foreach (var kvp in data)
        {
            estimatedBuildSize += kvp.Value;
        }
    }

    private void Add(string type, float size, GameObject go)
    {
        if (!data.ContainsKey(type))
            data[type] = 0;

        if (!objectsByType.ContainsKey(type))
            objectsByType[type] = new List<(GameObject, float)>();

        data[type] += size;
        objectsByType[type].Add((go, size));
    }

    public Dictionary<string, float> GetData() => data;

    public void DrawAssetsOfType(string type)
    {
        if (!objectsByType.ContainsKey(type)) return;

        scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(200));

        foreach (var entry in objectsByType[type])
        {
            var go = entry.go;
            float size = entry.size;

            string label = $"{go.name} - {FormatSize(size)}";

            // Mesh polycount
            var mf = go.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                int tris = mf.sharedMesh.triangles.Length / 3;
                label += $" | {tris} tris";
            }

            if (GUILayout.Button(label, EditorStyles.miniButton))
            {
                Selection.activeGameObject = go;
                EditorGUIUtility.PingObject(go);
                SceneView.lastActiveSceneView?.FrameSelected();
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