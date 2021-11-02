using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.Map
{
    public class HumanoidsGenerator : CreationStepGenerator
    {
        /// <summary>
        /// Reference to the mapGenerator managing the generation of the map that contains this generator's vegetation.
        /// </summary>
        [Tooltip("Reference to the mapGenerator managing the generation of the map that contains this generator's vegetation.")]
        [SerializeField] private MapGenerator mapGenerator;
        
        /// <summary>
        ///  Reference to the manager of the AI navigation
        /// </summary>
        [Tooltip("Reference to the manager of the AI navigation")]
        [SerializeField] public MapNavigationManager navigationManager;
        
        /// <summary>
        /// The seed used by the VegetationGenerator to generate vegetation. It is an alteration of the main map's seed. 
        /// </summary>
        private int humanoidsSeed => _randomNumberToAlterMainSeed + mapGenerator.mapConfiguration.seed; //IT MUST NEVER CHANGE
        private const int _randomNumberToAlterMainSeed = 345678; //IT MUST NEVER CHANGE and be completely unique per generator (except the mapGenerator and those that do not need randomness) //TODO: add to terrainGenerator so it doesn't use the main
    
        private void GenerateHumanoids(bool clearPrevious)
        {
            if (clearPrevious)
                DeleteHumanoids();
        
            NavMeshSurface navMeshSurface = navigationManager.SetupNewNavMeshFor(mapGenerator.mapConfiguration.humanoidsSettings.spawnableMapElements[0].GetComponentRequired<NavMeshAgent>(), mapGenerator.mapConfiguration, false);

            if (mapGenerator.mapConfiguration.humanoidsSettings.spawnableMapElements.Length > 1)
            {
                Debug.LogWarning("The spawning of more than one type of humanoids has not been implemented");
            }
            
            mapGenerator.SpawnMapElementsRandomly(
                mapGenerator.mapConfiguration.humanoidsSettings.spawnableMapElements[0], 
                humanoidsSeed, 
                mapGenerator.mapConfiguration.humanoidsSettings.spawningHeightRange, 
                mapGenerator.mapConfiguration.humanoidsSettings.quantity, 
                this.transform,
                true
            );


            InvokeOnFinishStepGeneration();
        }
    
        [ContextMenu("DeleteHumanoids")]
        public void DeleteHumanoids()
        {
            mapGenerator.DestroyAllMapElementsChildOf(this.transform);
        }
        protected override void GenerateStep(bool clearPrevious, bool generateNextStepOnFinish)
        {
            Debug.Log($"Generating in {this.name}.generateNextStepOnFinish = {generateNextStepOnFinish}", this);
            //base.GenerateStep(clearPrevious, generateNextStepOnFinish);
            GenerateHumanoids(clearPrevious);
        }
    }
}
