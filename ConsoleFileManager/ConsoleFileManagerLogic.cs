using ConsoleFileManager.Commands;
using ConsoleFileManager.Commands.Base;
using ConsoleFileManager.Services;
using FileManager;
using FileManager.Services;

namespace ConsoleFileManager;

public class ConsoleFileManagerLogic
{
    private FileManagerLogic _FileManager;

    private bool _CanWork;

    public IReadOnlyList<Command> Commands { get; private set; }

    public IMessageService MessageService => _FileManager.MessageService;

    public ConsoleFileManagerLogic()
    {
        _FileManager = new FileManagerLogic(OSNavigator.Navigator, new ConsoleMessageService());

        Commands = new Command[]
        {
            new ChangeDirectoryCommand("cd", _FileManager),
            new ItemsListCommand("ls", _FileManager),
            new CopyCommand("cp", _FileManager, new ConsoleClipboard()),
            new InfoCommand("info", _FileManager),
            new HelpCommand("help", this),
            new ExitCommand("exit", this)
        };
    }

    public void Start()
    {
        _CanWork = true;

        FineCommand("help").Execute();

        while (_CanWork)
        {
            Console.Write(_FileManager.CurrentDirectory);
            Console.Write("> ");
            var input = Console.ReadLine()!.Split(' ');
            var command = FineCommand(input[0]);
            if (command is not null)
                command.Execute(input);
            else
                _FileManager.MessageService.ShowError($"Команда {input[0]} не найдена!");
        }
    }

    private Command FineCommand(string keyWord)
    {
        foreach (var command in Commands)
            if (command.KeyWord == keyWord) 
                return command;

        return null!;
    }

    public void Stop() => _CanWork = false;
}
