using System.Collections.Generic;

namespace Converter.List.Long
{
    public interface IConverterCollectorSystem<TListList, TList>
        where TListList: IList<TList>, new()
        where TList: IList<long>, new()
    {
        /// 添加源数据
        void AddSource(TListList source);
        
        /// 编码字典
        bool TryEncodeMap<TConverterCollector, TConverter>(out Dictionary<int, TList> sourceMap) 
            where TConverterCollector : class, IConverterCollector<TConverter, TList>
            where TConverter : class, IConverter<TList>, new();

        /// 编码
        bool TryEncode<TConverterCollector, TConverter>(int typeValue, out TList source) 
            where TConverterCollector : class, IConverterCollector<TConverter, TList>
            where TConverter : class, IConverter<TList>, new();

        /// 编码
        bool TryEncode(out TListList source);
        
        /// 获取转换器
        bool TryGetConverter<TConverterCollector, TConverter>(int typeValue, out TConverter converter) 
            where TConverterCollector : class, IConverterCollector<TConverter, TList>
            where TConverter : class, IConverter<TList>, new();
    }
}
