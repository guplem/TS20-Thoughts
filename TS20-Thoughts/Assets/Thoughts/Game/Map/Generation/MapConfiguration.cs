using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MapConfiguration", menuName = "Thoughts/Map Configuration", order = 1)]
public class MapConfiguration : UpdatableData, IEquatable<MapConfiguration>
{
    
    [Range(0,6)]
    [Tooltip("Max level of detail (LOD) for the terrain is 0 (editorPreviewLOD = 0)")]
    public int editorPreviewLOD = 0; //Not the one used during the dynamic optimization/terrain generation
    
    [Space]
    public int seed;
    
    public const int chunkSize = 241; // This is the max size for unity. It will generate a mesh of dimensions of chunkSize-1 = 240
    public const int chunkSizeWithoutBorder = chunkSize-2; // This +2 (used by the border) is the max size for unity. It will generate a mesh of dimensions of chunkSize-1 (+2) = 240
    
    [Space]
    [SerializeField] public bool useFalloff;
    [NonSerialized] public float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(chunkSize);
    public float terrainScale = 0.5f; //ToDo: Do it so the noise scale and max height of the terrain dynamically adapts to this value so it becomes a "terrain resolution" slider. Small scale = more triangles

    [SerializeField] public TerrainData terrainData;
    private TerrainData _oldTerrainData;

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