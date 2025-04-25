namespace Extensions;

public static class ArrayExtensions
{
    public static bool None<T>(this T[] array)
    {
        return array.Length == 0;
    }
}
