namespace FileManager.Navigation;

public class OSNavigator : INavigator
{
    private static OSNavigator _Navigator = new OSNavigator();

    public static OSNavigator Navigator => _Navigator;

    private DirectoryInfo _CurrentInfo;

    private Stack<string> _Back = new();

    private Stack<string> _Forward = new();

    public string CurrentDirectory
    {
        get => Directory.GetCurrentDirectory();

        private set
        {
            Directory.SetCurrentDirectory(value);
            _CurrentInfo = new DirectoryInfo(CurrentDirectory);
        }
    }

    public string Back => BackExists ? _Back.Peek() : null!;

    public bool BackExists => _Back.Count > 0;

    public string Forward => ForwardExists ? _Forward.Peek() : null!;

    public bool ForwardExists => _Forward.Count > 0;

    public string Up => UpExists ? _CurrentInfo.Parent!.FullName : null!;

    public bool UpExists => _CurrentInfo.Parent is not null;

    private OSNavigator() => _CurrentInfo = new DirectoryInfo(CurrentDirectory);

    public void ClearBack() => _Back.Clear();

    public void ClearForward() => _Forward.Clear();

    public void ToBack()
    {
        if (!BackExists)
            throw new InvalidOperationException("История пуста!");

        if (!Directory.Exists(Back))
            throw new DirectoryNotFoundException($"Директория {Back} не найдена!");

        var temp = CurrentDirectory;

        try
        {
            CurrentDirectory = Back;
        }
        catch
        {
            throw new UnauthorizedAccessException($"Директория {Back} не доступна!");
        }

        AddForward(temp);
        _Back.Pop();
    }

    public void ToForward()
    {
        if (!ForwardExists)
            throw new InvalidOperationException("История пуста!");

        if (!Directory.Exists(Forward))
            throw new DirectoryNotFoundException($"Директория {Forward} не найдена!");

        var temp = CurrentDirectory;

        try
        {
            CurrentDirectory = Forward;
        }
        catch
        {
            throw new UnauthorizedAccessException($"Директория {Forward} не доступна!");
        }

        AddBack(temp);
        _Forward.Pop();
    }

    public void ToUp()
    {
        if (!UpExists)
            throw new InvalidOperationException("Текущая директория является корневой!");

        if (!Directory.Exists(Up))
            throw new DirectoryNotFoundException($"Директория {Up} не найдена!");

        var temp = CurrentDirectory;

        try
        {
            CurrentDirectory = Up;
        }
        catch
        {
            throw new UnauthorizedAccessException($"Директория {Up} не доступна!");
        }

        AddBack(temp);
        ClearForward();
    }

    public void ToPath(string path, bool onlyRootPath = false)
    {
        var temp = CurrentDirectory;

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!onlyRootPath && !Path.IsPathRooted(path))
            Path.GetFullPath(Path.Combine(CurrentDirectory, path));

        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Директория {path} не найдена!");

        try
        {
            CurrentDirectory = path;
        }
        catch
        {
            throw new UnauthorizedAccessException($"Директория {path} не доступна!");
        }

        if (temp == CurrentDirectory) return;

        AddBack(temp);
        if (temp != CurrentDirectory) ClearForward();
    }

    private void AddBack(string path)
    {
        if (BackExists && Back == path) return;
        _Back.Push(path);
    }

    private void AddForward(string path)
    {
        if (ForwardExists && Forward == path) return;
        _Forward.Push(path);
    }
}
