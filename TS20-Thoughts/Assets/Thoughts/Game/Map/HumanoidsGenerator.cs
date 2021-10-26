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
        /// How close the vegetation can be to the sea shore.
        /// </summary>
        [Tooltip("How close the vegetation can be to the sea shore. 1 means that it can get in the sea. 0.5 means a long distance to the sea shore.")]
        [Range(0,1)]
        [SerializeField] private float closenessToShore = 0.993f; //[0,1], 1 being that the vegetation can get on the sea

        /// <summary>
        /// Amount of humans to spawn
        /// </summary>
        [Tooltip("Amount of humans to spawn")]
        [SerializeField] private int quantity = 2;

        /// <summary>
        /// The seed used by the VegetationGenerator to generate vegetation. It is an alteration of the main map's seed. 
        /// </summary>
        private int humanoidsSeed => _randomNumberToAlterMainSeed + mapGenerator.mapConfiguration.seed; //IT MUST NEVER CHANGE
        private const int _randomNumberToAlterMainSeed = 345678; //IT MUST NEVER CHANGE and be completely unique per generator (except the mapGenerator and those that do not need randomness) //TODO: add to terrainGenerator so it doesn't use the main
    
        public void GenerateHumanoids(bool clearPrevious)
        {
            if (clearPrevious)
                DeleteHumanoids();
        
            NavMeshSurface navMeshSurface = mapGenerator.SetupNewNavMeshFor(mapGenerator.mapConfiguration.humanoidCollection.mapElements[0].GetComponentRequired<NavMeshAgent>());

            mapGenerator.SpawnMapElementsRandomly(
                mapGenerator.mapConfiguration.humanoidCollection.mapElements[0], 
                humanoidsSeed, 
                closenessToShore, 
                quantity, 
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
