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
        /// Reference to the section of the UI holding the information of the currently selected MapElement
        /// </summary>
        [Tooltip("Reference to the section of the UI holding the information of the currently selected MapElement")]
        [SerializeField] private SelectionUI selectionUI;
        
        [Header("Creation UI")]
        
        /// <summary>
        /// The index of the creation step to start with at the start of the application
        /// </summary>
        [Tooltip("The index of the creation step to start with at the start of the application")]
        [SerializeField] private int initialCreationStep; // Todo: Remove, force it to be 0
        
        /// <summary>
        /// Reference to the creation steps (sections of the UI) where the local user can create the environment/world/map
        /// </summary>
        [Tooltip("Reference to the creation steps (sections of the UI) where the local user can create the environment/world/map")]
        [SerializeField] private CreationStepUI[] creationSteps;
        
        /// <summary>
        /// Setup of the initial UI for the game (displays the UI for nothing, so no UI)
        /// </summary>
        private void Awake()
        {
            DisplayUIFor(null, true);

            for (int c = 0; c < creationSteps.Length; c++)
            {
                if (c == initialCreationStep)
                    creationSteps[c].Show();
                else
                    creationSteps[c].Hide();
            }
        }

        /// <summary>
        /// Displays the UI related to the given MapElement and subscribes the UI to changes on that mapElement that should be reflected.
        /// <para>It also unsubscribes the UI to the changes from the previous map element</para>
        /// </summary>
        /// <param name="mapElement">The MapElement from which you want to display the information in the UI.</param>
        /// <param name="forceUpdate">Ignore if the currently selected MapElement is the same as the new selected MapElement and update the UI as if they were different.</param>
        public void DisplayUIFor(MapElement mapElement, bool forceUpdate = false)
        {
            Debug.Log($"Displaying UI of '{mapElement}'");

            if (selectedMapElement != mapElement || forceUpdate)
            {
                // Unsubscribe to updates
                if (selectedMapElement != null)
                {
                    selectedMapElement.onExecutionPlansUpdated -= behaviorUI.DisplayExecutionPlans;
                    selectedMapElement.onObjectiveAttributeUpdated -= behaviorUI.DisplayObjectiveAttribute;
                }
                
                // Update
                selectedMapElement = mapElement;
                
                // Activate/Deactivate, get ready and show the current information
                selectionUI.Setup(selectedMapElement);
                behaviorUI.Setup(selectedMapElement);
                
                // Subscribe to updates
                if (selectedMapElement != null)
                {
                    selectedMapElement.onExecutionPlansUpdated += behaviorUI.DisplayExecutionPlans;
                    selectedMapElement.onObjectiveAttributeUpdated += behaviorUI.DisplayObjectiveAttribute;
                }
            }
        }
        
    }
}
