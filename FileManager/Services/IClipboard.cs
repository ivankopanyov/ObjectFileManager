namespace FileManager.Services;

public interface IClipboard<TValue, TDest>
{
    bool ContainsFiles { get; }

    void Cut(TValue value);

    void Cut(IEnumerable<TValue> values);

    void Copy(TValue value);

    void Copy(IEnumerable<TValue> values);

    void Paste(TDest dest);

    void Clear();
}

