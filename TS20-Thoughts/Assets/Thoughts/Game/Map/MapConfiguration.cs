using System;
using Thoughts.Game.Map.Terrain;
using Thoughts.Utils.Inspector;
using Thoughts.Utils.Maths;
using UnityEngine;

namespace Thoughts.Game.Map
{
    /// <summary>
    /// Holds all the configuration/settings used to generate the Map
    /// </summary>
    [CreateAssetMenu(fileName = "MapConfiguration", menuName = "Thoughts/Map/Map Configuration", order = 20)]
    public class MapConfiguration : UpdatableData, IEquatable<MapConfiguration>
    {

        /// <summary>
        /// The seed used for the whole world randomness
        /// </summary>
        [Space]
        public int seed = 420;

        /// <summary>
        /// The index of the size of the chunks (in world space)
        /// Making a chunk smaller will not make it more polygon dense, it will have less polygons overall 
        /// </summary>
        [Range(0, numSupportedChunkSizes-1)]
        [Tooltip("The index of the size of the chunks (in world space). Making a chunk smaller will not make it more polygon dense, it will have less polygons overall. Sizes (squares of Unity's units): 0=24, 1=48, 2=72, 3=96, 4=120, 5=144, 6=168, 7=192, 8=216, 9=240")]
        public int chunkSizeIndex = 9;
        /// <summary>
        /// All the available sizes (in world space) for the chunks
        /// </summary>
        public static readonly int[] supportedChunkSizes = {24, 48, 72, 96, 120, 144, 168, 192, 216, 240};
        /// <summary>
        /// Count of all the available/supported chunk sizes
        /// </summary>
        public const int numSupportedChunkSizes = 10; // supportedChunkSizes.Length
    
        /// <summary>
        /// Number of vertices per line of a mesh rendered at the max resolution (LOD = 0).
        /// It includes the 2 extra vertices that are excluded from final mesh, but used for calculating normals.
        /// </summary>
        public int numVertsPerLine => supportedChunkSizes[chunkSizeIndex] + 5; // This is the max size of a mesh for unity. It will generate a mesh of dimensions of chunkSize-1
        //public int chunkSize => supportedChunkSizes[chunkSizeIndex] + 1; // Changed in the last episode to +5 but seemed weird. //Todo: delete comment after testing that all works

        /// <summary>
        /// The space the terrain mesh takes up in the world.
        /// </summary>
        public float meshWorldSize => (numVertsPerLine - 3);

        /// <summary>
        /// The radius of the map from the center of the scene in Unity's units
        /// </summary>
        [Tooltip("The radius of the map from the center of the scene in Unity's units")]
        public float mapRadius = 500;

        /// <summary>
        /// The configuration of the height of the terrain of the map
        /// </summary>
        [Header("Terrain")]
        [Tooltip("The configuration of the height of the terrain of the map")]
        public HeightMapSettings heightMapSettings;

        /// <summary>
        /// The configuration of the texture of the terrain of the map
        /// </summary>
        [Tooltip("The configuration of the texture of the terrain of the map")]
        public TextureSettings textureSettings;


        /// <summary>
        /// The MapElementsCollection containing all the vegetation that should be used in the map
        /// </summary>
        [Header("Vegetation")]
        [Tooltip("The MapElementsCollection containing all the vegetation that should be used in the map")]
        public MapElementsCollection vegetationCollection;
        /// <summary>
        /// Settings of the noise map used for the vegetation
        /// </summary>
        [Tooltip("Settings of the noise map used for the vegetation")]
        public NoiseMapSettings vegetationNoiseSettings;
        
        
        /// <summary>
        /// The MapElementsCollection containing all the humanoids that should be used in the map
        /// </summary>
        [Header("Humanoids")]
        [Tooltip("The MapElementsCollection containing all the humanoids that should be used in the map")]
        public MapElementsCollection humanoidCollection;
        /// <summary>
        /// Settings of the noise map used for the humanoids
        /// </summary>
        [Tooltip("Settings of the noise map used for the humanoids")]
        public NoiseMapSettings humanoidNoiseSettings;
        
        
        
    
    #if UNITY_EDITOR
    
        /// <summary>
        /// A previously used HeightMapSettings. It shouldn't be used actively, only to check for updates of the in editor.
        /// </summary>
        private HeightMapSettings _oldHeightMapSettings;
        /// <summary>
        /// A previously used TextureSettings. It shouldn't be used actively, only to check for updates of the in editor.
        /// </summary>
        private TextureSettings _oldTextureSettings;
    

        //TODO: Improve the auto update system (time intervals, wait for the previous preview to fully load, ...)
        protected override void OnValidate()
        {
        
            // If settings have been updated
            if (_oldHeightMapSettings != heightMapSettings)
            {
                _oldHeightMapSettings = heightMapSettings;
                Debug.LogWarning("NoiseData updated. Preview won't work until the a map is manually generated using the MapGenerator's inspector.", this);
            }
            // If settings have been updated
            else if (_oldTextureSettings != textureSettings)
            {
                _oldTextureSettings = textureSettings;
                Debug.LogWarning("TextureData updated. Preview won't work until the a map is manually generated using the MapGenerator's inspector.", this);
            }
            else
                base.OnValidate();

        }
    
    #endif


    #region EqualityComparer
        
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// <para>The equality is considered checking only the name of the MapConfigurations.</para>
        /// </summary>
        /// <param name="obj">The object to check against</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((MapConfiguration) obj);
        }
        
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// <para>The equality is considered checking only the name of the MapConfigurations.</para>
        /// </summary>
        /// <param name="other">The object to check against</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(MapConfiguration other)
        {
            return other != null && other.name.Equals(this.name);
        }
        
        /// <summary>
        /// Returns the hash code for the object (given by its name).
        /// </summary>
        /// <returns>The hash code for the object.</returns>
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
        
        /// <summary>
        /// Override to the equal operator so two MapConfigurations are considered the same if their names are the same.
        /// <para>This is because the Equals method is used, and it uses the GetHasCode method to compare equality while it uses the name to obtain it. </para>
        /// </summary>
        /// <returns>True if the left object's name is equal to the right object's name; otherwise, false.</returns>
        public static bool operator ==(MapConfiguration left, MapConfiguration right)
        {
            if (left is null && right is null)
                return true;
            if ((left is null) && !(right is null))
                return false;
            if (!(left is null) && (right is null))
                return false;
            return left.Equals(right);
        }
        
        /// <summary>
        /// Override to the mot-equal operator so two MapConfigurations are considered different the same if their names are different.
        /// <para>This is because the Equals method is used, and it uses the GetHasCode method to compare equality while it uses the name to obtain it. </para>
        /// </summary>
        /// <returns>True if the left object's name is different to the right object's name; otherwise, false.</returns>
        public static bool operator !=(MapConfiguration left, MapConfiguration right)
        {
            return !(left == right);
        }
    #endregion
    

    }
}