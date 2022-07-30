using ConsoleFileManager.Commands.Base;
using FileManager;
using FileManager.Content;
using FileManager.Services;
using System.Text;

namespace ConsoleFileManager.Commands;

public class CopyCommand : Command
{
    private readonly FileManagerLogic _FileManager;

    private readonly IClipboard<string, string> _Clipboard;

    public override string Description => "Копирование файла или каталога.";

    public CopyCommand(string keyWord, FileManagerLogic fileManager, IClipboard<string, string> clipboard) : base(keyWord)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        if (clipboard is null)
            throw new ArgumentNullException(nameof(clipboard));

        _FileManager = fileManager;
        _Clipboard = clipboard;
    }

    public override void Execute(params string[] args)
    {
        if (args is null || args.Length < 3 || string.IsNullOrWhiteSpace(args[1]) || string.IsNullOrWhiteSpace(args[2]))
        {
            _FileManager.MessageService.ShowError("Не указаны параметры команды!");
            return;
        }

        var catalogItem = CatalogItem.GetCatalogItem(args![1]);

        if (catalogItem == null)
        {
            _FileManager.MessageService.ShowError($"Файл {args![1]} не найден!");
            return;
        }

        _FileManager.Copy(catalogItem, _Clipboard);
        if (_Clipboard.ContainsData)
            _FileManager.Paste(_Clipboard, args![2]);
    }
}
