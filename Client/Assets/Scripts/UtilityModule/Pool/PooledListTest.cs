using System;
using UnityEngine;

public class PooledListTest : MonoBehaviour
{
    int Iterations = 10; // 1亿次迭代
    private PooledList<int> T;

    private void Awake()
    {
        T = PooledList<int>.Get();
    }
    private void OnDestroy()
    {
        ((IDisposable)T).Dispose();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Test();
            Debug.Log(KeyCode.Space.ToString());
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
            Debug.Log(KeyCode.G.ToString());
        }
    }

    public void Test()
    {
        for (int i = 0; i < Iterations; i++)
        {
            WithTryFinally();
        }
    }
    
    // 使用 try-finally 的方法
    void WithTryFinally()
    {
        using (var l = PooledList<int>.Get())
        {
            l.Add(1);
            using (var l2 = PooledList<int>.Get())
            {
                l2.Add(1);
            } 
        } 
    }
}
