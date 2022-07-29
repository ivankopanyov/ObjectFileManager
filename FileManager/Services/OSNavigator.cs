namespace FileManager.Services;

/// <summary>Навигатор по файловой системе Windows.</summary>
public class OSNavigator : INavigator<string>
{
    /// <summary>Объект навигатора.</summary>
    private readonly static OSNavigator _Navigator = new OSNavigator();

    /// <summary>Объект навигатора.</summary>
    public static OSNavigator Navigator => _Navigator;

    /// <summary>Пройденные директории.</summary>
    private readonly Stack<string> _Back = new();

    /// <summary>Следущие директории.</summary>
    private readonly Stack<string> _Forward = new();

    /// <summary>Текущая директория.</summary>
    private DirectoryInfo _CurrentInfo;

    /// <summary>Путь к текущей директории.</summary>
    public string Current
    {
        get => Directory.GetCurrentDirectory();

        private set
        {
            Directory.SetCurrentDirectory(value);
            _CurrentInfo = new DirectoryInfo(Current);
        }
    }

    /// <summary>Предыдущая директория.</summary>
    public string Back => BackExists ? _Back.Peek() : null!;

    /// <summary>Проверка наличия предыдущей директории.</summary>
    public bool BackExists => _Back.Count > 0;

    /// <summary>Следущая директория.</summary>
    public string Forward => ForwardExists ? _Forward.Peek() : null!;

    /// <summary>Проверка наличия следущей директории.</summary>
    public bool ForwardExists => _Forward.Count > 0;

    /// <summary>Родительская директория.</summary>
    public string Up => UpExists ? _CurrentInfo.Parent!.FullName : null!;

    /// <summary>Проверка наличия родительской директории.</summary>
    public bool UpExists => _CurrentInfo.Parent is not null;

    /// <summary>Инициализауия объекта навигатора.</summary>
    private OSNavigator() => _CurrentInfo = new DirectoryInfo(Current);

    /// <summary>Очистка предыдущих директорий.</summary>
    public void ClearBack() => _Back.Clear();

    /// <summary>Очистка следущих директорий.</summary>
    public void ClearForward() => _Forward.Clear();

    /// <summary>Переход в предыдущую диреторию.</summary>
    /// <exception cref="InvalidOperationException">Не удалось перейти в предыдущую диреторию.</exception>
    /// <exception cref="DirectoryNotFoundException">Предыдущая директория не найдена.</exception>
    /// <exception cref="UnauthorizedAccessException">Нет доступа к предыдущей директории.</exception>
    public void GoToBack()
    {
        if (!BackExists)
            throw new InvalidOperationException("История пуста!");

        if (!Directory.Exists(Back))
            throw new DirectoryNotFoundException($"Директория {Back} не найдена!");

        var temp = Current;

        try
        {
            Current = Back;
        }
        catch
        {
            throw new UnauthorizedAccessException($"Директория {Back} не доступна!");
        }

        AddForward(temp);
        _Back.Pop();
    }

    /// <summary>Переход в следущую диреторию.</summary>
    /// <exception cref="InvalidOperationException">Не удалось перейти в следущую диреторию.</exception>
    /// <exception cref="DirectoryNotFoundException">Следущая директория не найдена.</exception>
    /// <exception cref="UnauthorizedAccessException">Нет доступа к следущей директории.</exception>
    public void GoToForward()
    {
        if (!ForwardExists)
            throw new InvalidOperationException("История пуста!");

        if (!Directory.Exists(Forward))
            throw new DirectoryNotFoundException($"Директория {Forward} не найдена!");

        var temp = Current;

        try
        {
            Current = Forward;
        }
        catch
        {
            throw new UnauthorizedAccessException($"Директория {Forward} не доступна!");
        }

        AddBack(temp);
        _Forward.Pop();
    }

    /// <summary>Переход в родительскую диреторию.</summary>
    /// <exception cref="InvalidOperationException">Текущая директория является корневой.</exception>
    /// <exception cref="DirectoryNotFoundException">Директория не найдена.</exception>
    /// <exception cref="UnauthorizedAccessException">Нет доступа к директории.</exception>
    public void GoToUp()
    {
        if (!UpExists)
            throw new InvalidOperationException("Текущая директория является корневой!");

        if (!Directory.Exists(Up))
            throw new DirectoryNotFoundException($"Директория {Up} не найдена!");

        var temp = Current;

        try
        {
            Current = Up;
        }
        catch
        {
            throw new UnauthorizedAccessException($"Директория {Up} не доступна!");
        }

        AddBack(temp);
        ClearForward();
    }

    /// <summary>Переход в указанную директорию.</summary>
    /// <param name="path">Путь к директории.</param>
    /// <param name="rootPath">Указанный путь является корневым.</param>
    /// <exception cref="ArgumentNullException">Путь не инициализирован или пустой.</exception>
    /// <exception cref="DirectoryNotFoundException">Директория по указанному пути не найдена.</exception>
    /// <exception cref="UnauthorizedAccessException">Нет доступа к указанной директории.</exception>
    public void GoTo(string path, bool rootPath = false)
    {
        var temp = Current;

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!rootPath && !Path.IsPathRooted(path))
            Path.GetFullPath(Path.Combine(Current, path));

        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Директория {path} не найдена!");

        try
        {
            Current = path;
        }
        catch
        {
            throw new UnauthorizedAccessException($"Директория {path} не доступна!");
        }

        if (temp == Current) return;

        AddBack(temp);
        if (temp != Current) ClearForward();
    }

    /// <summary>Сохрание предыдущей директории.</summary>
    /// <param name="path">Путь к предыдущей директории.</param>
    private void AddBack(string path)
    {
        if (BackExists && Back == path) return;
        _Back.Push(path);
    }

    /// <summary>Сохрание следущей директории.</summary>
    /// <param name="path">Путь к следущей директории.</param>
    private void AddForward(string path)
    {
        if (ForwardExists && Forward == path) return;
        _Forward.Push(path);
    }
}
