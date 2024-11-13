using System.Collections.Generic;

// 位图 (64bit)
public class BitmapLowMemoryBuffer: ILowMemoryBuffer
{
    private const int Size = sizeof(long) * 8;
    private int type;
    private List<long> source;
    
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
    }

    bool ILowMemoryBuffer.TryEncode(out List<long> buffer)
    {
        return TryEncode(out buffer);
    }

    private bool TryEncode(out List<long> buffer)
    {
        if (source == null || source.Count == 0)
        {
            buffer = null;
            return false;
        }
        buffer = source;
        return true;
    }

    public bool Contain(uint bitIndex)
    {
        int arrayIndex = (int)bitIndex / Size;
        if (arrayIndex >= source.Count)
        {
            return false;
        }
        int dataBitIndex = (int)bitIndex % Size;
        long l = 1 << dataBitIndex;
        return (source[arrayIndex] & l) != 0;
    }

    public void AddBit(uint bitIndex)
    {
        source ??= new List<long>();
        int arrayIndex = (int)bitIndex / Size;
        int arrayLeastCount = arrayIndex + 1;
        int dataBitIndex = (int)bitIndex % Size;
        // 不足补位
        for (int i = source.Count; i < arrayLeastCount; i++)
        {
            source.Add(0);
        }
        long l = 1 << dataBitIndex;
        source[arrayIndex] |= l;
    }

    public void RemoveBit(uint bitIndex)
    {
        int arrayIndex = (int)bitIndex / Size;
        if (arrayIndex >= source.Count)
        {
            return;
        }
        int dataBitIndex = (int)bitIndex % Size;
        long l = 1 << dataBitIndex;
        source[arrayIndex] &= ~l;
        if (source[arrayIndex] == 0)
        {
            // 检查 尾部 是否 有无用的 多余数据
            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (source[i] == 0)
                {
                    source.RemoveAt(i);
                    continue;
                }
                return;
            }
        }
    }
}
