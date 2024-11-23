using System.Collections.Generic;

namespace Converter.List.Long
{
    /// 键值对 key => int, value => bool
    /// 第一个 long 只存 0 键 是否有效
    /// 每 33 个数据 一个循环
    /// 第一个数据 为 Value 的存储空间 可存 64 个 bool
    /// 后 32 个数据 为 Key 的存储空间 可存 64 个 int (32 个 long)
    /// 存储顺序 从低位开始
    public class MapIntBoolConverter<TList> : IConverter<TList>
        where TList : IList<long>, new()
    {
        private const int Loop = 33;
        private const int Size = sizeof(long) * 8;
        private Dictionary<int, bool> map;
        private TList source;

        void IConverter<TList>.Decode(TList source)
        {
            map?.Clear();
            if (source == null || source.Count < 3)
            {
                return;
            }
            map ??= new Dictionary<int, bool>();
            long value = 0;
            bool validZeroKey = source[0] == 1;
            for (int i = 1; i < source.Count; i++)
            {
                int index = i - 1;
                int loopUnitIndex = index % Loop;
                if (loopUnitIndex == 0)
                {
                    value = source[i];
                }
                else
                {
                    long l = source[i];
                    (int high, int low) = LongConvertDoubleInt(l);
                    bool lowFlag = (value & (1 << ((loopUnitIndex - 1) * 2))) != 0;
                    if (low != 0 || validZeroKey)
                    {
                        map.TryAdd(low, lowFlag);
                    }
                    bool highFlag = (value & (1 << ((loopUnitIndex - 1) * 2 + 1))) != 0;
                    if (high != 0 || validZeroKey)
                    {
                        map.TryAdd(high, highFlag);
                    }
                }
            }
        }

        bool IConverter<TList>.TryEncode(out TList source)
        {
            if (map == null || map.Count == 0)
            {
                source = default;
                return false;
            }
            this.source ??= new TList();
            this.source.Clear();
            this.source.Add(map.ContainsKey(0) ? 1 : 0);
            int index = 0;
            foreach (var pair in map)
            {
                int loopCount = index / Size;
                int keyUnitIndex = loopCount * Loop + 1; // 从 1 开始实际储存数据
                int valueUintIndex = keyUnitIndex + 1 + index / 2 % (Loop - 1);
                for (int i = this.source.Count; i <= valueUintIndex; i++)
                {
                    this.source.Add(default);
                }
                bool isLow = ((index % Size + 1) & 1) != 0;
                if (!isLow)
                {
                    this.source[valueUintIndex] |= HighIntConvertLong(pair.Key);
                }
                else
                {
                    this.source[valueUintIndex] |= (uint)pair.Key;
                }
                if (pair.Value)
                {
                    this.source[keyUnitIndex] |= (uint)(1 << (index % Size));
                }
                index++;
            }
            source = this.source;
            return true;
        }

        void IConverter<TList>.Clear()
        {
            map?.Clear();
        }

        public static implicit operator Dictionary<int, bool>(MapIntBoolConverter<TList> converter)
        {
            converter.map ??= new Dictionary<int, bool>();
            return converter.map;
        }
        
        private (int high, int low) LongConvertDoubleInt(long l)
        {
            return ((int)(l >> 32), (int)(l & 0xFFFFFFFF));
        }

        private long HighIntConvertLong(int high)
        {
            return (long)high << 32;
        }
    }
}