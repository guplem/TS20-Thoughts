#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Thoughts.Game.Map
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : Editor
    {
        public CreationStep regenerationStep = CreationStep.Terrain;
        public override void OnInspectorGUI()
        {
            MapGenerator mapGenerator = (MapGenerator) target;

            if (DrawDefaultInspector())
            {
                /*
                if (mapGenerator.autoRegenerateInEditor)
                {
                    mapGenerator.GenerateFullMap(true);
                }
                */
            }

            GUILayout.BeginHorizontal();
            regenerationStep = (CreationStep)EditorGUILayout.EnumPopup("Step to regenerate:", regenerationStep);
            if (GUILayout.Button("Regenerate"))
            {
                //mapGenerator.OnValidate();
                mapGenerator.Regenerate(regenerationStep);
            }
            GUILayout.EndHorizontal();
        
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