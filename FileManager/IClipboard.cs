namespace FileManager;

public interface IClipboard
{
    bool ContainsItems { get; }

    void Cut(IEnumerable<CatalogItem> items);

    void Copy(IEnumerable<CatalogItem> items);

    void Paste(string path);
}

