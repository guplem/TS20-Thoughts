#if UNITY_EDITOR
using System;
using System.Linq;
using Thoughts.Needs;
using UnityEditor;
using UnityEngine;

namespace Thoughts
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Item))]
    public class ItemInspector : UnityEditor.Editor
    {
        private Type[] implementations;
        private int selectedImplementationIndex;
        private Item item;
        
        
        public override void OnInspectorGUI()
        {
            
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update ();
            
            
            
            
            //specify type
            item = target as Item;
            if (item == null) { return; }
            
            
            base.OnInspectorGUI();
            
            
            
            //find all implementations of INeed using System.Reflection.Module
            if (implementations == null)
                implementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<INeed>();
            
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space();
            if (serializedObject.FindProperty("consumible").boolValue)
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Covered needs", EditorStyles.boldLabel);
                    ShowCoveredNeedsArray(serializedObject.FindProperty("coveredNeeds"));
                }

                EditorGUI.indentLevel -= 1;

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                //select an implementation from all found using an editor popup
                selectedImplementationIndex = EditorGUILayout.Popup(new GUIContent("Need type"),
                    selectedImplementationIndex, implementations.Select(impl => impl.Name).ToArray());

                INeed newCoveredNeed = null;
                if (GUILayout.Button("Add covered need"))
                {
                    //Create a new need of the selected type
                    newCoveredNeed = (INeed) Activator.CreateInstance(implementations[selectedImplementationIndex]);
                }

                EditorGUILayout.EndHorizontal();

                //If a new need has been created...
                if (newCoveredNeed != null)
                {
                    //record the gameObject state to enable undo and prevent from exiting the scene without saving
                    Undo.RegisterCompleteObjectUndo(target, "Added new need");
                    //add the new need to the needs' list
//                if (needsHierarchy.needs == null)
//                    needsHierarchy.CreateNewNeedsList();
                    item.coveredNeeds.Add(newCoveredNeed);
                }

                // Draw horizontal line
                //EditorGUILayout.Space(); EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); EditorGUILayout.Space();

                if (item.coveredNeeds != null)
                {
                    for (int a = 0; a < item.coveredNeeds.Count; a++)
                    {
                        if (item.coveredNeeds[a] == null || ((Need) item.coveredNeeds[a]) == null)
                            EditorGUILayout.HelpBox("The need with index " + a + " is null.\nRecommended to delete the array element by right clicking on it.", MessageType.Warning);

                        //if (item.coveredNeeds.Count() != item.coveredNeeds.Distinct().Count()) // need to implement hash code override
                        for (int d = a + 1; d < item.coveredNeeds.Count; d++)
                        {
                            if (item.coveredNeeds[a] != null && ((Need) item.coveredNeeds[a]) != null && ((Need) item.coveredNeeds[a]).GetType() == ((Need) item.coveredNeeds[d]).GetType())
                                EditorGUILayout.HelpBox("The elements with index " + a + " and " + d + " are covering the same need.", MessageType.Warning);
                        }
                    }
                }


                // Draw horizontal line
                EditorGUILayout.Space();
                EditorGUILayout.Space();
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

            }


            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties ();
        }
        
        
        private void ShowCoveredNeedsArray(UnityEditor.SerializedProperty list)
        {
            //UnityEditor.EditorGUI.indentLevel += 1;
            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.Space();
                //using (new GUILayout.VerticalScope(EditorStyles.helpBox)) {
                    SerializedProperty transformProp = list.GetArrayElementAtIndex(i);

                    string itemName = $"NULL covered need {i}";
                    try
                    {
                        Need coveredNeed = (((Need)item.coveredNeeds[i]));
                        itemName = $"• {coveredNeed.GetType().Name}";
                    }
                    catch (Exception) { }

                    EditorGUILayout.PropertyField(transformProp, new GUIContent(itemName), false);
                    
                   // int oldIndentLevel = UnityEditor.EditorGUI.indentLevel;
                    
                    //EditorGUILayout.Space();
                //}
                //EditorGUILayout.Space();
                
            }
            //UnityEditor.EditorGUI.indentLevel -= 1;
        }
        
        
    }
}

#endif
