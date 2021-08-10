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

    private Queue<ThreadInfo<HeightMap>> terrainDataThreadInfoQueue = new Queue<ThreadInfo<HeightMap>>();
    private Queue<ThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<ThreadInfo<MeshData>>();
    
    

    public void DrawTerrainInEditor(MapConfiguration mapConfiguration)
    {
        mapConfiguration.terrainData.textureData.UpdateMeshHeights(mapConfiguration.terrainData.heightMapSettings.minHeight, mapConfiguration.terrainData.heightMapSettings.maxHeight);
        
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap( mapConfiguration.chunkSize, mapConfiguration.chunkSize, mapConfiguration.terrainData.heightMapSettings, Vector2.zero);
        
        terrainDrawer.DrawMesh(heightMap.values, mapConfiguration, mapConfiguration.editorPreviewLOD, mapConfiguration.scale);
        //terrainDrawer.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, size, size));
    }


    private void SetupTerrainMaterial(MapConfiguration mapConfiguration)
    {
        mapConfiguration.terrainData.textureData.ApplyToMaterial();
        mapConfiguration.terrainData.textureData.UpdateMeshHeights(mapConfiguration.terrainData.heightMapSettings.minHeight, mapConfiguration.terrainData.heightMapSettings.maxHeight);
    }
    
    public void RequestTerrainData(Vector2 centre, Action<HeightMap> callback, MapConfiguration mapConfiguration)
    {
         // Maybe not the best place to put it. Awake would so the trick as well, but this seemed more "controlled"
        
        ThreadStart threadStart = delegate
        {
            TerrainDataThread(centre, callback, mapConfiguration);
        };
        
        new Thread(threadStart).Start();
    }
    private void TerrainDataThread(Vector2 centre, Action<HeightMap> callback, MapConfiguration mapConfiguration)
    {
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap( mapConfiguration.chunkSize, mapConfiguration.chunkSize, mapConfiguration.terrainData.heightMapSettings, centre);
        lock (terrainDataThreadInfoQueue)
        {
            terrainDataThreadInfoQueue.Enqueue(new ThreadInfo<HeightMap>(callback, heightMap));
        }
    }
    
    public void RequestMeshData(HeightMap heightMap, MapConfiguration mapConfiguration, int LOD, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            HeightMapThread(heightMap, mapConfiguration, LOD, callback);
        };
        
        new Thread(threadStart).Start();
    }
    
    public void HeightMapThread(HeightMap heightMap, MapConfiguration mapConfiguration, int LOD, Action<MeshData> callback)
    {
        MeshData meshData = MapDisplay.GenerateTerrainMesh(heightMap.values, mapConfiguration, LOD);
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
                ThreadInfo<HeightMap> threadInfo = terrainDataThreadInfoQueue.Dequeue();
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