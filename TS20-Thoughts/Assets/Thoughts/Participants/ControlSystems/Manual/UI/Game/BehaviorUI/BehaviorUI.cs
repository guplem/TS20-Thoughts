using System;
using System.Collections.Generic;
using Thoughts.Game.Map.MapElements;
using Thoughts.Game.Map.MapElements.Properties;
using Thoughts.Game.Map.MapElements.Properties.MapEvents;
using Thoughts.Participants.ControlSystems.Manual.UI.Game.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Thoughts.Participants.ControlSystems.Manual.UI.Game.BehaviorUI
{
    /// <summary>
    /// Controls the display of the information in the UI related to the behavior of a MapElement (objective Property and planned ExecutionPlans)
    /// </summary>
    public class BehaviorUI : MonoBehaviour
    {
        /// <summary>
        /// Reference to the visualization of the objective property to cover
        /// </summary>
        [FormerlySerializedAs("objectivePropertyUI")]
        [Tooltip("Reference to the visualization of the objective property to cover")]
        [SerializeField] private PropertyUI objectivePropertyUI;

        /// <summary>
        /// Reference to the visualization of all the execution plans
        /// </summary>
        [Tooltip("Reference to the visualization of all the execution plans")]
        [SerializeField] private ExecutionPlansUI executionPlansUI;
        
        /// <summary>
        /// Reference to the TMP component that will display the status of the selected MapElement
        /// </summary>
        [Tooltip("Reference to the TMP component that will display the status of the selected MapElement")]
        [SerializeField] private TMP_Text statusText;
        
        /// <summary>
        /// The MapElement that is selected and the information of which is being displayed
        /// </summary>
        private MapElement selectedMapElement;

        /// <summary>
        /// Displays the given Property as the objective in the UI
        /// </summary>
        /// <param name="newObjectiveProperty">The PropertyOwnership that is the objective Property to display in the UI.</param>
        public void DisplayObjectiveProperty(PropertyOwnership newObjectiveProperty)
        {
            bool showObjectivePropertyUI = newObjectiveProperty != null;
            objectivePropertyUI.gameObject.SetActive(showObjectivePropertyUI);
            if (showObjectivePropertyUI)
                objectivePropertyUI.Setup(newObjectiveProperty);

        }

        /// <summary>
        /// Displays the given execution plan in the UI 
        /// </summary>
        /// <param name="executionPlans">The ExecutionPlans to display in the UI.</param>
        public void DisplayExecutionPlans(List<ExecutionPlan> executionPlans)
        {
            bool showExecutionPlansUI = executionPlans != null;
            executionPlansUI.gameObject.SetActive(showExecutionPlansUI);
            if (showExecutionPlansUI)
                executionPlansUI.Setup(executionPlans);
        }

        /// <summary>
        /// Displays the behavior information regarding the given selected MapElement
        /// </summary>
        /// <param name="selectedMapElement">The MapElement to show the behavior information of</param>
        public void Setup(MapElement selectedMapElement)
        {
            bool showUI = selectedMapElement != null;
            this.selectedMapElement = selectedMapElement;
            gameObject.SetActive(showUI);

            if (showUI)
            {
                DisplayExecutionPlans(selectedMapElement.executionPlans);
                DisplayObjectiveProperty(selectedMapElement.propertyOwnershipToCover);
            }
        }

        private void Update()
        {
            if (selectedMapElement != null)
                statusText.text = selectedMapElement.GetStatus();
            else
                statusText.text = $"";
        }
    }
}