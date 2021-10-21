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
        /// How close the vegetation can be to the sea shore.
        /// </summary>
        [Tooltip("How close the vegetation can be to the sea shore. 1 means that it can get in the sea. 0.5 means a long distance to the sea shore.")]
        [Range(0,1)]
        [SerializeField] private float closenessToShore = 0.993f; //[0,1], 1 being that the vegetation can get on the sea
        
        /// <summary>
        /// Probability of vegetation appearing.
        /// </summary>
        [Tooltip("Probability of vegetation appearing at any given spot")]
        [Range(0,1)]
        [SerializeField] private float probability = 0.5f;

        /// <summary>
        /// The seed used by the VegetationGenerator to generate vegetation. It is an alteration of the main map's seed. 
        /// </summary>
        private int vegetationSeed => _randomNumberToAlterMainSeed + mapGenerator.mapConfiguration.seed; //IT MUST NEVER CHANGE
        private const int _randomNumberToAlterMainSeed = 5151335; //IT MUST NEVER CHANGE and be completely unique per generator (except the mapGenerator and those that do not need randomness)
        
        
        public void GenerateVegetation(bool clearPrevious)
        {
            if (clearPrevious)
                DeleteVegetation();

            mapGenerator.GenerateMapElementsWithPerlinNoiseDistribution(
                mapGenerator.mapConfiguration.vegetationCollection.mapElements[0], 
                vegetationSeed, 
                closenessToShore, 
                probability, 
                this.transform,
                mapGenerator.mapConfiguration.vegetationNoiseSettings
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
