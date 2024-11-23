using System;
using System.Collections.Generic;
using System.Text;
using Converter.List.Long;
using UnityEngine;

/// 客户端配置 的 转换系统
public class ClientConfigConvertSystem : ConverterCollectorSystem<RepeatedField<RepeatedField<long>>, RepeatedField<long>>, IClientConfigConverSystem
{
    private static ClientConfigConvertSystem _instance;
    public static ClientConfigConvertSystem Instance => _instance ??= new ClientConfigConvertSystem();

    /// 收集器类型
    protected override Dictionary<Type, IConverterCollector<RepeatedField<long>>> converterCollectors { get; } = new()
    {
        // 集合
        [typeof(EListInt)] = new ConverterCollector<ListIntConverter>(),
        [typeof(EListLong)] = new ConverterCollector<ListLongConverter>(),
        
        // 映射
        [typeof(EMapIntBool)] = new ConverterCollector<MapIntBoolConverter>(),
        [typeof(EMapIntBit)] = new ConverterCollector<MapIntBitConverter>(),
        [typeof(EMapIntInt)] = new ConverterCollector<MapIntIntConverter>(),
        [typeof(EMapIntLong)] = new ConverterCollector<MapIntLongConverter>(),

        [typeof(EMapLongBool)] = new ConverterCollector<MapLongBoolConverter>(),
        [typeof(EMapLongBit)] = new ConverterCollector<MapLongBitConverter>(),
        [typeof(EMapLongInt)] = new ConverterCollector<MapLongIntConverter>(),
        [typeof(EMapLongLong)] = new ConverterCollector<MapLongLongConverter>(),
        
        // 位图
        [typeof(EBitmap)] = new ConverterCollector<BitmapConverter>(),

        // 自定义
        // 每日首次红点
        [typeof(EBitmapDailyFirstRedDot)] = new ConverterCollector<BitmapDailyFirstRedDotConverter>(),
    };

    /// 配置表
    protected override Dictionary<int, Type> config { get; } = new()
    {
        [(int)EMapIntInt.Test_0] = typeof(EMapIntInt),
        [(int)EMapIntBool.Test_1] = typeof(EMapIntBool),
        [(int)EMapIntBit.Test_2] = typeof(EMapIntBit),
        [(int)EMapLongLong.Test_3] = typeof(EMapLongLong),
        [(int)EMapLongBool.Test_4] = typeof(EMapLongBool),
        [(int)EMapLongBit.Test_5] = typeof(EMapLongBit),
        [(int)EBitmap.Test_6] = typeof(EBitmap),
        [(int)EMapIntInt.Test_7] = typeof(EMapIntInt),
        [(int)EBitmapDailyFirstRedDot.Unique] = typeof(EBitmapDailyFirstRedDot),
        [(int)EMapIntLong.Test_9] = typeof(EMapIntLong),
        [(int)EListInt.Test_10] = typeof(EListInt),
        [(int)EListLong.Test_11] = typeof(EListLong),
        [(int)EMapLongInt.Test_12] = typeof(EMapLongInt),
    };

    void IClientConfigConverSystem.Save<TConverterCollector, TConverter>(int typeValue)
    {
        Debug.LogError($"保存类型 => {typeValue}");
        if (!TryEncode<TConverterCollector, TConverter>(typeValue, out RepeatedField<long> source))
        {
            Debug.LogWarning($"没有数据，无需保存 typeValue => {typeValue}");
            return;
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"类型 => {typeValue}");
        for (int i = 0; i < source.Count; i++)
        {
            sb.AppendLine($"[{i}] => {source[i]}");
        }
        Debug.LogError(sb.ToString());
    }

    public void SaveAll()
    {
        Debug.LogError("保存所有");
        if (!TryEncode(out RepeatedField<RepeatedField<long>> source))
        {
            Debug.LogWarning("没有数据，无需保存");
            return;
        }
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < source.Count; i++)
        {
            var tmpSource = source[i];
            sb.AppendLine($"类型 => {i}");
            for (int j = 0; j < tmpSource?.Count; j++)
            {
                sb.AppendLine($"[{j}] => {tmpSource[j]}");
            }
        }
        Debug.LogError(sb.ToString());
    }

    public enum EListInt
    {
        Test_10 = 10,
    }

    public enum EListLong
    {
        Test_11 = 11,
    }

    public enum EMapIntBool
    {
        Test_1 = 1,
    }

    public enum EMapIntBit
    {
        Test_2 = 2,
    }
    
    public enum EMapIntInt
    {
        Test_0 = 0,
        Test_7 = 7,
    }

    public enum EMapIntLong
    {
        Test_9 = 9,
    }
    
    public enum EMapLongBool
    {
        Test_4 = 4,
    }

    public enum EMapLongBit
    {
        Test_5 = 5,
    }
    
    public enum EMapLongInt
    {
        Test_12 = 12,
    }
    
    public enum EMapLongLong
    {
        Test_3 = 3,
    }

    public enum EBitmap
    {
        Test_6 = 6,
    }

    public enum EBitmapDailyFirstRedDot
    {
        Unique = 8,
    }
}

