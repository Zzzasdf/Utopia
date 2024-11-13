using System;
using System.Collections.Generic;
using UnityEngine;

/// 低内存缓存器，所有定义的枚举值必须唯一，递增
public partial class LowMemoryBufferSystem
{
    /// 收集器类型
    private Dictionary<Type, ILowMemoryBufferCollector> bufferCollectors = new()
    {
        [typeof(EKIntVInt)] = new LowMemoryBufferCollector<KIntVIntLowMemoryBuffer>(),
        [typeof(EKIntVBool)] = new LowMemoryBufferCollector<KIntVBoolLowMemoryBuffer>(),
        [typeof(EKIntVBit)] = new LowMemoryBufferCollector<KIntVBitLowMemoryBuffer>(),
        
        [typeof(EKLongVLong)] = new LowMemoryBufferCollector<KLongVLongLowMemoryBuffer>(),
        [typeof(EKLongVBool)] = new LowMemoryBufferCollector<KLongVBoolLowMemoryBuffer>(),
        [typeof(EKLongVBit)] = new LowMemoryBufferCollector<KLongVBitLowMemoryBuffer>(),
        
        [typeof(EBitmap)] = new LowMemoryBufferCollector<BitmapLowMemoryBuffer>(),
    };
    
    /// 配置表
    private Dictionary<int, Type> config = new()
    {
        [(int)EKIntVInt.Test_0] = typeof(EKIntVInt),
        [(int)EKIntVBool.Test_1] = typeof(EKIntVBool),
        [(int)EKIntVBit.Test_2] = typeof(EKIntVBit),
        [(int)EKLongVLong.Test_3] = typeof(EKLongVLong),
        [(int)EKLongVBool.Test_4] = typeof(EKLongVBool),
        [(int)EKLongVBit.Test_5] = typeof(EKLongVBit),
        [(int)EBitmap.Test_6] = typeof(EBitmap),
        [(int)EKIntVInt.Test_7] = typeof(EKIntVInt),
    };

    /// 获取配置
    private bool TryGetTypeConfig(int typeValue, out ILowMemoryBufferCollector bufferCollector)
    {
        if (!config.TryGetValue(typeValue, out Type type))
        {
            Debug.LogError($"缺少对应的配置 => {typeValue}");
            bufferCollector = null;
            return false;
        }
        if (!bufferCollectors.TryGetValue(type, out bufferCollector))
        {
            Debug.LogError($"缺少对应的收集器类型 => {type}");
            return false;
        }
        return true;
    }
    
    /// 获取对应缓存器
    public bool TryGetBuffer(int typeValue, out ILowMemoryBuffer lowMemoryBuffer)
    {
        if (!TryGetTypeConfig(typeValue, out ILowMemoryBufferCollector bufferCollector))
        {
            lowMemoryBuffer = null;
            return false;
        }
        lowMemoryBuffer = bufferCollector.GetBuffer(typeValue);
        return true;
    }
}


public enum EKIntVInt
{
    Test_0 = 0,
    Test_7 = 7,
}

public enum EKIntVBool
{
    Test_1 = 1,
}

public enum EKIntVBit
{
    Test_2 = 2,
}

public enum EKLongVLong
{
    Test_3 = 3,
}

public enum EKLongVBool
{
    Test_4 = 4,
}

public enum EKLongVBit
{
    Test_5 = 5,
}

public enum EBitmap
{
    Test_6 = 6,
}

public static class LowMemoryBufferSystemExtern
{
    public static Dictionary<int, int> GetBuffer(this EKIntVInt self)
    {
        if(!LowMemoryBufferSystem.Instance.TryGetBuffer((int)self, out ILowMemoryBuffer lowMemoryBuffer))
        {
            throw new Exception("配置有问题，请检查");
        }
        return lowMemoryBuffer as KIntVIntLowMemoryBuffer;
    }
    
    public static Dictionary<int, bool> GetBuffer(this EKIntVBool self)
    {
        if(!LowMemoryBufferSystem.Instance.TryGetBuffer((int)self, out ILowMemoryBuffer lowMemoryBuffer))
        {
            throw new Exception("配置有问题，请检查");
        }
        return lowMemoryBuffer as KIntVBoolLowMemoryBuffer;
    }
    
    public static KIntVBitLowMemoryBuffer GetBuffer(this EKIntVBit self)
    {
        if(!LowMemoryBufferSystem.Instance.TryGetBuffer((int)self, out ILowMemoryBuffer lowMemoryBuffer))
        {
            throw new Exception("配置有问题，请检查");
        }
        return lowMemoryBuffer as KIntVBitLowMemoryBuffer;
    }

    public static Dictionary<long, long> GetBuffer(this EKLongVLong self)
    {
        if(!LowMemoryBufferSystem.Instance.TryGetBuffer((int)self, out ILowMemoryBuffer lowMemoryBuffer))
        {
            throw new Exception("配置有问题，请检查");
        }
        return lowMemoryBuffer as KLongVLongLowMemoryBuffer;
    }
    
    public static Dictionary<long, bool> GetBuffer(this EKLongVBool self)
    {
        if(!LowMemoryBufferSystem.Instance.TryGetBuffer((int)self, out ILowMemoryBuffer lowMemoryBuffer))
        {
            throw new Exception("配置有问题，请检查");
        }
        return lowMemoryBuffer as KLongVBoolLowMemoryBuffer;
    }
    
    public static KLongVBitLowMemoryBuffer GetBuffer(this EKLongVBit self)
    {
        if(!LowMemoryBufferSystem.Instance.TryGetBuffer((int)self, out ILowMemoryBuffer lowMemoryBuffer))
        {
            throw new Exception("配置有问题，请检查");
        }
        return lowMemoryBuffer as KLongVBitLowMemoryBuffer;
    }

    public static BitmapLowMemoryBuffer GetBuffer(this EBitmap self)
    {
        if(!LowMemoryBufferSystem.Instance.TryGetBuffer((int)self, out ILowMemoryBuffer lowMemoryBuffer))
        {
            throw new Exception("配置有问题，请检查");
        }
        return lowMemoryBuffer as BitmapLowMemoryBuffer;
    }
}