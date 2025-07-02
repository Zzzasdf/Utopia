using System;
using System.Collections.Generic;
using UnityEngine.Pool;

public class TimerManager: ITimerManager
{
    private long CurrentTime => 0; // TODO ZZZ 当前时间
    
    // 自增 id
    private int incrementId;
    private TimerSpanPool pool;
    private TimerHeap heap;
    private Dictionary<int, TimerSpan> dic;

    void IInit.OnInit()
    {
        heap = new TimerHeap();
        dic = new Dictionary<int, TimerSpan>();
    }
    void IReset.OnReset()
    {
        dic.Clear();
        heap.Clear();
    }
    void IDestroy.OnDestroy()
    {
        
    }

    void IUpdate.OnUpdate(float deltaTime)
    {
        while (!heap.IsEmpty)
        {
            TimerSpan timerSpan = heap.Peek();
            if (CurrentTime < timerSpan.Time) return;
            timerSpan.Callback?.Invoke();
            heap.Pop();
            dic.Remove(timerSpan.Id);
        }
    }
    
    /// <summary>
    /// 创建定时器
    /// </summary>
    /// <param name="time">触发的时点</param>
    /// <param name="callback">触发的回调</param>
    /// <returns>唯一id</returns>
    public int Create(long time, Action callback)
    {
        int id = CreateUniqueId();
        TimerSpan timerSpan = pool.Get(id, time, callback);
        heap.Push(timerSpan);
        dic.Add(id, timerSpan);
        return id;
    }

    /// <summary>
    /// 取消定时器
    /// </summary>
    /// <param name="id"></param>
    public void Cancel(int id)
    {
        if (!dic.Remove(id, out TimerSpan timerSpan)) return;
        heap.Remove(timerSpan);
    }

    /// 生成唯一 id
    private int CreateUniqueId()
    {
        do
        {
            incrementId++;
        } while (!dic.ContainsKey(incrementId));
        return incrementId;
    }

    private class TimerHeap : Heap<TimerSpan>
    {
        public void Remove(TimerSpan timerSpan)
        {
            heap.Remove(timerSpan);
        }
    }

    private class TimerSpanPool
    {
        private readonly ObjectPool<TimerSpan> s_Pool = new ObjectPool<TimerSpan>
        (
            () => new TimerSpan(), 
            null, 
            x => x.Clear()
        );
        public TimerSpan Get(int id, long time, Action callback)
        {
            TimerSpan result = s_Pool.Get();
            result.Init(id, time, callback);
            return result;
        }
        public void Release(TimerSpan toRelease) => s_Pool.Release(toRelease);
    }

    private class TimerSpan: IComparable<TimerSpan>
    {
        public int Id { get; private set; }
        public long Time { get; private set; }
        public Action Callback { get; private set; }

        public void Init(int id, long time, Action callback)
        {
            Id = id;
            Time = time;
            Callback = callback;
        }
        public void Clear()
        {
            Callback = null;
            Time = 0;
            Id = 0;
        }

        public int CompareTo(TimerSpan other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Time.CompareTo(other.Time);
        }
    }
}
