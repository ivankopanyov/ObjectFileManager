using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FileManager.Services;
using ObjectFileManager.Services;

namespace ObjectFileManager.ViewModels.Base;

public abstract class ViewModel : INotifyPropertyChanged
{
    protected readonly IDialogService<object> _DialogService;

    protected readonly IMessageService _MessageService;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName]string PropertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    public ViewModel(IDialogService<object> dialogService, IMessageService messageService)
    {
        if (dialogService is null)
            throw new ArgumentNullException(nameof(dialogService));

        if (messageService is null)
            throw new ArgumentNullException(nameof(messageService));

        _DialogService = dialogService;
        _MessageService = messageService;
    }
}