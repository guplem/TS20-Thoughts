using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Thoughts.ControlSystems;
using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.Game
{
    public class GameManager : MonoBehaviour
    {
        [Header("Control Systems")]
        [SerializeField] private GameObject manualControlSystemPrefab;
        
        [Header("Game Elements")]
        [SerializeField] public GameMap.Map map;
        
        private readonly List<Participant> participants = new List<Participant>();

        public void StartNewGame()
        {
            // Create a manual control system
            GameObject controlSystem = Instantiate(manualControlSystemPrefab);
            
            // Add a participant to the game (controlled manually)
            participants.Add(new Participant(controlSystem));
            
            // Build a new scenario
            map.BuildNew(participants);
        }

        /// <summary>
        /// Look for all MapEvents that, as consequence of the event, they make the attribute value increase for the owner/executer/target (the needed participant).
        /// </summary>
        /// <param name="ownedAttributeToCover"></param>
        /// <param name="remainingValueToCover"></param>
        /// <param name="caregiver">Map element that wants to take care og the attribute</param>
        /// <returns></returns>
        public List<ExecutionPlan> GetExecutionPlanToCoverThisAttribute([NotNull] OwnedAttribute ownedAttributeToCover, int remainingValueToCover, MapElement caregiver, List<ExecutionPlan> mapEventsToExecute = null, int iteration = 0)
        {
            if (ownedAttributeToCover == null)
                throw new ArgumentNullException(nameof(ownedAttributeToCover));
            
            Debug.Log($" ◌ Searching for an execution plan to cover '{ownedAttributeToCover.attribute}' owned by '{ownedAttributeToCover.ownerMapElement}' executed by '{caregiver}'.    Iteration {iteration}.\n");
            
            if (iteration >= 50)
            {
                Debug.LogWarning($" ◙ Stopping the search of an execution plan for {ownedAttributeToCover.attribute} after {iteration} iterations.\n");
                mapEventsToExecute.DebugLog("\n - ", " ◙ The execution path found was: \n");
                return null;
            }
            
            if (mapEventsToExecute == null) 
                mapEventsToExecute = new List<ExecutionPlan>();

            ExecutionPlan lastExecutionPlan = map.GetExecutionPlanToTakeCareOf(ownedAttributeToCover, remainingValueToCover, caregiver);
            
            //if (lastExecutionPlan != null) Debug.Log($" ◍ Execution plan for covering '{ownedAttribute.attribute}' in '{ownedAttribute.ownerMapElement}' is -> {lastExecutionPlan}\n");
            //else Debug.LogWarning($" ◍ No execution plan for covering '{ownedAttribute.attribute}' in '{ownedAttribute.ownerMapElement}' could be found using the 'Map.GetExecutionPlanToTakeCareOf()'.\n");

            if (lastExecutionPlan != null)
            {
                mapEventsToExecute.Add(lastExecutionPlan);

                List<int> remainingValueToCoverInRequirementsNotMet;
                List<OwnedAttribute> requirementsNotMet = lastExecutionPlan.GetRequirementsNotMet(out remainingValueToCoverInRequirementsNotMet);
                if (!requirementsNotMet.IsNullOrEmpty())
                    mapEventsToExecute = GetExecutionPlanToCoverThisAttribute(requirementsNotMet[0], remainingValueToCoverInRequirementsNotMet[0]*lastExecutionPlan.executionTimes, caregiver, mapEventsToExecute, iteration+1);
            }

            return mapEventsToExecute;
        }   
    }
}
