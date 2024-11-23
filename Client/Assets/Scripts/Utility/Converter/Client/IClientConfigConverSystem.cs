using Converter.List.Long;

public interface IClientConfigConverSystem
{
    void Save<TConverterCollector, TConverter>(int typeValue)
        where TConverterCollector : class, IConverterCollector<TConverter, RepeatedField<long>>
        where TConverter : class, IConverter<RepeatedField<long>>, new();
}
