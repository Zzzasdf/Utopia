using System;
using System.Collections.Generic;
using System.Text;
using Converter.List.Long;
using Google.Protobuf.Collections;
using DataSaver;

/// 客户端配置 的 转换系统
public class ClientConverterCollectorSystem : ConverterCollectorSystem<RepeatedField<RepeatedField<long>>, RepeatedField<long>>
{
    private static ClientConverterCollectorSystem _instance;
    public static ClientConverterCollectorSystem Instance => _instance ??= new ClientConverterCollectorSystem();

    /// 收集器类型
    protected override Dictionary<Type, IConverterCollector<RepeatedField<long>>> converterCollectors { get; } = new()
    {
        // Bitmap
        [typeof(EBitmap)] = new ConverterCollector<BitmapConverter>(),
        [typeof(EBitmapReverse)] = new ConverterCollector<BitmapReverseConverter>(),
        
        // List
        [typeof(EListInt)] = new ConverterCollector<ListIntConverter>(),
        [typeof(EListLong)] = new ConverterCollector<ListLongConverter>(),
        
        // HashSet
        [typeof(EHashSetInt)] = new ConverterCollector<HashSetIntConverter>(),
        [typeof(EHashSetIntReverse)] = new ConverterCollector<HashSetIntReverseConverter>(),
        [typeof(EHashSetLong)] = new ConverterCollector<HashSetLongConverter>(),
        [typeof(EHashSetLongReverse)] = new ConverterCollector<HashSetLongReverseConverter>(),
        
        // Map
        [typeof(EMapIntBit)] = new ConverterCollector<MapIntBitConverter>(),
        [typeof(EMapIntBitReverse)] = new ConverterCollector<MapIntBitReverseConverter>(),
        [typeof(EMapIntInt)] = new ConverterCollector<MapIntIntConverter>(),
        [typeof(EMapIntLong)] = new ConverterCollector<MapIntLongConverter>(),
        [typeof(EMapLongBit)] = new ConverterCollector<MapLongBitConverter>(),
        [typeof(EMapLongBitReverse)] = new ConverterCollector<MapLongBitReverseConverter>(),
        [typeof(EMapLongInt)] = new ConverterCollector<MapLongIntConverter>(),
        [typeof(EMapLongLong)] = new ConverterCollector<MapLongLongConverter>(),

        // Custom
        [typeof(EDailyFirstRedDot)] = new ConverterCollector<ClientDailyFirstRedDotConverter>(),
    };

    /// 配置表
    protected override Dictionary<int, Type> config { get; } = new()
    {
        [(int)EBitmap.Test_0] = typeof(EBitmap),
        [(int)EBitmapReverse.Test_1] = typeof(EBitmapReverse),
        [(int)EListInt.Test_2] = typeof(EListInt),
        [(int)EListLong.Test_3] = typeof(EListLong),
        [(int)EHashSetInt.Test_4] = typeof(EHashSetInt),
        [(int)EHashSetIntReverse.Test_5] = typeof(EHashSetIntReverse),
        [(int)EHashSetLong.Test_6] = typeof(EHashSetLong),
        [(int)EHashSetLongReverse.Test_7] = typeof(EHashSetLongReverse),
        [(int)EMapIntBit.Test_8] = typeof(EMapIntBit),
        [(int)EMapIntBitReverse.Test_9] = typeof(EMapIntBitReverse),
        [(int)EMapIntInt.Test_10] = typeof(EMapIntInt),
        [(int)EMapIntLong.Test_11] = typeof(EMapIntLong),
        [(int)EMapLongBit.Test_12] = typeof(EMapLongBit),
        [(int)EMapLongBitReverse.Test_13] = typeof(EMapLongBitReverse),
        [(int)EMapLongInt.Test_14] = typeof(EMapLongInt),
        [(int)EMapLongLong.Test_15] = typeof(EMapLongLong),
        [(int)EDailyFirstRedDot.Unique_16] = typeof(EDailyFirstRedDot),
    };

    public override void Save<TConverterCollector, TConverter>(int typeValue)
    {
        // Debug.LogError($"保存类型 => {typeValue}");
        // if (!TryEncode<TConverterCollector, TConverter>(typeValue, out RepeatedField<long> source))
        // {
        //     Debug.LogWarning($"没有数据");
        //     return;
        // }
        SaveAll();
    }

    public override void SaveAll()
    {
        // Debug.LogError("保存所有");
        if (!TryEncode(out RepeatedField<RepeatedField<long>> source))
        {
            BinaryDataSaver.TrySaveData<RepeatedField<RepeatedField<long>>>(BinaryDataSaver.EFile.ClientConverterData, null);
            return;
        }
        BinaryDataSaver.TrySaveData(BinaryDataSaver.EFile.ClientConverterData, source);
    }

