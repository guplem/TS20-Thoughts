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

    public void GenerateTerrain(int widthResolution, int levelOfDetail, int seed, float noiseScale, int octaves, float persistance, float lacunarity, Vector2 offset, float maxHeight, AnimationCurve heightCurve)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(widthResolution, levelOfDetail, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[widthResolution * widthResolution];
        for (int y = 0; y < widthResolution; y++)
        {
            for (int x = 0; x < widthResolution; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].maxHeight)
                    {
                        colourMap[y * widthResolution + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        
        //terrainDrawer.DrawTexture(TextureGenerator.TextureFromColorMap(colourMap, widthResolution, heightResolution));
        terrainDrawer.DrawMesh(noiseMap, maxHeight, heightCurve, TextureGenerator.TextureFromColorMap(colourMap, widthResolution, widthResolution), levelOfDetail);
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
