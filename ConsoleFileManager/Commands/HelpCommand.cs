using ConsoleFileManager.Commands.Base;
using System.Text;

namespace ConsoleFileManager.Commands;

/// <summary>Класс, описывающий команду вывода информации о командах приложения.</summary>
public class HelpCommand : Command
{
    /// <summary>Объект логики консольного файлового менеджера.</summary>
    private readonly ConsoleFileManagerLogic _FileManager;

    /// <summary>Описание команды.</summary>
    public override string Description => "Список команд с описанием или описание команды с примерами использования.";

    /// <summary>Примеры использования команды.</summary>
    public override string[] Examples => new[] 
    { 
        string.Empty,
        "CommandName"
    };


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
        var stringBuilder = new StringBuilder();

        if (args is not null && args.Length > 1)
        {
            var commandName = string.Join(' ', args, 1, args.Length - 1).ToLower().Trim();
            if (!string.IsNullOrWhiteSpace(commandName) && _FileManager.Commands.TryGetValue(commandName, out var command))
            {
                stringBuilder.AppendLine($"{args[0]} - {command.Description}\r\n\r\nПримеры использования:\r\n");

                foreach (var example in command.Examples) 
                    stringBuilder.AppendLine($"{commandName} {example}");

                _FileManager.MessageService.ShowOk(stringBuilder.ToString());

                return;
            }
        }

        if (_FileManager.Commands is null || _FileManager.Commands.Count == 0) return;

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
