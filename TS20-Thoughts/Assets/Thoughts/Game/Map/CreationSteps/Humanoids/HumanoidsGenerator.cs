using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.Map
{
    public class HumanoidsGenerator : MonoBehaviour
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
    
        public void GenerateHumanoids(bool clearPrevious)
        {
            if (clearPrevious)
                DeleteHumanoids();
        
            NavMeshSurface navMeshSurface = navigationManager.SetupNewNavMeshFor(mapGenerator.mapConfiguration.humanoidsSettings.spawnableMapElements[0].GetComponentRequired<NavMeshAgent>(), mapGenerator.mapConfiguration, false);

            mapGenerator.SpawnMapElementsRandomly(
                mapGenerator.mapConfiguration.humanoidsSettings.spawnableMapElements[0], 
                humanoidsSeed, 
                mapGenerator.mapConfiguration.humanoidsSettings.spawningHeightRange, 
                mapGenerator.mapConfiguration.humanoidsSettings.quantity, 
                this.transform,
                true
            );

        }
    
        public void DeleteHumanoids()
        {
            if (Application.isPlaying)
                this.transform.DestroyAllChildren(); 
            else
                this.transform.DestroyImmediateAllChildren();
        }
    }
}
