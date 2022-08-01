namespace FileManager.Content;

/// <summary>Класс, описывающий директорию.</summary>
internal sealed class CIDirectory : DirectoryItem
{
    /// <summary>Директория, описываемая текущим классом.</summary>
    private readonly DirectoryInfo _Directory;

    /// <summary>Имя директории.</summary>
    public override string Name
    {
        get => _Directory.Name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            foreach (char c in _BadSymbols)
                if (value.Contains(c))
                    throw new ArgumentException($"Имя файла не должно содержать символы {string.Join(' ', _BadSymbols)}");

            var newName = Path.Combine(_Directory.Parent!.FullName, value);

            if (File.Exists(newName) || Directory.Exists(newName))
                throw new InvalidOperationException($"Файл {newName} уже существует!");

            try
            {
                _Directory.MoveTo(newName);
            }

            catch (Exception ex) when (ex is System.Security.SecurityException || ex is UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("Нет доступа для переименования директории!");
            }
            catch (DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException("Директория не найден!");
            }
            catch
            {
                throw new InvalidOperationException("Не удалось переименовать директорию!");
            }
        }
    }

    /// <summary>Имя директории без расширения. Совпадает с именем директории.</summary>
    public override string NameWithoutExtension => Name;

    /// <summary>Расширение директории. Всегда возвращает null.</summary>
    public override string Exstension => null!;

    /// <summary>Полное имя директории, включающее путь к директории.</summary>
    public override string FullName => Path.GetFullPath(_Directory.FullName);

    /// <summary>Тип. Всегда возвращает Directory.</summary>
    public override DirectoryItemType Type => DirectoryItemType.Directory;

    /// <summary>Тип, отображаемый в интерфейсе пользователя.</summary>
    public override string DisplayType => "Директория";

    /// <summary>Размер директории. Всегда возвращает null.</summary>
    public override long? Size => null;

    /// <summary>Вычисляемый размер директории в килобайтах, 
    /// включающий размер всех содержащихся в нем файлов и сабдиректорий.</summary>
    public override long? ComputedSize
    {
        get
        {
            try
            {
                return (int)_Directory.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length) / 1000;
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>Дата и время создания директории.</summary>
    public override DateTime CreateDate
    {
        get
        {
            try
            {
                return _Directory.CreationTime;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }

    /// <summary>Дата и вемя последнего изменения директории.</summary>
    public override DateTime UpdateDate
    {
        get
        {
            try
            {
                return _Directory.LastWriteTime;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }

    /// <summary>Проверка на существование директории.</summary>
    public override bool Exists => Directory.Exists(FullName);

    /// <summary>Инициализация объекта директории.</summary>
    /// <param name="directory">Директория, описываемая текущим классом.</param>
    public CIDirectory(DirectoryInfo directory) => _Directory = directory;

    /// <summary>Удаление директории.</summary>
    /// <exception cref="UnauthorizedAccessException">Нет доступа для удаления директории.</exception>
    public override void Remove()
    {
        if (!_Directory.Exists) return;

        try
        {
            _Directory.Delete(true);
        }
        catch
        {
            throw new UnauthorizedAccessException("Нет доступа для удаления директории!");
        }
    }
}

