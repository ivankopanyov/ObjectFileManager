namespace FileManager;

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

    public bool ReadOnly
    {
        get
        {
            try
            {
                return HasAttribute(FileAttributes.ReadOnly);
            }
            catch
            {
                return false;
            }
        }

        set
        {
            try
            {
                ChangeAttribute(FileAttributes.ReadOnly, value);
            }
            catch
            {
                if (_MessageService is not null)
                    _MessageService.ShowError("Изменение атрибутов не доступно!");
            }
        }
    }

    public bool Hidden
    {
        get
        {
            try
            {
                return HasAttribute(FileAttributes.Hidden);
            }
            catch
            {
                return false;
            }
        }

        set
        {
            try
            {
                ChangeAttribute(FileAttributes.Hidden, value);
            }
            catch
            {
                if (_MessageService is not null)
                    _MessageService.ShowError("Изменение атрибутов не доступно!");
            }
        }
    }

    public CatalogItem(IMessageService messageService) => _MessageService = messageService;

    public abstract bool Remove();

    protected bool HasAttribute(FileAttributes attribute) =>
        (File.GetAttributes(FullName) & attribute) == attribute;

    protected void ChangeAttribute(FileAttributes attribute, bool value)
    {
        FileAttributes attributes = File.GetAttributes(FullName);
        if ((attributes & attribute) == attribute && !value)
            File.SetAttributes(FullName, attributes & ~attribute);
        else if ((attributes & attribute) != attribute && value)
            File.SetAttributes(FullName, File.GetAttributes(FullName) | attribute);
    }

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

    public static bool CreateFile(string path, IMessageService messageService = null!)
    {
        if (!Directory.Exists(path))
        {
            if (messageService is not null)
                messageService.ShowError($"Не удалось создать файл!");
            return false;
        }

        var name = "Новый текстовый файл";
        var exstansion = ".txt";
        var newName = name;

        for (int i = 2; File.Exists(Path.Combine(path, $"{newName}{exstansion}")); i++)
            newName = $"{name} ({i})";

        try
        {
            File.Create(Path.Combine(path, $"{newName}{exstansion}"));
            return true;
        }
        catch
        {
            if (messageService is not null)
                messageService.ShowError($"Не удалось создать файл!");
            return false;
        }
    }

    public static bool CreateCatalog(string path, IMessageService messageService = null!)
    {
        if (!Directory.Exists(path))
        {
            if (messageService is not null)
                messageService.ShowError($"Не удалось создать папку!");
            return false;
        }

        var name = "Новая папка";
        var newName = name;

        for (int i = 2; Directory.Exists(Path.Combine(path, newName)); i++)
            newName = $"{name} ({i})";

        try
        {
            Directory.CreateDirectory(Path.Combine(path, newName));
            return true;
        }
        catch
        {
            if (messageService is not null)
                messageService.ShowError($"Не удалось создать папку!");
            return false;
        }
    }
}
