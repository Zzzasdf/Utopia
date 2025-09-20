using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 倒计时
/// CPU 优化点：记录一次当前时间，固定频率修正
/// GC 优化点：记录类型的显示的数字字符串
/// </summary>
public class CountDownManager : ICountDownManager
{
    private long CurrentTime => (long)(Time.time * 1_000); // TODO ZZZ 当前时间
    
    // 自增 id
    private int incrementId;
    private SortHeap<CountDownSpan> spanSortHeap;
    private Dictionary<int, CountDownSpan> dict;

    void IInit.OnInit()
    {
        spanSortHeap ??= new SortHeap<CountDownSpan>();
        dict ??= new Dictionary<int, CountDownSpan>();
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
            CountDownSpan countDownSpan = spanSortHeap.Peek();
            if (CurrentTime < countDownSpan.EndMilliseconds) return;
            spanSortHeap.Pop();
            dict.Remove(countDownSpan.Id);
            countDownSpan.Callback.Invoke(true);
            ((IDisposable)countDownSpan).Dispose();
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
        CountDownSpan countDownSpan = CountDownSpan.Get(id, endMilliseconds, callback);
        spanSortHeap.Push(countDownSpan);
        dict.Add(id, countDownSpan);
        return id;
    }

    /// <summary>
    /// 取消定时器
    /// </summary>
    /// <param name="id"></param>
    public void Cancel(int id)
    {
        if (!dict.Remove(id, out CountDownSpan timerSpan)) return;
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

    private class CountDownSpan: IDisposable, IComparable<CountDownSpan>
    {
        private static readonly MonitoredObjectPool.ObjectPool<CountDownSpan, CountDownSpan> s_Pool = 
            new(nameof(CountDownSpan), () => new CountDownSpan(), 
                null,
                t =>
                {
                    t.Callback = null;
                    t.EndMilliseconds = 0;
                    t.Id = 0;
                });

        public static CountDownSpan Get(int id, long endMilliseconds, Action<bool> callback)
        {
            CountDownSpan countDownSpan = s_Pool.Get();
            countDownSpan.Id = id;
            countDownSpan.EndMilliseconds = endMilliseconds;
            countDownSpan.Callback = callback ?? throw new ArgumentNullException();
            return countDownSpan;
        }
        private CountDownSpan() { }

        void IDisposable.Dispose() => s_Pool.Release(this);

#if !POOL_RELEASES
        ~CountDownSpan() => s_Pool.FinalizeDebug();
#endif
        
        public int Id { get; private set; }
        public long EndMilliseconds { get; private set; }
        public Action<bool> Callback { get; private set; }

        public int CompareTo(CountDownSpan other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return EndMilliseconds.CompareTo(other.EndMilliseconds);
        }
    }
}
