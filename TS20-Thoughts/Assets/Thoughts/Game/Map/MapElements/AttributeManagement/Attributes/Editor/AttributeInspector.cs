#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Thoughts.Game.GameMap;
using Thoughts.Game.Map.MapElements.InventorySystem.Items.Needs;
using Thoughts.Needs;
using UnityEditor;
using UnityEngine;

namespace Thoughts
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Attribute))]
    public class AttributeInspector : UnityEditor.Editor
    {
        private Attribute attribute;
        private Type[] actionsImplementations;
        private int selectedActionImplementationIndex = -1;
        //private Type[] needsImplementations;
        //private int[] selectedSatisfiedNeedImplementationIndex;
        //private int[] selectedNeedImplementationIndex;

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update ();
            
            //specify target type
            attribute = target as Attribute;
            if (attribute == null) { return; }

            /*if (selectedSatisfiedNeedImplementationIndex == null || selectedNeedImplementationIndex == null)
                UpdateAllNeedsImplementationIndexes();*/
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("relatedStats"), new GUIContent("Related Stats"), true);
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            
                GUILayout.Label("Attribute's events", EditorStyles.boldLabel);
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
                actionsImplementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<IMapEvent>();

            selectedActionImplementationIndex = EditorGUILayout.Popup(new GUIContent(""),
                selectedActionImplementationIndex, actionsImplementations.Select(impl => impl.Name).ToArray());
            
            IMapEvent newEvent = null;
            if (GUILayout.Button("Add event"))
            {
                //Create a new action of the selected type
                newEvent = (IMapEvent) Activator.CreateInstance(actionsImplementations[selectedActionImplementationIndex]);
            }

            //If a new action has been created...
            if (newEvent != null)
            {
                //record the gameObject state to enable undo and prevent from exiting the scene without saving
                Undo.RegisterCompleteObjectUndo(target, "Added new event");
                //add the new action to the action's list
                if (attribute.events == null)
                    attribute.events = new List<IMapEvent>();
                attribute.events.Add(newEvent);
                //UpdateAllNeedsImplementationIndexes();
            }
        }
        
        private void CheckActionsListConfiguration()
        {
            if (attribute.events != null)
            {
                for (int a = 0; a < attribute.events.Count; a++)
                {
                    if (attribute.events[a] == null)
                        EditorGUILayout.HelpBox("The action with index " + a + " is null.\nRecommended to delete the array element by right clicking on it.", MessageType.Warning);

                    if (attribute.events.Count() != attribute.events.Distinct().Count())
                    {
                        for (int d = a + 1; d < attribute.events.Count; d++)
                        {
                            if (attribute.events[a] != null && (attribute.events[a] == attribute.events[d]))
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
                actionsImplementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<IMapEvent>();
            EditorGUILayout.EndHorizontal();
            
           /* // NEEDS
            EditorGUILayout.BeginHorizontal();
            if (needsImplementations != null) EditorGUILayout.LabelField($"Found {needsImplementations.Count()} needs implementations", EditorStyles.helpBox);
            if (needsImplementations == null || GUILayout.Button(" Search needs implementations "))
                needsImplementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<INeed>();
            EditorGUILayout.EndHorizontal();*/
        }
        
        /*private void UpdateAllNeedsImplementationIndexes()
        {
            selectedSatisfiedNeedImplementationIndex = new int[item.actions.Count];
            for (int i = 0; i < selectedSatisfiedNeedImplementationIndex.Length; i++)
                selectedSatisfiedNeedImplementationIndex[i] = -1;
            
            selectedNeedImplementationIndex = new int[item.actions.Count];
            for (int i = 0; i < selectedNeedImplementationIndex.Length; i++)
                selectedNeedImplementationIndex[i] = -1;
        }*/

        private void ShowActionsArray()
        {
            UnityEditor.SerializedProperty actionsList = serializedObject.FindProperty("events");
            
            UnityEditor.EditorGUI.indentLevel += 1;
            for (int actionIndex = 0; actionIndex < actionsList.arraySize; actionIndex++)
            {
                EditorGUILayout.Space();
                
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    SerializedProperty actionProperty = actionsList.GetArrayElementAtIndex(actionIndex);

                    MapEvent @event = ((MapEvent) attribute.events[actionIndex]);
                    
                    // Action name
                    string eventName;
                    if (@event.GetName().IsNullOrEmpty()) eventName = $"Event [{actionIndex}] ({@event.GetType().Name})";
                    else eventName = $"'{@event.GetName()}' event [{actionIndex}] ({@event.GetType().Name})";

                    EditorGUILayout.PropertyField(actionProperty, new GUIContent(eventName), true);

                    /*//SatisfiedNeedsOfActionSection(action, actionIndex, actionProperty);
                    //DemandedNeedsOfActionSection(action, actionIndex, actionProperty);
                    //AddNeedsSection(action, actionIndex);*/
                    
                    EditorGUILayout.Space();
                }
                
                EditorGUILayout.Space();
                
            }
            UnityEditor.EditorGUI.indentLevel -= 1;
        }
        
        /*private void SatisfiedNeedsOfActionSection(MapAction action, int actionIndex, SerializedProperty actionProperty)
        {
            // START
            EditorGUI.indentLevel += 1;

            // CURRENT CONSEQUENCE NEEDS UPDATE
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Consequence Update of Needs: ", EditorStyles.boldLabel);
            //Debug.Log($"actionProperty 1 {actionProperty.type}");
            SerializedProperty consequenceNeedsList = actionProperty.FindPropertyRelative("consequenceNeeds");
            for (int needIndex = 0; needIndex < action.consequenceNeeds.Count; needIndex++)
            {
                SerializedProperty needProperty = consequenceNeedsList.GetArrayElementAtIndex(needIndex);
                Debug.Log(needProperty != null);
                //SerializedProperty needProperty = actionProperty.FindPropertyRelative($"satisfiedNeeds.Array.data[{needIndex}]");

                EditorGUILayout.BeginHorizontal();
                string needName = $" • {needProperty.FindPropertyRelative($"need.name").stringValue}";
                EditorGUILayout.PropertyField(needProperty, new GUIContent(needName), true);

                SerializedProperty satisfactionProperty = needProperty.FindPropertyRelative("deltaSatisfactionAmount");
                GUILayout.Label("Delta satisfaction:");
                satisfactionProperty.intValue = EditorGUILayout.IntField(satisfactionProperty.intValue);
                GUILayout.Label("  ");

                EditorGUILayout.EndHorizontal();
            }

            
            // END
            EditorGUI.indentLevel -= 1;
        }*/
        
        /*private void DemandedNeedsOfActionSection(MapAction action, int actionIndex, SerializedProperty actionProperty)
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
        }*/
        
        /*private void AddNeedsSection(MapAction action, int actionIndex)
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
                        action.requiredNeeds = new List<RequiredNeed>();
                    Need newNeed = (Need) Activator.CreateInstance(needsImplementations[selectedSatisfiedNeedImplementationIndex[actionIndex]]);
                    action.requiredNeeds.Add(new RequiredNeed(newNeed));
                }

            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndHorizontal();
        }*/
        
    }
}

#endif
