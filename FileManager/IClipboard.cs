namespace FileManager;

public interface IClipboard
{
    bool ContainsItems { get; }

    void Cut(CatalogItem item);

    void Cut(IEnumerable<CatalogItem> items);

    void Copy(CatalogItem item);

    void Copy(IEnumerable<CatalogItem> items);

    void Paste(string path, IMessageService messageService = null!);

    void Clear();
}

