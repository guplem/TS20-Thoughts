using System;
using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
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
        terrainGenerator.GenerateTerrain(width, height, seed, noiseScale, octaves, persistance, lacunarity, offset);
    }

    private void OnValidate()
    {
        if (width < 1)
            width = 1;
        if (height < 1)
            height = 1;
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
    }

}
