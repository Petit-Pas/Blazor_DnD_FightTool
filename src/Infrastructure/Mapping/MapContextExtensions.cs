using System.Linq.Expressions;
using Mapster;

namespace Mapping;

public static class MapContextExtensions
{
    public static TypeAdapterSetter<T, T> IgnoreWhenDuplicating<T>(this TypeAdapterSetter<T, T> typeAdapter, Expression<Func<T, object>> ignoredPropertyAccessor)
    {
        return typeAdapter
            .IgnoreIf((_, _) => MapContext.Current!.IsADuplication(), ignoredPropertyAccessor);
    }

    public static bool IsADuplication(this MapContext context)
    {
        if (context.Parameters.TryGetValue("IsADuplication", out var isADuplication))
        {
            if (isADuplication is bool ignore && ignore)
            {
                return true;
            }
        }
        return false;
    }
}
