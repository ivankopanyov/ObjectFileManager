namespace FileManager.Content;

internal sealed class CICatalog : CatalogItem
{
    private readonly DirectoryInfo _Directory;

    public override string Name
    {
        get => _Directory.Name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            foreach (char c in _Chars)
                if (value.Contains(c))
                    throw new ArgumentException($"Имя файла не должно содержать символы {string.Join(' ', _Chars)}");

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

    public override string NameWithoutExtension => Name;

    public override string Exstension => null!;

    public override string FullName => Path.GetFullPath(_Directory.FullName);

    public override CatalogItemType Type => CatalogItemType.Catalog;

    public override string DisplayType => "Папка с файлами";

    public override long? Size => null;

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

    public override bool Exists => Directory.Exists(FullName);

    public CICatalog(DirectoryInfo directory) => _Directory = directory;

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

