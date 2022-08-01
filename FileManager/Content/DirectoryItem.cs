using FileManager.Services;

namespace FileManager.Content;

/// <summary>Класс, описывающий элемент директории.</summary>
public abstract class DirectoryItem
{
    /// <summary>Недопустимые символы в имени элемента директории.</summary>
    protected readonly char[] _BadSymbols = new char[] { '\\', '/', '*', ':', '?', '<', '>', '|' };

    /// <summary>Имя элемента директории.</summary>
    public abstract string Name { get; set; }

    /// <summary>Имя элемента директории без расширения.</summary>
    public abstract string NameWithoutExtension { get; }

    /// <summary>Расширение элемента директории.</summary>
    public abstract string Exstension { get; }

    /// <summary>Полное имя, включающее путь к элементу директории.</summary>
    public abstract string FullName { get; }

    /// <summary>Тип элемента директории.</summary>
    public abstract DirectoryItemType Type { get; }

    /// <summary>Тип элемента директории для отображения в интерфейсе пользователя.</summary>
    public abstract string DisplayType { get; }

    /// <summary>Размер элемента директории.</summary>
    public abstract long? Size { get; }

    /// <summary>Вычисляемый размер элемента директории, 
    /// включающий размер всех содержащихся в нем файлов и сабдиректорий.</summary>
    public abstract long? ComputedSize { get; }

    /// <summary>Дата и время создания элемента директории.</summary>
    public abstract DateTime CreateDate { get; }

    /// <summary>Дата и вемя последнего изменения элемента директории.</summary>
    public abstract DateTime UpdateDate { get; }

    /// <summary>Проверка на существование элемента директории.</summary>
    public abstract bool Exists { get; }

    /// <summary>Нахождение элемента директории в режиме только для чтения.</summary>
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

    /// <summary>Нахождение элемента директории в скрытом режиме.</summary>
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

    /// <summary>Вырезание элемента директории в буфер обмена.</summary>
    /// <param name="clipboard">Буфер обмена.</param>
    /// <exception cref="ArgumentNullException">Буфер обмена не инициализирован.</exception>
    /// <exception cref="FileNotFoundException">Элемент директории не найден.</exception>
    public void Cut(IClipboard<string, string> clipboard)
    {
        if (clipboard is null)
            throw new ArgumentNullException(nameof(clipboard));

        if (!Exists)
            throw new FileNotFoundException($"Источник {FullName} не найден!");

        clipboard.Cut(FullName);
    }

    /// <summary>Копирование элемента директории в буфер обмена.</summary>
    /// <param name="clipboard">Буфер обмена.</param>
    /// <exception cref="ArgumentNullException">Буфер обмена не инициализирован.</exception>
    /// <exception cref="FileNotFoundException">Элемент директории не найден.</exception>
    public void Copy(IClipboard<string, string> clipboard)
    {
        if (clipboard is null)
            throw new ArgumentNullException(nameof(clipboard));

        if (!Exists)
            throw new FileNotFoundException($"Источник {FullName} не найден!");

        clipboard.Copy(FullName);
    }

    /// <summary>Удаление элемента директории.</summary>
    public abstract void Remove();

    /// <summary>Получение значения атрибута элемента директории.</summary>
    /// <param name="attribute">Атрибут.</param>
    /// <returns>Значение атрибута.</returns>
    protected bool HasAttribute(FileAttributes attribute) =>
        (File.GetAttributes(FullName) & attribute) == attribute;

    /// <summary>Изменение значния атрибута элемента директории.</summary>
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

    /// <summary>Определение типа элемента директории.</summary>
    /// <param name="path">Путь к элементу директории.</param>
    /// <returns>Тип элемента директории.</returns>
    public static DirectoryItemType GetItemType(string path)
    {
        if (Directory.Exists(path)) return DirectoryItemType.Directory;

        if (File.Exists(path)) return DirectoryItemType.File;

        return DirectoryItemType.None;
    }

    /// <summary>Получение элемента директории по пути.</summary>
    /// <param name="path">Путь к элементу директории.</param>
    /// <returns>Элемент директории.</returns>
    public static DirectoryItem GetDirectoryItem(string path) => GetItemType(path) switch
    {
        DirectoryItemType.Directory => new CIDirectory(new DirectoryInfo(path)),
        DirectoryItemType.File => new CIFile(new FileInfo(path)),
        _ => null!
    };

    /// <summary>Получение элементов директории из директории по указанному пути.</summary>
    /// <param name="path">Путь к директории.</param>
    /// <returns>Элементы, содержащиеся в директории.</returns>
    /// <exception cref="ArgumentNullException">Путь не инициализирован или пустой.</exception>
    /// <exception cref="DirectoryNotFoundException">Директория по указанному пути не найден.</exception>
    /// <exception cref="UnauthorizedAccessException">Нет доступа к содержимому директории.</exception>
    public static DirectoryItem[] GetDirectoryItems(string path)
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

            var dirItems = new DirectoryItem[directories.Length + files.Length];

            for (int i = 0; i < directories.Length; i++)
                dirItems[i] = new CIDirectory(directories[i]);

            for (int i = directories.Length; i < dirItems.Length; i++)
                dirItems[i] = new CIFile(files[i - directories.Length]);

            return dirItems;
        }
        catch
        {
            throw new UnauthorizedAccessException($"Нет доступа к содержимому директории {path}");
        }
    }

    /// <summary>Поиск элементов директории по заданному фильтру.</summary>
    /// <param name="path">Путь к директории для поиска.</param>
    /// <param name="filter">Фильтр для поиска элементов.</param>
    /// <param name="allDirectorys">Поиск по всем поддиректориям.</param>
    /// <returns>Найденные элементы.</returns>
    /// <exception cref="ArgumentNullException">Путь к директории или фильтр не инициализированы или пустые.</exception>
    /// <exception cref="DirectoryNotFoundException">Директория не найден.</exception>
    /// <exception cref="InvalidOperationException">Не удалось произвести поиск по директории.</exception>
    public static DirectoryItem[] FindDirectoryItems(string path, string filter, bool allDirectorys)
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

            var directories = directory.EnumerateDirectories(filter, allDirectorys ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToArray();

            var files = directory.EnumerateFiles(filter, allDirectorys ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToArray();

            var dirItems = new DirectoryItem[directories.Length + files.Length];

            for (int i = 0; i < directories.Length; i++)
                dirItems[i] = new CIDirectory(directories[i]);

            for (int i = directories.Length; i < dirItems.Length; i++)
                dirItems[i] = new CIFile(files[i - directories.Length]);

            return dirItems;
        }
        catch
        {
            throw new InvalidOperationException("Поиск по директории не доступен!");
        }
    }
}
