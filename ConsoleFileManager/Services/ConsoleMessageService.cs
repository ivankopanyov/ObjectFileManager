using FileManager.Services;

namespace ConsoleFileManager.Services;

/// <summary>Сервис сообщений для консольного интерфейса.</summary>
public class ConsoleMessageService : IConsoleMessageService
{
    /// <summary>Ввод данных пользователем.</summary>
    /// <returns>Результат ввода .</returns>
    public string InputLine() => Console.ReadLine()!;

    /// <summary>Вывод сщщбщения в консоль.</summary>
    /// <param name="message">Текст сообщения.</param>
    public void ShowMessage(string message) => Console.Write(message);

    /// <summary>Вывод сщщбщения в консоль с переносом строки.</summary>
    /// <param name="message">Текст сообщения.</param>
    public void ShowMessageLine(string message) => Console.WriteLine(message);

    /// <summary>Вывод сообщения об ошибке в консоль.</summary>
    /// <param name="message">Сообщение об ошибке.</param>
    public void ShowError(string message)
    {
        Console.WriteLine();
        Console.Write("Ошибка: ");
        Console.WriteLine(message);
        Console.WriteLine();
    }

    /// <summary>Вывод сообщения об удачном завершении операции в консоль.</summary>
    /// <param name="message">Сообщения об удачном завершении операции.</param>
    public void ShowOk(string message)
    {
        Console.WriteLine();
        Console.WriteLine(message);
        Console.WriteLine();
    }

    /// <summary>Вывод сообщения с запросом на подтверждение в консоль.</summary>
    /// <param name="message">Текст сообщения.</param>
    /// <returns>Ответ пользователя.</returns>
    public bool ShowYesNo(string message)
    {
        Console.WriteLine();
        while (true)
        {
            Console.Write($"{message} (y/n) > ");
            var input = Console.ReadLine()!.ToLower().Trim();
            switch (input)
            {
                case "y":
                case "yes":
                case "д":
                case "да":
                    Console.WriteLine();
                    return true;
                case "n":
                case "no":
                case "н":
                case "нет":
                    Console.WriteLine();
                    return false;
                default:
                    Console.WriteLine("Некорректный ввод! Повторите попытку...");
                    break;
            }
        }
    }

    /// <summary>Вывод сообщения с запросом на подтверждение или отмену в консоль.</summary>
    /// <param name="message">Текст сообщения.</param>
    /// <returns>Ответ пользователя.</returns>
    public MessageResult ShowYesNoCancel(string message)
    {
        throw new NotImplementedException();
    }
}
