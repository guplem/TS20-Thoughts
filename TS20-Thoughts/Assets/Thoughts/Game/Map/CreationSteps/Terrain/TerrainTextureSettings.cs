using Thoughts.Utils.Inspector;
using UnityEngine;

namespace Thoughts.Game.Map.CreationSteps.Terrain
{
    /// <summary>
    /// The configuration of the texture of the terrain of the map
    /// </summary>
    [CreateAssetMenu(fileName = "TextureSettings", menuName = "Thoughts/Map/Terrain Texture Settings", order = 22)]
    public class TerrainTextureSettings : UpdatableData
    {
        /// <summary>
        /// The material used for the terrain. Used in the meshRenderer to displays the visuals.
        /// </summary>
        [Tooltip("The material used for the terrain. Used in the meshRenderer to displays the visuals.")]
        [SerializeField] public Material material;

    #region Colors

        // Color 0
        [Space]
        public Color color0;
    
        // Color 1
        [Space]
        public Color color1;
        [Range(0, 1)]
        public float color1Start;
        [Range(0,1)]
        public float color1BaseBlend;
    
        // Color 2
        [Space]
        public Color color2;
        [Range(0, 1)]
        public float color2Start;
        [Range(0,1)]
        public float color2BaseBlend;
    
        // Color 3
        [Space]
        public Color color3;
        [Range(0, 1)]
        public float color3Start;
        [Range(0,1)]
        public float color3BaseBlend;

    #endregion
    
        /// <summary>
        /// The minimum height of the terrain
        /// </summary>
        private float minHeight; //Used by the shader
        /// <summary>
        /// The maximum height of the terrain
        /// </summary>
        private float maxHeight; //Used by the shader
    
        /// <summary>
        /// Applies the current texture settings to the terrain's material 
        /// </summary>
        /// <param name="minHeight">The minimum height of the terrain</param>
        /// <param name="maxHeight">The maximum height of the terrain</param>
        public void ApplyToMaterial(float minHeight, float maxHeight)
        {
            material.SetColor("color0", color0);
        
            material.SetFloat("color1Start", color1Start);
            material.SetColor("color1", color1);
            material.SetFloat("color1BaseBlend", color1BaseBlend);
        
            material.SetFloat("color2Start", color2Start);
            material.SetColor("color2", color2);
            material.SetFloat("color2BaseBlend", color2BaseBlend);

            material.SetFloat("color3Start", color3Start);
            material.SetColor("color3", color3);
            material.SetFloat("color3BaseBlend", color3BaseBlend);
        
            UpdateMeshHeights(minHeight, maxHeight);
        }

        /// <summary>
        /// Updates the minimum and maximum height of the terrain
        /// </summary>
        /// <param name="minHeight">The minimum height of the terrain</param>
        /// <param name="maxHeight">The maximum height of the terrain</param>
        public void UpdateMeshHeights(float minHeight, float maxHeight)
        {
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
        
            material.SetFloat("minHeight", minHeight);
            material.SetFloat("maxHeight", maxHeight);
        
            //Debug.Log($"Set the minHeight to {minHeight} and maxHeight to {maxHeight}");
        }

    }
}
