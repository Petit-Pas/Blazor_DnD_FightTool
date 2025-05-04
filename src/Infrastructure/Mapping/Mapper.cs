using Mapster;

namespace Mapping;

public class Mapper : IMapper
{
    public TTarget Map<TSource, TTarget>(TSource source)
            where TSource : Enum
            where TTarget : Enum
    {
        var target = source.Adapt<TTarget>();
        if (!Enum.IsDefined(typeof(TTarget), target))
        {
            throw new ArgumentException($"The value {source} does not exist in the enum {typeof(TTarget).FullName}");
        }
        return target;
    }

    public TTarget Map<TSource, TTarget>(TSource source, params ValueTuple<string, object>[] runtimeParameters)
            where TSource : class
            where TTarget : class
    {
        // When using the MapContext to set properties within the mapping configuration, we need it to exist.
        // By default, it is only set when we have additional parameters, this ensures that the context is always set.
        using var scope = new MapContextScope();

        var adapter = source.BuildAdapter();
        foreach (var (parameterName, parameterValue) in runtimeParameters)
        {
            adapter.AddParameters(parameterName, parameterValue);
        }

        return adapter.AdaptToType<TTarget>();
    }

    public T Clone<T>(T source, params ValueTuple<string, object>[] runtimeParameters)
            where T : class
    {
        return Map<T, T>(source, [.. runtimeParameters, ("IsADuplication", true)]);
    }

    public T Copy<T>(T source, params ValueTuple<string, object>[] runtimeParameters)
            where T : class
    {
        return Map<T, T>(source);
    }
}
