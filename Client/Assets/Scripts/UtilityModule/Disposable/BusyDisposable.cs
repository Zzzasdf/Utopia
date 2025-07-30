using System;

public class BusyDisposable : IDisposable
{
    private static readonly MonitoredObjectPool.ObjectPool<BusyDisposable, BusyDisposable> s_Pool = 
        new(nameof(BusyDisposable), () => new BusyDisposable(), 
            null,
            b => b.callback = null);

    public static BusyDisposable Get(Action callback)
    {
        BusyDisposable busyDisposable = s_Pool.Get();
        busyDisposable.callback = callback ?? throw new ArgumentNullException();
        return busyDisposable;
    }
    private BusyDisposable() { }

    void IDisposable.Dispose()
    {
        callback.Invoke();
        s_Pool.Release(this);
    }

#if !POOL_RELEASES
    ~BusyDisposable() => s_Pool.FinalizeDebug();
#endif
    
    private Action callback;
}
