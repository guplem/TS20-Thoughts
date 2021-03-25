#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Thoughts.Game.GameMap;
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
        private int[] selectedSatisfiedNeedImplementationIndex;
        private int[] selectedNeedImplementationIndex;

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update ();
            
            //specify target type
            item = target as Item;
            if (item == null) { return; }

            if (selectedSatisfiedNeedImplementationIndex == null || selectedNeedImplementationIndex == null)
                UpdateAllNeedsImplementationIndexes();

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
                actionsImplementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<IMapAction>();

            selectedActionImplementationIndex = EditorGUILayout.Popup(new GUIContent(""),
                selectedActionImplementationIndex, actionsImplementations.Select(impl => impl.Name).ToArray());
            
            IMapAction newAction = null;
            if (GUILayout.Button("Add action"))
            {
                //Create a new action of the selected type
                newAction = (IMapAction) Activator.CreateInstance(actionsImplementations[selectedActionImplementationIndex]);
            }

            //If a new action has been created...
            if (newAction != null)
            {
                //record the gameObject state to enable undo and prevent from exiting the scene without saving
                Undo.RegisterCompleteObjectUndo(target, "Added new action");
                //add the new action to the action's list
                if (item.actions == null)
                    item.actions = new List<IMapAction>();
                item.actions.Add(newAction);
                UpdateAllNeedsImplementationIndexes();
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
                actionsImplementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<IMapAction>();
            EditorGUILayout.EndHorizontal();
            
            // NEEDS
            EditorGUILayout.BeginHorizontal();
            if (needsImplementations != null) EditorGUILayout.LabelField($"Found {needsImplementations.Count()} needs implementations", EditorStyles.helpBox);
            if (needsImplementations == null || GUILayout.Button(" Search needs implementations "))
                needsImplementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<INeed>();
            EditorGUILayout.EndHorizontal();
        }
        
        private void UpdateAllNeedsImplementationIndexes()
        {
            selectedSatisfiedNeedImplementationIndex = new int[item.actions.Count];
            for (int i = 0; i < selectedSatisfiedNeedImplementationIndex.Length; i++)
                selectedSatisfiedNeedImplementationIndex[i] = -1;
            
            selectedNeedImplementationIndex = new int[item.actions.Count];
            for (int i = 0; i < selectedNeedImplementationIndex.Length; i++)
                selectedNeedImplementationIndex[i] = -1;
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

                    MapAction action = ((MapAction) item.actions[actionIndex]);
                    
                    // Action name
                    string itemName;
                    if (action.GetActionName().IsNullOrEmpty()) itemName = $"Action [{actionIndex}] ({action.GetType().Name})";
                    else itemName = $"'{action.GetActionName()}' action [{actionIndex}] ({action.GetType().Name})";

                    EditorGUILayout.PropertyField(actionProperty, new GUIContent(itemName), true);

                    SatisfiedNeedsSection(action, actionIndex, actionProperty);
                    DemandedNeedsSection(action, actionIndex, actionProperty);
                    AddNeedsSection(action, actionIndex);
                    
                    EditorGUILayout.Space();
                }
                
                EditorGUILayout.Space();
                
            }
            UnityEditor.EditorGUI.indentLevel -= 1;
        }
        
        private void SatisfiedNeedsSection(MapAction action, int actionIndex, SerializedProperty actionProperty)
        {
            // START
            EditorGUI.indentLevel += 1;

            // CURRENT CONSEQUENCE NEEDS UPDATE
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Consequence Update of Needs: ", EditorStyles.boldLabel);
            //Debug.Log($"actionProperty 1 {actionProperty.type}");
            SerializedProperty satisfiedNeedsList = actionProperty.FindPropertyRelative("consequenceNeeds");
            for (int needIndex = 0; needIndex < action.consequenceNeeds.Count; needIndex++)
            {
                SerializedProperty needProperty = satisfiedNeedsList.GetArrayElementAtIndex(needIndex);
                //SerializedProperty needProperty = actionProperty.FindPropertyRelative($"satisfiedNeeds.Array.data[{needIndex}]");

                EditorGUILayout.BeginHorizontal();
                string needName = $" • {needProperty.FindPropertyRelative($"needType.m_Name").stringValue}";
                EditorGUILayout.PropertyField(needProperty, new GUIContent(needName), true);

                SerializedProperty satisfactionProperty = needProperty.FindPropertyRelative("deltaSatisfactionAmount");
                GUILayout.Label("Delta satisfaction:");
                satisfactionProperty.intValue = EditorGUILayout.IntField(satisfactionProperty.intValue);
                GUILayout.Label("  ");

                EditorGUILayout.EndHorizontal();
            }

            
            // END
            EditorGUI.indentLevel -= 1;
        }
        
        private void DemandedNeedsSection(MapAction action, int actionIndex, SerializedProperty actionProperty)
        {
            // START
            EditorGUI.indentLevel += 1;

            // CURRENT DEMANDED NEEDS
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Required Needs: ", EditorStyles.boldLabel);
            //Debug.Log($"actionProperty 1 {actionProperty.type}");
            SerializedProperty demandedNeeds = actionProperty.FindPropertyRelative("requiredNeeds");
            for (int needIndex = 0; needIndex < action.requiredNeeds.Count; needIndex++)
            {
                SerializedProperty needProperty = demandedNeeds.GetArrayElementAtIndex(needIndex);
                //SerializedProperty needProperty = actionProperty.FindPropertyRelative($"satisfiedNeeds.Array.data[{needIndex}]");

                EditorGUILayout.BeginHorizontal();
                string needName = $" • {needProperty.FindPropertyRelative($"needType.m_Name").stringValue}";
                EditorGUILayout.PropertyField(needProperty, new GUIContent(needName), true);

                SerializedProperty satisfactionProperty = needProperty.FindPropertyRelative("requiredAmount");
                GUILayout.Label("Demanded:");
                satisfactionProperty.intValue = EditorGUILayout.IntField(satisfactionProperty.intValue);
                GUILayout.Label("  ");

                EditorGUILayout.EndHorizontal();
            }

            
            // END
            EditorGUI.indentLevel -= 1;
        }
        private void AddNeedsSection(MapAction action, int actionIndex)
        {
            EditorGUILayout.Space();
            EditorGUI.indentLevel += 1;
            
            if (needsImplementations == null)
                needsImplementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<INeed>();

            EditorGUILayout.BeginHorizontal();                
            GUILayout.Label(" ");
            selectedSatisfiedNeedImplementationIndex[actionIndex] = EditorGUILayout.Popup(new GUIContent(""), selectedSatisfiedNeedImplementationIndex[actionIndex], needsImplementations.Select(impl => impl.Name).ToArray());
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();

                GUILayout.Label("     ");

                if (GUILayout.Button("Add consequence need"))
                {
                    if (action.consequenceNeeds == null)
                        action.consequenceNeeds = new List<ConsequenceNeed>();
                    Need newNeed = (Need) Activator.CreateInstance(needsImplementations[selectedSatisfiedNeedImplementationIndex[actionIndex]]);
                    action.consequenceNeeds.Add(new ConsequenceNeed(newNeed));
                }

                if (GUILayout.Button("Add required need"))
                {
                    if (action.requiredNeeds == null)
                        action.requiredNeeds = new List<RequireddNeed>();
                    Need newNeed = (Need) Activator.CreateInstance(needsImplementations[selectedSatisfiedNeedImplementationIndex[actionIndex]]);
                    action.requiredNeeds.Add(new RequireddNeed(newNeed));
                }

            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndHorizontal();
        }
    }
}

#endif