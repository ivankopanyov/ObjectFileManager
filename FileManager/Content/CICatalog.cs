namespace FileManager.Content;

/// <summary>Класс, описывающий каталог.</summary>
internal sealed class CICatalog : CatalogItem
{
    /// <summary>Директория, описываемая текущим классом.</summary>
    private readonly DirectoryInfo _Directory;

    /// <summary>Имя каталога.</summary>
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
                throw new UnauthorizedAccessException("Нет доступа для переименования папки!");
            }
            catch (DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException("Папка не найден!");
            }
            catch
            {
                throw new InvalidOperationException("Не удалось переименовать папку!");
            }
        }
    }

    /// <summary>Имя каталога без разрешения. Совпадает с именем каталога.</summary>
    public override string NameWithoutExtension => Name;

    /// <summary>Разрешение каталога. Всегда возвращает null.</summary>
    public override string Exstension => null!;

    /// <summary>Полное имя каталога, включающее путь к каталогу.</summary>
    public override string FullName => Path.GetFullPath(_Directory.FullName);

    /// <summary>Тип. Всегда возвращает Catalog.</summary>
    public override CatalogItemType Type => CatalogItemType.Catalog;

    /// <summary>Тип, отображаемый в интерфейсе пользователя.</summary>
    public override string DisplayType => "Папка с файлами";

    /// <summary>Размер каталога. Всегда возвращает null.</summary>
    public override long? Size => null;

    /// <summary>Вычисляемый размер каталога в килобайтах, 
    /// включающий размер всех содержащихся в нем фалов и подкаталогов.</summary>
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

    /// <summary>Дата и время создания каталога.</summary>
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

    /// <summary>Дата и вемя последнего изменения каталога.</summary>
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

    /// <summary>Проверка на существование каталога.</summary>
    public override bool Exists => Directory.Exists(FullName);

    /// <summary>Инициализация объекта каталога.</summary>
    /// <param name="directory">Директория, описываемая текущим классом.</param>
    public CICatalog(DirectoryInfo directory) => _Directory = directory;

    /// <summary>Удаление каталога.</summary>
    /// <exception cref="UnauthorizedAccessException">Нет доступа для удаления каталога.</exception>
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

