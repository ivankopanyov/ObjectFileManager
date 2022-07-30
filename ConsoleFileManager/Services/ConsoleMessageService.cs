using FileManager.Services;

namespace ConsoleFileManager.Services;

public class ConsoleMessageService : IMessageService
{
    public void ShowError(string message)
    {
        Console.WriteLine();
        Console.Write("Ошибка: ");
        Console.WriteLine(message);
        Console.WriteLine();
    }

    public void ShowOk(string message)
    {
        Console.WriteLine();
        Console.WriteLine(message);
        Console.WriteLine();
    }

    public bool ShowYesNo(string message)
    {
        throw new NotImplementedException();
    }

    public MessageResult ShowYesNoCancel(string message)
    {
        throw new NotImplementedException();
    }
}
