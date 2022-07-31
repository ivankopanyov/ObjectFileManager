using ConsoleFileManager.Commands.Base;
using FileManager;

namespace ConsoleFileManager.Commands;

/// <summary>Класс, описывающий команду изменения текущей директории.</summary>
public class ChangeDirectoryCommand : Command
{
    /// <summary>Объект логики файлового менеджера.</summary>
    private readonly IConsoleFileManager _FileManager;

    /// <summary>Описание команды.</summary>
    public override string Description => "Изменение текущей директории.";

    /// <summary>Примеры использования команды.</summary>
    public override string[] Examples => new[]
    {
        @"C:\FolderName\FolderName",
        @"..\FolderName"
    };

    /// <summary>Инициализация объекта команды изменения текущей директории.</summary>
    /// <param name="fileManager">Объект логики файлового менеджера.</param>
    /// <exception cref="ArgumentNullException">Объект файлового менеджера не инициализирован.</exception>
    public ChangeDirectoryCommand(IConsoleFileManager fileManager)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    /// <summary>Выполнение команды изменения текущей директории.</summary>
    /// <param name="args">Значения параметров команды.</param>
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
