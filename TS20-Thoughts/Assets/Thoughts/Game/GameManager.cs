using System.Collections.Generic;
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
    /// <param name="caregiver">Map element that wants to take care og the attribute</param>
    /// <returns></returns>
    public List<ExecutionPlan> GetExecutionPlanToCoverThisAttribute(OwnedAttribute ownedAttribute, MapElement caregiver, List<ExecutionPlan> mapEventsToTakeCare = null, int iteration = 0)
    {
        Debug.Log($" ◌ Searching for an execution plan to cover '{ownedAttribute.attribute}' at '{ownedAttribute.ownerMapElement}' by '{caregiver}'\n");
        
        if (iteration >= 50)
        {
            Debug.LogWarning($" ◙ Stopping the search of an execution plan for {ownedAttribute.attribute} after {iteration} iterations.\n");
            return null;
        }
        
        if (mapEventsToTakeCare == null) 
            mapEventsToTakeCare = new List<ExecutionPlan>();

        ExecutionPlan lastExecutionPlan = map.GetExecutionPlanToTakeCareOf(ownedAttribute, caregiver);
        Debug.Log($" ◍ Execution plan for covering '{ownedAttribute.attribute}' in '{ownedAttribute.ownerMapElement}' is -> {lastExecutionPlan}\n");

        if (lastExecutionPlan != null)
        {
            List<OwnedAttribute> requirementsNotMet = lastExecutionPlan.GetRequirementsNotMet();
            if (!requirementsNotMet.IsNullOrEmpty())
                mapEventsToTakeCare = GetExecutionPlanToCoverThisAttribute(requirementsNotMet[0], caregiver, mapEventsToTakeCare, iteration+1);
        }
        
        //Todo: check if last item in 'mapEventsToTakeCare' covers the attribute in the parameter 'ownedAttribute'. If not, throw a warning - ExecutionPlanNotFound
        return mapEventsToTakeCare;
    }
    }
}
