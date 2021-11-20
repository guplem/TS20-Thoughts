using Thoughts.Utils.Inspector;
using Thoughts.Utils.Maths;
using UnityEngine;
using UnityEngine.Serialization;

namespace Thoughts.Game.Map.CreationSteps.Terrain
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
        /// The area compared to the radius of the map (normalized) in which the falloff will not be applied (starting from the center)
        /// </summary>
        [Tooltip("The area compared to the radius of the map (normalized: [0,1]) in which the falloff will not be applied (starting from the center)")]
        [Range(0.01f, 0.99f)]
        [SerializeField] public float freeOfFalloffAreaNormalized = 0.75f;
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
        /// How much the height of the mesh (that is above water) should be affected by the maxHeight (AKA: "height multiplier for terrain")
        /// </summary>
        [Tooltip("How much the height of the mesh (that is above water) should be affected by the maxHeight (AKA: 'height multiplier for terrain')")]
        [FormerlySerializedAs("heightCurve")]
        public AnimationCurve aboveWaterHeightCurve;
        
        /// <summary>
        /// How much the height of the mesh (that is under water) should be affected by the maxHeight (AKA: "height multiplier for sea floor")
        /// </summary>
        [Tooltip("How much the height of the mesh (that is under water) should be affected by the maxHeight (AKA: 'height multiplier for sea floor')")]
        public AnimationCurve underWaterHeightCurve = AnimationCurve.EaseInOut(0,0,1,1);
    
        /// <summary>   
        /// The minimum height of the terrain (including under-water terrain)
        /// </summary>
        public float minHeight{
            get
            {
                float ret = heightMultiplier * underWaterHeightCurve.Evaluate(0);
                //Debug.Log($"MIN: {ret}");
                return ret;
            }
        }
        /// <summary>
        /// The maximum height of the terrain
        /// </summary>
        public float maxHeight{
            get {
                float ret = heightMultiplier * aboveWaterHeightCurve.Evaluate(1);
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
