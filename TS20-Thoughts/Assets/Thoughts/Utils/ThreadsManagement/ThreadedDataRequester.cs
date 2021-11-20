using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Thoughts.Utils.ThreadsManagement
{
    /// <summary>
    /// Component in charge doing threaded requests of data
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
    
        /// <summary>
        /// Queue containing data obtained in threads waiting to be shared with the callbacks that were given during the request
        /// </summary>
        private Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

        /// <summary>
        /// Requests to a thread to execute the given method to obtain desired data and, once the logic is completed, call the callback method to share the data with it
        /// </summary>
        /// <param name="generateDataMethod">The method containing the logic to generate the data in a thread</param>
        /// <param name="callback">The method to call once the computation has been completed</param>
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
            //HeightMap heightMapAbsolute = HeightMapGenerator.GenerateHeightMap( mapConfiguration.chunkSize, mapConfiguration.chunkSize, mapConfiguration.terrainData.heightMapSettings, centre);
            lock (dataQueue)
            {
                dataQueue.Enqueue(new ThreadInfo(callback, data));
            }
        }
    
        /*public void RequestMeshData(HeightMap heightMapAbsolute, MapConfiguration mapConfiguration, int LOD, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            HeightMapThread(heightMapAbsolute, mapConfiguration, LOD, callback);
        };
        
        new Thread(threadStart).Start();
    }*/
    
        /*public void HeightMapThread(HeightMap heightMapAbsolute, MapConfiguration mapConfiguration, int LOD, Action<MeshData> callback)
    {
        MeshData meshData = MapDisplay.GenerateTerrainMesh(heightMapAbsolute.values, mapConfiguration, LOD);
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
                    threadInfo.callback(threadInfo.data);
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
}
