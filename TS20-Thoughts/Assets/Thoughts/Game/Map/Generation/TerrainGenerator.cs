using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the generation of the terrain of the map
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
    /// <summary>
    /// The visualization manager for the terrain.
    /// </summary>
    [SerializeField] private MapDisplay terrainDrawer;

    public TerrainType[] regions;

    public void GenerateTerrain(int width, int height, int seed, float noiseScale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].maxHeight)
                    {
                        colourMap[y * width + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        
        terrainDrawer.DrawTexture(TextureGenerator.TextureFromColorMap(colourMap, width, height));
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    [Range(0,1)]
    public float maxHeight;
    public Color color;
}