/// 枚举扩展
public static class ClientConfigConvertSystemExtend
{
    public static RepeatedField<int> GetConverter(this ClientConfigConvertSystem.EListInt self)
    {
        if(!ClientConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<ListIntConverter>, ListIntConverter>((int)self, out ListIntConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ClientConfigConvertSystem.EListInt self) 
        => ((IClientConfigConverSystem)ClientConfigConvertSystem.Instance).Save<ConverterCollector<ListIntConverter>, ListIntConverter>((int)self);
    
    public static RepeatedField<long> GetConverter(this ClientConfigConvertSystem.EListLong self)
    {
        if(!ClientConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<ListLongConverter>, ListLongConverter>((int)self, out ListLongConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ClientConfigConvertSystem.EListLong self) 
        => ((IClientConfigConverSystem)ClientConfigConvertSystem.Instance).Save<ConverterCollector<ListLongConverter>, ListLongConverter>((int)self);
    
    public static Dictionary<int, bool> GetConverter(this ClientConfigConvertSystem.EMapIntBool self)
    {
        if(!ClientConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapIntBoolConverter>, MapIntBoolConverter>((int)self, out MapIntBoolConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ClientConfigConvertSystem.EMapIntBool self) 
        => ((IClientConfigConverSystem)ClientConfigConvertSystem.Instance).Save<ConverterCollector<MapIntBoolConverter>, MapIntBoolConverter>((int)self);
    
    public static MapIntBitConverter GetConverter(this ClientConfigConvertSystem.EMapIntBit self)
    {
        if(!ClientConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapIntBitConverter>, MapIntBitConverter>((int)self, out MapIntBitConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ClientConfigConvertSystem.EMapIntBit self) 
        => ((IClientConfigConverSystem)ClientConfigConvertSystem.Instance).Save<ConverterCollector<MapIntBitConverter>, MapIntBitConverter>((int)self);
    
    public static Dictionary<int, int> GetConverter(this ClientConfigConvertSystem.EMapIntInt self)
    {
        if(!ClientConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapIntIntConverter>, MapIntIntConverter>((int)self, out MapIntIntConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ClientConfigConvertSystem.EMapIntInt self) 
        => ((IClientConfigConverSystem)ClientConfigConvertSystem.Instance).Save<ConverterCollector<MapIntIntConverter>, MapIntIntConverter>((int)self);
    
    public static Dictionary<int, long> GetConverter(this ClientConfigConvertSystem.EMapIntLong self)
    {
        if(!ClientConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapIntLongConverter>, MapIntLongConverter>((int)self, out MapIntLongConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ClientConfigConvertSystem.EMapIntLong self) 
        => ((IClientConfigConverSystem)ClientConfigConvertSystem.Instance).Save<ConverterCollector<MapIntLongConverter>, MapIntLongConverter>((int)self);
    
    public static Dictionary<long, bool> GetConverter(this ClientConfigConvertSystem.EMapLongBool self)
    {
        if(!ClientConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapLongBoolConverter>, MapLongBoolConverter>((int)self, out MapLongBoolConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ClientConfigConvertSystem.EMapLongBool self) 
        => ((IClientConfigConverSystem)ClientConfigConvertSystem.Instance).Save<ConverterCollector<MapLongBoolConverter>, MapLongBoolConverter>((int)self);

    public static MapLongBitConverter GetConverter(this ClientConfigConvertSystem.EMapLongBit self)
    {
        if(!ClientConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapLongBitConverter>, MapLongBitConverter>((int)self, out MapLongBitConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ClientConfigConvertSystem.EMapLongBit self) 
        => ((IClientConfigConverSystem)ClientConfigConvertSystem.Instance).Save<ConverterCollector<MapLongBitConverter>, MapLongBitConverter>((int)self);
    
    public static Dictionary<long, int> GetConverter(this ClientConfigConvertSystem.EMapLongInt self)
    {
        if(!ClientConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapLongIntConverter>, MapLongIntConverter>((int)self, out MapLongIntConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ClientConfigConvertSystem.EMapLongInt self) 
        => ((IClientConfigConverSystem)ClientConfigConvertSystem.Instance).Save<ConverterCollector<MapLongIntConverter>, MapLongIntConverter>((int)self);

    public static Dictionary<long, long> GetConverter(this ClientConfigConvertSystem.EMapLongLong self)
    {
        if(!ClientConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapLongLongConverter>, MapLongLongConverter>((int)self, out MapLongLongConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ClientConfigConvertSystem.EMapLongLong self) 
        => ((IClientConfigConverSystem)ClientConfigConvertSystem.Instance).Save<ConverterCollector<MapLongLongConverter>, MapLongLongConverter>((int)self);
    
    public static BitmapConverter GetConverter(this ClientConfigConvertSystem.EBitmap self)
    {
        if(!ClientConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<BitmapConverter>, BitmapConverter>((int)self, out BitmapConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ClientConfigConvertSystem.EBitmap self) 
        => ((IClientConfigConverSystem)ClientConfigConvertSystem.Instance).Save<ConverterCollector<BitmapConverter>, BitmapConverter>((int)self);

    public static BitmapDailyFirstRedDotConverter GetConverter(this ClientConfigConvertSystem.EBitmapDailyFirstRedDot self)
    {
        if(!ClientConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<BitmapDailyFirstRedDotConverter>, BitmapDailyFirstRedDotConverter>((int)self, out BitmapDailyFirstRedDotConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ClientConfigConvertSystem.EBitmapDailyFirstRedDot self) 
        => ((IClientConfigConverSystem)ClientConfigConvertSystem.Instance).Save<ConverterCollector<BitmapDailyFirstRedDotConverter>, BitmapDailyFirstRedDotConverter>((int)self);
}