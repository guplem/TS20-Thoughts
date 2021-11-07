#if UNITY_EDITOR
using Thoughts.Game.Map.MapElements.Properties;
using Thoughts.Game.Map.MapElements.Properties.MapEvents;
using UnityEditor;
using UnityEngine;

namespace Thoughtskk
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Property))]
    public class PropertyInspector : UnityEditor.Editor
    {
        private Property property;
        
        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update ();
            
            property = target as Property;
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

                    MapEvent mapEvent = property.mapEvents[mapEventIndex];
                    
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
        private Property property;
        private Type[] mapEventsImplementations;
        private int selectedMapEventImplementationIndex = -1;

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update ();
            
            //specify target type
            property = target as Property;
            if (property == null) { return; }
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("relatedStats"), new GUIContent("Related Stats"), true);
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            
                GUILayout.Label("Property's events", EditorStyles.boldLabel);
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
                if (property.mapEvents == null)
                    property.mapEvents = new List<MapEvent>();
                property.mapEvents.Add(newEvent);
                //UpdateAllNeedsImplementationIndexes();
            }
        }
        
        private void CheckMapEventsListConfiguration()
        {
            if (property.mapEvents != null)
            {
                for (int a = 0; a < property.mapEvents.Count; a++)
                {
                    if (property.mapEvents[a] == null)
                        EditorGUILayout.HelpBox("The MapEvent with index " + a + " is null.\nRecommended to delete the array element by right clicking on it.", MessageType.Warning);

                    if (property.mapEvents.Count() != property.mapEvents.Distinct().Count())
                    {
                        for (int d = a + 1; d < property.mapEvents.Count; d++)
                        {
                            if (property.mapEvents[a] != null && (property.mapEvents[a] == property.mapEvents[d]))
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

                    MapEvent @event = ((MapEvent) property.mapEvents[mapEventIndex]);
                    
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
