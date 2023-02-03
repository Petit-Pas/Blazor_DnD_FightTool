namespace Extensions;

public static class EnumExtensions
{
    public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (name == default)
        {
            Console.WriteLine($"WARNING: Could not find the name {value} in enum {type}.");
            return Array.Empty<TAttribute>();
        }
        var field = type.GetField(name);
        if (field == default)
        {
            Console.WriteLine($"WARNING: Could not find the field {name} in enum {type}.");
            return Array.Empty<TAttribute>();
        }
        return field.GetCustomAttributes(false)
            .OfType<TAttribute>();
    }

    public static TAttribute? GetAttribute<TAttribute>(this Enum value)
    {
        var type = value.GetType();
        
        var name = Enum.GetName(type, value);
        if (name == default)
        {
            Console.WriteLine($"WARNING: Could not find the name {value} in enum {type}.");
            return default;
        }
        
        var field = type.GetField(name);
        if (field == default)
        {
            Console.WriteLine($"WARNING: Could not find the field {name} in enum {type}.");
            return default;
        }

        var attributes = field.GetCustomAttributes(false)
            .OfType<TAttribute>().ToArray();
        if (!attributes.Any())
        {
            Console.WriteLine($"WARNING: Could not find an attribute of type {typeof(TAttribute).Name} in enum {type}.");
            return default;
        }

        if (attributes.Length > 1)
        {
            Console.WriteLine($"WARNING: More than one attribute of type {typeof(TAttribute).Name} was found in enum {type}.");
            return default;
        }

        return attributes.First();
    }
}