using System;

/// <summary>
/// Struct containing data obtained in threads that should be shared with the stored callback
/// </summary>
struct ThreadInfo
{
    /// <summary>
    /// The method to share the stored data with
    /// </summary>
    public readonly Action<object> callback;
    
    /// <summary>
    /// Stored data obtained from the execution of a threaded request
    /// </summary>
    public readonly object data;

    /// <summary>
    /// Constructor of a ThreadInfo
    /// </summary>
    /// <param name="callback">The method to share the stored data with</param>
    /// <param name="data">The stored data to share with the callback method</param>
    public ThreadInfo(Action<object> callback, object data)
    {
        this.callback = callback;
        this.data = data;
    }
}