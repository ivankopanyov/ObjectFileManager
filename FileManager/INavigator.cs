namespace FileManager;

public interface INavigator
{
    string Current { get; }

    string CurrentName { get; }

    string Back { get; }

    bool BackExists { get; }

    string Forward { get; }

    bool ForwardExists { get; }

    bool ToPath(string path, bool showMessage = true);

    void ToUp();

    void ToBack();

    void ToForward();

    void ClearBack();

    void ClearForward();
}
