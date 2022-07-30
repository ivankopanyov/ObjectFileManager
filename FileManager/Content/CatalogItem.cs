using FileManager.Services;

namespace FileManager.Content;

/// <summary>Класс, описывающий элемент каталога.</summary>
public abstract class CatalogItem
{
    /// <summary>Недопустимые символы в имени элемента каталога.</summary>
    protected readonly char[] _BadSymbols = new char[] { '\\', '/', '*', ':', '?', '<', '>', '|' };

    /// <summary>Имя элемента каталога.</summary>
    public abstract string Name { get; set; }

    /// <summary>Имя элемента каталога без разрешения.</summary>
    public abstract string NameWithoutExtension { get; }

    /// <summary>Разрешение элемента каталога.</summary>
    public abstract string Exstension { get; }

    /// <summary>Полное имя, включающее путь к элементу каталога.</summary>
    public abstract string FullName { get; }

    /// <summary>Тип элемента каталога.</summary>
    public abstract CatalogItemType Type { get; }

    /// <summary>Тип элемента каталога для отображения в интерфейсе пользователя.</summary>
    public abstract string DisplayType { get; }

    /// <summary>Размер элемента каталога.</summary>
    public abstract long? Size { get; }

    /// <summary>Вычисляемый размер элемента каталога, 
    /// включающий размер всех содержащихся в нем фалов и подкаталогов.</summary>
    public abstract long? ComputedSize { get; }

    /// <summary>Дата и время создания элемента каталога.</summary>
    public abstract DateTime CreateDate { get; }

    /// <summary>Дата и вемя последнего изменения элемента каталога.</summary>
    public abstract DateTime UpdateDate { get; }

    /// <summary>Проверка на существование элемента каталога.</summary>
    public abstract bool Exists { get; }

    /// <summary>Нахождение элемента каталога в режиме только для чтения.</summary>
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

    /// <summary>Нахождение элемента каталога в скрытом режиме.</summary>
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

    /// <summary>Вырезание элемента каталога в буфер обмена.</summary>
    /// <param name="clipboard">Буфер обмена.</param>
    /// <exception cref="ArgumentNullException">Буфер обмена не инициализирован.</exception>
    /// <exception cref="FileNotFoundException">Элемент каталога не найден.</exception>
    public void Cut(IClipboard<string, string> clipboard)
    {
        if (clipboard is null)
            throw new ArgumentNullException(nameof(clipboard));

        if (!Exists)
            throw new FileNotFoundException($"Источник {FullName} не найден!");

        clipboard.Cut(FullName);
    }

    /// <summary>Копирование элемента каталога в буфер обмена.</summary>
    /// <param name="clipboard">Буфер обмена.</param>
    /// <exception cref="ArgumentNullException">Буфер обмена не инициализирован.</exception>
    /// <exception cref="FileNotFoundException">Элемент каталога не найден.</exception>
    public void Copy(IClipboard<string, string> clipboard)
    {
        if (clipboard is null)
            throw new ArgumentNullException(nameof(clipboard));

        if (!Exists)
            throw new FileNotFoundException($"Источник {FullName} не найден!");

        clipboard.Copy(FullName);
    }

    /// <summary>Удаление элемента каталога.</summary>
    public abstract void Remove();

    /// <summary>Получение значения атрибута элемента каталога.</summary>
    /// <param name="attribute">Атрибут.</param>
    /// <returns>Значение атрибута.</returns>
    protected bool HasAttribute(FileAttributes attribute) =>
        (File.GetAttributes(FullName) & attribute) == attribute;

    /// <summary>Изменение значния атрибута элемента каталога.</summary>
    /// <param name="attribute">Атрибут.</param>
    /// <param name="value">Новое значение атрибута.</param>
    protected void ChangeAttribute(FileAttributes attribute, bool value)
    {
        FileAttributes attributes = File.GetAttributes(FullName);
        if ((attributes & attribute) == attribute && !value)
            File.SetAttributes(FullName, attributes & ~attribute);
        else if ((attributes & attribute) != attribute && value)
            File.SetAttributes(FullName, File.GetAttributes(FullName) | attribute);
    }

