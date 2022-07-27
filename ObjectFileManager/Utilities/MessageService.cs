using System.Windows;
using FileManager.Information;

namespace ObjectFileManager.Utilities;

public class MessageService : IMessageService
{
    public void ShowError(string message) =>
        MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

    public bool ShowYesNo(string message) =>
        MessageBox.Show(message, "Подтвердите действие!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

    public bool ShowOkCancel(string message) =>
        MessageBox.Show(message, "Ошибка!", MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK;
}
