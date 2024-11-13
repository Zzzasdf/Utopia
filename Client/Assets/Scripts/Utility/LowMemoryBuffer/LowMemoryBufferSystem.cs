using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

/// 低内存缓存器
public partial class LowMemoryBufferSystem
{
    private static LowMemoryBufferSystem _instance;
    public static LowMemoryBufferSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LowMemoryBufferSystem();
            }
            return _instance;
        }
    }
    
    private List<List<long>> source;
    private Dictionary<int, List<long>> encodeCollect;
    private List<int> usingListType;
    
    public void AddSource(List<List<long>> source)
    {
        this.source = source;
        for (int i = 0; i < source?.Count; i++)
        {
            List<long> dataSource = source[i];
            if (!TryGetTypeConfig(i, out  ILowMemoryBufferCollector bufferCollector))
            {
                Debug.LogError($"缺少对应的类型解析器 typeValue => {i}");
                continue;
            }
            bufferCollector.AddSource(i, dataSource);
        }
    }
    
    public void SaveAll()
    {
        if (!TryEncode(out Dictionary<int, List<long>> collect))
        {
            return;
        }
        usingListType ??= new List<int>();
        source ??= new List<List<long>>();
        foreach (var pair in collect)
        {
            usingListType.Add(pair.Key);
            // 当前索引位处理
            if (pair.Key > source.Count - 1)
            {
                source.Add(new List<long>());
            }
            if(!ReferenceEquals(source[pair.Key], pair.Value))
            {
                if (source[pair.Key] == null)
                {
                    source[pair.Key] = pair.Value;
                }
                else
                {
                    source[pair.Key].Clear();
                    source[pair.Key].AddRange(pair.Value);
                }
            }
        }
        usingListType.Sort();
        int maxCount = usingListType[^1] + 1;
        // 去除尾部无用列表
        for (int i = source.Count - 1; i >= maxCount; i--)
        {
            source.RemoveAt(i);
        }
        
        // 中间的无用列表置空
        for (int i = 0; i < source.Count; i++)
        {
            if(usingListType.Contains(i))
            {
                continue;
            }
            source[i] = null;
        }
        
        // TODO ZZZ => 打印日志
        for (int i = 0; i < source.Count; i++)
        {
            List<long> value = source[i];
            if (value == null)
            {
                continue;
            }
            StringBuilder sb = new StringBuilder().AppendLine($"Index => {i}");
            for (int j = 0; j < value.Count; j++)
            {
                sb.AppendLine($"[{j}] => {value[j]}");
            }
            Debug.Log(sb.ToString());
        }
    }

    public void Save(int type, List<long> collect)
    {
        // TODO ZZZ => 打印日志
        List<long> value = collect;
        StringBuilder sb = new StringBuilder($"Index => {type}");
        for (int j = 0; j < value.Count; j++)
        {
            sb.AppendLine($"[j] => {value[j]}");
        }
        Debug.Log(sb.ToString());
    }
    
    private bool TryEncode(out Dictionary<int, List<long>> collect)
    {
        bool isExistEncodeData = false;
        this.encodeCollect?.Clear();
        foreach (var pair in bufferCollectors)
        {
            if (pair.Value.TryEncode(out Dictionary<int, List<long>> encodeCollect))
            {
                this.encodeCollect ??= new Dictionary<int, List<long>>();
                this.encodeCollect.AddRange(encodeCollect);
                isExistEncodeData = true;
            }
        }
        if (!isExistEncodeData)
        {
            collect = null;
            return false;
        }
        collect = this.encodeCollect;
        return true;
    }
}
