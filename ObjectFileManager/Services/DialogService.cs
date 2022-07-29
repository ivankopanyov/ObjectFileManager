using System;
using System.Collections.Generic;

namespace ObjectFileManager.Services;

public class DialogService : IDialogService<object>
{
    private readonly Dictionary<string, Action<object>> _DialogViews = new Dictionary<string, Action<object>>();

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

    public void Unregister(string key)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        if (!_DialogViews.ContainsKey(key))
            throw new InvalidOperationException($"Ключ {key} не зарегистрирован!");

        _DialogViews.Remove(key);
    }

    public void ShowDialog(string key, object path)
    {
        if (_DialogViews.ContainsKey(key))
            _DialogViews[key](path);
    }
}
