namespace FileManager.Editor;

public interface IEditor<T>
{
    string SourceName { get; }

    T Content { get; set; }

    bool UpdateContent { get; }

    void Save();
}
