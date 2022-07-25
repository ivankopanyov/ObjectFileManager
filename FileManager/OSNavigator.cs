namespace FileManager;

public class OSNavigator : INavigator
{
    private static OSNavigator _OSNavigator = new OSNavigator();

    public static OSNavigator Navigator => _OSNavigator;

    private DirectoryInfo _CurrentInfo;

    private Stack<string> _Back = new();

    private Stack<string> _Forward = new();

    public string Current
    {
        get => Directory.GetCurrentDirectory();

        private set
        {
            Directory.SetCurrentDirectory(value);
            _CurrentInfo = new DirectoryInfo(Current);
        }
    }

    public string CurrentName => _CurrentInfo.Name;

    public string Back => BackExists ? _Back.Peek() : null!;

    public bool BackExists => _Back.Count > 0;

    public string Forward => ForwardExists ? _Forward.Peek() : null!;

    public bool ForwardExists => _Forward.Count > 0;

    public string Up => UpExists ? _CurrentInfo.Parent!.FullName : null!;

    public bool UpExists => _CurrentInfo.Parent is not null;

    private OSNavigator() => _CurrentInfo = new DirectoryInfo(Current);

    public void ClearBack() => _Back.Clear();

    public void ClearForward() => _Forward.Clear();

    public void ToBack(IMessageService messageService = null!)
    {
        if (!BackExists)
        {
            messageService.ShowError("История пуста!");
            return;
        }

        var temp = Current;

        if (!ToDirectory(Back, messageService)) return;

        AddForward(temp);
        _Back.Pop();
    }

    public void ToForward(IMessageService messageService = null!)
    {
        if (!ForwardExists)
        {
            messageService.ShowError("История пуста!");
            return;
        }

        var temp = Current;

        if (!ToDirectory(Forward, messageService)) return;

        AddBack(temp);
        _Forward.Pop();
    }

    public void ToUp(IMessageService messageService = null!)
    {
        if (!UpExists)
        {
            messageService.ShowError("Текущая директория является корневой!");
            return;
        }

        var temp = Current;

        if (!ToDirectory(Up, messageService)) return;

        AddBack(temp);
        ClearForward();
    }

    public bool ToPath(string path, IMessageService messageService = null!)
    {
        var temp = Current;

        if (!ToDirectory(path, messageService)) return false;

        if (temp == Current) return true;

        AddBack(temp);
        if (temp != Current) ClearForward();

        return true;
    }

    private bool ToDirectory(string path, IMessageService messageService)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            if (messageService != null)
                messageService.ShowError("Путь не указан!");
            return false;
        }

        if (!Directory.Exists(path))
        {
            if (messageService != null)
                messageService.ShowError($"Директория {path} не существует!");
            return false;
        }

        try
        {
            Current = path;
            return true;
        }
        catch
        {
            if (messageService != null)
                messageService.ShowError($"Директория {path} не доступна!");
            return false;
        }
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
