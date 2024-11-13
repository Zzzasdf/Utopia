using System.Collections.Generic;

/// 键值对 key => int, value => bool
public class KIntVBoolLowMemoryBuffer : ILowMemoryBuffer
{
    private int type;
    private List<long> source; // 只做内存复用，不处理逻辑
    private Dictionary<int, bool> buffer;
    
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
        buffer ??= new Dictionary<int, bool>();
        buffer.Clear();
        foreach (long l in source)
        {
            (int high, int low) = LongConvertDoubleInt(l);
            buffer.Add(high, low == 1);
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
            source.Add(DoubleIntCombineLong(pair.Key, pair.Value ? 1 : 0));    
        }
        buffer = source;
        return true;
    }
    
    public static implicit operator Dictionary<int, bool>(KIntVBoolLowMemoryBuffer buffer)
    {
        buffer.buffer ??= new Dictionary<int, bool>();
        return buffer.buffer;
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