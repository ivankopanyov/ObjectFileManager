using ConsoleFileManager.Commands;
using ConsoleFileManager.Commands.Base;
using ConsoleFileManager.Services;
using FileManager;
using FileManager.Services;

namespace ConsoleFileManager;

public class ConsoleFileManagerLogic
{
    private FileManagerLogic _FileManager;

    private Command[] _Commands;

    public ConsoleFileManagerLogic()
    {
        _FileManager = new FileManagerLogic(OSNavigator.Navigator, new MessageService());

        _Commands = new Command[]
        {
            new ChangeDirectoryCommands("cd", _FileManager)
        };
    }

    public void Start()
    {
        while (true)
        {
            Console.Write(_FileManager.CurrentDirectory);
            Console.Write("> ");
            var input = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var found = false;
            foreach (var command in _Commands)
            {
                if (command.KeyWord == input[0])
                {
                    command.Execute(input);
                    found = true;
                    continue;
                }
            }
            if (!found)
                _FileManager.MessageService.ShowError($"Команда {input[0]} не найдена!");
        }
    }
}
