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
    
    public void GenerateTerrain(int width, int height, float noiseScale)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, noiseScale);
        terrainDrawer.DrawMap(noiseMap);
        
    }
}
