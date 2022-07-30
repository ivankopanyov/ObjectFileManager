using ConsoleFileManager.Commands.Base;
using FileManager;
using FileManager.Content;
using System.Text;

namespace ConsoleFileManager.Commands;

public class ItemsListCommand : Command
{
    private readonly FileManagerLogic _FileManager;

    public override string Description => "Список файлов и папок из текущей директории.";

    public ItemsListCommand(string keyWord, FileManagerLogic fileManager) : base(keyWord)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    public override void Execute(params string[] args)
    {
        var items = _FileManager.ItemsList;

        if (items.Length == 0) return;

        var stringBuilder = new StringBuilder();

        foreach (var item in items)
        {
            var size = item.ComputedSize;

            stringBuilder
                .Append(item.Type == CatalogItemType.Catalog ? " - d - " : " - f - ")
                .Append(' ')
                .Append(item.Name)
                .Append(' ')
                .Append(size)
                .Append(' ')
                .Append(size is not null ? "KB" : string.Empty)
                .Append(item != items[items.Length - 1] ? '\n' : string.Empty);
        }
        _FileManager.MessageService.ShowOk(stringBuilder.ToString());
    }
}
