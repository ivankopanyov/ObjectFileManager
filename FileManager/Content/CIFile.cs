namespace FileManager.Content;

internal sealed class CIFile : CatalogItem
{
    private readonly FileInfo _File;

    public override string Name
    {
        get => _File.Name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            foreach (char c in _Chars)
                if (value.Contains(c))
                    throw new ArgumentException($"Имя файла не должно содержать символы {string.Join(' ', _Chars)}");

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

    public override string NameWithoutExtension => Path.GetFileNameWithoutExtension(_File.Name);

    public override string Exstension => _File.Extension.TrimStart('.').ToUpper();

    public override string FullName => _File.FullName;

    public override CatalogItemType Type => CatalogItemType.File;

    public override string DisplayType => $"Файл \"{Exstension}\"";

    public override long? Size => _File.Length / 1000;

    public override long? ComputedSize => Size;

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

    public override bool Exists => File.Exists(FullName);

    public CIFile(FileInfo file) => _File = file;

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
