using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定时器
/// CPU 优化点：记录一次当前时间，固定频率修正
/// </summary>
public class TimerManager: ITimerManager
{
    private long CurrentTime => (long)(Time.time * 1_000); // TODO ZZZ 当前时间
    
    // 自增 id
    private int incrementId;
    private SortHeap<TimerSpan> spanSortHeap;
    private Dictionary<int, TimerSpan> dict;

    void IInit.OnInit()
    {
        spanSortHeap ??= new SortHeap<TimerSpan>();
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
        spanSortHeap.Clear();
    }
    void IDestroy.OnDestroy()
    {
        foreach (var pair in dict)
        {
            ((IDisposable)pair.Value).Dispose();   
        }
        dict.Clear();
        spanSortHeap.Clear();
    }

    void IUpdate.OnUpdate(float deltaTime)
    {
        while (!spanSortHeap.IsEmpty)
        {
            TimerSpan timerSpan = spanSortHeap.Peek();
            if (CurrentTime < timerSpan.EndMilliseconds) return;
            spanSortHeap.Pop();
            dict.Remove(timerSpan.Id);
            timerSpan.Callback.Invoke(true);
            ((IDisposable)timerSpan).Dispose();
        }
    }

    /// <summary>
    /// 创建定时器
    /// </summary>
    /// <param name="milliseconds">当前时间经过毫秒数后执行</param>
    /// <param name="callback">触发的回调 (true => 成功触发，false => 取消)</param>
    /// <returns>唯一id</returns>
    public int SetAfterMilliseconds(long milliseconds, Action<bool> callback)
    {
        long time = CurrentTime + milliseconds;
        return SetEndMilliseconds(time, callback);
    }
    /// <summary>
    /// 创建定时器
    /// </summary>
    /// <param name="endMilliseconds">结束的时间（毫秒）</param>
    /// <param name="callback">触发的回调 (true => 成功触发，false => 取消)</param>
    /// <returns></returns>
    public int SetEndMilliseconds(long endMilliseconds, Action<bool> callback)
    {
        int id = CreateUniqueId();
        TimerSpan timerSpan = TimerSpan.Get(id, endMilliseconds, callback);
        spanSortHeap.Push(timerSpan);
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
        spanSortHeap.Remove(timerSpan);
        timerSpan.Callback.Invoke(false);
        ((IDisposable)timerSpan).Dispose();
    }

    /// 生成唯一 id
    private int CreateUniqueId()
    {
        do
        {
            incrementId++;
        } while (dict.ContainsKey(incrementId));
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
                    t.EndMilliseconds = 0;
                    t.Id = 0;
                });

        public static TimerSpan Get(int id, long endMilliseconds, Action<bool> callback)
        {
            TimerSpan timerSpan = s_Pool.Get();
            timerSpan.Id = id;
            timerSpan.EndMilliseconds = endMilliseconds;
            timerSpan.Callback = callback ?? throw new ArgumentNullException();
            return timerSpan;
        }
        private TimerSpan() { }

        void IDisposable.Dispose() => s_Pool.Release(this);

#if !POOL_RELEASES
        ~TimerSpan() => s_Pool.FinalizeDebug();
#endif
        
        public int Id { get; private set; }
        public long EndMilliseconds { get; private set; }
        public Action<bool> Callback { get; private set; }

        public int CompareTo(TimerSpan other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return EndMilliseconds.CompareTo(other.EndMilliseconds);
        }
    }
}
