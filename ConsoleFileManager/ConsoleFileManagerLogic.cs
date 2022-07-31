using ConsoleFileManager.Commands;
using ConsoleFileManager.Commands.Base;
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

    /// <summary>Команды консольного файлового менеджера.</summary>
    public IReadOnlyDictionary<string, Command> Commands { get; private set; }

    /// <summary>Сервис сообщений.</summary>
    public IConsoleMessageService MessageService { get; init; }

    /// <summary>Инициализация объекта консольного файлового менеджера.</summary>
    /// <param name="navigator">Навигатор по файловой системе.</param>
    /// <param name="messageService">Сервис сообщений.</param>
    /// <exception cref="ArgumentNullException">Параметр не инициализирован.</exception>
    public ConsoleFileManagerLogic(INavigator<string> navigator, IConsoleMessageService messageService)
    {
        if (navigator is null)
            throw new ArgumentNullException(nameof(navigator));

        if (messageService is null)
            throw new ArgumentNullException(nameof(messageService));

        MessageService = messageService;

        _FileManager = new FileManagerLogic(navigator, messageService);

        var changeDirectoryCommand = new ChangeDirectoryCommand(_FileManager);
        var itemsListCommand = new ItemsListCommand(_FileManager);
        var createFileCommand = new CreateFileCommand(_FileManager);
        var createDirectoryCommand = new CreateDirectoryCommand(_FileManager);
        var copyCommand = new CopyCommand(_FileManager);
        var moveCommand = new MoveCommand(_FileManager);
        var changeAttrsCommand = new ChangeAttrsCommand(_FileManager);
        var removeCommand = new RemoveCommand(_FileManager);
        var infoCommand = new InfoCommand(_FileManager);
        var helpCommand = new HelpCommand(this);
        var exitCommand = new ExitCommand(this);

        Commands = new Dictionary<string, Command>
        {
            { "cd", changeDirectoryCommand },
            { "ls", itemsListCommand },
            { "file", createFileCommand },
            { "dir", createDirectoryCommand },
            { "copy", copyCommand },
            { "move", moveCommand },
            { "attr", changeAttrsCommand },
            { "rm", removeCommand },
            { "info", infoCommand },
            { "help", helpCommand},
            { "exit", exitCommand }
        };
    }

    /// <summary>Запуск консольного файлового менеджера.</summary>
    public void Start()
    {
        _CanWork = true;

        Commands["help"].Execute();

        while (_CanWork)
        {
            MessageService.ShowMessage($"{_FileManager.CurrentDirectory}> ");
            var input = MessageService.InputLine().Split(' ');
            var commandKey = input[0];
            if (Commands.TryGetValue(commandKey, out var command))
            {
                command!.Execute(input);
                continue;
            }
            _FileManager.MessageService.ShowError($"Команда {input[0]} не найдена!");
        }
    }

    /// <summary>Остановка работы консольного файлового менеджера.</summary>
    public void Stop() => _CanWork = false;
}
