using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

/// <summary>
/// 延迟缓冲器
/// </summary>
public class DelayBuffer<T>
{
    /// 延迟的毫秒数
    private long delayMilliseconds;
    private Action<HashSet<T>> fireBatchCallback;
    private Action fireAllCallback;
    private Action<T> fireCallback;
    
    /// 是否准备执行所有类型
    private bool isPreFireAll;
    
    /// 收集的状态
    private bool collecting;
    /// 延迟中的收集的类型
    private HashSet<T> uniqueHashSet;

    /// 取消令牌
    private CancellationTokenSource cts;
    
    /// <summary>
    /// 注册
    /// </summary>
    /// <param name="delayMilliseconds">收集延迟毫秒内的所有类型</param>
    /// <param name="fireBatchCallback">执行批量的类型回调</param>
    /// <param name="fireCallback">执行单类型回调，只用于 FireTypeNow(Type) </param>
    /// <param name="fireAllCallback">执行所有的类型回调</param>
    public DelayBuffer(long delayMilliseconds, Action<HashSet<T>> fireBatchCallback, Action fireAllCallback, Action<T> fireCallback)
    {
        this.delayMilliseconds = delayMilliseconds;
        this.fireBatchCallback = fireBatchCallback;
        this.fireAllCallback = fireAllCallback;
        this.fireCallback = fireCallback;
        this.uniqueHashSet = new HashSet<T>();
    }
    public void Reset()
    {
        if (cts != null)
        {
            cts.Dispose();
            cts = null;
        }
        uniqueHashSet.Clear();
        collecting = false;
        isPreFireAll = false;
    }
    public void Destroy()
    {
        fireCallback = null;
        fireAllCallback = null;
        fireBatchCallback = null;
        if (cts != null) cts.Cancel();
        else Reset();
        uniqueHashSet = null;
    }
    
    public void FireType(T t)
    {
        if (isPreFireAll) return;
        uniqueHashSet.Add(t);
        OnCollectHandle();
    }
    public void FireAllType()
    {
        if (fireAllCallback == null) return;
        isPreFireAll = true;
        OnCollectHandle();
    }

    public void FireTypeNow(T t)
    {
        uniqueHashSet.Remove(t);
        fireCallback?.Invoke(t);
    }
    public void FireAllTypeNow()
    {
        cts?.Cancel();
        fireAllCallback?.Invoke();
    }

    private void OnCollectHandle()
    {
        if (collecting) return;
        collecting = !collecting;
        cts ??= new CancellationTokenSource();
        OnCollectHandleAsync(cts.Token).Forget();
    }
    
    private async UniTaskVoid OnCollectHandleAsync(CancellationToken cancellationToken)
    {
        // 收集延迟时间 内需要推送的数据类型，统一处理
        bool isCanceled = await UniTask.Delay(TimeSpan.FromMilliseconds(delayMilliseconds), cancellationToken: cancellationToken)
            .SuppressCancellationThrow();
        if (!isCanceled)
        {
            if (isPreFireAll)
            {
                fireAllCallback?.Invoke();
            }
            else
            {
                fireBatchCallback?.Invoke(uniqueHashSet);
            }
        }
        Reset();
    }
}
