using System;

struct ThreadInfo<T>
{
    public readonly Action<T> callback;
    public readonly T parameter;

    public ThreadInfo(Action<T> callback, T parameter)
    {
        this.callback = callback;
        this.parameter = parameter;
    }
}