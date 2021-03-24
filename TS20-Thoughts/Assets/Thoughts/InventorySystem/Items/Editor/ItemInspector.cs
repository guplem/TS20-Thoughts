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
        private Item item;
        private Type[] actionsImplementations;
        private int selectedActionImplementationIndex = -1;
        private Type[] needsImplementations;
        private int[] selectedCoveredNeedImplementationIndex;

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update ();
            
            //specify target type
            item = target as Item;
            if (item == null) { return; }

            if (selectedCoveredNeedImplementationIndex == null)
                UpdateSelectedCoveredNeedImplementationIndex();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            
                GUILayout.Label("Item's actions", EditorStyles.boldLabel);
                GUILayout.Label(" ");
                //select an implementation from all found using an editor popup
                NewActionsSection();
                
            EditorGUILayout.EndHorizontal();
            
            CheckActionsListConfiguration();

            ShowActionsArray();

            EditorGUILayout.Space();  
            EditorGUILayout.Space(); 
            // Draw horizontal line
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); 
            
            ImplementationsSearchSection();

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties ();
        }
        private void NewActionsSection()
        {
            //find all implementations of IMobAction using System.Reflection.Module
            if (actionsImplementations == null)
                actionsImplementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<IMobAction>();

            selectedActionImplementationIndex = EditorGUILayout.Popup(new GUIContent(""),
                selectedActionImplementationIndex, actionsImplementations.Select(impl => impl.Name).ToArray());
            
            IMobAction newAction = null;
            if (GUILayout.Button("Add action"))
            {
                //Create a new action of the selected type
                newAction = (IMobAction) Activator.CreateInstance(actionsImplementations[selectedActionImplementationIndex]);
            }

            //If a new action has been created...
            if (newAction != null)
            {
                //record the gameObject state to enable undo and prevent from exiting the scene without saving
                Undo.RegisterCompleteObjectUndo(target, "Added new action");
                //add the new action to the action's list
                if (item.actions == null)
                    item.actions = new List<IMobAction>();
                item.actions.Add(newAction);
                UpdateSelectedCoveredNeedImplementationIndex();
            }
        }
        
        private void CheckActionsListConfiguration()
        {
            if (item.actions != null)
            {
                for (int a = 0; a < item.actions.Count; a++)
                {
                    if (item.actions[a] == null)
                        EditorGUILayout.HelpBox("The action with index " + a + " is null.\nRecommended to delete the array element by right clicking on it.", MessageType.Warning);

                    if (item.actions.Count() != item.actions.Distinct().Count())
                    {
                        for (int d = a + 1; d < item.actions.Count; d++)
                        {
                            if (item.actions[a] != null && (item.actions[a] == item.actions[d]))
                                EditorGUILayout.HelpBox("The actions with index " + a + " and " + d + " are the same object.", MessageType.Warning);
                        }
                    }
                }
            }
        }
        
        private void ImplementationsSearchSection()
        {
            // ACTIONS
            EditorGUILayout.BeginHorizontal();
            if (actionsImplementations != null) EditorGUILayout.LabelField($"Found {actionsImplementations.Count()} actions implementations", EditorStyles.helpBox);
            if (actionsImplementations == null || GUILayout.Button("Search actions implementations"))
                actionsImplementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<IMobAction>();
            EditorGUILayout.EndHorizontal();
            
            // NEEDS
            EditorGUILayout.BeginHorizontal();
            if (needsImplementations != null) EditorGUILayout.LabelField($"Found {needsImplementations.Count()} needs implementations", EditorStyles.helpBox);
            if (needsImplementations == null || GUILayout.Button(" Search needs implementations "))
                needsImplementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<INeed>();
            EditorGUILayout.EndHorizontal();
        }
        
        private void UpdateSelectedCoveredNeedImplementationIndex()
        {
            selectedCoveredNeedImplementationIndex = new int[item.actions.Count];
            for (int i = 0; i < selectedCoveredNeedImplementationIndex.Length; i++)
                selectedCoveredNeedImplementationIndex[i] = -1;
        }

        private void ShowActionsArray()
        {
            UnityEditor.SerializedProperty actionsList = serializedObject.FindProperty("actions");
            
            UnityEditor.EditorGUI.indentLevel += 1;
            for (int actionIndex = 0; actionIndex < actionsList.arraySize; actionIndex++)
            {
                EditorGUILayout.Space();
                
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    SerializedProperty actionProperty = actionsList.GetArrayElementAtIndex(actionIndex);

                    MobAction action = ((MobAction) item.actions[actionIndex]);
                    
                    // Action name
                    string itemName;
                    if (action.GetActionName().IsNullOrEmpty()) itemName = $"Action [{actionIndex}] ({action.GetType().Name})";
                    else itemName = $"'{action.GetActionName()}' action [{actionIndex}] ({action.GetType().Name})";

                    EditorGUILayout.PropertyField(actionProperty, new GUIContent(itemName), true);

                    CoveredNeedsSection(action, actionIndex, actionProperty);
                    
                    EditorGUILayout.Space();
                }
                
                EditorGUILayout.Space();
                
            }
            UnityEditor.EditorGUI.indentLevel -= 1;
        }
        
        private void CoveredNeedsSection(MobAction action, int actionIndex, SerializedProperty actionProperty)
        {
            // START
            EditorGUI.indentLevel += 1;

            // CURRENT COVERED NEEDS
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Covered Needs: ", EditorStyles.boldLabel);
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
                GUILayout.Label("  ");

                EditorGUILayout.EndHorizontal();
            }

            //ADD NEED ELEMENTS
            EditorGUILayout.Space();
            if (needsImplementations == null)
                needsImplementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<INeed>();     
            
            EditorGUILayout.BeginHorizontal();
            
                GUILayout.Label("    ");
                
                selectedCoveredNeedImplementationIndex[actionIndex] = EditorGUILayout.Popup(new GUIContent(""), selectedCoveredNeedImplementationIndex[actionIndex], needsImplementations.Select(impl => impl.Name).ToArray());
                
                if (GUILayout.Button("Add covered need"))
                {
                    Need newNeed = (Need) Activator.CreateInstance(needsImplementations[selectedCoveredNeedImplementationIndex[actionIndex]]);
                    action.needsCovered.Add(new NeedSatisfaction(newNeed));
                }
                
            EditorGUILayout.EndHorizontal();

            // END
            EditorGUI.indentLevel -= 1;
        }
    }
}

#endif
