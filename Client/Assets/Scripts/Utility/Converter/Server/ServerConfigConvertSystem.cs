using System;
using System.Collections.Generic;
using System.Text;
using Converter.List.Long;
using UnityEngine;

/// 服务端配置 的 转换系统
public class ServerConfigConvertSystem : ConverterCollectorSystem<RepeatedField<RepeatedField<long>>, RepeatedField<long>>, IServerConfigConverSystem
{
    private static ServerConfigConvertSystem _instance;
    public static ServerConfigConvertSystem Instance => _instance ??= new ServerConfigConvertSystem();

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
        [(int)EMapIntLong.Test_9] = typeof(EMapIntLong),
        [(int)EListInt.Test_10] = typeof(EListInt),
        [(int)EListLong.Test_11] = typeof(EListLong),
        [(int)EMapLongInt.Test_12] = typeof(EMapLongInt),
    };

    void IServerConfigConverSystem.Save<TConverterCollector, TConverter>(int typeValue)
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
}

/// 枚举扩展
public static class ServerConfigConvertSystemExtend
{
    public static RepeatedField<int> GetConverter(this ServerConfigConvertSystem.EListInt self)
    {
        if(!ServerConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<ListIntConverter>, ListIntConverter>((int)self, out ListIntConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ServerConfigConvertSystem.EListInt self) 
        => ((IServerConfigConverSystem)ServerConfigConvertSystem.Instance).Save<ConverterCollector<ListIntConverter>, ListIntConverter>((int)self);
    
    public static RepeatedField<long> GetConverter(this ServerConfigConvertSystem.EListLong self)
    {
        if(!ServerConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<ListLongConverter>, ListLongConverter>((int)self, out ListLongConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ServerConfigConvertSystem.EListLong self) 
        => ((IServerConfigConverSystem)ServerConfigConvertSystem.Instance).Save<ConverterCollector<ListLongConverter>, ListLongConverter>((int)self);
    
    public static Dictionary<int, bool> GetConverter(this ServerConfigConvertSystem.EMapIntBool self)
    {
        if(!ServerConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapIntBoolConverter>, MapIntBoolConverter>((int)self, out MapIntBoolConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ServerConfigConvertSystem.EMapIntBool self) 
        => ((IServerConfigConverSystem)ServerConfigConvertSystem.Instance).Save<ConverterCollector<MapIntBoolConverter>, MapIntBoolConverter>((int)self);
    
    public static MapIntBitConverter GetConverter(this ServerConfigConvertSystem.EMapIntBit self)
    {
        if(!ServerConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapIntBitConverter>, MapIntBitConverter>((int)self, out MapIntBitConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ServerConfigConvertSystem.EMapIntBit self) 
        => ((IServerConfigConverSystem)ServerConfigConvertSystem.Instance).Save<ConverterCollector<MapIntBitConverter>, MapIntBitConverter>((int)self);
    
    public static Dictionary<int, int> GetConverter(this ServerConfigConvertSystem.EMapIntInt self)
    {
        if(!ServerConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapIntIntConverter>, MapIntIntConverter>((int)self, out MapIntIntConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ServerConfigConvertSystem.EMapIntInt self) 
        => ((IServerConfigConverSystem)ServerConfigConvertSystem.Instance).Save<ConverterCollector<MapIntIntConverter>, MapIntIntConverter>((int)self);
    
    public static Dictionary<int, long> GetConverter(this ServerConfigConvertSystem.EMapIntLong self)
    {
        if(!ServerConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapIntLongConverter>, MapIntLongConverter>((int)self, out MapIntLongConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ServerConfigConvertSystem.EMapIntLong self) 
        => ((IServerConfigConverSystem)ServerConfigConvertSystem.Instance).Save<ConverterCollector<MapIntLongConverter>, MapIntLongConverter>((int)self);
    
    public static Dictionary<long, bool> GetConverter(this ServerConfigConvertSystem.EMapLongBool self)
    {
        if(!ServerConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapLongBoolConverter>, MapLongBoolConverter>((int)self, out MapLongBoolConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ServerConfigConvertSystem.EMapLongBool self) 
        => ((IServerConfigConverSystem)ServerConfigConvertSystem.Instance).Save<ConverterCollector<MapLongBoolConverter>, MapLongBoolConverter>((int)self);

    public static MapLongBitConverter GetConverter(this ServerConfigConvertSystem.EMapLongBit self)
    {
        if(!ServerConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapLongBitConverter>, MapLongBitConverter>((int)self, out MapLongBitConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ServerConfigConvertSystem.EMapLongBit self) 
        => ((IServerConfigConverSystem)ServerConfigConvertSystem.Instance).Save<ConverterCollector<MapLongBitConverter>, MapLongBitConverter>((int)self);
    
    public static Dictionary<long, int> GetConverter(this ServerConfigConvertSystem.EMapLongInt self)
    {
        if(!ServerConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapLongIntConverter>, MapLongIntConverter>((int)self, out MapLongIntConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ServerConfigConvertSystem.EMapLongInt self) 
        => ((IServerConfigConverSystem)ServerConfigConvertSystem.Instance).Save<ConverterCollector<MapLongIntConverter>, MapLongIntConverter>((int)self);

    public static Dictionary<long, long> GetConverter(this ServerConfigConvertSystem.EMapLongLong self)
    {
        if(!ServerConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<MapLongLongConverter>, MapLongLongConverter>((int)self, out MapLongLongConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ServerConfigConvertSystem.EMapLongLong self) 
        => ((IServerConfigConverSystem)ServerConfigConvertSystem.Instance).Save<ConverterCollector<MapLongLongConverter>, MapLongLongConverter>((int)self);
    
    public static BitmapConverter GetConverter(this ServerConfigConvertSystem.EBitmap self)
    {
        if(!ServerConfigConvertSystem.Instance.TryGetConverter<ConverterCollector<BitmapConverter>, BitmapConverter>((int)self, out BitmapConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    public static void Save(this ServerConfigConvertSystem.EBitmap self) 
        => ((IServerConfigConverSystem)ServerConfigConvertSystem.Instance).Save<ConverterCollector<BitmapConverter>, BitmapConverter>((int)self);
}