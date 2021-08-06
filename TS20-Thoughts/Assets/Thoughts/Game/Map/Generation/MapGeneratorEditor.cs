#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator) target;

        if (DrawDefaultInspector())
        {
            if (mapGenerator.autoRegenerateInEditor)
            {
                mapGenerator.GenerateMapInEditor ();
            }
        }

        if (GUILayout.Button("Generate Map"))
        {
            mapGenerator.OnValidate();
            mapGenerator.GenerateMapInEditor();
        }


    }
}
#endif