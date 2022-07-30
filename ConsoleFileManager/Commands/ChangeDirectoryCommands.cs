using ConsoleFileManager.Commands.Base;
using FileManager;

namespace ConsoleFileManager.Commands;

public class ChangeDirectoryCommands : Command
{
    public FileManagerLogic _FileManager;

    public ChangeDirectoryCommands(string keyWord, FileManagerLogic fileManager) : base(keyWord)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    public override void Execute(params string[] args)
    {
        if (args is null)
            throw new ArgumentNullException(nameof(args));

        if (args.Length <= 1)
        {
            _FileManager.MessageService.ShowError($"Не указан путь!");
            return;
        }

        if (args.Length > 2)
        {
            _FileManager.MessageService.ShowError($"Неизвестный параметр {args[2]}");
            return;
        }

        _FileManager.ChangeDirectory(args[1]);
    }
}
