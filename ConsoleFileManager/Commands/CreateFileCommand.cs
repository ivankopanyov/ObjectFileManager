using ConsoleFileManager.Commands.Base;
using FileManager;
using FileManager.Content;

namespace ConsoleFileManager.Commands;

/// <summary>Класс, описывающий команду создания нового файл.</summary>
public class CreateFileCommand : Command
{
    /// <summary>Объект логики файлового менеджера.</summary>
    private readonly IConsoleFileManager _FileManager;

    /// <summary>Примеры использования команды.</summary>
    private readonly string[] _Examples = new[]
    {
        @"C:\dir_name\new_file_name",
        "\"..\\dir_name\\new file name\""
    };

    /// <summary>Описание команды.</summary>
    public override string Description => "Создание нового файла.";

    /// <summary>Примеры использования команды.</summary>
    public override string[] Examples => _Examples;

    /// <summary>Инициализация объекта команды создания нового файл.</summary>
    /// <param name="fileManager">Объект логики файлового менеджера.</param>
    /// <exception cref="ArgumentNullException">Объект файлового менеджера не инициализирован.</exception>
    public CreateFileCommand(IConsoleFileManager fileManager)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    /// <summary>Выполнение команды создания нового файл.</summary>
    /// <param name="args">Значения параметров команды.</param>
    public override void Execute(params string[] args)
    {
        if (args is null)
            throw new ArgumentNullException(nameof(args));

        if (args.Length <= 1)
        {
            _FileManager.MessageService.ShowError($"Не указано имя нового файла!");
            return;
        }

        var path = string.Join(' ', args, 1, args.Length - 1).Trim('"', ' ');

        if (string.IsNullOrWhiteSpace(path))
        {
            _FileManager.MessageService.ShowError($"Не указано имя нового файла!");
            return;
        }

        _FileManager.CreateFile(path);
    }
}
