using System;
using System.Collections.Generic;
using Thoughts.Game.Attributes;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.ControlSystems.UI
{
    /// <summary>
    /// Controls the UI of the game
    /// </summary>
    public class GameUIManager : MonoBehaviour
    {

        /// <summary>
        /// The currently selected MapElement being displayed in the UI
        /// </summary>
        private MapElement selectedMapElement;
        
        /// <summary>
        /// Reference to the section of the UI holding the information of the current ExecutionPlans related to the objective Attribute
        /// </summary>
        [Tooltip("Reference to the section of the UI holding the information of the execution plans related to the Attribute objective")]
        [SerializeField] private BehaviorUI behaviorUI;
        
        /// <summary>
        /// Setup of the initial UI for the game (displays the UI for nothing, so no UI)
        /// </summary>
        private void Awake()
        {
            DisplayUIFor(null);
        }

        /// <summary>
        /// Displays the UI related to the given MapElement.
        /// </summary>
        /// <param name="mapElement">The MapElement from which you want to display the information in the UI.</param>
        public void DisplayUIFor(MapElement mapElement)
        {
            Debug.Log($"Displaying UI of '{mapElement}'");

            if (selectedMapElement != null)
            {
                selectedMapElement.onExecutionPlansUpdated -= UpdateExecutionPlanUI;
                selectedMapElement.onObjectiveAttributeUpdated -= UpdateObjectiveAttribute;
            }
            selectedMapElement = mapElement;
            if (selectedMapElement != null)
            {
                selectedMapElement.onExecutionPlansUpdated += UpdateExecutionPlanUI;
                selectedMapElement.onObjectiveAttributeUpdated += UpdateObjectiveAttribute;
                
                UpdateExecutionPlanUI(selectedMapElement.executionPlans);
                UpdateObjectiveAttribute(selectedMapElement.attributeOwnershipToCover);
            }
        }
        
        /// <summary>
        /// Updates the currently displayed UI related to the current Attribute objective and the ExecutionPlans of the selected MapElement.
        /// </summary>
        private void UpdateExecutionPlanUI(List<ExecutionPlan> newExecutionPlan)
        {
            //Debug.Log("*** UPDATE UpdateExecutionPlanUI");
            behaviorUI.DisplayExecutionPlans(newExecutionPlan);
        }
        
        /// <summary>
        /// Updates the currently displayed UI related to the objective Attribute 
        /// </summary>
        private void UpdateObjectiveAttribute(AttributeOwnership newObjectiveAttribute)
        {
            behaviorUI.DisplayObjectiveAttribute(newObjectiveAttribute);
        }

    }
}
