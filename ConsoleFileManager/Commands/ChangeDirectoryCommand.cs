using ConsoleFileManager.Commands.Base;
using FileManager;

namespace ConsoleFileManager.Commands;

public class ChangeDirectoryCommand : Command
{
    private readonly FileManagerLogic _FileManager;

    public override string Description => "Изменение текущей директории."; 

    public ChangeDirectoryCommand(string keyWord, FileManagerLogic fileManager) : base(keyWord)
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

        _FileManager.ChangeDirectory(string.Join(' ', args, 1, args.Length - 1).Trim());
    }
}
