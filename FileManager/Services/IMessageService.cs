namespace FileManager.Services;

/// <summary>Сервис вывода сообщений.</summary>
public interface IMessageService
{
    /// <summary>Вывод сообщения об удачном завершении операции.</summary>
    /// <param name="message">Текст сообщения об удачном завершении операции.</param>
    void ShowOk(string message);

    /// <summary>Вывод сообщения об ошибке.</summary>
    /// <param name="message">Текст сообщения об ошибке.</param>
    void ShowError(string message);

    /// <summary>Сообщение с запросом на подтверждение.</summary>
    /// <param name="message">Текст сообщения.</param>
    /// <returns>Ответ пользователя.</returns>
    bool ShowYesNo(string message);

    /// <summary>Сообщение с запросом на подтверждение или отмену.</summary>
    /// <param name="message">Текст сообщения.</param>
    /// <returns>Ответ пользователя.</returns>
    MessageResult ShowYesNoCancel(string message);
}
