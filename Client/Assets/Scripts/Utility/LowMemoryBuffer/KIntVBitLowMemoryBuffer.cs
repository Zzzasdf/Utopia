using System.Collections.Generic;

/// 键值对 key => int, value => bit (32bit)
public class KIntVBitLowMemoryBuffer: ILowMemoryBuffer
{
    private int type;
    private List<long> source; // 只做内存复用，不处理逻辑
    private Dictionary<int, int> buffer;
    
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
        if (source == null || source.Count == 0)
        {
            return;
        }
        buffer ??= new Dictionary<int, int>();
        buffer.Clear();
        foreach (long l in source)
        {
            (int high, int low) = LongConvertDoubleInt(l);
            buffer.Add(high, low);
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
            source.Add(DoubleIntCombineLong(pair.Key, pair.Value));    
        }
        buffer = source;
        return true;
    }

    public bool ContainsKey(int key) => buffer?.ContainsKey(key) ?? false;

    public bool ContainsFlagValue(int key, int flagValue)
    {
        if (buffer == null || !buffer.TryGetValue(key, out int value))
        {
            return false;
        }
        return (value & flagValue) != 0;
    }

    public void AddFlagValue(int key, int flagValue)
    {
        buffer ??= new Dictionary<int, int>();
        if (buffer.TryAdd(key, flagValue))
        {
            return;
        }
        buffer[key] |= flagValue;
    }

    public void RemoveFlagValue(int key, int flagValue)
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
    
    private (int high, int low) LongConvertDoubleInt(long l)
    {
        return ((int)(l >> 32), (int)(l & 0xFFFFFFFF));
    }

    private long DoubleIntCombineLong(int high, int low)
    {
        return ((long) high << 32) | (low & 0xFFFFFFFFL);
    }
}
