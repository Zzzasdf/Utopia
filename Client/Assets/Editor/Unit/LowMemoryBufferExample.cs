using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using UnityEngine;

[TestFixture]
public class LowMemoryBuffer
{
    [Test]
    public void Foo()
    {
        LowMemoryBufferSystem.Instance.AddSource(new List<List<long>>
        {
            new List<long>
            {
                4294967300, // 1, 4
                17179869193, // 4, 9
                8, // 0, 8
                8589934592, // 2, 0
                30064771074, // 7, 2
            },
            new List<long>
            {
                4294967297, // 1, true
                8589934592, // 2, false
                12884901889 // 3, true
            },
            new List<long>
            {
                4294967302, // 1, 6, (1 << 1) | (1 << 2)
                8589934616, // 2, 24, (1 << 3) | (1 << 4)
            },
            new List<long>
            {
                1,4,
                4,9,
                0,8,
                2,0,
                7,2
            },
            new List<long>
            {
                4,0, // 4, false
                5,1, // 5, true
                6,0, // 6, false
            },
            new List<long>
            {
                1, 6, // 1, (1 << 1) | (1 << 2)
                2, 24, // 2, (1 << 3) | (1 << 4)
            },
            new List<long> // 1, 3, 64 
            {
                10,
                1,
            }
        });
        Dictionary<int, int> int2int = EKIntVInt.Test_0.GetBuffer();
        // int2int.Add(1, 4);
        // int2int.Add(4, 9);
        // int2int.Add(0, 8);
        // int2int.Add(2, 0);
        // int2int.Add(7, 2);
        Dictionary<int, bool> int2bool = EKIntVBool.Test_1.GetBuffer();
        // int2bool.Add(1, true);
        // int2bool.Add(2, false);
        // int2bool.Add(3, true);
        KIntVBitLowMemoryBuffer int2bit = EKIntVBit.Test_2.GetBuffer(); 
        // int2bit.AddFlagValue(1, 1 << 1);
        // int2bit.AddFlagValue(1, 1 << 2);
        // int2bit.AddFlagValue(2, 1 << 3);
        // int2bit.AddFlagValue(2, 1 << 4);
        Dictionary<long, long> long2long = EKLongVLong.Test_3.GetBuffer();
        // long2long.Add(1, 4);
        // long2long.Add(4, 9);
        // long2long.Add(0, 8);
        // long2long.Add(2, 0);
        // long2long.Add(7, 2);
        Dictionary<long, bool> long2bool = EKLongVBool.Test_4.GetBuffer();
        // long2bool.Add(4, false);
        // long2bool.Add(5, true);
        // long2bool.Add(6, false);
        KLongVBitLowMemoryBuffer long2bit = EKLongVBit.Test_5.GetBuffer();
        // long2bit.AddFlagValue(1, 1 << 1);
        // long2bit.AddFlagValue(1, 1 << 2);
        // long2bit.AddFlagValue(2, 1 << 3);
        // long2bit.AddFlagValue(2, 1 << 4);
        BitmapLowMemoryBuffer bitmap = EBitmap.Test_6.GetBuffer();
        // bitmap.AddBit(3);
        // bitmap.AddBit(1);
        // bitmap.AddBit(64);
        LowMemoryBufferSystem.Instance.SaveAll(); 
    }
}
