using ConsoleFileManager.Commands.Base;
using FileManager;
using FileManager.Content;

namespace ConsoleFileManager.Commands;

/// <summary>Класс, описывающий команду изменения значения атрибута файла или директории.</summary>
public class ChangeAttrsCommand : Command
{
    /// <summary>Объект логики файлового менеджера.</summary>
    private readonly IConsoleFileManager _FileManager;

    /// <summary>Описание команды.</summary>
    public override string Description => "Изменение атрибутов файла или каталога.";

    /// <summary>Инициализация объекта команды изменения значения атрибута файла или директории.</summary>
    /// <param name="fileManager">Объект логики файлового менеджера.</param>
    /// <exception cref="ArgumentNullException">Объект файлового менеджера не инициализирован.</exception>
    public ChangeAttrsCommand(IConsoleFileManager fileManager)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    /// <summary>Выполнение команды изменения значения атрибута файла или директории.</summary>
    /// <param name="args">Значения параметров команды.</param>
    public override void Execute(params string[] args)
    {
        if (args is null || args.Length < 3)
        {
            _FileManager.MessageService.ShowError("Не указаны параметры команды!");
            return;
        }

        var path = string.Empty;
        List<(string AttrName, bool Value)> attrs = new();

        for (int i = 1; i < args.Length; i++)
        {
            if (args[i].Contains('='))
            {
                var attr = args[i].Split('=', StringSplitOptions.TrimEntries);
                if (attr.Length != 2 || string.IsNullOrWhiteSpace(attr[0]) || !bool.TryParse(attr[1], out var flag))
                {
                    _FileManager.MessageService.ShowError("Параметр указан некорректно!");
                    return;
                }

                attrs.Add((attr[0].ToLower(), flag));
            }
            else
            {
                path = string.Join(' ', args, i, args.Length - i);
                break;
            }
        }

        if (attrs.Count == 0)
        {
            _FileManager.MessageService.ShowError("Не указан путь!");
            return;
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            _FileManager.MessageService.ShowError("Не указаны параметры!");
            return;
        }

        if (!Path.IsPathRooted(path))
            path = Path.GetFullPath(Path.Combine(_FileManager.CurrentDirectory, path));

        var item = CatalogItem.GetCatalogItem(path);

        if (item == null)
        {
            _FileManager.MessageService.ShowError("Некорректно указан путь!");
            return;
        }

        foreach (var attr in attrs)
        {
            switch (attr.AttrName)
            {
                case "readonly":
                    try
                    {
                        item.ReadOnly = attr.Value;
                        _FileManager.MessageService.ShowOk("Аттрибут readonly успешно изменен!");
                    }
                    catch (InvalidOperationException ex)
                    {
                        _FileManager.MessageService.ShowError(ex.Message);
                    }
                    break;

                case "hidden":
                    try
                    {
                        item.Hidden = attr.Value;
                        _FileManager.MessageService.ShowOk("Аттрибут hidden успешно изменен!");
                    }
                    catch (InvalidOperationException ex)
                    {
                        _FileManager.MessageService.ShowError(ex.Message);
                    }
                    break;

                default:
                    _FileManager.MessageService.ShowError($"Аттрибут {attr.AttrName} не поддерживается!");
                    break;
            }
        }
    }
}
