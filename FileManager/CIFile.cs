namespace FileManager;

public class CIFile : CatalogItem
{
    private FileInfo _File;

    public override string Name
    { 
        get => _File.Name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (_MessageService is not null)
                    _MessageService.ShowError("Имя файла не должно быть пустым!");
                return;
            }

            foreach (char c in _Chars)
                if (value.Contains(c))
                {
                    if (_MessageService is not null)
                        _MessageService.ShowError($"Имя файла не должно содержать символы {string.Join(' ', _Chars)}");
                    return;
                }

            var newName = Path.Combine(_File.Directory!.FullName, value);

            if (File.Exists(newName) || Directory.Exists(newName))
            {
                if (_MessageService is not null)
                    _MessageService.ShowError($"Файл {newName} уже существует!");
                return;
            }

            try
            {
                _File.MoveTo(newName);
            }
            catch (Exception ex) when (ex is System.Security.SecurityException || ex is UnauthorizedAccessException)
            {
                if (_MessageService is not null)
                    _MessageService.ShowError("Нет доступа для переименования файла!");
                return;
            }
            catch (FileNotFoundException)
            {
                if (_MessageService is not null)
                    _MessageService.ShowError("Файл не найден!");
                return;
            }
            catch
            {
                if (_MessageService is not null)
                    _MessageService.ShowError("Не удалось переименовать файл!");
                return;
            }
        }
    }

    public override string NameWithoutExtension => Path.GetFileNameWithoutExtension(_File.Name);

    public override string Exstension => _File.Extension.TrimStart('.').ToUpper();

    public override string FullName => _File.FullName;

    public override string Type => $"Файл \"{Exstension}\"";

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

    internal CIFile(FileInfo file, IMessageService messageService = null!) : base(messageService) => _File = file;

    public override bool Remove()
    {
        if (!_File.Exists) return true;

        try
        {
            _File.Delete();
            return true;
        }
        catch
        {
            if (_MessageService is not null)
                _MessageService.ShowError("Нет доступа для удаления файла!");
            return false;
        }
    }
}
