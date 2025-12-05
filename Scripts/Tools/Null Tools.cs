using System.Collections.Generic;
using System.Linq;

public static class NullTools
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) => collection == null || !collection.Any();
    public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> collection) => collection != null && collection.Any();
}
