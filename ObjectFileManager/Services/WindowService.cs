using System;
using System.Collections.Generic;

namespace ObjectFileManager.Services;

/// <summary>Класс, описывабющий сервис работы с окнами.</summary>
public class WindowService : IWindowService<object>
{
    /// <summary>Словарь, содержащий делегаты создания и отображения окон.</summary>
    private readonly Dictionary<string, Action<object>> _DialogViews = new Dictionary<string, Action<object>>();

    /// <summary>Регистрация делегата нового окна.</summary>
    /// <param name="key">Ключ.</param>
    /// <param name="action">Делегат.</param>
    /// <exception cref="ArgumentNullException">Параметр не инициализирован.</exception>
    /// <exception cref="InvalidOperationException">Переданный ключ уже содержится в реестре.</exception>
    public void Register(string key, Action<object> action)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        if (action is null)
            throw new ArgumentNullException(nameof(action));

        if (_DialogViews.ContainsKey(key))
            throw new InvalidOperationException($"Ключ {key} уже зарегистрирован!");

        _DialogViews[key] = action;
    }

    /// <summary>Удаление объекта из реестра.</summary>
    /// <param name="key">Ключ удаляемого объекта.</param>
    /// <exception cref="ArgumentNullException">Параметр не инициализирован.</exception>
    /// <exception cref="InvalidOperationException">Переданный ключ не содержится в реестре.</exception>
    public void Unregister(string key)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        if (!_DialogViews.ContainsKey(key))
            throw new InvalidOperationException($"Ключ {key} не зарегистрирован!");

        _DialogViews.Remove(key);
    }

    /// <summary>Отображение окна.</summary>
    /// <param name="key">Ключ.</param>
    /// <param name="obj">Переданный параметр.</param>
    public void ShowWindow(string key, object obj)
    {
        if (_DialogViews.ContainsKey(key))
            _DialogViews[key](obj);
    }
}
