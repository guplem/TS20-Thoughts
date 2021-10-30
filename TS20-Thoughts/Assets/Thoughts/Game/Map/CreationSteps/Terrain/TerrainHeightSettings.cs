using Thoughts.Utils.Inspector;
using Thoughts.Utils.Maths;
using UnityEngine;
using UnityEngine.Serialization;

namespace Thoughts.Game.Map.Terrain
{
    /// <summary>
    /// The configuration of the height of the terrain of the map
    /// </summary>
    [CreateAssetMenu(fileName = "TerrainHeightSettings", menuName = "Thoughts/Map/Terrain Height Settings", order = 20)]
    public class TerrainHeightSettings : UpdatableData
    {
        /// <summary>
        /// Reference to a collection of settings used to generate the noise map for the terrain
        /// </summary>
        [FormerlySerializedAs("noiseMapSettings")]
        [Tooltip("Collection of settings used to generate the noise map for the terrain")]
        [FormerlySerializedAs("noiseSettings")]
        public NoiseMapSettings noiseMapSettings;
    
        /// <summary>
        /// The amount of different types of LOD supported in the terrain generation. Tested only with a value of 5.
        /// </summary>
        public const int numSupportedTerrainLODs = 5;
    

        [Header("Falloff")]
        [SerializeField] public bool useFalloff;
        /// <summary>
        /// An area in which the falloff will not be applied starting from the center
        /// </summary>
        [Tooltip("An area in which the falloff will not be applied starting from the center")]
        [SerializeField] public float freeFalloffAreaRadius = 30f;
        //[Range(0.1f, 1f)]
        //[SerializeField] public float percentageOfMapWithoutMaxFalloff = 1;
        [SerializeField] public AnimationCurve falloffIntensity;
        [SerializeField] public NoiseMapSettings falloffNoiseMapSettings;
    

        /// <summary>
        /// For how much each cell height will be multiplied.
        /// <para>The height of the cell is by default [0,1], multiplying it by 5, the maximum height will be 5 (cell height [0,5]</para>
        /// </summary>
        [Header("Height")]
        public float heightMultiplier = 30f;
        /// <summary>
        /// How much the height of the mesh should be affected by the maxHeight (AKA: "height multiplier")
        /// </summary>
        public AnimationCurve heightCurve;
    
        /// <summary>   
        /// The minimum height of the terrain
        /// </summary>
        public float minHeight{
            get
            {
                float ret = heightMultiplier * heightCurve.Evaluate(0);
                //Debug.Log($"MIN: {ret}");
                return ret;
            }
        }
        /// <summary>
        /// The maximum height of the terrain
        /// </summary>
        public float maxHeight{
            get {
                float ret = heightMultiplier * heightCurve.Evaluate(1);
                //Debug.Log($"MAX: {ret}");
                return ret;
            }
        }

    #if UNITY_EDITOR
    
        protected override void OnValidate()
        {
            noiseMapSettings.ValidateValues();

            base.OnValidate();
        }
    
    #endif
    }
}
