using System.Collections.Generic;

/// 键值对 key => long, value => bit (64)
public class KLongVBitLowMemoryBuffer : ILowMemoryBuffer
{
    private int type;
    private List<long> source; // 只做内存复用，不处理逻辑
    private Dictionary<long, long> buffer;
    
    public void Save()
    {
        if (!TryEncode(out List<long> source))
        {
            LowMemoryBufferSystem.Instance.Save(type, null);
            return;
        }
        LowMemoryBufferSystem.Instance.Save(type, source);
    }
    
    void ILowMemoryBuffer.Decode(int typeValue, List<long> source)
    {
        this.type = typeValue;
        this.source = source;
        if (source == null || source.Count < 2)
        {
            return;
        }
        buffer ??= new Dictionary<long, long>();
        buffer.Clear();
        for (int i = 0; i + 1< source.Count; i += 2)
        {
            buffer.Add(source[i], source[i + 1]);
        }
    }

    bool ILowMemoryBuffer.TryEncode(out List<long> buffer)
    {
        return TryEncode(out buffer);
    }

    private bool TryEncode(out List<long> buffer)
    {
        if (this.buffer == null || this.buffer.Count == 0)
        {
            buffer = null;
            return false;
        }
        source ??= new List<long>();
        source.Clear();
        foreach (var pair in this.buffer)
        {
            source.Add(pair.Key);
            source.Add(pair.Value);
        }
        buffer = source;
        return true;
    }
    
    public bool ContainsKey(long key) => buffer?.ContainsKey(key) ?? false;

    public bool ContainsFlagValue(long key, long flagValue)
    {
        if (buffer == null || !buffer.TryGetValue(key, out long value))
        {
            return false;
        }
        return (value & flagValue) != 0;
    }

    public void AddFlagValue(long key, long flagValue)
    {
        buffer ??= new Dictionary<long, long>();
        if (buffer.TryAdd(key, flagValue))
        {
            return;
        }
        buffer[key] |= flagValue;
    }

    public void RemoveFlagValue(long key, long flagValue)
    {
        if (buffer == null || !buffer.ContainsKey(key))
        {
            return;
        }
        buffer[key] -= flagValue;
        if (buffer[key] == 0)
        {
            buffer.Remove(key);
        }
    }
}