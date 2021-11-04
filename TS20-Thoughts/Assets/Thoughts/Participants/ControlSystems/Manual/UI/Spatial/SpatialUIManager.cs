using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using Thoughts.Game.Map.MapElements;
using UnityEngine;

public class SpatialUIManager : MonoBehaviour
{
        /// <summary>
        /// The currently selected MapElement being displayed in the UI
        /// </summary>
        private MapElement selectedMapElement;
        
        /// <summary>
        /// Reference to the section of the spatial UI to display the selected element
        /// </summary>
        [Tooltip("Reference to the section of the spatial UI to display the selected element")]
        [SerializeField] private SelectionSpatialUI selectionSpatialUI;

        /// <summary>
        /// Displays the UI related to the given MapElement and subscribes the UI to changes on that mapElement that should be reflected.
        /// <para>It also unsubscribes the UI to the changes from the previous map element</para>
        /// </summary>
        /// <param name="mapElement">The MapElement from which you want to display the information in the UI.</param>
        /// <param name="forceUpdate">Ignore if the currently selected MapElement is the same as the new selected MapElement and update the UI as if they were different.</param>
        public void DisplayUIFor(MapElement mapElement, bool forceUpdate = false)
        {
                Debug.Log($"Displaying UI of MapElement: '{mapElement}'");

                if (selectedMapElement != mapElement || forceUpdate)
                {
                        // Unsubscribe to updates
                        if (selectedMapElement != null)
                        {
                                //selectedMapElement.onExecutionPlansUpdated -= behaviorUI.DisplayExecutionPlans;
                                //selectedMapElement.onObjectiveAttributeUpdated -= behaviorUI.DisplayObjectiveAttribute;
                        }
                
                        // Update
                        this.selectedMapElement = mapElement;
                
                        // Activate/Deactivate, get ready and show the current information
                        selectionSpatialUI.Setup(selectedMapElement);

                        // Subscribe to updates
                        if (selectedMapElement != null)
                        {
                                //selectedMapElement.onExecutionPlansUpdated += behaviorUI.DisplayExecutionPlans;
                                //selectedMapElement.onObjectiveAttributeUpdated += behaviorUI.DisplayObjectiveAttribute;
                        }
                }
        }
}
