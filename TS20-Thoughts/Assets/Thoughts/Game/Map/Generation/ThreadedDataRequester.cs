using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// component in charge doing threaded requests of data
/// </summary>
[ExecuteAlways]
public class ThreadedDataRequester : MonoBehaviour
{
    /*private static ThreadedDataRequester instance { get => _instance; set => _instance = value; }
    private static ThreadedDataRequester _instance;*/
    
    /*private void Awake()
    {
        if (instance != null)
            Debug.LogWarning($"More than 1 ThreadedDataRequester objects exist: '{instance.ToString()}' and '{this.ToString()}'", this);
        else
            instance = this;
    }*/
    private Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

    public void RequestData(Func<object> generateDataMethod, Action<object> callback/*, MapConfiguration mapConfiguration*/)
    {
        /*if (instance == null)
            Debug.LogWarning($"The static instance (singleton) for {nameof(ThreadedDataRequester)} is null.");*/
        
        ThreadStart threadStart = delegate
        {
            DataThread(generateDataMethod, callback/*, mapConfiguration*/);
        };
        new Thread(threadStart).Start();
    }
    
    private void DataThread(Func<object> generateDataMethod, Action<object> callback/*, MapConfiguration mapConfiguration*/)
    {
        object data = generateDataMethod();
        //HeightMap heightMap = HeightMapGenerator.GenerateHeightMap( mapConfiguration.chunkSize, mapConfiguration.chunkSize, mapConfiguration.terrainData.heightMapSettings, centre);
        lock (dataQueue)
        {
            dataQueue.Enqueue(new ThreadInfo(callback, data));
        }
    }
    
    /*public void RequestMeshData(HeightMap heightMap, MapConfiguration mapConfiguration, int LOD, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            HeightMapThread(heightMap, mapConfiguration, LOD, callback);
        };
        
        new Thread(threadStart).Start();
    }*/
    
    /*public void HeightMapThread(HeightMap heightMap, MapConfiguration mapConfiguration, int LOD, Action<MeshData> callback)
    {
        MeshData meshData = MapDisplay.GenerateTerrainMesh(heightMap.values, mapConfiguration, LOD);
        lock (terrainDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new ThreadInfo<MeshData>(callback, meshData));
        }
    }*/
    
    private void Update()
    {
        if (dataQueue.Count > 0)
        {
            for (int i = 0; i < dataQueue.Count; i++)
            {
                ThreadInfo threadInfo = dataQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        /*if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                ThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }*/
    }
}
