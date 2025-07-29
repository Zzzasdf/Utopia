using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager: ITimerManager
{
    private long CurrentTime => (long)(Time.time *1000); // TODO ZZZ 当前时间
    
    // 自增 id
    private int incrementId;
    private TimerSpanHeap spanHeap;
    private Dictionary<int, TimerSpan> dict;

    void IInit.OnInit()
    {
        spanHeap = new TimerSpanHeap();
        dict = new Dictionary<int, TimerSpan>();
    }
    void IReset.OnReset()
    {
        dict.Clear();
        spanHeap.Clear();
    }
    void IDestroy.OnDestroy()
    {
        
    }

    void IUpdate.OnUpdate(float deltaTime)
    {
        while (!spanHeap.IsEmpty)
        {
            TimerSpan timerSpan = spanHeap.Peek();
            if (CurrentTime < timerSpan.Time) return;
            spanHeap.Pop();
            dict.Remove(timerSpan.Id);
            timerSpan.Callback.Invoke(true);
            ((IDisposable)timerSpan).Dispose();
        }
    }
    
    /// <summary>
    /// 创建定时器
    /// </summary>
    /// <param name="millisecond">当前时间经过毫秒数后执行</param>
    /// <param name="callback">触发的回调 (true => 成功触发，false => 取消)</param>
    /// <returns>唯一id</returns>
    public int GetAfterMillisecond(long millisecond, Action<bool> callback)
    {
        long time = CurrentTime + millisecond;
        return Get(time, callback);
    }
    /// <summary>
    /// 创建定时器
    /// </summary>
    /// <param name="time">触发的时点</param>
    /// <param name="callback">触发的回调 (true => 成功触发，false => 取消)</param>
    /// <returns>唯一id</returns>
    public int Get(long time, Action<bool> callback)
    {
        int id = CreateUniqueId();
        TimerSpan timerSpan = TimerSpan.Get(id, time, callback);
        spanHeap.Push(timerSpan);
        dict.Add(id, timerSpan);
        return id;
    }

    /// <summary>
    /// 取消定时器
    /// </summary>
    /// <param name="id"></param>
    public void Cancel(int id)
    {
        if (!dict.Remove(id, out TimerSpan timerSpan)) return;
        spanHeap.Remove(timerSpan);
        timerSpan.Callback.Invoke(false);
        ((IDisposable)timerSpan).Dispose();
    }

    /// 生成唯一 id
    private int CreateUniqueId()
    {
        do
        {
            incrementId++;
        } while (!dict.ContainsKey(incrementId));
        return incrementId;
    }

    private class TimerSpanHeap : Heap<TimerSpan>
    {
        public void Remove(TimerSpan timerSpan)
        {
            heap.Remove(timerSpan);
        }
    }
    
    private class TimerSpan: IDisposable, IComparable<TimerSpan>
    {
        private static readonly bool collectionCheck = true; 
        private static readonly int defaultCapacity = 10;
        // 能够精确定位未回收的类型，有且只能通过内部池创建、回收
        private static readonly int maxSize = 10000;
        
#if UNITY_EDITOR
        private static readonly MonitoredObjectPool.ObjectPool<TimerSpan, TimerSpan> s_Pool = 
            new(nameof(TimerSpan), () => new TimerSpan(), 
                OnGet,
                OnRelease,
                null,
                collectionCheck,
                defaultCapacity, maxSize);
        private static void OnGet(TimerSpan timerSpan)
        {
            GC.ReRegisterForFinalize(timerSpan);
            if (s_Pool.CountAll <= maxSize) return;
            UnityEngine.Debug.LogError($"pool maxsize eg!! {timerSpan.GetType()} => 当前 active:{s_Pool.CountActive} + inactive:{s_Pool.CountInactive} > maxSize:{maxSize}, " +
                $"当所有 active 对象回收时，将存在销毁，若前无集合重复回收报错，则请将最大容量至少提高到 {s_Pool.CountAll}");
        }
        private static void OnRelease(TimerSpan timerSpan)
        {
            timerSpan.Callback = null;
            timerSpan.Time = 0;
            timerSpan.Id = 0;
            GC.SuppressFinalize(timerSpan);
        }
#else
    private static readonly MonitoredObjectPool.ObjectPool<TimerSpan, TimerSpan> s_Pool = 
        new(nameof(TimerSpan), () => new TimerSpan(), 
            null,
            t =>
            {
                t.Callback = null;
                t.Time = 0;
                t.Id = 0;
            },
            null,
            collectionCheck,
            defaultCapacity, maxSize);
#endif

        public static TimerSpan Get(int id, long time, Action<bool> callback)
        {
            TimerSpan timerSpan = s_Pool.Get();
            timerSpan.Id = id;
            timerSpan.Time = time;
            timerSpan.Callback = callback ?? throw new ArgumentNullException();
            return timerSpan;
        }
        private TimerSpan() { }

        void IDisposable.Dispose()
        {
            s_Pool.Release(this);
        }

#if UNITY_EDITOR
        ~TimerSpan()
        {
            UnityEngine.Debug.LogError($"pool item gc eg!! {GetType()} => 当前对象被销毁，代码中存在未回收该类型的地方");
        }
#endif
        
        public int Id { get; private set; }
        public long Time { get; private set; }
        public Action<bool> Callback { get; private set; }

        public int CompareTo(TimerSpan other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Time.CompareTo(other.Time);
        }
    }
}
