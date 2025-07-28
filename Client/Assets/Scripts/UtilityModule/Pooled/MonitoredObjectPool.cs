using System;
using System.Collections.Generic;

/// 可监控的对象池
public class MonitoredObjectPool
{
#if UNITY_EDITOR
    private static readonly Dictionary<string, Dictionary<Type, HashSet<IMonitoredPool>>> _Pools;
    public static readonly Dictionary<string, Dictionary<Type, HashSet<IMonitoredPool>>> Pools = _Pools ??= new Dictionary<string, Dictionary<Type, HashSet<IMonitoredPool>>>();
#endif
    public sealed class ObjectPool<TItem, TMonitoredKey>: UnityEngine.Pool.ObjectPool<TItem>, IMonitoredPool
        where TItem : class
    {
        public ObjectPool(string groupName, Func<TItem> createFunc, Action<TItem> actionOnGet = null, Action<TItem> actionOnRelease = null, Action<TItem> actionOnDestroy = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000) 
            : base(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize)
        {
#if UNITY_EDITOR
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
        int IMonitoredPool.CountAll => CountAll;
        int IMonitoredPool.CountActive => CountActive;
        int IMonitoredPool.CountInactive => CountInactive;
    }
    
    public interface IMonitoredPool 
    {
        int CountAll { get; }
        int CountActive { get; }
        int CountInactive { get; }
    }
}