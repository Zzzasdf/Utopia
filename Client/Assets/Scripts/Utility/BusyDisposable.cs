using System;

public class BusyDisposable : IDisposable
{
    private readonly Action callback;

    public BusyDisposable(Action callback)
    {
        this.callback = callback ?? throw new ArgumentNullException();
    }

    public void Dispose()
    {
        callback.Invoke();
    }
}
