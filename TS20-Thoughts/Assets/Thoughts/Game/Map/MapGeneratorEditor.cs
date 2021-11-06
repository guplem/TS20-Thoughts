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
            if (GUILayout.Button("Delete all"))
            {
                mapGenerator.DeleteMap();
            }
            if (GUILayout.Button("Del. all & regenerate terrain"))
            {
                mapGenerator.DeleteMap();
                mapGenerator.RegenerateTerrain();
            }
            
            if (GUILayout.Button("Regenerate all"))
            {
                mapGenerator.RegenerateFullMap();
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