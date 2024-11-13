using System.Collections.Generic;

public class LowMemoryBufferCollector<T> : ILowMemoryBufferCollector
    where T: ILowMemoryBuffer, new()
{
    private Dictionary<int, T> collect;
    private Dictionary<int, List<long>> encodeCollect;
    
    void ILowMemoryBufferCollector.AddSource(int typeValue, List<long> source)
    {
        collect ??= new Dictionary<int, T>();
        if (!collect.TryGetValue(typeValue, out T value))
        {
            collect.TryAdd(typeValue, value = new T());
        }
        value.Decode(typeValue, source);
    }

    bool ILowMemoryBufferCollector.TryEncode(out Dictionary<int, List<long>> encodeCollect)
    {
        if (this.collect == null || this.collect.Count == 0)
        {
            encodeCollect = null;
            return false;
        }
        this.encodeCollect ??= new Dictionary<int, List<long>>();
        this.encodeCollect.Clear();
        foreach (var pair in this.collect)
        {
            if (!pair.Value.TryEncode(out List<long> buffer))
            {
                continue;
            }
            this.encodeCollect.Add(pair.Key, buffer);
        }
        encodeCollect = this.encodeCollect;
        return true;
    }

    ILowMemoryBuffer ILowMemoryBufferCollector.GetBuffer(int typeValue)
    {
        this.collect ??= new Dictionary<int, T>();
        if (!this.collect.TryGetValue(typeValue, out T lowMemoryBufferCollector))
        {
            lowMemoryBufferCollector = new T();
            this.collect.Add(typeValue, lowMemoryBufferCollector);
        }
        return lowMemoryBufferCollector;
    }
}