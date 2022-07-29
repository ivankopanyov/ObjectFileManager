using System.Windows;
using FileManager.Information;

namespace ObjectFileManager.Services;

public class MessageService : IMessageService
{
    public void ShowError(string message) =>
        MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

    public bool ShowYesNo(string message) =>
        MessageBox.Show(message, "Подтвердите действие!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

    public MessageResult ShowYesNoCancel(string message) =>
        (MessageResult)MessageBox.Show(message, "Подтвердите действие!", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
}
