using System;
using System.Collections.Generic;
using UnityEngine.Pool;

public sealed class PooledList<T> : List<T>, IDisposable
{
    private static readonly bool collectionCheck = true;
    private static readonly int defaultCapacity = 10;
    // 能够精确定位未回收的类型，有且只能通过内部池创建、回收
    private static readonly int maxSize = 10000;
#if UNITY_EDITOR
    private static readonly ObjectPool<PooledList<T>> s_Pool = 
        new(() => new PooledList<T>(), 
            OnGet,
            OnRelease,
            null,
            collectionCheck,
            defaultCapacity, maxSize);
    private static void OnGet(PooledList<T> list)
    {
        GC.ReRegisterForFinalize(list);
        if (s_Pool.CountAll <= maxSize) return;
        UnityEngine.Debug.LogError($"pool maxsize eg!! {list.GetType()} => 当前 active:{s_Pool.CountActive} + inactive:{s_Pool.CountInactive} > maxSize:{maxSize}, " +
                                   $"当所有 active 对象回收时，将存在销毁，若前无集合重复回收报错，则请将最大容量至少提高到 {s_Pool.CountAll}");
    }
    private static void OnRelease(PooledList<T> list)
    {
        list.Clear();
        GC.SuppressFinalize(list);
    }
#else
    private static readonly ObjectPool<PooledList<T>> s_Pool = 
        new(() => new PooledList<T>(), 
            null,
            l => l.Clear(),
            null,
            collectionCheck,
            defaultCapacity, maxSize);
#endif

    public static UnityEngine.Pool.PooledObject<PooledList<T>> Get(out PooledList<T> value) => s_Pool.Get(out value);
    public static PooledList<T> Get() => s_Pool.Get();
    private PooledList() { }

    void IDisposable.Dispose()
    {
        s_Pool.Release(this);
    }

#if UNITY_EDITOR
    ~PooledList()
    {
        UnityEngine.Debug.LogError($"pool item gc eg!! {GetType()} => 当前对象被销毁，代码中存在未回收该类型的地方");
    }
#endif

#if UNITY_EDITOR
    private class Example
    {
        // 修改 list 方法
        private void SetList(in IList<int> list)
        {
            // list = new List<int>(); error!! in 限制，不允许在方法内 修改引用对象
            list.Add(0); // 支持 元子 操作
        }
            
        #region 局部变量
        private void Foo()
        {
            // 第一种写法，作用域为整个方法，在离开 Foo 方法后 自动释放
            using var list = PooledList<int>.Get();
            SetList(list);
            // list = PooledList<int>.Get(); error!! using 的引用对象不可变
        
            // 第二种写法
            using (var list2 = PooledList<int>.Get())
            {
                SetList(list2);
            } // 离开作用域，自动释放 进池 
        }
        #endregion
        
        #region 全局变量
        // eg!!! 千万不要使用 list3 = new() 的方式定义
        // 脚本重新编译触发重置，每次修改脚本并返回运行模式时，Unity 会重新编译并重新初始化所有字段
        // 导致会执行多次脚本，触发多次调用，大于实际同时存在的数量变多，error 日志不精确
        // 快速检查 预制体上的所有 mono 脚本中是否存在该类型定义：
        //      1、运行前选中过所需预制体，然后运行
        //      2、重新编译前选中过所需预制体
        private PooledList<int> list3;
        private void Init()
        {
            list3 = PooledList<int>.Get(); // eg!! 初始化后不要在脚本中修改引用的对象，除非先释放
        }
        private void Foo2()
        {
            list3.Clear();
            SetList(list3);
        }
        public void DestroyModel()
        {
            // eg!! 在销毁时，需要手动释放 !! 
            // 若缺少此步骤，当给这个池化集合初始化后，在触发 gc 或者 关闭编辑器时 会出现 error 日志
            ((IDisposable)list3).Dispose();
        }
        #endregion
    }
#endif
}