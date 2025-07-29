using System;

public class BusyDisposable : IDisposable
{
    private static readonly bool collectionCheck = true; 
    private static readonly int defaultCapacity = 10;
    // 能够精确定位未回收的类型，有且只能通过内部池创建、回收
    private static readonly int maxSize = 10000;
    
#if UNITY_EDITOR
    private static readonly MonitoredObjectPool.ObjectPool<BusyDisposable, BusyDisposable> s_Pool = 
        new(nameof(BusyDisposable), () => new BusyDisposable(), 
            OnGet,
            OnRelease,
            null,
            collectionCheck,
            defaultCapacity, maxSize);
    private static void OnGet(BusyDisposable busyDisposable)
    {
        GC.ReRegisterForFinalize(busyDisposable);
        if (s_Pool.CountAll <= maxSize) return;
        UnityEngine.Debug.LogError($"pool maxsize eg!! {busyDisposable.GetType()} => 当前 active:{s_Pool.CountActive} + inactive:{s_Pool.CountInactive} > maxSize:{maxSize}, " +
            $"当所有 active 对象回收时，将存在销毁，若前无集合重复回收报错，则请将最大容量至少提高到 {s_Pool.CountAll}");
    }
    private static void OnRelease(BusyDisposable busyDisposable)
    {
        busyDisposable.callback = null;
        GC.SuppressFinalize(busyDisposable);
    }
#else
    private static readonly MonitoredObjectPool.ObjectPool<BusyDisposable, BusyDisposable> s_Pool = 
        new(nameof(BusyDisposable), () => new BusyDisposable(), 
            null,
            b => b.callback = null,
            null,
            collectionCheck,
            defaultCapacity, maxSize);
#endif

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

#if UNITY_EDITOR
    ~BusyDisposable()
    {
        UnityEngine.Debug.LogError($"pool item gc eg!! {GetType()} => 当前对象被销毁，代码中存在未回收该类型的地方");
    }
#endif
    
    private Action callback;
}
