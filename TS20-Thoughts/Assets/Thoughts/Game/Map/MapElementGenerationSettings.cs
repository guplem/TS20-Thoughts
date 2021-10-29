using System;
using Thoughts.Utils.Maths;
using UnityEngine;

namespace Thoughts.Game.Map
{
    [Serializable]
    public class MapElementGenerationSettings
    {
        /// <summary>
        /// The prefab of the mapElement that this object's spawning settings is for
        /// </summary>
        [Tooltip("The prefab of the mapElement that this object's spawning settings is for")]
        public GameObject mapElementPrefab;
        
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
        public float closenessToShore = 0.993f; //[0,1], 1 being that the vegetation can get on the sea
        
        /// <summary>
        /// Probability of vegetation appearing. Only used when the spawning uses the perlin noise distribution, not by count.
        /// </summary>
        [Tooltip("Probability of vegetation appearing at any given spot. Only used when the spawning uses the perlin noise distribution, not by count.")]
        [Range(0,1)]
        public float probability = 0.5f;
        
        /// <summary>
        /// Density of vegetation appearing. Only used when the spawning uses the perlin noise distribution, not by count.
        /// </summary>
        [Tooltip("Density of vegetation appearing at any given spot. Only used when the spawning uses the perlin noise distribution, not by count.")]
        [Range(0,1)]
        public float density = 0.5f;
        
    }
}
