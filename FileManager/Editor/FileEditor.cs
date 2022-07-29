namespace FileManager.Editor;

public class FileEditor : IEditor<string>
{
    private readonly string _FilePath;

    private string _FileContent;

    private bool _UpdateContent;

    public string SourceName { get; }

    public string Content
    {
        get => _FileContent;

        set
        {
            _FileContent = value is null ? string.Empty : value;
            _UpdateContent = true;
        }
    }

    public bool UpdateContent => _UpdateContent;

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
