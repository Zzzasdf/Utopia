using System;
using Unity.Collections.LowLevel.Unsafe;

namespace EnumModule
{
    public static class Utility
    {
        // 辅助方法：将枚举转换为整数
        public static int ToInt<T>(T tEnum) where T : Enum
        {
            return UnsafeUtility.As<T, int>(ref tEnum);
        }

        // 辅助方法：将整数转换为枚举
        public static T ToEnum<T>(int value) where T : Enum
        {
            return UnsafeUtility.As<int, T>(ref value);
        }
    }
    public struct EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3>
        where TEnumFlags1 : struct, Enum
        where TEnumFlags2 : struct, Enum
        where TEnumFlags3 : struct, Enum
    {
        private EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> group;
        private TEnumFlags3 tFlags3;
        
        public EnumIntFlagsGroup(TEnumFlags1 initialFlags1 = default, TEnumFlags2 initialFlags2 = default, TEnumFlags3 initialFlags3 = default)
            : this(new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2>(initialFlags1, initialFlags2), initialFlags3)
        {
        }
        public EnumIntFlagsGroup(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> initialGroup = default, TEnumFlags3 initialFlags3 = default)
        {
            group = initialGroup;
            tFlags3 = initialFlags3;
        }

        public override string ToString()
        {
            return $"{group.ToString()}, {typeof(TEnumFlags3).Name}: {tFlags3}";
        }
        
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> operator ~(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> group)
        {
            int flagsValue3 = Utility.ToInt(group.tFlags3);
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3>(~group.group, Utility.ToEnum<TEnumFlags3>(~flagsValue3));
        }

#region Flags1
        public bool HasFlag(TEnumFlags1 tFlag)
        {
            return group.HasFlag(tFlag);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> operator |(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> group, TEnumFlags1 flag)
        {
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3>(group.group | flag, group.tFlags3);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> operator &(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> group, TEnumFlags1 flag)
        {
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3>(group.group & flag, default);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> operator ^(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> group, TEnumFlags1 flag)
        {
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3>(group.group ^ flag, group.tFlags3);
        }
#endregion

#region Flag2
        public bool HasFlag(TEnumFlags2 tFlag)
        {
            return group.HasFlag(tFlag);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> operator |(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> group, TEnumFlags2 flag)
        {
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3>(group.group | flag, group.tFlags3);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> operator &(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> group, TEnumFlags2 flag)
        {
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3>(group.group & flag, default);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> operator ^(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> group, TEnumFlags2 flag)
        {
            int flagsValue3 = Utility.ToInt(group.tFlags3);
            TEnumFlags3 tEnumFlag3 = Utility.ToEnum<TEnumFlags3>(flagsValue3 ^ 0);
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3>(group.group ^ flag, tEnumFlag3);
        }
#endregion

#region Flag3
        public bool HasFlag(TEnumFlags3 tFlag)
        {
            return tFlags3.HasFlag(tFlag);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> operator |(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> group, TEnumFlags3 flag)
        {
            int value = Utility.ToInt(flag);
            int flagsValue3 = Utility.ToInt(group.tFlags3);
            TEnumFlags3 tEnumFlags3 = Utility.ToEnum<TEnumFlags3>(flagsValue3 | value);
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3>(group.group, tEnumFlags3);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> operator &(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> group, TEnumFlags3 flag)
        {
            int value = Utility.ToInt(flag);
            int flagsValue3 = Utility.ToInt(group.tFlags3);
            TEnumFlags3 tEnumFlags3 = Utility.ToEnum<TEnumFlags3>(flagsValue3 & value);
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3>(default, tEnumFlags3);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> operator ^(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3> group, TEnumFlags3 flag)
        {
            int value = Utility.ToInt(flag);
            int flagsValue3 = Utility.ToInt(group.tFlags3);
            TEnumFlags3 tEnumFlag3 = Utility.ToEnum<TEnumFlags3>(flagsValue3 ^ value);
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2, TEnumFlags3>(group.group ^ default(TEnumFlags1), tEnumFlag3);
        }
#endregion
    }

    public struct EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2>
        where TEnumFlags1 : Enum
        where TEnumFlags2 : Enum
    {
        private TEnumFlags1 tFlags1;
        private TEnumFlags2 tFlags2;

        public EnumIntFlagsGroup(TEnumFlags1 initialFlags1 = default, TEnumFlags2 initialFlags2 = default)
        {
            tFlags1 = initialFlags1;
            tFlags2 = initialFlags2;
        }
        
        public override string ToString()
        {
            return $"{typeof(TEnumFlags1).Name}: {tFlags1}, {typeof(TEnumFlags2).Name}: {tFlags2}";
        }
        
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> operator ~(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> group)
        {
            int flagsValue1 = Utility.ToInt(group.tFlags1);
            int flagsValue2 = Utility.ToInt(group.tFlags2);
            TEnumFlags1 tEnumFlag1 = Utility.ToEnum<TEnumFlags1>(~flagsValue1);
            TEnumFlags2 tEnumFlag2 = Utility.ToEnum<TEnumFlags2>(~flagsValue2);
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2>(tEnumFlag1, tEnumFlag2);
        }
        
#region Flags1
        public bool HasFlag(TEnumFlags1 tFlag)
        {
            return tFlags1.HasFlag(tFlag);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> operator |(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> group, TEnumFlags1 flag)
        {
            int value = Utility.ToInt(flag);
            int flagsValue1 = Utility.ToInt(group.tFlags1);
            TEnumFlags1 tEnumFlags1 = Utility.ToEnum<TEnumFlags1>(flagsValue1 | value);
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2>(tEnumFlags1, group.tFlags2);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> operator &(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> group, TEnumFlags1 flag)
        {
            int value = Utility.ToInt(flag);
            int flagsValue1 = Utility.ToInt(group.tFlags1);
            TEnumFlags1 tEnumFlags1 = Utility.ToEnum<TEnumFlags1>(flagsValue1 & value);
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2>(tEnumFlags1, default);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> operator ^(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> group, TEnumFlags1 flag)
        {
            int value = Utility.ToInt(flag);
            int flagsValue1 = Utility.ToInt(group.tFlags1);
            int flagsValue2 = Utility.ToInt(group.tFlags2);
            TEnumFlags1 tEnumFlags1 = Utility.ToEnum<TEnumFlags1>(flagsValue1 ^ value);
            TEnumFlags2 tEnumFlags2 = Utility.ToEnum<TEnumFlags2>(flagsValue2 ^ 0);
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2>(tEnumFlags1, tEnumFlags2);
        }
#endregion

#region Flags2
        public bool HasFlag(TEnumFlags2 tFlag)
        {
            return tFlags2.HasFlag(tFlag);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> operator |(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> group, TEnumFlags2 flag)
        {
            int value = Utility.ToInt(flag);
            int flagsValue2 = Utility.ToInt(group.tFlags2);
            TEnumFlags2 tEnumFlags2 = Utility.ToEnum<TEnumFlags2>(flagsValue2 | value);
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2>(group.tFlags1, tEnumFlags2);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> operator &(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> group, TEnumFlags2 flag)
        {
            int value = Utility.ToInt(flag);
            int flagsValue2 = Utility.ToInt(group.tFlags2);
            TEnumFlags2 tEnumFlags2 = Utility.ToEnum<TEnumFlags2>(flagsValue2 & value);
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2>(default, tEnumFlags2);
        }
        public static EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> operator ^(EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2> group, TEnumFlags2 flag)
        {
            int value = Utility.ToInt(flag);
            int flagsValue1 = Utility.ToInt(group.tFlags1);
            int flagsValue2 = Utility.ToInt(group.tFlags2);
            TEnumFlags1 tEnumFlags1 = Utility.ToEnum<TEnumFlags1>(flagsValue1 ^ 0);
            TEnumFlags2 tEnumFlags2 = Utility.ToEnum<TEnumFlags2>(flagsValue2 ^ value);
            return new EnumIntFlagsGroup<TEnumFlags1, TEnumFlags2>(tEnumFlags1, tEnumFlags2);
        }
#endregion
    }
}