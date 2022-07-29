using System.Windows;
using FileManager.Services;

namespace ObjectFileManager.Services;

/// <summary>Сервис вывода сообщений в интерфейс пользователя.</summary>
public class MessageBoxService : IMessageService
{
    /// <summary>Вывод сообщения об ошибке.</summary>
    /// <param name="message">Текст сообщения об ошибке.</param>
    public void ShowError(string message) =>
        MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

    /// <summary>Сообщение с запросом на подтверждение.</summary>
    /// <param name="message">Текст сообщения.</param>
    /// <returns>Ответ пользователя.</returns>
    public bool ShowYesNo(string message) =>
        MessageBox.Show(message, "Подтвердите действие!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

    /// <summary>Сообщение с запросом на подтверждение или отмену.</summary>
    /// <param name="message">Текст сообщения.</param>
    /// <returns>Ответ пользователя.</returns>
    public MessageResult ShowYesNoCancel(string message) =>
        (MessageResult)MessageBox.Show(message, "Подтвердите действие!", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
}
