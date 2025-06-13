namespace MaproSSO.Shared.Extensions;

public static class CollectionExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection)
    {
        return collection == null || !collection.Any();
    }

    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> collection) where T : class
    {
        return collection.Where(item => item != null)!;
    }

    public static Dictionary<TKey, TValue> ToDictionarySafe<TSource, TKey, TValue>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> valueSelector) where TKey : notnull
    {
        var dictionary = new Dictionary<TKey, TValue>();

        foreach (var item in source)
        {
            var key = keySelector(item);
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = valueSelector(item);
            }
        }

        return dictionary;
    }

    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        using var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            yield return GetBatch(enumerator, batchSize);
        }
    }

    private static IEnumerable<T> GetBatch<T>(IEnumerator<T> enumerator, int batchSize)
    {
        var count = 0;
        do
        {
            yield return enumerator.Current;
            count++;
        } while (count < batchSize && enumerator.MoveNext());
    }
}