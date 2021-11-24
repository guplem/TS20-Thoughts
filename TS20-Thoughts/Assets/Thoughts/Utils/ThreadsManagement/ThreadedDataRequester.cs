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
    
        /// <summary>
        /// Queue containing data obtained in threads waiting to be shared with the callbacks that were given during the request
        /// </summary>
        private Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

        /// <summary>
        /// Requests to a thread to execute the given method to obtain desired data and, once the logic is completed, call the callback method to share the data with it
        /// </summary>
        /// <param name="generateDataMethod">The method containing the logic to generate the data in a thread</param>
        /// <param name="callback">The method to call once the computation has been completed</param>
        public void RequestData(Func<object> generateDataMethod, Action<object> callback)
        {
            ThreadStart threadStart = delegate
            {
                DataThread(generateDataMethod, callback);
            };
            new Thread(threadStart).Start();
        }
    
        private void DataThread(Func<object> generateDataMethod, Action<object> callback)
        {
            object data = generateDataMethod();
            
            lock (dataQueue)
            {
                dataQueue.Enqueue(new ThreadInfo(callback, data));
            }
        }

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

        }
    }
}
