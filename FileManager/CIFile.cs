namespace FileManager;

public class CIFile : CatalogItem
{
    private FileInfo _File;

    public override string Name => _File.Name;

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

    internal CIFile(FileInfo file) => _File = file;
}
