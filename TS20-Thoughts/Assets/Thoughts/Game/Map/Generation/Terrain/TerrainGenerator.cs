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
    [SerializeField] private MapDisplay terrainDrawer;

    public TerrainType[] regions;

    private Queue<ThreadInfo<MapData>> terrainDataThreadInfoQueue = new Queue<ThreadInfo<MapData>>();

    public void DrawTerrainInEditor(MapConfiguration mapConfiguration)
    {
        MapData mapData = GenerateTerrainData( mapConfiguration);
        
        terrainDrawer.DrawMesh(mapData.heightMap, mapConfiguration.maxHeight, mapConfiguration.heightCurve, TextureGenerator.TextureFromColorMap(mapData.colorMap, MapConfiguration.chunkSize, MapConfiguration.chunkSize), mapConfiguration.levelOfDetail);
        //terrainDrawer.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, size, size));
    }

    public void RequestTerrainData(Action<MapData> callback, MapConfiguration mapConfiguration)
    {
        ThreadStart threadStart = delegate
        {
            RequestTerrainDataThread(callback, mapConfiguration);
        };
        
        new Thread(threadStart).Start();
    }
    private void RequestTerrainDataThread(Action<MapData> callback, MapConfiguration mapConfiguration)
    {
        MapData mapData = GenerateTerrainData(mapConfiguration);
        lock (terrainDataThreadInfoQueue)
        {
            terrainDataThreadInfoQueue.Enqueue(new ThreadInfo<MapData>(callback, mapData));
        }
    }

    private void Update()
    {
        if (terrainDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < terrainDataThreadInfoQueue.Count; i++)
            {
                ThreadInfo<MapData> threadInfo = terrainDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public MapData GenerateTerrainData(MapConfiguration mapConfiguration)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(MapConfiguration.chunkSize, mapConfiguration.seed, mapConfiguration.noiseScale, mapConfiguration.octaves, mapConfiguration.persistance, mapConfiguration.lacunarity, mapConfiguration.offset);

        Color[] colourMap = new Color[MapConfiguration.chunkSize * MapConfiguration.chunkSize];
        for (int y = 0; y < MapConfiguration.chunkSize; y++)
        {
            for (int x = 0; x < MapConfiguration.chunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].maxHeight)
                    {
                        colourMap[y * MapConfiguration.chunkSize + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        
        
        return new MapData(noiseMap, colourMap);
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

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;
    
    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}