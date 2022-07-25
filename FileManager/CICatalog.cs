namespace FileManager;

public class CICatalog : CatalogItem
{
    private DirectoryInfo _Directory;

    public override string Name => _Directory.Name;

    public override string NameWithoutExtension => Name;

    public override string Exstension => null!;

    public override string FullName => _Directory.FullName;

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

    public CICatalog(DirectoryInfo directory) => _Directory = directory;
}
