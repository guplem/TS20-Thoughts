#if UNITY_EDITOR
using Thoughts.Game.Map.MapElements.Attributes;
using Thoughts.Game.Map.MapElements.Attributes.MapEvents;
using UnityEditor;
using UnityEngine;

namespace Thoughtskk
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Attribute))]
    public class AttributeInspector : UnityEditor.Editor
    {
        private Attribute attribute;
        
        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update ();
            
            attribute = target as Attribute;
            base.OnInspectorGUI();
            //ShowMapEventsArray();
            
            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties ();
        }
        
        private void ShowMapEventsArray()
        {
            UnityEditor.SerializedProperty mapEventsList = serializedObject.FindProperty("mapEvents");
            
            UnityEditor.EditorGUI.indentLevel += 1;
            for (int mapEventIndex = 0; mapEventIndex < mapEventsList.arraySize; mapEventIndex++)
            {
                EditorGUILayout.Space();
                
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    SerializedProperty mapEventProperty = mapEventsList.GetArrayElementAtIndex(mapEventIndex);

                    MapEvent mapEvent = attribute.mapEvents[mapEventIndex];
                    
                    // MapEvent name
                    string eventName = "";
                    if (mapEvent.name.IsNullOrEmpty()) 
                        eventName = $"MapEvent [{mapEventIndex}]";
                    else 
                        eventName = $"'{@mapEvent.name}' MapEvent [{mapEventIndex}]";

                    EditorGUILayout.PropertyField(mapEventProperty, new GUIContent(eventName), true);

                    EditorGUILayout.Space();
                }
                
                EditorGUILayout.Space();
                
            }
            UnityEditor.EditorGUI.indentLevel -= 1;
        }


        /*
        private Attribute attribute;
        private Type[] mapEventsImplementations;
        private int selectedMapEventImplementationIndex = -1;

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update ();
            
            //specify target type
            attribute = target as Attribute;
            if (attribute == null) { return; }
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("relatedStats"), new GUIContent("Related Stats"), true);
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            
                GUILayout.Label("Attribute's events", EditorStyles.boldLabel);
                GUILayout.Label(" ");
                //select an implementation from all found using an editor popup
                NewMapEventSection();
                
            EditorGUILayout.EndHorizontal();
            
            CheckMapEventsListConfiguration();

            ShowMapEventsArray();

            EditorGUILayout.Space();  
            EditorGUILayout.Space(); 
            // Draw horizontal line
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); 
            
            ImplementationsSearchSection();

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties ();
        }
        private void NewMapEventSection()
        {
            //find all implementations of IMapEvent using System.Reflection.Module
            if (mapEventsImplementations == null)
                mapEventsImplementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<MapEvent>();

            selectedMapEventImplementationIndex = EditorGUILayout.Popup(new GUIContent(""),
                selectedMapEventImplementationIndex, mapEventsImplementations.Select(impl => impl.Name).ToArray());
            
            MapEvent newEvent = null;
            if (GUILayout.Button("Add event"))
            {
                //Create a new MapEvent of the selected type
                newEvent = (MapEvent) Activator.CreateInstance(mapEventsImplementations[selectedMapEventImplementationIndex]);
            }

            //If a new MapEvent has been created...
            if (newEvent != null)
            {
                //record the gameObject state to enable undo and prevent from exiting the scene without saving
                Undo.RegisterCompleteObjectUndo(target, "Added new event");
                //add the new mapEvent to the mapEvents' list
                if (attribute.mapEvents == null)
                    attribute.mapEvents = new List<MapEvent>();
                attribute.mapEvents.Add(newEvent);
                //UpdateAllNeedsImplementationIndexes();
            }
        }
        
        private void CheckMapEventsListConfiguration()
        {
            if (attribute.mapEvents != null)
            {
                for (int a = 0; a < attribute.mapEvents.Count; a++)
                {
                    if (attribute.mapEvents[a] == null)
                        EditorGUILayout.HelpBox("The MapEvent with index " + a + " is null.\nRecommended to delete the array element by right clicking on it.", MessageType.Warning);

                    if (attribute.mapEvents.Count() != attribute.mapEvents.Distinct().Count())
                    {
                        for (int d = a + 1; d < attribute.mapEvents.Count; d++)
                        {
                            if (attribute.mapEvents[a] != null && (attribute.mapEvents[a] == attribute.mapEvents[d]))
                                EditorGUILayout.HelpBox("The mapEvents with index " + a + " and " + d + " are the same object.", MessageType.Warning);
                        }
                    }
                }
            }
        }
        
        private void ImplementationsSearchSection()
        {
            // MapEvents
            EditorGUILayout.BeginHorizontal();
            if (mapEventsImplementations != null) EditorGUILayout.LabelField($"Found {mapEventsImplementations.Count()} MapEvents implementations", EditorStyles.helpBox);
            if (mapEventsImplementations == null || GUILayout.Button("Search MapEvent implementations"))
                mapEventsImplementations = Essentials.Utils.GetTypeImplementationsNotUnityObject<MapEvent>();
            EditorGUILayout.EndHorizontal();
            
        }
        

        private void ShowMapEventsArray()
        {
            UnityEditor.SerializedProperty mapEventsList = serializedObject.FindProperty("mapEvents");
            
            UnityEditor.EditorGUI.indentLevel += 1;
            for (int mapEventIndex = 0; mapEventIndex < mapEventsList.arraySize; mapEventIndex++)
            {
                EditorGUILayout.Space();
                
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    SerializedProperty mapEventProperty = mapEventsList.GetArrayElementAtIndex(mapEventIndex);

                    MapEvent @event = ((MapEvent) attribute.mapEvents[mapEventIndex]);
                    
                    // MapEvent name
                    string eventName;
                    if (@event.name.IsNullOrEmpty()) eventName = $"Event [{mapEventIndex}] ({@event.GetType().Name})";
                    else eventName = $"'{@event.name}' event [{mapEventIndex}] ({@event.GetType().Name})";

                    EditorGUILayout.PropertyField(mapEventProperty, new GUIContent(eventName), true);

                    EditorGUILayout.Space();
                }
                
                EditorGUILayout.Space();
                
            }
            UnityEditor.EditorGUI.indentLevel -= 1;
        }
        */
    }
}

#endif
