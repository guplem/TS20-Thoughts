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

    private Queue<ThreadInfo<MapData>> terrainDataThreadInfoQueue = new Queue<ThreadInfo<MapData>>();
    private Queue<ThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<ThreadInfo<MeshData>>();
    
    

    public void DrawTerrainInEditor(MapConfiguration mapConfiguration)
    {
        mapConfiguration.terrainData.textureData.UpdateMeshHeights(mapConfiguration.minHeight, mapConfiguration.maxHeight);
        
        MapData mapData = GenerateTerrainData( Vector2.zero, mapConfiguration);
        
        terrainDrawer.DrawMesh(mapData.heightMap, mapConfiguration.terrainData.maxHeight, mapConfiguration.terrainData.heightCurve, mapConfiguration.editorPreviewLOD, mapConfiguration.terrainScale);
        //terrainDrawer.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, size, size));
    }


    private void SetupTerrainMaterial(MapConfiguration mapConfiguration)
    {
        mapConfiguration.terrainData.textureData.ApplyToMaterial();
        mapConfiguration.terrainData.textureData.UpdateMeshHeights(mapConfiguration.minHeight, mapConfiguration.maxHeight);
    }
    
    public void RequestTerrainData(Vector2 centre, Action<MapData> callback, MapConfiguration mapConfiguration)
    {
         // Maybe not the best place to put it. Awake would so the trick as well, but this seemed more "controlled"
        
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
        MeshData meshData = MapDisplay.GenerateTerrainMesh(mapData.heightMap, mapConfiguration.terrainData.maxHeight, mapConfiguration.terrainData.heightCurve, LOD);
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
        float[,] noiseMap = Noise.GenerateNoiseMap(MapConfiguration.chunkSize, mapConfiguration.seed, mapConfiguration.terrainData.noiseData.noiseScale, mapConfiguration.terrainData.noiseData.octaves, mapConfiguration.terrainData.noiseData.persistance, mapConfiguration.terrainData.noiseData.lacunarity, center+mapConfiguration.terrainData.noiseData.offset, Noise.NormalizeMode.Global);

        if (mapConfiguration.useFalloff)
        {
            for (int y = 0; y < MapConfiguration.chunkSize; y++)
            {
                for (int x = 0; x < MapConfiguration.chunkSize; x++)
                {
                    if (mapConfiguration.useFalloff)
                    {
                        noiseMap[x, y] = Mathf.Clamp(noiseMap[x, y] - mapConfiguration.falloffMap[x, y], 0, float.MaxValue) ;
                    }
                }
            }
        }

        return new MapData(noiseMap);
    }
    
}

public struct MapData
{
    public readonly float[,] heightMap;
    
    public MapData(float[,] heightMap)
    {
        this.heightMap = heightMap;
    }
}