namespace FileManager.Services;

public interface IRegistry<TKey, TValue>
{
    void Register(TKey key, TValue value);

    void Unregister(TKey key);
}
