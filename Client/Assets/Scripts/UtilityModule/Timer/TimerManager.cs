using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager: ITimerManager
{
    private long CurrentTime => (long)(Time.time * 1_000); // TODO ZZZ 当前时间
    
    // 自增 id
    private int incrementId;
    private Heap<TimerSpan> spanHeap;
    private Dictionary<int, TimerSpan> dict;

    void IInit.OnInit()
    {
        spanHeap ??= new Heap<TimerSpan>();
        dict ??= new Dictionary<int, TimerSpan>();
    }
    void IReset.OnReset()
    {
        foreach (var pair in dict)
        {
            pair.Value.Callback.Invoke(false);
            ((IDisposable)pair.Value).Dispose();   
        }
        dict.Clear();
        spanHeap.Clear();
    }
    void IDestroy.OnDestroy()
    {
        foreach (var pair in dict)
        {
            ((IDisposable)pair.Value).Dispose();   
        }
        dict.Clear();
        spanHeap.Clear();
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
    /// <param name="seconds">当前时间经过秒数后执行</param>
    /// <param name="callback">触发的回调 (true => 成功触发，false => 取消)</param>
    /// <returns>唯一id</returns>
    public int GetAfterSeconds(long seconds, Action<bool> callback)
    {
        long time = CurrentTime + seconds * 1_000;
        return Get(time, callback);
    }
    /// <summary>
    /// 创建定时器
    /// </summary>
    /// <param name="milliseconds">当前时间经过毫秒数后执行</param>
    /// <param name="callback">触发的回调 (true => 成功触发，false => 取消)</param>
    /// <returns>唯一id</returns>
    public int GetAfterMilliseconds(long milliseconds, Action<bool> callback)
    {
        long time = CurrentTime + milliseconds;
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

    private class TimerSpan: IDisposable, IComparable<TimerSpan>
    {
        private static readonly MonitoredObjectPool.ObjectPool<TimerSpan, TimerSpan> s_Pool = 
            new(nameof(TimerSpan), () => new TimerSpan(), 
                null,
                t =>
                {
                    t.Callback = null;
                    t.Time = 0;
                    t.Id = 0;
                });

        public static TimerSpan Get(int id, long time, Action<bool> callback)
        {
            TimerSpan timerSpan = s_Pool.Get();
            timerSpan.Id = id;
            timerSpan.Time = time;
            timerSpan.Callback = callback ?? throw new ArgumentNullException();
            return timerSpan;
        }
        private TimerSpan() { }

        void IDisposable.Dispose() => s_Pool.Release(this);

#if !POOL_RELEASES
        ~TimerSpan() => s_Pool.FinalizeDebug();
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
