using System;
using FileManager.Services;

namespace ObjectFileManager.Services;

public interface IDialogService<T> : IRegistry<string, Action<T>>
{
    public void ShowDialog(string key, T value);
}
