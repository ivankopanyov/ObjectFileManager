using System;
using FileManager.Services;

namespace ObjectFileManager.Services;

/// <summary>Сервис работы с окнами.</summary>
/// <typeparam name="T">Тип передаваемого параметра.</typeparam>
public interface IWindowService<T> : IRegistry<string, Action<T>>
{
    /// <summary>Отображение окна.</summary>
    /// <param name="key">Ключ.</param>
    /// <param name="value">Переданный параметр.</param>
    public void ShowWindow(string key, T value);
}
