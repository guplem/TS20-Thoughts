#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Thoughts.Needs
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NeedsHierarchy))]
    public class NeedsHierarchyInspector : UnityEditor.Editor
    {
        private Type[] implementations;
        private int selectedImplementationIndex;
        private NeedsHierarchy needsHierarchy;
        
        
        public override void OnInspectorGUI()
        {
            
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update ();
            
            
            
            
            //specify type
            needsHierarchy = target as NeedsHierarchy;
            if (needsHierarchy == null) { return; }
            
            //find all implementations of INeed using System.Reflection.Module
            if (implementations == null)
                implementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<INeed>();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            //select an implementation from all found using an editor popup
            selectedImplementationIndex = EditorGUILayout.Popup(new GUIContent("Need type"),
                selectedImplementationIndex, implementations.Select(impl => impl.Name).ToArray());

            INeed newNeed = null;
            if (GUILayout.Button("Add need"))
            {
                //Create a new need of the selected type
                newNeed = (INeed) Activator.CreateInstance(implementations[selectedImplementationIndex]);
            }
            EditorGUILayout.EndHorizontal();

            //If a new need has been created...
            if (newNeed != null)
            {
                //record the gameObject state to enable undo and prevent from exiting the scene without saving
                Undo.RegisterCompleteObjectUndo(target, "Added new need");
                //add the new need to the needs' list
//                if (needsHierarchy.needs == null)
//                    needsHierarchy.CreateNewNeedsList();
                needsHierarchy.AddNeed((Need)newNeed);
            }

            // Draw horizontal line
            EditorGUILayout.Space(); EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); EditorGUILayout.Space();

            if (needsHierarchy.needs != null)
            {
                for (int a = 0; a < needsHierarchy.needs.Count; a++)
                {
                    if (needsHierarchy.needs[a] == null)
                        EditorGUILayout.HelpBox("The need with index " + a + " is null.\nRecommended to delete the array element by right clicking on it.", MessageType.Warning);
                
                    if (needsHierarchy.needs.Count() != needsHierarchy.needs.Distinct().Count())
                    {
                        for (int d = a+1; d < needsHierarchy.needs.Count; d++)
                        {
                            if (needsHierarchy.needs[a] != null && (needsHierarchy.needs[a] == needsHierarchy.needs[d]) )
                                EditorGUILayout.HelpBox("The needs with index " + a + " and " + d + " are the same object.", MessageType.Warning);
                        }
                    }
                }
            }
        
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space(); 
            GUILayout.Label("Needs Configuration", EditorStyles.boldLabel);
            ShowNeedsArray(serializedObject.FindProperty("_needs"));
            EditorGUI.indentLevel -= 1;
            if (GUILayout.Button("Sort"))
                needsHierarchy.SortNeeds();
            
            // Draw horizontal line
            EditorGUILayout.Space(); EditorGUILayout.Space();  
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); 
            
            // Implementations search
            EditorGUILayout.BeginHorizontal();
            if (implementations != null) EditorGUILayout.LabelField($"Found {implementations.Count()} implementations", EditorStyles.helpBox);
            if (implementations == null || GUILayout.Button("Search implementations"))
            {
                //find all implementations of INeed using System.Reflection.Module
                implementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<INeed>();
            }
            EditorGUILayout.EndHorizontal();
            
            
            
            
            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties ();
        }
        
        
        private void ShowNeedsArray(UnityEditor.SerializedProperty list)
        {
            UnityEditor.EditorGUI.indentLevel += 1;
            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.Space();
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    SerializedProperty transformProp = list.GetArrayElementAtIndex(i);
                    
                    Need need = ((Need) needsHierarchy.needs[i]);
                    string itemName = $"'{need.GetName()}' need ({need.GetType().Name}) [{i}]";

                    EditorGUILayout.PropertyField(transformProp, new GUIContent(itemName), true);
                    
                    int oldIndentLevel = UnityEditor.EditorGUI.indentLevel;
                    
                    EditorGUILayout.Space();
                }
                EditorGUILayout.Space();
                
            }
            UnityEditor.EditorGUI.indentLevel -= 1;
        }
        
        
    }
}

#endif