    public override string ToString()
    {
        if (!TryEncode(out RepeatedField<RepeatedField<long>> source))
        {
            return string.Empty;
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
        return sb.ToString();
    }

    private enum UniqueRefCollector
    {
        Test_0 = 0,
        Test_1 = 1,
        Test_2 = 2,
        Test_3 = 3,
        Test_4 = 4,
        Test_5 = 5,
        Test_6 = 6,
        Test_7 = 7,
        Test_8 = 8,
        Test_9 = 9,
        Test_10 = 10,
        Test_11 = 11,
        Test_12 = 12,
        Test_13 = 13,
        Test_14 = 14,
        Test_15 = 15,
        Test_16 = 16,
    }

    #region Bitmap
    public enum EBitmap { Test_0 = UniqueRefCollector.Test_0 }
    public enum EBitmapReverse { Test_1 = UniqueRefCollector.Test_1, }
    #endregion

    #region List
    public enum EListInt { Test_2 = UniqueRefCollector.Test_2, }
    public enum EListLong { Test_3 = UniqueRefCollector.Test_3, }
    #endregion

    #region HashSet
    public enum EHashSetInt { Test_4 = UniqueRefCollector.Test_4, }
    public enum EHashSetIntReverse { Test_5 = UniqueRefCollector.Test_5, }
    public enum EHashSetLong { Test_6 = UniqueRefCollector.Test_6, }
    public enum EHashSetLongReverse { Test_7 = UniqueRefCollector.Test_7, }
    #endregion

    #region Map
    public enum EMapIntBit { Test_8 = UniqueRefCollector.Test_8, }
    public enum EMapIntBitReverse { Test_9 = UniqueRefCollector.Test_9, }
    public enum EMapIntInt { Test_10 = UniqueRefCollector.Test_10, }
    public enum EMapIntLong { Test_11 = UniqueRefCollector.Test_11, }
    public enum EMapLongBit { Test_12 = UniqueRefCollector.Test_12, }
    public enum EMapLongBitReverse { Test_13 = UniqueRefCollector.Test_13, }
    public enum EMapLongInt { Test_14 = UniqueRefCollector.Test_14, }
    public enum EMapLongLong { Test_15 = UniqueRefCollector.Test_15, }
    #endregion

    #region Custom
    public enum EDailyFirstRedDot { Unique_16 = UniqueRefCollector.Test_16, }
    #endregion
}

/// 枚举扩展
public static class ClientConfigConvertSystemExtend
{
    private static TConverter GetConverter<TCollector, TConverter>(int self)
        where TCollector: ConverterCollector<TConverter, RepeatedField<long>>
        where TConverter: class, IConverter<RepeatedField<long>>, new()
    {
        if(!ClientConverterCollectorSystem.Instance.TryGetConverter<TCollector, TConverter>(self, out TConverter converter))
        {
            throw new Exception("配置有问题，请检查");
        }
        return converter;
    }
    private static void Save<TCollector, TConverter>(int self) 
        where TCollector: ConverterCollector<TConverter, RepeatedField<long>>
        where TConverter: class, IConverter<RepeatedField<long>>, new()
        => ClientConverterCollectorSystem.Instance.Save<TCollector, TConverter>(self);
    
#region Bitmap
    public static BitmapConverter GetConverter(this ClientConverterCollectorSystem.EBitmap self) => GetConverter<ConverterCollector<BitmapConverter>, BitmapConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EBitmap self) => Save<ConverterCollector<BitmapConverter>, BitmapConverter>((int)self);

    public static BitmapReverseConverter GetConverter(this ClientConverterCollectorSystem.EBitmapReverse self) => GetConverter<ConverterCollector<BitmapReverseConverter>, BitmapReverseConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EBitmapReverse self) => Save<ConverterCollector<BitmapReverseConverter>, BitmapReverseConverter>((int)self);
#endregion

#region List
    public static RepeatedField<int> GetConverter(this ClientConverterCollectorSystem.EListInt self) => GetConverter<ConverterCollector<ListIntConverter>, ListIntConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EListInt self) => Save<ConverterCollector<ListIntConverter>, ListIntConverter>((int)self);

    public static RepeatedField<long> GetConverter(this ClientConverterCollectorSystem.EListLong self) => GetConverter<ConverterCollector<ListLongConverter>, ListLongConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EListLong self) => Save<ConverterCollector<ListLongConverter>, ListLongConverter>((int)self);
