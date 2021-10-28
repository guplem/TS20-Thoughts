using Thoughts.Utils.Maths;
using UnityEngine;

namespace Thoughts.Game.Map
{
    public class VegetationGenerator : MonoBehaviour
    {
        /// <summary>
        /// Reference to the mapGenerator managing the generation of the map that contains this generator's vegetation.
        /// </summary>
        [Tooltip("Reference to the mapGenerator managing the generation of the map that contains this generator's vegetation.")]
        [SerializeField] private MapGenerator mapGenerator;

        /// <summary>
        /// The seed used by the VegetationGenerator to generate vegetation. It is an alteration of the main map's seed. 
        /// </summary>
        private int vegetationSeed => _randomNumberToAlterMainSeed + mapGenerator.mapConfiguration.seed; //IT MUST NEVER CHANGE
        private const int _randomNumberToAlterMainSeed = 5151335; //IT MUST NEVER CHANGE and be completely unique per generator (except the mapGenerator and those that do not need randomness)
        
        
        public void GenerateVegetation(bool clearPrevious)
        {
            if (clearPrevious)
                DeleteVegetation();

            mapGenerator.SpawnMapElementsWithPerlinNoiseDistribution(
                mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn[0].mapElementPrefab, 
                vegetationSeed, 
                mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn[0].closenessToShore, 
                mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn[0].probability, 
                this.transform,
                mapGenerator.mapConfiguration.vegetationSettings.mapElementsToSpawn[0].noiseSettings,
                false
            );
        }
        public void DeleteVegetation()
        {
            if (Application.isPlaying)
                this.transform.DestroyAllChildren(); 
            else
                this.transform.DestroyImmediateAllChildren();
        }
    }
}
