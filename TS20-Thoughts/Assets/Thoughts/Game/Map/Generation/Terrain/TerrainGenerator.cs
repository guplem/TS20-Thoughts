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
    private Queue<ThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<ThreadInfo<MeshData>>();

    public void DrawTerrainInEditor(MapConfiguration mapConfiguration, float scale)
    {
        MapData mapData = GenerateTerrainData( Vector2.zero, mapConfiguration);
        
        terrainDrawer.DrawMesh(mapData.heightMap, mapConfiguration.maxHeight, mapConfiguration.heightCurve, TextureGenerator.TextureFromColorMap(mapData.colorMap, MapConfiguration.chunkSize, MapConfiguration.chunkSize), mapConfiguration.editorPreviewLOD, scale);
        //terrainDrawer.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, size, size));
    }

    
    
    
    public void RequestTerrainData(Vector2 centre, Action<MapData> callback, MapConfiguration mapConfiguration)
    {
        ThreadStart threadStart = delegate
        {
            TerrainDataThread(centre, callback, mapConfiguration);
        };
        
        new Thread(threadStart).Start();
    }
    private void TerrainDataThread(Vector2 centre, Action<MapData> callback, MapConfiguration mapConfiguration)
    {
        MapData mapData = GenerateTerrainData(centre, mapConfiguration);
        lock (terrainDataThreadInfoQueue)
        {
            terrainDataThreadInfoQueue.Enqueue(new ThreadInfo<MapData>(callback, mapData));
        }
    }




    public void RequestMeshData(MapData mapData, MapConfiguration mapConfiguration, int LOD, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, mapConfiguration, LOD, callback);
        };
        
        new Thread(threadStart).Start();
    }
    public void MeshDataThread(MapData mapData, MapConfiguration mapConfiguration, int LOD, Action<MeshData> callback)
    {
        MeshData meshData = MapDisplay.GenerateTerrainMesh(mapData.heightMap, mapConfiguration.maxHeight, mapConfiguration.heightCurve, LOD);
        lock (terrainDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new ThreadInfo<MeshData>(callback, meshData));
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
        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                ThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public MapData GenerateTerrainData(Vector2 center, MapConfiguration mapConfiguration)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(MapConfiguration.chunkSize + 2, mapConfiguration.seed, mapConfiguration.noiseScale, mapConfiguration.octaves, mapConfiguration.persistance, mapConfiguration.lacunarity, center+mapConfiguration.offset, Noise.NormalizeMode.Global);

        Color[] colourMap = new Color[MapConfiguration.chunkSize * MapConfiguration.chunkSize];
        for (int y = 0; y < MapConfiguration.chunkSize; y++)
        {
            for (int x = 0; x < MapConfiguration.chunkSize; x++)
            {
                if (mapConfiguration.useFalloff)
                {
                    noiseMap[x, y] = Mathf.Clamp(noiseMap[x, y] - mapConfiguration.falloffMap[x, y], 0, float.MaxValue) ;
                }
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight >= regions[i].maxHeight)
                    {
                        colourMap[y * MapConfiguration.chunkSize + x] = regions[i].color;
                    }
                    else
                    {
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