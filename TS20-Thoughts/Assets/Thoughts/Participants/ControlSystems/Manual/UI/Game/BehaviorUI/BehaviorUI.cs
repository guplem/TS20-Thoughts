using System.Collections.Generic;
using Thoughts.Game.Map.MapElements;
using Thoughts.Game.Map.MapElements.Attributes;
using Thoughts.Game.Map.MapElements.Attributes.MapEvents;
using Thoughts.Participants.ControlSystems.Manual.UI.Game.Shared;
using UnityEngine;

namespace Thoughts.Participants.ControlSystems.Manual.UI.Game.BehaviorUI
{
    /// <summary>
    /// Controls the display of the information in the UI related to the behavior of a MapElement (objective Attribute and planned ExecutionPlans)
    /// </summary>
    public class BehaviorUI : MonoBehaviour
    {
        /// <summary>
        /// Reference to the visualization of the objective attribute to cover
        /// </summary>
        [Tooltip("Reference to the visualization of the objective attribute to cover")]
        [SerializeField] private AttributeUI objectiveAttributeUI;

        /// <summary>
        /// Reference to the visualization of all the execution plans
        /// </summary>
        [Tooltip("Reference to the visualization of all the execution plans")]
        [SerializeField] private ExecutionPlansUI executionPlansUI;

        /// <summary>
        /// Displays the given Attribute as the objective in the UI
        /// </summary>
        /// <param name="newObjectiveAttribute">The AttributeOwnership that is the objective Attribute to display in the UI.</param>
        public void DisplayObjectiveAttribute(AttributeOwnership newObjectiveAttribute)
        {
            bool showObjectiveAttributeUI = newObjectiveAttribute != null;
            objectiveAttributeUI.gameObject.SetActive(showObjectiveAttributeUI);
            if (showObjectiveAttributeUI)
                objectiveAttributeUI.Setup(newObjectiveAttribute);

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

            gameObject.SetActive(showUI);

            if (showUI)
            {
                DisplayExecutionPlans(selectedMapElement.executionPlans);
                DisplayObjectiveAttribute(selectedMapElement.attributeOwnershipToCover);
            }
        }
    }
}