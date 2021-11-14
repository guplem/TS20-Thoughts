using System;
using Sirenix.OdinInspector;
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
        [AssetsOnly]
        public GameObject mapElementPrefab;
        
        /// <summary>
        /// Settings of the noise map used for the vegetation
        /// </summary>
        [Tooltip("Settings of the noise map used for the vegetation")]
        public NoiseMapSettings noiseSettings;
        
        /// <summary>
        /// In which area of the map this is wanted to spawn. -1 means the bottom of the sea. 1 means the highest points in the world. 0 is the shoreline.
        /// </summary>
        [Tooltip("In which area of the map this is wanted to spawn. -1 means the bottom of the sea. 1 means the highest points in the world. 0 is the shoreline.")]
        [GD.MinMaxSlider.MinMaxSlider(-1,1)]
        //[MinMaxSlider(-1f, 1f, true)] // Odin. It is not working, IDK why // ToDo: fix Odin --> https://odininspector.com/tutorials/using-attributes/how-to-use-odin-inspector-with-scriptable-objects
        public Vector2 spawningHeightRange;
        
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
