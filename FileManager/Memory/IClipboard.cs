namespace FileManager.Memory;

public interface IClipboard
{
    bool ContainsFiles { get; }

    void Cut(string path);

    void Cut(IEnumerable<string> paths);

    void Copy(string path);

    void Copy(IEnumerable<string> paths);

    void Paste(string path);

    void Clear();
}

