namespace FileManager;

public interface IClipboard
{
    bool ContainsItems { get; }

    void Cut(IEnumerable<ICatalogItem> items);

    void Copy(IEnumerable<ICatalogItem> items);

    void Paste(string path);
}

