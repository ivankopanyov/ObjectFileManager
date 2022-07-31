using ConsoleFileManager.Commands.Base;
using FileManager;
using FileManager.Content;

namespace ConsoleFileManager.Commands;

/// <summary>Класс, описывающий команду создания новой директории.</summary>
public class CreateDirectoryCommand : Command
{
    /// <summary>Объект логики файлового менеджера.</summary>
    private readonly IConsoleFileManager _FileManager;

    /// <summary>Примеры использования команды.</summary>
    private readonly string[] _Examples = new[]
    {
        @"C:\folder_name\new_folder_name",
        "\"..\\folder_name\\new folder name\""
    };

    /// <summary>Описание команды.</summary>
    public override string Description => "Создание нового файла.";

    /// <summary>Примеры использования команды.</summary>
    public override string[] Examples => _Examples;

    /// <summary>Инициализация объекта команды создания новой директории.</summary>
    /// <param name="fileManager">Объект логики файлового менеджера.</param>
    /// <exception cref="ArgumentNullException">Объект файлового менеджера не инициализирован.</exception>
    public CreateDirectoryCommand(IConsoleFileManager fileManager)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    /// <summary>Выполнение команды создания новой директории.</summary>
    /// <param name="args">Значения параметров команды.</param>
    public override void Execute(params string[] args)
    {
        if (args is null)
            throw new ArgumentNullException(nameof(args));

        if (args.Length <= 1)
        {
            _FileManager.MessageService.ShowError($"Не указано имя новой папки!");
            return;
        }

        var path = string.Join(' ', args, 1, args.Length - 1).Trim('"', ' ');

        if (string.IsNullOrWhiteSpace(path))
        {
            _FileManager.MessageService.ShowError($"Не указано имя новой папки!");
            return;
        }

        _FileManager.CreateCatalog(path);
    }
}
