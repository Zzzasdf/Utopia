using System;
using System.Collections.Generic;
using UnityEngine.Pool;

/// 可监控的对象池
public class MonitoredObjectPool
{
#if !POOL_RELEASES
    private static readonly Dictionary<string, Dictionary<Type, HashSet<IMonitoredPool>>> _Pools;
    public static readonly Dictionary<string, Dictionary<Type, HashSet<IMonitoredPool>>> Pools = _Pools ??= new Dictionary<string, Dictionary<Type, HashSet<IMonitoredPool>>>();
#endif
    public sealed class ObjectPool<TItem, TMonitoredKey>: IMonitoredPool
        where TItem : class 
    {
        private ObjectPool<TItem> s_Pool;
        
        public ObjectPool(string groupName, Func<TItem> createFunc, Action<TItem> actionOnGet = null, Action<TItem> actionOnRelease = null, Action<TItem> actionOnDestroy = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
        {
            var onGet = OnGet(actionOnGet, maxSize);
            var onRelease = OnRelease(actionOnRelease);
#if POOL_PERFORNANCE
            collectionCheck = false;
#endif
            s_Pool = new ObjectPool<TItem>(createFunc, onGet, onRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize);
#if !POOL_RELEASES
            if (!Pools.TryGetValue(groupName, out Dictionary<Type, HashSet<IMonitoredPool>> dict))
            {
                Pools.Add(groupName, dict = new Dictionary<Type, HashSet<IMonitoredPool>>());
            }
            if (!dict.TryGetValue(typeof(TMonitoredKey), out HashSet<IMonitoredPool> pools))
            {
                dict.Add(typeof(TMonitoredKey), pools = new HashSet<IMonitoredPool>());
            }
            pools.Add(this);
 #endif
        }
        
        private Action<TItem> OnGet(Action<TItem> actionOnGet, int maxSize)
        {
#if !POOL_RELEASES
            return tItem =>
            {
                actionOnGet?.Invoke(tItem);
                GC.ReRegisterForFinalize(tItem);
                if (s_Pool.CountAll <= maxSize) return;
                UnityEngine.Debug.LogError($"pool maxsize eg!! {tItem.GetType()} => 当前 active:{s_Pool.CountActive} + inactive:{s_Pool.CountInactive} > maxSize:{maxSize}, " +
                    $"当所有 active 对象回收时，将存在销毁，若前无集合重复回收报错，则请将最大容量至少提高到 {s_Pool.CountAll}");
            };
#else
            return actionOnGet;
#endif
        }
        
        private Action<TItem> OnRelease(Action<TItem> actionOnRelease)
        {
#if !POOL_RELEASES
            return tItem =>
            {
                actionOnRelease?.Invoke(tItem);
                GC.SuppressFinalize(tItem);
            };
#else
            return actionOnRelease;
#endif
        }
        
        public PooledObject<TItem> Get(out TItem value) => s_Pool.Get(out value);
        public TItem Get() => s_Pool.Get();
        public void Release(TItem toRelease) => s_Pool.Release(toRelease);
        
        int IMonitoredPool.CountAll => s_Pool.CountAll;
        int IMonitoredPool.CountActive => s_Pool.CountActive;
        int IMonitoredPool.CountInactive => s_Pool.CountInactive;
    }
    
    public interface IMonitoredPool 
    {
        int CountAll { get; }
        int CountActive { get; }
        int CountInactive { get; }
    }
}