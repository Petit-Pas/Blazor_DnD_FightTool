using System.Security.Cryptography;
using System.Text;
using System.Collections;
using System.Reflection;

namespace Memory.Hashes;

public static class HashableExtensions
{
    public static string HashExcept(this IHashable hashable, PropertyInfo[] propertiesToIgnore)
    {
        var combinedProperties = new StringBuilder();

        var properties = hashable.GetType().GetProperties()
            .Where(p => p.DeclaringType != typeof(IEnumerable))
            .Where(p => !propertiesToIgnore.Contains(p))
            .OrderBy(x => x.Name);

        return hashable.Hash(properties.ToArray());
    }

    /// <summary>
    ///     This method is a generic method to use on any IHashable class. 
    ///     It will recursively hash any property that also implements IHashable.
    ///     
    ///     A property that does not implement IHashable will be added to the Hash with a ToString() call.
    /// </summary>
    /// <param name="hashable"></param>
    /// <returns></returns>
    public static string Hash(this IHashable hashable)
    {
        var combinedProperties = new StringBuilder();

        if (hashable is IEnumerable _enumerable)
        {
            foreach (var item in _enumerable)
            {
                combinedProperties.Append(HashProperty(item));
                combinedProperties.Append('|');
            }
        }

        var properties = hashable.GetType().GetProperties()
            .Where(p => p.DeclaringType != typeof(IEnumerable))
            .OrderBy(x => x.Name);
        
        combinedProperties.Append(hashable.Hash(properties.ToArray()));
        
        var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(combinedProperties.ToString()));
        return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
    }

    /// <summary>
    ///     This method is a generic method to use on any IHashable class. 
    ///     It will recursively hash any property that also implements IHashable.
    ///     
    ///     A property that does not implement IHashable will be added to the Hash with a ToString() call.
    /// </summary>
    /// <param name="hashable"></param>
    /// <returns></returns>
    public static string Hash(this IHashable hashable, PropertyInfo[] propertiesToHash)
    {
        var combinedProperties = new StringBuilder();
       
        foreach (var property in propertiesToHash.OrderBy(x => x.Name))
        {
            // Somehow, when a class inherits List, this condition is true, and it gets impossible to call GetValue without additional parameters.
            // Anyway the elements of the list have been handled above in the IEnumerable loop so we can simply skip them.
            // See "Indexed Properties" for more info
            if (property.GetIndexParameters().Length != 0)
            {
                continue;
            }

            var value = property.GetValue(hashable);
            if (value == null)
            {
                continue;
            }

            // TODO check that string don't fall into this condition.
            // Also, this if can probably be replaced by a recursive call? 
            if (value is IEnumerable enumerable && value is not string)
            {
                foreach (var item in enumerable)
                {
                    combinedProperties.Append(HashProperty(item));
                    combinedProperties.Append('|');
                }
            }
            else
            {
                combinedProperties.Append(HashProperty(value));
                combinedProperties.Append('|');
            }
        }

        var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(combinedProperties.ToString()));
        return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
    }

    private static string HashProperty(object value)
    {
        if (value is IHashable hashableObject)
        {
            return hashableObject.Hash();
        }
        else
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
