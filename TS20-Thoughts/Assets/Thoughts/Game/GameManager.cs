using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Thoughts.ControlSystems;
using Thoughts.Game.Attributes;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.Game
{
    /// <summary>
    /// Controls the core aspects of the a game: Participant (with ControlSystem), Map setup, ... 
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// The GameObject prefab of the local human (manual) control system for a participant.
        /// </summary>
        [Header("Control Systems")]
        [SerializeField] private GameObject manualControlSystemPrefab;
        
        /// <summary>
        /// The map of the game.
        /// <para>A component in a GameObject</para>
        /// </summary>
        [Header("Game Elements")]
        [SerializeField] public Map map;
        
        /// <summary>
        /// The different participants (players, AI, ...) of the game.
        /// </summary>
        private readonly List<Participant> participants = new List<Participant>();
        
        /// <summary>
        /// The amount of time between each update of the MapElements of the game
        /// </summary>
        [SerializeField] public float gameClockInterval = 1f;

        /// <summary>
        /// Starts a new game by setting up the participants and generating a new world.
        /// </summary>
        public void StartNewGame()
        {
            // Create a manual control system
            GameObject controlSystem = Instantiate(manualControlSystemPrefab);
            
            // Add a participant to the game (controlled manually)
            participants.Add(new Participant(controlSystem));
            
            // Build a new scenario
            map.GenerateNew(participants);
        }

        /// <summary>
        /// Recursively look for all MapEvents available in the game's map that, as consequence of the event, they make a desired attribute value increase for the owner/executer/target (the needed participant).
        /// </summary>
        /// <param name="attributeOwnershipToCovered attribute to increase the value of.</param>
        /// <param name="valueToCover">The amount of value needed to be covered (increased).</param>
        /// <param name="executer">Map element that is going to execute the list of ExecutionPlans.</param>
        /// <param name="mapEventsToExecute">Execution plans wanted to be executed previously to the ones to cover the attributeToCover.</param>
        /// <param name="iteration">The iteration number of the this method's recursive execution. Should start as 0.</param>
        /// <returns>An ordered list of the Execution Plans needed to achieve the goal (to increase the value of the attributeToCover by valueToCover)</returns>
        public List<ExecutionPlan> GetExecutionPlanToCover(AttributeOwnership attributeOwnershipToCover, int valueToCover, MapElement executer, List<ExecutionPlan> mapEventsToExecute = null, int iteration = 0)
        {
            if (iteration >= 50)
            {
                Debug.LogWarning($" ◙ Stopping the search of an execution plan to cover '{valueToCover}' of '{attributeOwnershipToCover.attribute}' after {iteration} iterations.\n");
                mapEventsToExecute.DebugLog("\n - ", " ◙ So far, the execution path found was: \n");
                return null;
            }
            
            Debug.Log($" ◌ Searching for an execution plan to cover '{valueToCover}' of '{attributeOwnershipToCover.attribute}' owned by '{attributeOwnershipToCover.owner}' executed by '{executer}'.    Iteration {iteration}.\n");
            
            if (mapEventsToExecute == null) 
                mapEventsToExecute = new List<ExecutionPlan>();

            ExecutionPlan lastExecutionPlan = map.GetExecutionPlanToCover(attributeOwnershipToCover, valueToCover, executer);
            
            //if (lastExecutionPlan != null) Debug.Log($" ◍ Execution plan for covering '{ownedAttribute.attribute}' in '{ownedAttribute.ownerMapElement}' is -> {lastExecutionPlan}\n");
            //else Debug.LogWarning($" ◍ No execution plan for covering '{ownedAttribute.attribute}' in '{ownedAttribute.ownerMapElement}' could be found using the 'Map.GetExecutionPlanToTakeCareOf()'.\n");
            //Debug.Log($" ◍ Found Execution Plan: {lastExecutionPlan}\n");
            if (lastExecutionPlan != null)
            {
                mapEventsToExecute.Add(lastExecutionPlan);

                List<AttributeOwnership> requirementsNotMet = lastExecutionPlan.GetRequirementsNotMet(out List<int> remainingValueToCoverInRequirementsNotMet);
                if (!requirementsNotMet.IsNullOrEmpty())
                    mapEventsToExecute = GetExecutionPlanToCover(requirementsNotMet[0], remainingValueToCoverInRequirementsNotMet[0], executer, mapEventsToExecute, iteration+1);
            }
            else
            {
                Debug.LogWarning($" ◙ An execution plan to cover '{valueToCover}' of '{attributeOwnershipToCover.attribute}' was not found (at the iteration: {iteration}).   The previously found execution plans were:\n    ● {mapEventsToExecute.ToStringAllElements("\n    ● ")}\n", gameObject);
                return null;
            }

            return mapEventsToExecute;
        }   
    }
}
