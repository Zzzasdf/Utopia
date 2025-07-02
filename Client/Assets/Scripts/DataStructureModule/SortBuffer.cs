using System;
using System.Collections.Generic;

/// 排序缓存器
public interface ISortBuffer<TKey, TValue>
{
    void Sort(TKey key, Comparison<TValue> comparison);
    bool TryGetValueByIndex(TKey key, int index, out TValue value);
    bool TryGetValues(TKey key, out IReadOnlyList<TValue> values);
}

public class SortBuffer<TKey, TValue>: ISortBuffer<TKey, TValue>
{
    private Dictionary<TKey, List<TValue>> buffer;
    private Func<List<TValue>> sourceFunc;

    public SortBuffer(Func<List<TValue>> sourceFunc)
    {
        this.sourceFunc = sourceFunc;
    }
    
    public void Sort(TKey key, Comparison<TValue> comparison)
    {
        buffer ??= new Dictionary<TKey, List<TValue>>();
        buffer.Remove(key);
        List<TValue> value = sourceFunc.Invoke();
        value.Sort(comparison);
        buffer.Add(key, value);
    }

    public bool TryGetValues(TKey key, out IReadOnlyList<TValue> values)
    {
        buffer ??= new Dictionary<TKey, List<TValue>>();
        if (!buffer.TryGetValue(key, out List<TValue> valueList))
        {
            values = default(IReadOnlyList<TValue>);
            return false;
        }
        values = valueList;
        return true;
    }
    
    public bool TryGetValueByIndex(TKey key, int index, out TValue value)
    {
        buffer ??= new Dictionary<TKey, List<TValue>>();
        if (!buffer.TryGetValue(key, out List<TValue> values)
            || index >= values.Count)
        {
            value = default(TValue);
            return false;
        }
        value = values[index];
        return true;
    }
}
