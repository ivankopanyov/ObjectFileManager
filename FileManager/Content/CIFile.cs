namespace FileManager.Content;

/// <summary>Класс, описывающий файл.</summary>
internal sealed class CIFile : CatalogItem
{
    /// <summary>Файл, описываемый текущим классом.</summary>
    private readonly FileInfo _File;

    /// <summary>Имя файла.</summary>
    public override string Name
    {
        get => _File.Name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            foreach (char c in _BadSymbols)
                if (value.Contains(c))
                    throw new ArgumentException($"Имя файла не должно содержать символы {string.Join(' ', _BadSymbols)}");

            var newName = Path.Combine(_File.Directory!.FullName, value);

            if (File.Exists(newName) || Directory.Exists(newName))
                throw new InvalidOperationException($"Файл {newName} уже существует!");

            try
            {
                _File.MoveTo(newName);
            }
            catch (Exception ex) when (ex is System.Security.SecurityException || ex is UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("Нет доступа для переименования файла!");
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Файл не найден!");
            }
            catch
            {
                throw new InvalidOperationException("Не удалось переименовать файл!");
            }
        }
    }

    /// <summary>Имя файла без расширения.</summary>
    public override string NameWithoutExtension => Path.GetFileNameWithoutExtension(_File.Name);

    /// <summary>Расширение файла.</summary>
    public override string Exstension => _File.Extension.TrimStart('.').ToUpper();

    /// <summary>Полное имя файла, включающее путь к файлу.</summary>
    public override string FullName => _File.FullName;

    /// <summary>Тип. Всегда возвращает File.</summary>
    public override CatalogItemType Type => CatalogItemType.File;

    /// <summary>Тип, отображаемый в интерфейсе пользователя.</summary>
    public override string DisplayType => $"Файл \"{Exstension}\"";

    /// <summary>Размер файла в килобайтах.</summary>
    public override long? Size => _File.Length / 1000;

    /// <summary>Вычисляемый размер файла в килобайтах. 
    /// Совпадает с размером файла.</summary>
    public override long? ComputedSize => Size;

    /// <summary>Дата и время создания файла.</summary>
    public override DateTime CreateDate
    {
        get
        {
            try
            {
                return _File.CreationTime;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }

    /// <summary>Дата и вемя последнего изменения файла.</summary>
    public override DateTime UpdateDate
    {
        get
        {
            try
            {
                return _File.LastWriteTime;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }

    /// <summary>Проверка на существование файла.</summary>
    public override bool Exists => File.Exists(FullName);

    /// <summary>Инициализация объекта файла.</summary>
    /// <param name="directory">Файл, описываемый текущим классом.</param>
    public CIFile(FileInfo file) => _File = file;

    /// <summary>Удаление файла.</summary>
    /// <exception cref="UnauthorizedAccessException">Нет доступа для удаления файла.</exception>
    public override void Remove()
    {
        if (!_File.Exists) return;

        try
        {
            _File.Delete();
        }
        catch
        {
            throw new UnauthorizedAccessException("Нет доступа для удаления файла!");
        }
    }
}
