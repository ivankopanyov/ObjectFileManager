using ConsoleFileManager.Commands.Base;
using FileManager;
using FileManager.Content;

namespace ConsoleFileManager.Commands;

/// <summary>Класс, описывающий команду удаления файла или директории.</summary>
public class RemoveCommand : Command
{
    /// <summary>Объект логики файлового менеджера.</summary>
    private readonly IConsoleFileManager _FileManager;

    /// <summary>Описание команды.</summary>
    public override string Description => "Удаление файла или каталога.";

    /// <summary>Инициализация объекта команды удаления файла или директории..</summary>
    /// <param name="fileManager">Объект логики файлового менеджера.</param>
    /// <exception cref="ArgumentNullException">Объект файлового менеджера не инициализирован.</exception>
    public RemoveCommand(IConsoleFileManager fileManager)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    /// <summary>Выполнение команды удаления файла или директории.</summary>
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

        var path = string.Join(' ', args, 1, args.Length - 1).Trim();

        if (CatalogItem.GetItemType(path) == CatalogItemType.None)
        {
            _FileManager.MessageService.ShowError($"Файл не найден!");
            return;
        }

        _FileManager.Remove(CatalogItem.GetCatalogItem(path));
    }
}
