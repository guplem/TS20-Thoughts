#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Thoughts.Game.Map
{
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
                    mapGenerator.GenerateMap(true);
                }
            }

            if (GUILayout.Button("Regenerate Map"))
            {
                mapGenerator.OnValidate();
                mapGenerator.GenerateMap(true);
            }
        
            if (GUILayout.Button("Delete current Map"))
            {
                mapGenerator.DeleteCurrentMap();
            }
        
            /*if (GUILayout.Button("Repaint All"))
        {
            UnityEditor.SceneView.RepaintAll();
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
        }*/
        


        }
    }
}
#endif