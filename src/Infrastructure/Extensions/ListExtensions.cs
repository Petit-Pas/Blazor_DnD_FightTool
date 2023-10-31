namespace Extensions;

public static class ListExtensions
{
    public static void RemoveAll<T>(this List<T> list, Type typeToRemove)
    {
        list.RemoveAll(x => x.GetType() == typeToRemove);
    }
}
