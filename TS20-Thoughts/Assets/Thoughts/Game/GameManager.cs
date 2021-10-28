using System.Collections.Generic;
using Thoughts.Game.Map;
using Thoughts.Game.Map.MapElements;
using Thoughts.Game.Map.MapElements.Attributes;
using Thoughts.Game.Map.MapElements.Attributes.MapEvents;
using Thoughts.Participants;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Thoughts.Game
{
    /// <summary>
    /// Controls the core aspects of the a game: Participant (with ControlSystem), Map setup, ... 
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// The singleton reference to this class
        /// </summary>
        public static GameManager instance { get; private set; }
        
        /// <summary>
        /// The GameObject prefab of the local human (manual) control system for a participant.
        /// </summary>
        [Header("Control Systems")]
        [SerializeField] private GameObject manualControlSystemPrefab;
        
        /// <summary>
        /// The map of the game.
        /// <para>A component in a GameObject</para>
        /// </summary>
        [FormerlySerializedAs("map")]
        [Header("Game Elements")]
        [SerializeField] public MapManager mapManager;

        /// <summary>
        /// The different participants (players, AI, ...) of the game.
        /// </summary>
        private readonly List<Participant> participants = new List<Participant>();
        
        /// <summary>
        /// The participant playing manually in the local machine
        /// </summary>
        public Participant localManualParticipant { get; private set; }
        
        /// <summary>
        /// The amount of time between each update of the MapElements of the game
        /// </summary>
        [SerializeField] public float gameClockInterval = 1f;
        
        /// <summary>
        /// Starts a new game by setting up the participants and deleting the previously generated world.
        /// </summary>
        public void StartNewGame()
        {
            // Create a manual control system
            GameObject controlSystem = Instantiate(manualControlSystemPrefab);
            
            // Add a participant to the game (manually controlled)
            Participant localManualParticipant = new Participant(controlSystem);
            participants.Add(localManualParticipant);
            this.localManualParticipant = localManualParticipant;
            
            // Delete the previously generated world
            mapManager.DeleteMap();
        }

        /// <summary>
        /// Recursively look for all MapEvents available in the game's map that, as consequence of the event, they make a desired attribute value increase for the owner/executer/target (the needed participant).
        /// </summary>
        /// <param name="attributeOwnershipToCover">AttributeOwnership to increase the value of.</param>
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

            ExecutionPlan lastExecutionPlan = mapManager.GetExecutionPlanToCover(attributeOwnershipToCover, valueToCover, executer);
            
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

        /// <summary>
        /// Tries to set up this instance as the singleton instance.
        /// Additionally, it checks for the existence of an AppManager, which might be needed in some parts of the game
        /// </summary>
        private void Awake()
        {
            if (AppManager.instance == null)
                Debug.LogWarning($"An {nameof(AppManager)} is required to manage the App/Software/Program from within the game. A scene with an {nameof(AppManager)} prefab in it should load the necessary components (such as the Game Scene) to start the game, so the scene from which the 'Play Mode' is started shouldn't need a {nameof(GameManager)} only an {nameof(AppManager)}.", this);
            
            InitializeSingleton();
        }
        
        /// <summary>
        /// If there is no other singleton instance of this class, this is set as it. Being the only one, following the Singleton pattern.
        /// </summary>
        /// <returns>True if it became the singleton instance, false if another instance declared as the singleton one already exists.</returns>
        private bool InitializeSingleton()
        {
            if (instance == null)
            {
                instance = this;
                return true;
            }
            
            Debug.LogError($"More than one {nameof(GameManager)} exist. The gameObject '{instance.gameObject.name}' was assigned before '{this.gameObject.name}'");
            return false;
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        private void Start()
        {
            string gameSceneName = gameObject.scene.name;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(gameSceneName)); // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.SetActiveScene.html
            Debug.Log($"New active scene: '{SceneManager.GetActiveScene().name}'.");

            
            StartNewGame();
        }
    }
}
