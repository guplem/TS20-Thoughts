using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.Map.CreationSteps.Humanoids
{
    public class HumanoidsGenerator : CreationStepGenerator
    {

        /// <summary>
        ///  Reference to the manager of the AI navigation
        /// </summary>
        [Tooltip("Reference to the manager of the AI navigation")]
        [SerializeField] public MapNavigationManager navigationManager;
        
        /// <summary>
        /// The seed used by the VegetationGenerator to generate vegetation. It is an alteration of the main map's seed. 
        /// </summary>
        private int humanoidsSeed => _randomNumberToAlterMainSeed + mapManager.mapConfiguration.seed; //IT MUST NEVER CHANGE
        private const int _randomNumberToAlterMainSeed = 345678; //IT MUST NEVER CHANGE and be completely unique per generator (except the mapGenerator and those that do not need randomness)
        
    
        
        protected override void _DeleteStep()
        {
            mapManager.mapGenerator.DestroyAllMapElementsChildOf(this.transform);
            InvokeOnFinishStepDeletion();
        }
        
        protected override void _GenerateStep(bool clearPrevious)
        {
            if (clearPrevious)
                Delete();
        
            NavMeshSurface navMeshSurface = navigationManager.SetupNewNavMeshFor(mapManager.mapConfiguration.humanoidsSettings.spawnableMapElements[0].GetComponentRequired<NavMeshAgent>(), mapManager.mapConfiguration, false);

            if (mapManager.mapConfiguration.humanoidsSettings.spawnableMapElements.Length > 1)
            {
                Debug.LogWarning("The spawning of more than one type of humanoids has not been implemented");
            }
            
            mapManager.SpawnMapElementsRandomly(
                mapManager.mapConfiguration.humanoidsSettings.spawnableMapElements[0], 
                humanoidsSeed, 
                mapManager.mapConfiguration.humanoidsSettings.spawningHeightRange, 
                mapManager.mapConfiguration.humanoidsSettings.quantity, 
                this.transform,
                true
            );


            InvokeOnFinishStepGeneration();
        }
    }
}
