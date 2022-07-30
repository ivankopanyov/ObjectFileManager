using ConsoleFileManager.Commands.Base;
using FileManager.Services;
using System.Text;

namespace ConsoleFileManager.Commands;

public class ExitCommand : Command
{
    private readonly ConsoleFileManagerLogic _FileManager;

    public override string Description => "Выход из приложения.";

    public ExitCommand(string keyWord, ConsoleFileManagerLogic fileManager) : base(keyWord)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    public override void Execute(params string[] args) => _FileManager.Stop();
}
