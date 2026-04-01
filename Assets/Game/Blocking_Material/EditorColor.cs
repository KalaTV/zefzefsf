using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BlockingColorModular))]
public class ProBuilderColorEditorInspector : Editor
{
    private readonly Color[] presets = new Color[]
    {
        Color.white,
        Color.black,
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.cyan,
        Color.magenta,
        new Color(1f, 0.5f, 0f), // orange
        new Color(0.5f, 0f, 1f)  // violet
    };

    public override void OnInspectorGUI()
    {
        BlockingColorModular script = (BlockingColorModular)target;

        EditorGUILayout.LabelField("Color Settings", EditorStyles.boldLabel);
        
        Color newColor = EditorGUILayout.ColorField("Color", script.GetColor());

        if (newColor != script.GetColor())
        {
            Undo.RecordObject(script, "Change Color");
            script.SetColor(newColor);
            EditorUtility.SetDirty(script);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Presets", EditorStyles.boldLabel);
        
        int columns = 5;
        for (int i = 0; i < presets.Length; i += columns)
        {
            EditorGUILayout.BeginHorizontal();

            for (int j = 0; j < columns && i + j < presets.Length; j++)
            {
                Color preset = presets[i + j];

                GUI.backgroundColor = preset;
                if (GUILayout.Button("", GUILayout.Width(40), GUILayout.Height(40)))
                {
                    Undo.RecordObject(script, "Preset Color");
                    script.SetColor(preset);
                    EditorUtility.SetDirty(script);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        GUI.backgroundColor = Color.white;
    }
}