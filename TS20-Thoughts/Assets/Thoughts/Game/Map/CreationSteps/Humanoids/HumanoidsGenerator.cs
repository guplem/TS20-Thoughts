using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.Map
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
        private int humanoidsSeed => _randomNumberToAlterMainSeed + mapManager.mapGenerator.mapConfiguration.seed; //IT MUST NEVER CHANGE
        private const int _randomNumberToAlterMainSeed = 345678; //IT MUST NEVER CHANGE and be completely unique per generator (except the mapGenerator and those that do not need randomness)
    
        private void GenerateHumanoids(bool clearPrevious)
        {
            if (clearPrevious)
                Delete();
        
            NavMeshSurface navMeshSurface = navigationManager.SetupNewNavMeshFor(mapManager.mapGenerator.mapConfiguration.humanoidsSettings.spawnableMapElements[0].GetComponentRequired<NavMeshAgent>(), mapManager.mapGenerator.mapConfiguration, false);

            if (mapManager.mapGenerator.mapConfiguration.humanoidsSettings.spawnableMapElements.Length > 1)
            {
                Debug.LogWarning("The spawning of more than one type of humanoids has not been implemented");
            }
            
            mapManager.mapGenerator.SpawnMapElementsRandomly(
                mapManager.mapGenerator.mapConfiguration.humanoidsSettings.spawnableMapElements[0], 
                humanoidsSeed, 
                mapManager.mapGenerator.mapConfiguration.humanoidsSettings.spawningHeightRange, 
                mapManager.mapGenerator.mapConfiguration.humanoidsSettings.quantity, 
                this.transform,
                true
            );


            InvokeOnFinishStepGeneration();
        }
    
        
        protected override void _DeleteStep()
        {
            mapManager.mapGenerator.DestroyAllMapElementsChildOf(this.transform);
        }
        
        protected override void _GenerateStep(bool clearPrevious)
        {
            GenerateHumanoids(clearPrevious);
        }
    }
}
