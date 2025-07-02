using System;
using EnumModule;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class EnumModuleUnit
{
    private static EnumIntFlagsGroup<ETest1, ETest2, ETest3> group;
    [Test]
    public static void Foo()
    {
        Debug.Log(group.ToString());

        group |= ETest1.A;
        group |= ETest1.B;
        group |= ETest1.C;
        Debug.Log(group.ToString());
        
        group |= ETest2.B; 
        Debug.Log(group.ToString());
        
        group |= ETest3.C;
        Debug.Log(group.ToString());

        Debug.Log(group.HasFlag(ETest1.A));
        
        
        Debug.Log(group.HasFlag(ETest2.B));
        
        
        Debug.Log(group.HasFlag(ETest3.C));

        group ^= ETest2.B;
        Debug.Log(group.ToString());

        group = ~group;
        Debug.Log(group.ToString());
        
        group &= ETest1.C;
        Debug.Log(group.ToString());
        
        group |= ETest1.C;
        Debug.Log(group.ToString());
        
        group |= ETest1.B;
        Debug.Log(group.ToString());
        
        group |= ETest2.B;
        Debug.Log(group.ToString());
        
        group &= ETest1.C;
        Debug.Log(group.ToString());
    }
    
    [Flags]
    public enum ETest1: int
    {
        A = 1 << 0,
        B = 1 << 1,
        C = 1 << 2,
    }
    [Flags]
    public enum ETest2: int
    {
        B = 1 << 0,
    }
    [Flags]
    public enum ETest3: int
    {
        C = 1 << 1,
    }
}
