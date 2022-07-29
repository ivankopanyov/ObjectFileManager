using FileManager.Services;

namespace FileManager.Content;

public abstract class CatalogItem
{
    protected readonly char[] _Chars = new char[] { '\\', '/', '*', ':', '?', '<', '>', '|' };

    public abstract string Name { get; set; }

    public abstract string NameWithoutExtension { get; }

    public abstract string Exstension { get; }

    public abstract string FullName { get; }

    public abstract CatalogItemType Type { get; }

    public abstract string DisplayType { get; }

    public abstract long? Size { get; }

    public abstract long? ComputedSize { get; }

    public abstract DateTime CreateDate { get; }

    public abstract DateTime UpdateDate { get; }

    public abstract bool Exists { get; }

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
                throw new InvalidOperationException("Изменение атрибутов не доступно!");
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
                throw new InvalidOperationException("Изменение атрибутов не доступно!");
            }
        }
    }

    public void Cut(IClipboard<string, string> clipboard)
    {
        if (clipboard is null)
            throw new ArgumentNullException(nameof(clipboard));

        if (!Exists)
            throw new FileNotFoundException($"Источник {FullName} не найден!");

        clipboard.Cut(FullName);
    }

    public void Copy(IClipboard<string, string> clipboard)
    {
        if (clipboard is null)
            throw new ArgumentNullException(nameof(clipboard));

        if (!Exists)
            throw new FileNotFoundException($"Источник {FullName} не найден!");

        clipboard.Copy(FullName);
    }

    public abstract void Remove();

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

    public static CatalogItem[] GetCatalogItems(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Директория {path} не существует!");

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
            throw new UnauthorizedAccessException($"Нет доступа к содержимому директории {path}");
        }
    }

    public static CatalogItem[] FindCatalogItems(string path, string filter, bool allCatalogs)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (string.IsNullOrWhiteSpace(filter))
            throw new ArgumentNullException(nameof(filter));

        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Директория {path} не найдена!");

        filter = $"*{filter}*";

        try
        {
            var directory = new DirectoryInfo(path);

            var directories = directory.EnumerateDirectories(filter, allCatalogs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToArray();

            var files = directory.EnumerateFiles(filter, allCatalogs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToArray();

            var catalogItems = new CatalogItem[directories.Length + files.Length];

            for (int i = 0; i < directories.Length; i++)
                catalogItems[i] = new CICatalog(directories[i]);

            for (int i = directories.Length; i < catalogItems.Length; i++)
                catalogItems[i] = new CIFile(files[i - directories.Length]);

            return catalogItems;
        }
        catch
        {
            throw new InvalidOperationException("Поиск по каталогу не доступен!");
        }
    }

    public static void CreateFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Директория {path} не найдена!");

        var name = "Новый текстовый файл";
        var exstansion = ".txt";
        var newName = name;

        for (int i = 2; File.Exists(Path.Combine(path, $"{newName}{exstansion}")); i++)
            newName = $"{name} ({i})";

        try
        {
            File.Create(Path.Combine(path, $"{newName}{exstansion}"));
        }
        catch
        {
            throw new InvalidOperationException("Не удалось создать файл!");
        }
    }

    public static void CreateCatalog(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Директория {path} не найдена!");

        var name = "Новая папка";
        var newName = name;

        for (int i = 2; Directory.Exists(Path.Combine(path, newName)); i++)
            newName = $"{name} ({i})";

        try
        {
            Directory.CreateDirectory(Path.Combine(path, newName));
        }
        catch
        {
            throw new InvalidOperationException("Не удалось создать папку!");
        }
    }
}
