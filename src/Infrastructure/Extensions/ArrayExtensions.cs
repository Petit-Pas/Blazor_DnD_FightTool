namespace DnDFightTool.Infrastructure.Extensions;

public static class ArrayExtensions
{
    public static bool IsEmpty<T>(this T[] array)
    {
        return array.Length == 0;
    }
}
