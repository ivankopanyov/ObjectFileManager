namespace FileManager.Services;

public interface INavigator
{
    string CurrentDirectory { get; }

    bool BackExists { get; }

    bool ForwardExists { get; }

    bool UpExists { get; }

    void ToPath(string path, bool onlyRootPath = false);

    void ToUp();

    void ToBack();

    void ToForward();
}
