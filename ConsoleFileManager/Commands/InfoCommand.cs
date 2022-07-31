using ConsoleFileManager.Commands.Base;
using FileManager;
using FileManager.Content;
using System.Globalization;

namespace ConsoleFileManager.Commands;

/// <summary>Класс, описывающий команду вывода информации о файле или директории.</summary>
public class InfoCommand : Command
{
    /// <summary>Объект логики файлового менеджера.</summary>
    private readonly IConsoleFileManager _FileManager;

    /// <summary>Формат форматирования даты.</summary>
    private readonly CultureInfo _Ru = CultureInfo.CreateSpecificCulture("ru-RU");

    /// <summary>Описание команды.</summary>
    public override string Description => "Информация о файле или директории.";

    /// <summary>Инициализация объекта команды вывода информации о файле или директории.</summary>
    /// <param name="fileManager">Объект логики файлового менеджера.</param>
    /// <exception cref="ArgumentNullException">Объект файлового менеджера не инициализирован.</exception>
    public InfoCommand(IConsoleFileManager fileManager)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    /// <summary>Выполнение команды вывода информации о файле или директории.</summary>
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

        var item = CatalogItem.GetCatalogItem(path);

        if (item == null)
        {
            _FileManager.MessageService.ShowError($"Файл {path} не найден!");
            return;
        }

        var result = $"{item.Name}\r\n\tПуть: {item.FullName}\r\n\tТип: {item.DisplayType}\r\n\tРазмер: {item.ComputedSize} KB\r\n\tДата создания: {item.CreateDate.ToString(_Ru)}\r\n\tДата изменения: {item.UpdateDate.ToString(_Ru)}\r\n\tТолько для чтения: {item.ReadOnly}\r\n\tСкрытый: {item.ReadOnly}";

        _FileManager.MessageService.ShowOk(result);
    }
}
