using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class ConverterExample
{
    [Test]
    public void Foo()
    {
        ClientConfigConvertSystem.Instance.AddSource(new RepeatedField<RepeatedField<long>>
        {
            new RepeatedField<long> // 0
            {
                4294967300, // 1L << 32 | 4
                17179869193, // 4L << 32 | 9
                8, // 0L << 32 | 8
                8589934592, // 2L << 32 | 0
                30064771074, // 7L << 32 | 2
            },
            new RepeatedField<long> // 1
            {
                1, // 最后一个高位是否有效
                21, // 1 << 4 | 1 << 2 | 1 << 0
                4294967296, // 1L << 32 | 0
                -4294967294, // -1L << 32 | 2 eg! 负数表示最高位有值
                -8589934594, // -3L << 32 | -2 
            },
            new RepeatedField<long> // 2
            {
                4294967302, // 1L << 32 | 6 ((1 << 1) | (1 << 2))
                8589934616, // 2L << 32 | 24 ((1 << 3) | (1 << 4))
            },
            new RepeatedField<long> // 3
            {
                1,
                4,
                4,
                9,
                0,
                8,
                2,
                0,
                7,
                2
            },
            new RepeatedField<long> // 4
            {
                2, // false, true, false
                4, 
                5,
                6,
            },
            new RepeatedField<long> // 5
            {
                1,
                6, // 1 << 1 | 1 << 2
                2,
                24, // 1 << 3 | 1 << 4
            },
            new RepeatedField<long> // 6
            {
                10, // 1 << 1 | 1 << 3
                1, // 1L << 64
            },
            null,   // 7
            new RepeatedField<long> // 8
            {
                1732383398663, // 当前时间 
                3, // 1 << 0 | 1 << 1
                68719476736, // 1L << 100
                1, // 1L << 128
            },
            new RepeatedField<long> // 9
            {
                8589934593, // 2L << 32 | 1
                1,
                2,
                3, 
                3,
            },
            new RepeatedField<long> // 10
            {
                0,
                4294967297, // 1L << 32 | 1
                4294967298, // 1L << 32 | 2
                3,
            },
            new RepeatedField<long> // 11
            {
                10001,
                10002,
            },
            new RepeatedField<long> // 12
            {
                8589934593, // 2L << 32 | 1
                1,
                2,
                3,
                3,
            }
        });
        Dictionary<int, int> mapIntInt = ClientConfigConvertSystem.EMapIntInt.Test_0.GetConverter();
        // mapIntInt.Add(1, 4);
        // mapIntInt.Add(4, 9);
        // mapIntInt.Add(0, 8);
        // mapIntInt.Add(2, 0);
        // mapIntInt.Add(7, 2);
        // ClientConfigConvertSystem.EMapIntInt.Test_0.Save();
        Dictionary<int, bool> mapIntBool = ClientConfigConvertSystem.EMapIntBool.Test_1.GetConverter();
        // mapIntBool.Add(0, true);
        // mapIntBool.Add(1, false);
        // mapIntBool.Add(2, true);
        // mapIntBool.Add(-1, false);
        // mapIntBool.Add(-2, true);
        // mapIntBool.Add(-3, false);
        // ClientConfigConvertSystem.EMapIntBool.Test_1.Save();
        MapIntBitConverter mapIntBit = ClientConfigConvertSystem.EMapIntBit.Test_2.GetConverter();
        // mapIntBit.AddFlag(1, 1);
        // mapIntBit.AddFlag(1, 2);
        // mapIntBit.AddFlag(2, 3);
        // mapIntBit.AddFlag(2, 4);
        // ClientConfigConvertSystem.EMapIntBit.Test_2.Save();
        Dictionary<long, long> mapLongLong = ClientConfigConvertSystem.EMapLongLong.Test_3.GetConverter();
        // mapLongLong.Add(1, 4);
        // mapLongLong.Add(4, 9);
        // mapLongLong.Add(0, 8);
        // mapLongLong.Add(2, 0);
        // mapLongLong.Add(7, 2);
        // ClientConfigConvertSystem.EMapLongLong.Test_3.Save();
        Dictionary<long, bool> mapLongBool = ClientConfigConvertSystem.EMapLongBool.Test_4.GetConverter();
        // mapLongBool.Add(4, false);
        // mapLongBool.Add(5, true);
        // mapLongBool.Add(6, false);
        // ClientConfigConvertSystem.EMapLongBool.Test_4.Save();
        MapLongBitConverter long2bit = ClientConfigConvertSystem.EMapLongBit.Test_5.GetConverter();
        // long2bit.AddFlag(1, 1);
        // long2bit.AddFlag(1, 2);
        // long2bit.AddFlag(2, 3);
        // long2bit.AddFlag(2, 4);
        // ClientConfigConvertSystem.EMapLongBit.Test_5.Save();
        BitmapConverter bitmap = ClientConfigConvertSystem.EBitmap.Test_6.GetConverter();
        // bitmap.AddFlag(3);
        // bitmap.AddFlag(1);
        // bitmap.AddFlag(64);
        // ClientConfigConvertSystem.EBitmap.Test_6.Save();
        BitmapDailyFirstRedDotConverter bitmapDailyFirstRedDot = ClientConfigConvertSystem.EBitmapDailyFirstRedDot.Unique.GetConverter();
        // bitmapDailyFirstRedDot.AddFlag(EDailyFirstRedDot.Test0);
        // bitmapDailyFirstRedDot.AddFlag(EDailyFirstRedDot.Test1);
        // bitmapDailyFirstRedDot.AddFlag(EDailyFirstRedDot.Test100);
        // bitmapDailyFirstRedDot.AddFlag(EDailyFirstRedDot.Test128);
        // ClientConfigConvertSystem.EBitmapDailyFirstRedDot.Unique.Save();
        Dictionary<int, long> mapIntLong = ClientConfigConvertSystem.EMapIntLong.Test_9.GetConverter();
        // mapIntLong.Add(1, 1);
        // mapIntLong.Add(2, 2);
        // mapIntLong.Add(3, 3);
        // ClientConfigConvertSystem.EMapIntLong.Test_9.Save();
        RepeatedField<int> listInt = ClientConfigConvertSystem.EListInt.Test_10.GetConverter();
        // listInt.Add(1);
        // listInt.Add(1);
        // listInt.Add(2);
        // listInt.Add(1);
        // listInt.Add(3);
        // ClientConfigConvertSystem.EListInt.Test_10.Save();
        RepeatedField<long> listLong = ClientConfigConvertSystem.EListLong.Test_11.GetConverter();
        // listLong.Add(10001);
        // listLong.Add(10002);
        // ClientConfigConvertSystem.EListLong.Test_11.Save();
        Dictionary<long, int> mapLongInt = ClientConfigConvertSystem.EMapLongInt.Test_12.GetConverter();
        // mapLongInt.Add(1, 1);
        // mapLongInt.Add(2, 2);
        // mapLongInt.Add(3, 3);
        // ClientConfigConvertSystem.EMapLongInt.Test_12.Save();
        ClientConfigConvertSystem.Instance.SaveAll();
    }
}
