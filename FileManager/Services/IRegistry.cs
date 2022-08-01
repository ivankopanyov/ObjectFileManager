namespace FileManager.Services;

/// <summary>Реестр.</summary>
/// <typeparam name="TKey">Тип ключа.</typeparam>
/// <typeparam name="TValue">Тип значения.</typeparam>
public interface IRegistry<TKey, TValue>
{
    /// <summary>Регистрация объетка.</summary>
    /// <param name="key">Ключ регистрируемого объекта.</param>
    /// <param name="value">регистрируемый объект.</param>
    void Register(TKey key, TValue value);

    /// <summary>Удаление объекта из реестра.</summary>
    /// <param name="key">Ключ удаляемого объекта.</param>
    void Unregister(TKey key);
}
