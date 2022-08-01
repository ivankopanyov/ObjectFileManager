using System;
using System.Windows.Input;

namespace ObjectFileManager.Commands;

/// <summary>Класс, реализующий интерфейс ICommand.</summary>
public class Command : ICommand
{
    /// <summary>Дейстрие, выполняемое при вызове команды.</summary>
    private readonly Action<object> _Execute;

    /// <summary>Условие для выполнения команды.</summary>
    private readonly Predicate<object> _CanExecute;

    /// <summary>Событие изменения условия выполнениия команды.</summary>
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>Инициализация объекта команды.</summary>
    /// <param name="execute">Дейстрие, выполняемое при вызове команды.</param>
    /// <param name="canExecute">Условие для выполнения команды.</param>
    public Command(Action<object> execute, Predicate<object> canExecute = null)
        => (_Execute, _CanExecute) = (execute, canExecute);

    /// <summary>Условие для выполнения команды.</summary>
    /// <param name="parameter">Переданный параметр.</param>
    /// <returns>Результат проверки условия.</returns>
    public bool CanExecute(object parameter) => _CanExecute == null || _CanExecute(parameter);

    /// <summary>Условие для выполнения команды.</summary>
    /// <param name="parameter">Переданный параметр.</param>
    public virtual void Execute(object parameter) => _Execute(parameter);
}
