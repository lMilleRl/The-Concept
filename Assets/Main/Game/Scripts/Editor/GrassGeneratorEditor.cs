#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GrassGenerator))]
public class GrassGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        var generator = (GrassGenerator)target;

        if (GUILayout.Button("Generate"))
            generator.Generate();

        if (GUILayout.Button("Clear"))
            generator.Clear();
    }
}
#endif
