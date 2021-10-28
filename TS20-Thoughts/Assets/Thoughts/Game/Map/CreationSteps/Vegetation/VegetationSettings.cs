using Thoughts.Utils.Inspector;
using Thoughts.Utils.Maths;
using UnityEngine;

namespace Thoughts.Game.Map
{
    public class VegetationSettings : UpdatableData
    {
        /// <summary>
        /// Settings of the noise map used for the vegetation
        /// </summary>
        [Tooltip("Settings of the noise map used for the vegetation")]
        public NoiseMapSettings noiseSettings;
        
        /// <summary>
        /// How close the vegetation can be to the sea shore.
        /// </summary>
        [Tooltip("How close the vegetation can be to the sea shore. 1 means that it can get in the sea. 0.5 means a long distance to the sea shore.")]
        [Range(0,1)]
        [SerializeField]
        public float closenessToShore = 0.993f; //[0,1], 1 being that the vegetation can get on the sea
        
        /// <summary>
        /// Probability of vegetation appearing.
        /// </summary>
        [Tooltip("Probability of vegetation appearing at any given spot")]
        [Range(0,1)]
        [SerializeField]
        public float probability = 0.5f;
        
        /// <summary>
        /// Density of vegetation appearing.
        /// </summary>
        [Tooltip("Density of vegetation appearing at any given spot")]
        [Range(0,1)]
        [SerializeField]
        public float density = 0.5f; //Todo: use

        public GameObject[] spawnableMapElements;
    }
}
