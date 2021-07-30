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

    [Space]
    public int seed;
    
    [Space]
    public bool autoRegenerate = false;

    [SerializeField] private TerrainGenerator terrainGenerator;

    public void GenerateMap()
    {
        terrainGenerator.GenerateTerrain(widthResolution, heightResolution, seed, noiseScale, octaves, persistance, lacunarity, offset);
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
