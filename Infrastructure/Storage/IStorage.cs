namespace Infrastructure.Storage;

public interface IStorage<TKey, TValue>
{
    bool TryGetValue(TKey key, out TValue value);
    void Add(TKey key, TValue value);
    bool Remove(TKey key);
}