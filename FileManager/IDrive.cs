namespace FileManager;

public interface IDrive : ICatalog
{
    string Name { get; }

    long TotalSize { get; }

    long FillSize { get; }

    long FreeSize { get; }
}
