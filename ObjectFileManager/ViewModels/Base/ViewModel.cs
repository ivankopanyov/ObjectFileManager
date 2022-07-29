using ObjectFileManager.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ObjectFileManager.ViewModels.Base;

public abstract class ViewModel : INotifyPropertyChanged
{
    protected DialogService _DialogService;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName]string PropertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    public ViewModel(DialogService dialogService)
    {
        if (dialogService is null)
            throw new ArgumentNullException(nameof(dialogService));

        _DialogService = dialogService;
    }
}