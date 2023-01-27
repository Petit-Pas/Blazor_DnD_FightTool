namespace Extensions;

public static class EnumExtensions
{
    public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (name == default)
        {
            Console.WriteLine($"WARNING: Could not find the name {value} in enum {type}");
            return Array.Empty<TAttribute>();
        }
        var field = type.GetField(name);
        if (field == default)
        {
            Console.WriteLine($"WARNING: Could not find the field {name} in enum {type}");
            return Array.Empty<TAttribute>();
        }
        return field.GetCustomAttributes(false)
            .OfType<TAttribute>();
    }
}