using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FileManager.Services;
using ObjectFileManager.Services;

namespace ObjectFileManager.ViewModels.Base;

/// <summary>Базовый класс для ViewModel.</summary>
public abstract class ViewModel : INotifyPropertyChanged
{
    /// <summary>Сервис работы с окнами.</summary>
    protected readonly IWindowService<object> _WindowService;

    /// <summary>Событие изменения свойств ViewModel.</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Вызов события изменения свойства ViewModel.</summary>
    protected virtual void OnPropertyChanged([CallerMemberName]string PropertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    /// <summary>Инициализация объекта ViewModel.</summary>
    /// <param name="windowService">Сервис сообщений.</param>
    /// <exception cref="ArgumentNullException">Параметр не инициализирован.</exception>
    public ViewModel(IWindowService<object> windowService)
    {
        if (windowService is null)
            throw new ArgumentNullException(nameof(windowService));

        _WindowService = windowService;
    }
}