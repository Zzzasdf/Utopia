using System.Collections.Generic;

/// 收集器
public interface ILowMemoryBufferCollector
{
    /// 添加源数据
    public void AddSource(int typeValue, List<long> source);

    /// 编码
    public bool TryEncode(out Dictionary<int, List<long>> encodeCollect);

    /// 获取缓存器
    public ILowMemoryBuffer GetBuffer(int typeValue);
}

/// 低内存缓存器
public interface ILowMemoryBuffer
{
    /// 解码
    void Decode(int typeValue, List<long> source);

    /// 编码
    bool TryEncode(out List<long> buffer);

    /// 保存
    void Save();
}
