namespace Extensions;

public static class ListExtensions
{
    public static void RemoveAll<T>(this List<T> list, Type typeToRemove)
    {
        list.RemoveAll(x => x.GetType() == typeToRemove);
    }

    public static IEnumerable<T> WithoutAll<T>(this IEnumerable<T> elements, Type typeToRemove)
    {
        return elements.Where(x => x.GetType() != typeToRemove);
    }

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
