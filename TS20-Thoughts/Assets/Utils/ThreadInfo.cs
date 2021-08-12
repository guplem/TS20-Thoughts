using System;

struct ThreadInfo
{
    public readonly Action<object> callback;
    public readonly object parameter;

    public ThreadInfo(Action<object> callback, object parameter)
    {
        this.callback = callback;
        this.parameter = parameter;
    }
}