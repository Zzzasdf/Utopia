using System;
using System.Collections;
using System.Text;
using UnityEngine;

public class CoroutineTest : MonoBehaviour
{
    [SerializeField][Header("迭代次数")] private int Iterations = 1_000; // 一千次迭代
    [SerializeField][Header("性能测试 KeyCode")] private KeyCode keyCode = KeyCode.Space;
    private static float delayTime = 0.01f;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(delayTime);
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            Debug.Log("手动触发GC");
        }
        if (Input.GetKeyDown(keyCode))
        {
            StartCoroutine(PerformanceTest());
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartCoroutine(NormalTest());
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(CacheTest());
        }
    }

    /// 性能测试
    private IEnumerator PerformanceTest()
    {
        long milliseconds;
        long before = GC.GetTotalMemory(true);
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < Iterations; i++)
            {
                yield return new WaitForSecondsValue(delayTime);
            }
            milliseconds = stopwatch.ElapsedMilliseconds;
        }
        long after = GC.GetTotalMemory(true);
        StringBuilder sb = new StringBuilder("Performance");
        sb.AppendLine($"耗时 => {milliseconds}毫秒");
        sb.AppendLine($"GC分配总量: {after - before} bytes");
        Debug.Log(sb.ToString());
    }

    private IEnumerator NormalTest()
    {
        long milliseconds;
        long before = GC.GetTotalMemory(true);
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < Iterations; i++)
            {
                yield return new WaitForSeconds(delayTime);
            }
            milliseconds = stopwatch.ElapsedMilliseconds;
        }
        long after = GC.GetTotalMemory(true);
        StringBuilder sb = new StringBuilder("Normal");
        sb.AppendLine($"耗时 => {milliseconds}毫秒");
        sb.AppendLine($"GC分配总量: {after - before} bytes");
        Debug.Log(sb.ToString());
    }
    
    private IEnumerator CacheTest()
    {
        long milliseconds;
        long before = GC.GetTotalMemory(true);
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < Iterations; i++)
            {
                yield return waitForSeconds;
                yield return null;
            }
            milliseconds = stopwatch.ElapsedMilliseconds;
        }
        long after = GC.GetTotalMemory(true);
        StringBuilder sb = new StringBuilder("Normal");
        sb.AppendLine($"耗时 => {milliseconds}毫秒");
        sb.AppendLine($"GC分配总量: {after - before} bytes");
        Debug.Log(sb.ToString());
    }
}
