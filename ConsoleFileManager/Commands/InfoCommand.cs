using ConsoleFileManager.Commands.Base;
using FileManager;
using FileManager.Content;

namespace ConsoleFileManager.Commands;

public class InfoCommand : Command
{
    private readonly FileManagerLogic _FileManager;
    
    public override string Description => "Информация о файле или директории.";

    public InfoCommand(string keyWord, FileManagerLogic fileManager) : base(keyWord)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

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

        var catalogItem = CatalogItem.GetCatalogItem(path);

        if (catalogItem == null)
        {
            _FileManager.MessageService.ShowError($"Файл {path} не найден!");
            return;
        }

        var result = $"{catalogItem.Name}\r\n\tТип: {catalogItem.DisplayType}\r\n\tРазмер: {catalogItem.ComputedSize} KB";

        _FileManager.MessageService.ShowOk(result);
    }
}
