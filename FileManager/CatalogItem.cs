﻿namespace FileManager;

public abstract class CatalogItem
{
    protected char[] _Chars = new char[] { '\\', '/', '*', ':', '?', '<', '>', '|' };

    protected IMessageService _MessageService;

    public abstract string Name { get; set; }

    public abstract string NameWithoutExtension { get; }

    public abstract string Exstension { get; }

    public abstract string FullName { get; }

    public abstract string Type { get; }

    public abstract long? Size { get; }

    public abstract long? ComputedSize { get; }

    public abstract DateTime CreateDate { get; }

    public abstract DateTime UpdateDate { get; }

    public bool ReadOnly { get; }

    public bool Hidden { get; }

    public CatalogItem(IMessageService messageService) => _MessageService = messageService;

    public static CatalogItemType GetItemType(string path)
    {
        if (Directory.Exists(path)) return CatalogItemType.Catalog;

        if (File.Exists(path)) return CatalogItemType.File;

        return CatalogItemType.None;
    }

    public static CatalogItem[] GetCatalogItems(string path, IMessageService messageService = null!)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            if (messageService != null)
                messageService.ShowError("Путь не указан!");
            return new CatalogItem[0];
        }

        if (!Directory.Exists(path))
        {
            if (messageService != null)
                messageService.ShowError($"Директория {path} не существует!");
            return new CatalogItem[0];
        }

        try
        {
            var directory = new DirectoryInfo(path);
            var directories = directory.GetDirectories();
            var files = directory.GetFiles();

            var catalogItems = new CatalogItem[directories.Length + files.Length];

            for (int i = 0; i < directories.Length; i++)
                catalogItems[i] = new CICatalog(directories[i], messageService);

            for (int i = directories.Length; i < catalogItems.Length; i++)
                catalogItems[i] = new CIFile(files[i - directories.Length], messageService);

            return catalogItems;
        }
        catch
        {
            if (messageService != null)
                messageService.ShowError($"Нет доступа к содержимому директории {path}");
            return new CatalogItem[0];
        }
    }

    public static CatalogItem[] FindCatalogItems(string path, string Filter, bool AllCatalogs)
    {
        throw new NotImplementedException();
    }
}
