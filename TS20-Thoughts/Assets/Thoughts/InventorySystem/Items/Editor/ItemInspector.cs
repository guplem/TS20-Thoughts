#if UNITY_EDITOR
using System;
using System.Collections.Generic;
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
        private int selectedImplementationIndex = -1;
        private Item item;

        private int[] selectedNeedsImplementationIndex;
        
        
        public override void OnInspectorGUI()
        {
            
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update ();
            
            //specify type
            item = target as Item;
            if (item == null) { return; }
            
            //find all implementations of IMobAction using System.Reflection.Module
            if (implementations == null)
                implementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<IMobAction>();
            
            if (selectedNeedsImplementationIndex == null)
                UpdateSelectedNeedsImplementationIndex();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            //select an implementation from all found using an editor popup
            selectedImplementationIndex = EditorGUILayout.Popup(new GUIContent("Action type"),
                selectedImplementationIndex, implementations.Select(impl => impl.Name).ToArray());

            
            IMobAction newAction = null;
            if (GUILayout.Button("Create action"))
            {
                //Create a new action of the selected type
                newAction = (IMobAction) Activator.CreateInstance(implementations[selectedImplementationIndex]);
            }
            EditorGUILayout.EndHorizontal();

            //If a new action has been created...
            if (newAction != null)
            {
                //record the gameObject state to enable undo and prevent from exiting the scene without saving
                Undo.RegisterCompleteObjectUndo(target, "Added new action");
                //add the new action to the action's list
                if (item.actions == null)
                    item.actions = new List<IMobAction>();
                item.actions.Add(newAction);
                UpdateSelectedNeedsImplementationIndex();
            }

            // Draw horizontal line
            EditorGUILayout.Space(); EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); EditorGUILayout.Space();

            if (item.actions != null)
            {
                for (int a = 0; a < item.actions.Count; a++)
                {
                    if (item.actions[a] == null)
                        EditorGUILayout.HelpBox("The action with index " + a + " is null.\nRecommended to delete the array element by right clicking on it.", MessageType.Warning);
                
                    if (item.actions.Count() != item.actions.Distinct().Count())
                    {
                        for (int d = a+1; d < item.actions.Count; d++)
                        {
                            if (item.actions[a] != null && (item.actions[a] == item.actions[d]) )
                                EditorGUILayout.HelpBox("The actions with index " + a + " and " + d + " are the same object.", MessageType.Warning);
                        }
                    }
                }
            }
        
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space(); 
            GUILayout.Label("Item's available actions", EditorStyles.boldLabel);
            ShowActionsArray(serializedObject.FindProperty("actions"));

            EditorGUI.indentLevel -= 1;
            
            // Draw horizontal line
            EditorGUILayout.Space(); EditorGUILayout.Space();  
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); 
            
            // Implementations search
            EditorGUILayout.BeginHorizontal();
            if (implementations != null) EditorGUILayout.LabelField($"Found {implementations.Count()} implementations", EditorStyles.helpBox);
            if (implementations == null || GUILayout.Button("Search implementations"))
            {
                //find all implementations of IAction using System.Reflection.Module
                implementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<IMobAction>();
            }
            EditorGUILayout.EndHorizontal();
            
            
            
            
            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties ();
        }
        private void UpdateSelectedNeedsImplementationIndex()
        {
            Debug.Log("Updating selectedNeedsImplementationIndex");
            selectedNeedsImplementationIndex = new int[item.actions.Count];
            for (int i = 0; i < selectedNeedsImplementationIndex.Length; i++)
                selectedNeedsImplementationIndex[i] = -1;
        }





        private void ShowActionsArray(UnityEditor.SerializedProperty list)
        {
            UnityEditor.EditorGUI.indentLevel += 1;
            for (int actionIndex = 0; actionIndex < list.arraySize; actionIndex++)
            {
                EditorGUILayout.Space();
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    SerializedProperty actionProperty = list.GetArrayElementAtIndex(actionIndex);

                    MobAction action = ((MobAction) item.actions[actionIndex]);
                    
                    string itemName;
                    if (action.GetActionName().IsNullOrEmpty())
                        itemName = $"Action [{actionIndex}]";
                    else
                        itemName = $"'{action.GetActionName()}' action [{actionIndex}]";
                    
                    EditorGUILayout.PropertyField(actionProperty, new GUIContent(itemName), true);
                    //EditorGUILayout.LabelField(action.GetType().Name);
                    
                    //var childrenProperties = transformProp.GetVisibleChildren();
                    //foreach (var property in childrenProperties) {
                    //    Debug.Log($"Action {i}: {property.name}");
                    //}
                    DisplayActionEditor(action, actionIndex, actionProperty);
                    EditorGUILayout.Space();
                }
                
                EditorGUILayout.Space();
                
            }
            UnityEditor.EditorGUI.indentLevel -= 1;
        }
        
        
        
        
        
        private void DisplayActionEditor(MobAction action, int actionIndex, SerializedProperty actionProperty)
        {
            EditorGUI.indentLevel += 1;

            
            
            
            // COVERED NEEDS
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Covered Needs: ");
            SerializedProperty needsCoveredList = actionProperty.FindPropertyRelative("needsCovered");
            for (int needIndex = 0; needIndex < action.needsCovered.Count; needIndex++)
            {
                SerializedProperty needProperty = needsCoveredList.GetArrayElementAtIndex(needIndex);
                //SerializedProperty needProperty = actionProperty.FindPropertyRelative($"needsCovered.Array.data[{needIndex}]");

                EditorGUILayout.BeginHorizontal();
                string needName = $" • {needProperty.FindPropertyRelative($"needType.m_Name").stringValue}";
                EditorGUILayout.PropertyField(needProperty, new GUIContent(needName), true);

                SerializedProperty satisfactionProperty = needProperty.FindPropertyRelative("satisfactionAmount");
                GUILayout.Label("Satisfaction:");
                satisfactionProperty.intValue = EditorGUILayout.IntField(satisfactionProperty.intValue);
                GUILayout.Label(" ");

                EditorGUILayout.EndHorizontal();
            }

            /*foreach (NeedSatisfaction coveredNeed in action.needsCovered)
            {
                GUILayout.Label("NEED");
            }*/
            
            
            
            
            
            
            //ADD NEED ELEMENTS
            EditorGUILayout.Space();
            Type[] needsImplementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<INeed>();     
            
            EditorGUILayout.BeginHorizontal();
            
                selectedNeedsImplementationIndex[actionIndex] = EditorGUILayout.Popup(new GUIContent("Need type"), selectedNeedsImplementationIndex[actionIndex], needsImplementations.Select(impl => impl.Name).ToArray());
               
                Need newNeed = null;
                if (GUILayout.Button("Add need"))
                {
                    newNeed = (Need) Activator.CreateInstance(needsImplementations[selectedNeedsImplementationIndex[actionIndex]]);
                    action.needsCovered.Add(new NeedSatisfaction(newNeed));
                    action.needsCovered.DebugLog();
                }
                
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.indentLevel -= 1;

        }
    }
}

#endif
