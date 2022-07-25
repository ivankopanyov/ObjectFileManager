namespace FileManager;

public class Catalog
{
    private string _Path;

    public string Path => _Path;

    public Catalog(string path) => _Path = path;

    public CatalogItem[] GetCatalogItems(IMessageService messageService = null!) => 
        GetCatalogItems(_Path, messageService);

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
                catalogItems[i] = new CICatalog(directories[i]);

            for (int i = directories.Length; i < catalogItems.Length; i++)
                catalogItems[i] = new CIFile(files[i - directories.Length]);

            return catalogItems;
        }
        catch
        {
            if (messageService != null)
                messageService.ShowError($"Нет доступа к содержимому директории {path}");
            return new CatalogItem[0];
        }
    }

    public CatalogItem[] FindCatalogItems(string Filter, bool AllCatalogs)
    {
        throw new NotImplementedException();
    }
}
