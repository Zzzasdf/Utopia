using Converter.List.Long;

public class ConverterCollector<TConverter> : ConverterCollector<TConverter, RepeatedField<long>>
    where TConverter:  class, IConverter<RepeatedField<long>>, new()
{
    
}
