using System;
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
        /// Reference to the section of the UI holding the information of the execution plans related to the Attribute objective
        /// </summary>
        [Tooltip("Reference to the section of the UI holding the information of the execution plans related to the Attribute objective")]
        [SerializeField] private ExecutionPlanWithObjectiveUI executionPlanWithObjective;
        
        /// <summary>
        /// Setup of the initial UI for the game (displays the UI for nothing, so no UI)
        /// </summary>
        private void Awake()
        {
            DisplayUIFor(null);
        }

        /// <summary>
        /// Displays the UI related to the given MapElement. Hides the UI if the given MapElement is null
        /// </summary>
        /// <param name="mapElement">The MapElement from which you want to display the information in the UI.</param>
        public void DisplayUIFor(MapElement mapElement)
        {
            Debug.Log($"Displaying UI of '{mapElement}'");

            executionPlanWithObjective.Setup(mapElement);

        }
    
    }
}
