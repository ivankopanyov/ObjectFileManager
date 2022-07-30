using ConsoleFileManager.Commands.Base;
using System.Text;

namespace ConsoleFileManager.Commands;

public class HelpCommand : Command
{
    private readonly ConsoleFileManagerLogic _FileManager;

    public override string Description => "Список команд с описанием.";

    public HelpCommand(string keyWord, ConsoleFileManagerLogic fileManager) : base(keyWord)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    public override void Execute(params string[] args)
    {
        if (_FileManager.Commands is null || _FileManager.Commands.Count == 0) return;

        var stringBuilder = new StringBuilder();

        foreach (var command in _FileManager.Commands)
        {
            stringBuilder
                .Append(command.KeyWord)
                .Append(" - ")
                .Append(command.Description)
                .Append(command != _FileManager.Commands[_FileManager.Commands.Count - 1] ? '\n' : string.Empty);
        }
        _FileManager.MessageService.ShowOk(stringBuilder.ToString());
    }
}
