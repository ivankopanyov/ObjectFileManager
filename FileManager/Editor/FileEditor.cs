namespace FileManager.Editor;

/// <summary>Класс, описывающий редактор файлов.</summary>
public class FileEditor : IEditor<string>
{
    /// <summary>Путь к редактируемому файлу.</summary>
    private readonly string _FilePath;

    /// <summary>Содержимое редактируемого файла.</summary>
    private string _FileContent;

    /// <summary>Флаг изменения содержимого редактируемого файла.</summary>
    private bool _UpdateContent;

    /// <summary>Имя редактируемого файла.</summary>
    public string SourceName { get; }

    /// <summary>Содержимое редактируемого файла.</summary>
    public string Content
    {
        get => _FileContent;

        set
        {
            _FileContent = value is null ? string.Empty : value;
            _UpdateContent = true;
        }
    }

    /// <summary>Флаг изменения содержимого редактируемого файла.</summary>
    public bool UpdateContent => _UpdateContent;

    /// <summary>Инициализация объекта редактора файлов.</summary>
    /// <param name="filePath">Путь к редактируемому файлу.</param>
    /// <exception cref="ArgumentNullException">Путь к файлу не инициализирован или пустой.</exception>
    /// <exception cref="FileNotFoundException">Редактируемый файл не найден.</exception>
    public FileEditor(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Файл {filePath} не найден!");

        _FilePath = filePath;
        SourceName = Path.GetFileName(filePath);

        try
        {
            ReadFileContent();
            _UpdateContent = false;
        }
        catch (InvalidOperationException ex)
        {
            throw ex;
        }
    }

    /// <summary>Сохранение изменений содержимого файла.</summary>
    /// <exception cref="InvalidOperationException">Не удалось сохранить изменения.</exception>
    public void Save()
    {
        try
        {
            WriteFileContent();
        }
        catch
        {
            throw new InvalidOperationException("Не удалось сохранить изменения!");
        }
    }

    /// <summary>Чтение данных из редактируемого файла.</summary>
    /// <exception cref="InvalidOperationException">Не удалось получить данные файла.</exception>
    private void ReadFileContent()
    {
        try
        {
            using var reader = new StreamReader(_FilePath);
            Content = reader.ReadToEnd();
        }
        catch
        {
            throw new InvalidOperationException("Не удалось получить содержимое файла!");
        }
    }

    /// <summary>Запись данных в редактируемый файл.</summary>
    /// <exception cref="InvalidOperationException">Не удалось записать данные в файл.</exception>
    private void WriteFileContent()
    {
        try
        {
            using var writer = new StreamWriter(_FilePath);
            writer.Write(Content);
            _UpdateContent = false;
        }
        catch
        {
            throw new InvalidOperationException("Не удалось записать данные в файл!");
        }
    }
}
