namespace FileManager.Services;

/// <summary>Сервис сообщений для консольного интерфейса.</summary>
public interface IConsoleMessageService : IMessageService
{
    /// <summary>Ввод данных пользователем.</summary>
    /// <returns>Результат ввода .</returns>
    string InputLine();

    /// <summary>Вывод сщщбщения в консоль.</summary>
    /// <param name="message">Текст сообщения.</param>
    void ShowMessage(string message);

    /// <summary>Вывод сщщбщения в консоль с переносом строки.</summary>
    /// <param name="message">Текст сообщения.</param>
    void ShowMessageLine(string message);
}
