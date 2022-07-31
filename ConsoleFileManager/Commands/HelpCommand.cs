using ConsoleFileManager.Commands.Base;
using System.Text;

namespace ConsoleFileManager.Commands;

/// <summary>Класс, описывающий команду вывода информации о командах приложения.</summary>
public class HelpCommand : Command
{
    /// <summary>Объект логики консольного файлового менеджера.</summary>
    private readonly ConsoleFileManagerLogic _FileManager;

    /// <summary>Описание команды.</summary>
    public override string Description => "Список команд с описанием.";

    /// <summary>Инициализация объекта команды вывода информации о командах приложения.</summary>
    /// <param name="fileManager">Объект логики консольного файлового менеджера.</param>
    /// <exception cref="ArgumentNullException">Объект файлового менеджера не инициализирован.</exception>
    public HelpCommand(ConsoleFileManagerLogic fileManager)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    /// <summary>Выполнение команды вывода информации о командах приложения.</summary>
    /// <param name="args">Значения параметров команды.</param>
    public override void Execute(params string[] args)
    {
        if (_FileManager.Commands is null || _FileManager.Commands.Count == 0) return;

        var stringBuilder = new StringBuilder();

        foreach (var command in _FileManager.Commands)
        {
            stringBuilder
                .Append(command.Key)
                .Append(" - ")
                .AppendLine(command.Value.Description);
        }
        _FileManager.MessageService.ShowOk(stringBuilder.ToString());
    }
}
