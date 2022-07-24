namespace FileManager;

public interface ICatalogItem
{
    string Name { get; set; }

    string NameWithoutExtension { get; }

    string Exstension { get; }

    string Path { get; }

    string FullName { get; }

    string DisplayType { get; }

    CatalogItemType Type { get; }

    long? Size { get; }

    long? ComputedSize { get; }

    DateTime CreateDate { get; }

    DateTime UpdateTime { get; }

    bool ReadOnly { get; set; }

    bool Hidden { get; set; }

    void Remove();
}
