using System.Collections.Concurrent;

namespace Infrastructure.Storage;

public class KeyValueStorage<TKey, TValue> : IStorage<TKey, TValue>
{
    private readonly ConcurrentDictionary<TKey, TValue> _items = new();

    public bool TryGetValue(TKey key, out TValue value)
    {
        return _items.TryGetValue(key, out value);
    }

    public void Add(TKey key, TValue value)
    {
        _items[key] = value;
    }
}