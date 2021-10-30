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


            EditorGUILayout.Space(); EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); EditorGUILayout.Space(); //Horizontal line
            
            GUILayout.BeginHorizontal();
            regenerationStep = (CreationStep)EditorGUILayout.EnumPopup("Step to regenerate:", regenerationStep);
            if (GUILayout.Button("Regenerate"))
            {
                //mapGenerator.OnValidate();
                mapGenerator.Regenerate(regenerationStep);
            }
            GUILayout.EndHorizontal();
        
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete all map"))
            {
                mapGenerator.DeleteCurrentMap();
            }
            if (GUILayout.Button("Regenerate full map"))
            {
                mapGenerator.RegenerateFull();
                UnityEditor.SceneView.RepaintAll();
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            }
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button("Refresh Auto Update (UpdatableData links)"))
            {
                mapGenerator.OnValidate();
            }
        


        }
    }
}
#endif