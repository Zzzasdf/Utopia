using System;

public class BusyDisposable : IDisposable
{
    private static readonly bool collectionCheck = true; 
    private static readonly int defaultCapacity = 10;
    // 能够精确定位未回收的类型，有且只能通过内部池创建、回收
    private static readonly int maxSize = 10000;
    
    private static readonly MonitoredObjectPool.ObjectPool<BusyDisposable, BusyDisposable> s_Pool = 
        new(nameof(BusyDisposable), () => new BusyDisposable(), 
            null,
            b => b.callback = null,
            null,
            collectionCheck,
            defaultCapacity, maxSize);

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
    ~BusyDisposable()
    {
        UnityEngine.Debug.LogError($"pool item gc eg!! {GetType()} => 当前对象被销毁，代码中存在未回收该类型的地方");
    }
#endif
    
    private Action callback;
}
