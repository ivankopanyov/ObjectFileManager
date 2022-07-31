using ConsoleFileManager.Commands.Base;
using FileManager;
using FileManager.Content;
using System.Text;

namespace ConsoleFileManager.Commands;

/// <summary>Класс, описывающий команду вывода списка файлов
/// и сабдиректорий текущей директории.</summary>
public class ItemsListCommand : Command
{
    /// <summary>Объект логики файлового менеджера.</summary>
    private readonly IConsoleFileManager _FileManager;

    /// <summary>Примеры использования команды.</summary>
    private readonly string[] _Examples = new[] { string.Empty };

    /// <summary>Описание команды.</summary>
    public override string Description => "Список файлов и папок из текущей директории.";

    /// <summary>Примеры использования команды.</summary>
    public override string[] Examples => _Examples;

    /// <summary>Инициализация объекта команды вывода списка файлов
    /// и сабдиректорий текущей директории.</summary>
    /// <param name="fileManager">Объект логики файлового менеджера.</param>
    /// <exception cref="ArgumentNullException">Объект файлового менеджера не инициализирован.</exception>
    public ItemsListCommand(IConsoleFileManager fileManager)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    /// <summary>Выполнение команды вывода списка файлов
    /// и сабдиректорий текущей директории.</summary>
    /// <param name="args">Значения параметров команды.</param>
    public override void Execute(params string[] args)
    {
        var items = _FileManager.ItemsList;

        if (items.Length == 0) return;

        var stringBuilder = new StringBuilder();

        foreach (var item in items)
            stringBuilder
                .Append(item.Type == CatalogItemType.Catalog ? " -d- " : " -f- ")
                .AppendLine(item.Name);

        _FileManager.MessageService.ShowOk(stringBuilder.ToString());
    }
}
