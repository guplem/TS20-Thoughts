using System;
using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    public int widthResolution = 150;
    public int heightResolution = 150;
    public Vector2 offset;
    
    [Space]
    public float noiseScale = 27f;
    [Range(0,20)]
    public int octaves = 4;
    [Range(0,1)]
    public float persistance = 0.5f;
    public float lacunarity = 2f;
    /// <summary>
    /// For how much each cell height will be multiplied.
    /// <para>The height of the cell is by default [0,1], multiplying it by 5, the maximum height will be 5 (cell height [0,5]</para>
    /// </summary>
    public float maxHeight = 5f;
    /// <summary>
    /// How much the height of the mesh should be affected by the maxHeight (AKA: "height multiplier")
    /// </summary>
    public AnimationCurve heightCurve;

    [Space]
    public int seed;
    
    [Space]
    public bool autoRegenerate = false;

    [SerializeField] private TerrainGenerator terrainGenerator;

    public void GenerateMap()
    {
        terrainGenerator.GenerateTerrain(widthResolution, heightResolution, seed, noiseScale, octaves, persistance, lacunarity, offset, maxHeight, heightCurve);
    }

    private void OnValidate()
    {
        if (widthResolution < 1)
            widthResolution = 1;
        if (heightResolution < 1)
            heightResolution = 1;
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
    }

}