    /// <summary>Определение типа элемента каталога.</summary>
    /// <param name="path">Путь к элементу каталога.</param>
    /// <returns>Тип элемента каталога.</returns>
    public static CatalogItemType GetItemType(string path)
    {
        if (Directory.Exists(path)) return CatalogItemType.Catalog;

        if (File.Exists(path)) return CatalogItemType.File;

        return CatalogItemType.None;
    }

    /// <summary>Получение элемента каталога по пути.</summary>
    /// <param name="path">Путь к элементу каталога.</param>
    /// <returns>Элемент каталога.</returns>
    public static CatalogItem GetCatalogItem(string path) => GetItemType(path) switch
    {
        CatalogItemType.Catalog => new CICatalog(new DirectoryInfo(path)),
        CatalogItemType.File => new CIFile(new FileInfo(path)),
        _ => null!
    };

    /// <summary>Получение элементов каталога из каталога по укаанному пути.</summary>
    /// <param name="path">Путь к каталогу.</param>
    /// <returns>Элементы, содержащиеся в каталоге.</returns>
    /// <exception cref="ArgumentNullException">Путь не инициализирован или пустой.</exception>
    /// <exception cref="DirectoryNotFoundException">Каталог по указанному пути не найден.</exception>
    /// <exception cref="UnauthorizedAccessException">Нет доступа к содержимому каталога.</exception>
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

    /// <summary>Поиск элементов каталога по заданному фильтру.</summary>
    /// <param name="path">Путь к каталогу для поиска.</param>
    /// <param name="filter">Фильтр для поиска элементов.</param>
    /// <param name="allCatalogs">Поиск по всем подкаталогам.</param>
    /// <returns>Найденные элементы.</returns>
    /// <exception cref="ArgumentNullException">Путь к каталогу или фильтр не инициализированы или пустые.</exception>
    /// <exception cref="DirectoryNotFoundException">Каталог не найден.</exception>
    /// <exception cref="InvalidOperationException">Не удалось произвести поиск по каталогу.</exception>
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

    /// <summary>Создание нового файла.</summary>
    /// <param name="path">Путь к каталогу, в котором будет создан файл.</param>
    /// <exception cref="ArgumentNullException">Путь к каталогу не инциализирован или пустой.</exception>
    /// <exception cref="DirectoryNotFoundException">Каталог не найден.</exception>
    /// <exception cref="InvalidOperationException">Не удалось создать файл.</exception>
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

    /// <summary>Создание нового каталога.</summary>
    /// <param name="path">Путь к каталогу, в котором будет создан новый каталог.</param>
    /// <exception cref="ArgumentNullException">Путь к каталогу не инциализирован или пустой.</exception>
    /// <exception cref="DirectoryNotFoundException">Каталог не найден.</exception>
    /// <exception cref="InvalidOperationException">Не удалось создать новый каталог.</exception>
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

    /// <summary>Копирование директории.</summary>
    /// <param name="source">Директория для копирования.</param>
    /// <param name="dest">новая диретория.</param>
    /// <exception cref="InvalidOperationException">Не удалось скопировать директорию.</exception>
    public static void CopyDirectory(string source, string dest)
    {
        try
        {
            var directory = new DirectoryInfo(source);

            DirectoryInfo[] directories = directory.GetDirectories();

            Directory.CreateDirectory(dest);

            foreach (FileInfo file in directory.GetFiles())
            {
                string path = Path.Combine(dest, file.Name);
                file.CopyTo(path);
            }

            foreach (DirectoryInfo dir in directories)
            {
                string path = Path.Combine(dest, dir.Name);
                CopyDirectory(dir.FullName, path);
            }
        }
        catch
        {
            throw new InvalidOperationException($"Не удалось скопировать папку {source}");
        }
    }
}
