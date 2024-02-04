namespace Extensions;

/// <summary>
///     Extensions of <see cref="List{T}" />
/// </summary>
public static class ListExtensions
{
    /// <summary>
    ///     Remove all elements of a given type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="typeToRemove"></param>
    public static void RemoveAll<T>(this List<T> list, Type typeToRemove)
    {
        list.RemoveAll(x => x.GetType() == typeToRemove);
    }

    /// <summary>
    ///     Enumerate given list except for elements of a given type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="elements"></param>
    /// <param name="typeToRemove"></param>
    /// <returns></returns>
    public static IEnumerable<T> WithoutAll<T>(this IEnumerable<T> elements, Type typeToRemove)
    {
        return elements.Where(x => x.GetType() != typeToRemove);
    }

    /// <summary>
    ///     Creates a stack from an enumerable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="elements"></param>
    /// <returns></returns>
    public static Stack<T> ToStack<T>(this IEnumerable<T> elements)
    {
        var stack = new Stack<T>();

        foreach (var element in elements)
        {
            stack.Push(element);
        }

        return stack;
    }
}
