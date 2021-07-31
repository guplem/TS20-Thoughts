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
            if (mapGenerator.autoRegenerate)
            {
                mapGenerator.GenerateMapInEditor ();
            }
        }

        if (GUILayout.Button("Generate Map"))
        {
            mapGenerator.GenerateMapInEditor();
        }


    }
}
