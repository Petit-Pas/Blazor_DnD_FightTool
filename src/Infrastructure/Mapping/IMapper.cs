﻿namespace Mapping;

public interface IMapper
{
    TTarget Map<TSource, TTarget>(TSource source, params ValueTuple<string, object>[] runtimeParameters)
        where TSource : class
        where TTarget : class;

    /// <summary>
    ///     Specific version of Map that creates a copy of the source object, while setting IsADuplication to true.
    ///     This allows for ignoring properties when duplicating objects such as internal ids for instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="runtimeParameters"></param>
    /// <returns></returns>
    T Clone<T>(T source, params ValueTuple<string, object>[] runtimeParameters)
            where T : class;

    /// <summary>
    ///     Specific version of Map that creates a perfect copy of the source object.
    ///     So Ids are also duplicated, nothing except the reference can be used to identify the 2 objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="runtimeParameters"></param>
    /// <returns></returns>
    T Copy<T>(T source, params ValueTuple<string, object>[] runtimeParameters)
            where T : class;

    TTarget Map<TSource, TTarget>(TSource source)
        where TSource : Enum
        where TTarget : Enum;
}