#endregion

#region HashSet
    public static HashSet<int> GetConverter(this ClientConverterCollectorSystem.EHashSetInt self) => GetConverter<ConverterCollector<HashSetIntConverter>, HashSetIntConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EHashSetInt self) => Save<ConverterCollector<HashSetIntConverter>, HashSetIntConverter>((int)self);
    
    public static HashSetIntReverseConverter GetConverter(this ClientConverterCollectorSystem.EHashSetIntReverse self) => GetConverter<ConverterCollector<HashSetIntReverseConverter>, HashSetIntReverseConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EHashSetIntReverse self) => Save<ConverterCollector<HashSetIntReverseConverter>, HashSetIntReverseConverter>((int)self);
    
    public static HashSetLongConverter GetConverter(this ClientConverterCollectorSystem.EHashSetLong self) => GetConverter<ConverterCollector<HashSetLongConverter>, HashSetLongConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EHashSetLong self) => Save<ConverterCollector<HashSetLongConverter>, HashSetLongConverter>((int)self);

    public static HashSetLongReverseConverter GetConverter(this ClientConverterCollectorSystem.EHashSetLongReverse self) => GetConverter<ConverterCollector<HashSetLongReverseConverter>, HashSetLongReverseConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EHashSetLongReverse self) => Save<ConverterCollector<HashSetLongReverseConverter>, HashSetLongReverseConverter>((int)self);
#endregion

#region Map
    public static MapIntBitConverter GetConverter(this ClientConverterCollectorSystem.EMapIntBit self) => GetConverter<ConverterCollector<MapIntBitConverter>, MapIntBitConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EMapIntBit self) => Save<ConverterCollector<MapIntBitConverter>, MapIntBitConverter>((int)self);
    
    public static MapIntBitReverseConverter GetConverter(this ClientConverterCollectorSystem.EMapIntBitReverse self) => GetConverter<ConverterCollector<MapIntBitReverseConverter>, MapIntBitReverseConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EMapIntBitReverse self) => Save<ConverterCollector<MapIntBitReverseConverter>, MapIntBitReverseConverter>((int)self);

    public static Dictionary<int, int> GetConverter(this ClientConverterCollectorSystem.EMapIntInt self) => GetConverter<ConverterCollector<MapIntIntConverter>, MapIntIntConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EMapIntInt self) => Save<ConverterCollector<MapIntIntConverter>, MapIntIntConverter>((int)self);

    public static Dictionary<int, long> GetConverter(this ClientConverterCollectorSystem.EMapIntLong self) => GetConverter<ConverterCollector<MapIntLongConverter>, MapIntLongConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EMapIntLong self) => Save<ConverterCollector<MapIntLongConverter>, MapIntLongConverter>((int)self);

    public static MapLongBitConverter GetConverter(this ClientConverterCollectorSystem.EMapLongBit self) => GetConverter<ConverterCollector<MapLongBitConverter>, MapLongBitConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EMapLongBit self) => Save<ConverterCollector<MapLongBitConverter>, MapLongBitConverter>((int)self);
    
    public static MapLongBitReverseConverter GetConverter(this ClientConverterCollectorSystem.EMapLongBitReverse self) => GetConverter<ConverterCollector<MapLongBitReverseConverter>, MapLongBitReverseConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EMapLongBitReverse self) => Save<ConverterCollector<MapLongBitReverseConverter>, MapLongBitReverseConverter>((int)self);

    public static Dictionary<long, int> GetConverter(this ClientConverterCollectorSystem.EMapLongInt self) => GetConverter<ConverterCollector<MapLongIntConverter>, MapLongIntConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EMapLongInt self) => Save<ConverterCollector<MapLongIntConverter>, MapLongIntConverter>((int)self);

    public static Dictionary<long, long> GetConverter(this ClientConverterCollectorSystem.EMapLongLong self) => GetConverter<ConverterCollector<MapLongLongConverter>, MapLongLongConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EMapLongLong self) => Save<ConverterCollector<MapLongLongConverter>, MapLongLongConverter>((int)self);
#endregion

#region Custom
    public static ClientDailyFirstRedDotConverter GetConverter(this ClientConverterCollectorSystem.EDailyFirstRedDot self) => GetConverter<ConverterCollector<ClientDailyFirstRedDotConverter>, ClientDailyFirstRedDotConverter>((int)self);
    public static void Save(this ClientConverterCollectorSystem.EDailyFirstRedDot self) => Save<ConverterCollector<ClientDailyFirstRedDotConverter>, ClientDailyFirstRedDotConverter>((int)self);
#endregion
}