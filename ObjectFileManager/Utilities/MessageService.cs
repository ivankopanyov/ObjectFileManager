using System.Windows;
using FileManager;

namespace ObjectFileManager.Utilities;

public class MessageService : IMessageService
{
    public bool ShowYesNo(string message) =>
        MessageBox.Show(message, "Подтвердите действие!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

    public void ShowError(string message) =>
        MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
}
