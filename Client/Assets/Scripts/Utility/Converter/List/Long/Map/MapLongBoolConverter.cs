using System.Collections.Generic;

namespace Converter.List.Long
{
    /// 键值对 key => long, value => bool
    /// 每 65 个数据 一个循环
    /// 第一个数据 为 Value 的存储空间 可存 64 个 bool
    /// 后 64 个数据 为 Key 的存储空间 可存 64 个 long
    /// 存储顺序 从低位开始
    public class MapLongBoolConverter<TList> : IConverter<TList>
        where TList : IList<long>, new()
    {
        private const int Loop = 65;
        private const int Size = sizeof(long) * 8;
        private Dictionary<long, bool> map;
        private TList source;

        void IConverter<TList>.Decode(TList source)
        {
            map?.Clear();
            if (source == null || source.Count == 0)
            {
                return;
            }
            map ??= new Dictionary<long, bool>();
            long value = 0;
            for (int i = 0; i < source.Count; i++)
            {
                int loopUnitIndex = i % Loop;
                if (loopUnitIndex == 0)
                {
                    value = source[i];
                }
                else
                {
                    bool flag = (value & (1L << (loopUnitIndex - 1))) != 0;
                    map.Add(source[i], flag);
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
            int index = 0;
            foreach (var pair in map)
            {
                int loopCount = index / Size;
                int keyUnitIndex = loopCount * Loop;
                int valueUintIndex = keyUnitIndex + 1 + index % (Loop - 1);
                for (int i = this.source.Count; i <= valueUintIndex; i++)
                {
                    this.source.Add(default);
                }
                this.source[valueUintIndex] |= pair.Key;
                if (pair.Value)
                {
                    this.source[keyUnitIndex] |= 1L << (index % Size);
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

        public static implicit operator Dictionary<long, bool>(MapLongBoolConverter<TList> converter)
        {
            converter.map ??= new Dictionary<long, bool>();
            return converter.map;
        }
    }
}