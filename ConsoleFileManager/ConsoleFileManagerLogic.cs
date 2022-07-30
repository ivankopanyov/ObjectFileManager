using ConsoleFileManager.Commands;
using ConsoleFileManager.Commands.Base;
using ConsoleFileManager.Services;
using FileManager;
using FileManager.Services;

namespace ConsoleFileManager;

/// <summary>Класс, описывающий логику работы консольного файлового менеджера.</summary>
public class ConsoleFileManagerLogic
{
    /// <summary>Логика работы консольного файлового менеджера.</summary>
    private IConsoleFileManager _FileManager;

    /// <summary>Флаг работы консольного файлового менеджера.</summary>
    private bool _CanWork;

    /// <summary>Список команд консольного файлового менеджера.</summary>
    public IReadOnlyList<Command> Commands { get; private set; }

    /// <summary>Сервис сообщений.</summary>
    public IMessageService MessageService => _FileManager.MessageService;

    /// <summary>Инициализация объекта консольного файлового менеджера.</summary>
    /// <param name="fileManager">Логика работы консольного файлового менеджера.</param>
    public ConsoleFileManagerLogic(IConsoleFileManager fileManager)
    {
        _FileManager = fileManager;

        Commands = new Command[]
        {
            new ChangeDirectoryCommand("cd", _FileManager),
            new ItemsListCommand("ls", _FileManager),
            new RemoveCommand("rm", _FileManager),
            new InfoCommand("info", _FileManager),
            new HelpCommand("help", this),
            new ExitCommand("exit", this)
        };
    }

    /// <summary>Запуск консольного файлового менеджера.</summary>
    public void Start()
    {
        _CanWork = true;

        FindCommand("help").Execute();

        while (_CanWork)
        {
            Console.Write(_FileManager.CurrentDirectory);
            Console.Write("> ");
            var input = Console.ReadLine()!.Split(' ');
            var command = FindCommand(input[0]);
            if (command is not null)
                command.Execute(input);
            else
                _FileManager.MessageService.ShowError($"Команда {input[0]} не найдена!");
        }
    }

    /// <summary>Остановка работы консольного файлового менеджера.</summary>
    public void Stop() => _CanWork = false;

    /// <summary>Поиск команды по ключевому слову.</summary>
    /// <param name="keyWord">Ключевое слово.</param>
    /// <returns>Найденная команда.</returns>
    private Command FindCommand(string keyWord)
    {
        foreach (var command in Commands)
            if (command.KeyWord == keyWord.ToLower()) 
                return command;

        return null!;
    }
}
