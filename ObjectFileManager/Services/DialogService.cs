using ObjectFileManager.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ObjectFileManager.Services;

public class DialogService
{
    private readonly Dictionary<string, Window> _DialogViews = new Dictionary<string, Window>();

    public void Register(string key, Window window)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        if (window is null)
            throw new ArgumentNullException(nameof(window));

        if (_DialogViews.ContainsKey(key))
            throw new InvalidOperationException($"Ключ {key} уже зарегистрирован!");

        if (window.DataContext is null || !window.DataContext.GetType().IsSubclassOf(typeof(DialogViewModel)))
            throw new ArgumentException("Некорректный тип DataContext");

        _DialogViews[key] = window;
    }

    public void ShowDialog(string key)
    {
        if (_DialogViews.ContainsKey(key))
            _DialogViews[key].ShowDialog();
    }

    public void Update(string key, object data)
    {
        if (_DialogViews.ContainsKey(key))
            ((DialogViewModel)_DialogViews[key].DataContext).UpdateData(data);
    }
}
