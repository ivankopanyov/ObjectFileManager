namespace FileManager;

public abstract class CatalogItem
{
    public abstract string Name { get; }

    public abstract string NameWithoutExtension { get; }

    public abstract string Exstension { get; }

    public abstract string FullName { get; }

    public abstract string Type { get; }

    public abstract long? Size { get; }

    public abstract long? ComputedSize { get; }

    public abstract DateTime CreateDate { get; }

    public abstract DateTime UpdateDate { get; }

    public bool ReadOnly { get; }

    public bool Hidden { get; }

    public static CatalogItemType GetItemType(string path)
    {
        if (Directory.Exists(path)) return CatalogItemType.Catalog;

        if (File.Exists(path)) return CatalogItemType.File;

        return CatalogItemType.None;
    }
}
