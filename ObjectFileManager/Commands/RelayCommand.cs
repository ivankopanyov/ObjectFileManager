using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ObjectFileManager.Commands;

public class RelayCommand : ICommand
{
    private readonly Action<object> _Execute;
    private readonly Predicate<object> _CanExecute;

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        => (_Execute, _CanExecute) = (execute, canExecute);

    public bool CanExecute(object parameter)
        => _CanExecute == null || _CanExecute(parameter);

    public virtual void Execute(object parameter)
        => _Execute(parameter);
}
