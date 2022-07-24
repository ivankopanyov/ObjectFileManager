using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ObjectFileManager.ViewModels.Base;

public abstract class ViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName]string PropertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
}