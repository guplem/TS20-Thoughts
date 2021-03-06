using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MapConfiguration", menuName = "Thoughts/Map/Map Configuration", order = 20)]
public class MapConfiguration : UpdatableData, IEquatable<MapConfiguration>
{

    [Space]
    public int seed = 420;

    [Range(0, numSupportedChunkSizes-1)]
    public int chunkSizeIndex = 9;
    public static readonly int[] supportedChunkSizes = {24, 48, 72, 96, 120, 144, 168, 192, 216, 240};
    public const int numSupportedChunkSizes = 10; // supportedChunkSizes.Length
    
    /// <summary>
    /// Number of vertices per line of a mesh rendered at the max resolution (LOD = 0). It includes the 2 extra vertices that are excluded from final mesh, but used for calculating normals.
    /// numVertsPerLine 
    /// </summary>
    public int chunkSize => supportedChunkSizes[chunkSizeIndex] + 5; // This is the max size for unity. It will generate a mesh of dimensions of chunkSize-1
    //public int chunkSize => supportedChunkSizes[chunkSizeIndex] + 1; // Changed in the last episode to +5 but seemed weird. //Todo: delete comment after testing that all works
    public int numVertsPerLine => chunkSize; //todo: delete
    /// <summary>
    /// The space the (terrain) mesh takes up in the world.
    /// </summary>
    public float meshWorldSize => (chunkSize - 3) * scale;

    public float mapRadius = 500;
    public float mapPreviewRadius = 500;

    //public int chunkSizeWithoutBorder => chunkSize -2; // This +2 (used by the border) is the max size for unity. It will generate a mesh of dimensions of chunkSize-1(+2)
    
    public float scale = 1f; //ToDo: Do it so the noise scale and max height of the terrain dynamically adapts to this value so it becomes a "terrain resolution" slider. Small scale = more triangles

    [SerializeField] public TerrainData terrainData;
    private TerrainData _oldTerrainData;

    #if UNITY_EDITOR
    
    protected override void OnValidate()
    {
        if (terrainData != _oldTerrainData)
        {
            _oldTerrainData = terrainData;
            Debug.LogWarning("TerrainData updated. Preview won't work until the a map is manually generated using the MapGenerator's inspector.");
        }
        else
        {
            base.OnValidate();
        } 
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