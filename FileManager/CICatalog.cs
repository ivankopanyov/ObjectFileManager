namespace FileManager;

public class CICatalog : CatalogItem
{
    private DirectoryInfo _Directory;

    public override string Name
    { 
        get => _Directory.Name;
        set 
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (_MessageService is not null)
                    _MessageService.ShowError("Имя директории не должно быть пустым!");
                return;
            }

            foreach (char c in _Chars)
                if (value.Contains(c))
                {
                    if (_MessageService is not null)
                        _MessageService.ShowError($"Имя диретории не должно содержать символы {string.Join(' ', _Chars)}");
                    return;
                }

            var newName = Path.Combine(_Directory.Parent!.FullName, value);

            if (File.Exists(newName) || Directory.Exists(newName))
            {
                if (_MessageService is not null)
                    _MessageService.ShowError($"Директория {newName} уже существует!");
                return;
            }

            try
            {
                _Directory.MoveTo(newName);
            }
            catch (Exception ex) when (ex is System.Security.SecurityException || ex is UnauthorizedAccessException)
            {
                if (_MessageService is not null)
                    _MessageService.ShowError("Нет доступа для переименования директории!");
                return;
            }
            catch (DirectoryNotFoundException)
            {
                if (_MessageService is not null)
                    _MessageService.ShowError("Директория не найден!");
                return;
            }
            catch
            {
                if (_MessageService is not null)
                    _MessageService.ShowError("Не удалось переименовать директорию!");
                return;
            }
        }
    }

    public override string NameWithoutExtension => Name;

    public override string Exstension => null!;

    public override string FullName => Path.GetFullPath(_Directory.FullName);

    public override string Type => "Папка с файлами";

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

    internal CICatalog(DirectoryInfo directory, IMessageService messageService = null!) : base(messageService) => _Directory = directory;
}

