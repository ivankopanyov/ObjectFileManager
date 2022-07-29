using ObjectFileManager.Services;
using System;

namespace ObjectFileManager.ViewModels.Base;

public abstract class DialogViewModel : ViewModel
{
    protected Action _Close;

    public DialogViewModel(object data, Action close, DialogService dialogService) : base(dialogService)
    {
        if (close is null)
            throw new ArgumentNullException(nameof(close));

        _Close = close;
    }

    public abstract void UpdateData(object data);
}
