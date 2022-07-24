namespace FileManager;

public interface ICatalog
{
    string Path { get; }

    ICatalogItem[] CatalogItems { get; }

    ICatalogItem[] FindCatalogItems(string Filter, bool AllCatalogs);
}
