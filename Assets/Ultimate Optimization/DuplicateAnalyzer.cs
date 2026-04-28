using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

public class DuplicateAnalyzer
{
    private Dictionary<string, List<string>> duplicates = new Dictionary<string, List<string>>();
    private Vector2 scroll;

    public void Scan()
    {
        duplicates.Clear();

        var paths = AssetDatabase.GetAllAssetPaths()
            .Where(p => p.StartsWith("Assets") && File.Exists(p) && !p.EndsWith(".meta"));

        Dictionary<string, string> hashToPath = new Dictionary<string, string>();

        foreach (var path in paths)
        {
            string hash = ComputeHash(path);

            if (string.IsNullOrEmpty(hash)) continue;

            if (!hashToPath.ContainsKey(hash))
            {
                hashToPath[hash] = path;
            }
            else
            {
                if (!duplicates.ContainsKey(hash))
                {
                    duplicates[hash] = new List<string> { hashToPath[hash] };
                }

                duplicates[hash].Add(path);
            }
        }
    }

    private string ComputeHash(string path)
    {
        try
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(path))
            {
                var hash = md5.ComputeHash(stream);
                return System.BitConverter.ToString(hash);
            }
        }
        catch
        {
            return null;
        }
    }

    public void Draw()
    {
        GUILayout.Space(5);
        GUILayout.Label("Duplicate Assets", EditorStyles.boldLabel);

        if (duplicates.Count == 0)
        {
            GUILayout.Label("No duplicates found");
            return;
        }

        scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(150));

        foreach (var group in duplicates)
        {
            GUILayout.Label($"Duplicate Group ({group.Value.Count})", EditorStyles.miniBoldLabel);

            foreach (var path in group.Value)
            {
                if (GUILayout.Button(Path.GetFileName(path), EditorStyles.miniButton))
                {
                    var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                    Selection.activeObject = obj;
                }
            }

            GUILayout.Space(5);
        }

        GUILayout.EndScrollView();
    }
}