using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Handles the generation of the terrain of the map
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
    /// <summary>
    /// The visualization manager for the terrain.
    /// </summary>
    [SerializeField] private TerrainMeshGenerator terrainDrawer;
    
    [SerializeField] public EndlessTerrain endlessTerrain;
    
    /*public void DrawTerrainInEditor(MapConfiguration mapConfiguration)
    {
        SetupTerrainMaterial(mapConfiguration);
        
        mapConfiguration.terrainData.textureSettings.UpdateMeshHeights(mapConfiguration.terrainData.heightMapSettings.minHeight, mapConfiguration.terrainData.heightMapSettings.maxHeight);
        
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap( mapConfiguration.chunkSize, mapConfiguration.chunkSize, mapConfiguration.terrainData.heightMapSettings, Vector2.zero);
        
        terrainDrawer.DrawMesh(heightMap.values, mapConfiguration, mapConfiguration.editorPreviewLOD);
    }
    
    private void SetupTerrainMaterial(MapConfiguration mapConfiguration)
    {
        mapConfiguration.terrainData.textureSettings.ApplyToMaterial();
        mapConfiguration.terrainData.textureSettings.UpdateMeshHeights(mapConfiguration.terrainData.heightMapSettings.minHeight, mapConfiguration.terrainData.heightMapSettings.maxHeight);
    }
    */
    
    
    /*public HeightMap GenerateTerrainData(Vector2 center, MapConfiguration mapConfiguration)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapConfiguration.chunkSize, mapConfiguration.seed, mapConfiguration.terrainData.heightMapSettings.noiseScale, mapConfiguration.terrainData.heightMapSettings.octaves, mapConfiguration.terrainData.heightMapSettings.persistance, mapConfiguration.terrainData.heightMapSettings.lacunarity, center+mapConfiguration.terrainData.heightMapSettings.offset, Noise.NormalizeMode.Global);

        if (mapConfiguration.useFalloff)
        {
            for (int y = 0; y < mapConfiguration.chunkSize; y++)
            {
                for (int x = 0; x < mapConfiguration.chunkSize; x++)
                {
                    if (mapConfiguration.useFalloff)
                    {
                        if (mapConfiguration.falloffMap == null)
                        {
                            mapConfiguration.falloffMap = FalloffGenerator.GenerateFalloffMap(mapConfiguration.chunkSize);
                        }
                        noiseMap[x, y] = Mathf.Clamp(noiseMap[x, y] - mapConfiguration.falloffMap[x, y], 0, float.MaxValue) ;
                    }
                }
            }
        }

        return new HeightMap(noiseMap);
    }*/
    
}