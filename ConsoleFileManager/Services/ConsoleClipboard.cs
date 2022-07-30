using FileManager.Services;

namespace ConsoleFileManager.Services;

public class ConsoleClipboard : IClipboard<string, string>
{
    private string[] _Clipdoard = new string[0];

    private bool _IsMove;

    public bool ContainsData => _Clipdoard is not null && _Clipdoard.Length > 0;

    public void Clear() => _Clipdoard = new string[0];

    public void Copy(string path)
    {
        Clear();

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!File.Exists(path) && !Directory.Exists(path))
            throw new FileNotFoundException($"Источник {path} не найден!");

        _Clipdoard = new[] { path };
        _IsMove = false;
    }

    public void Copy(IEnumerable<string> paths)
    {
        Clear();

        if (paths is null)
            throw new ArgumentNullException(nameof(paths));

        var array = paths.ToArray();

        if (array.Length == 0)
            throw new ArgumentNullException(nameof(paths));

        foreach (var path in array)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Перечисление содержит пустой элемент.");
            else if (!File.Exists(path) && !Directory.Exists(path))
                throw new FileNotFoundException($"Источник {path} не найден!");
        }

        _Clipdoard = array;
        _IsMove = false;
    }

    public void Cut(string path)
    {
        Clear();

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!File.Exists(path) && !Directory.Exists(path))
            throw new FileNotFoundException($"Источник {path} не найден!");

        _Clipdoard = new[] { path };
        _IsMove = true;
    }

    public void Cut(IEnumerable<string> paths)
    {
        Clear();

        if (paths is null)
            throw new ArgumentNullException(nameof(paths));

        var array = paths.ToArray();

        if (array.Length == 0)
            throw new ArgumentNullException(nameof(paths));

        foreach (var path in array)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Перечисление содержит пустой элемент.");
            else if (!File.Exists(path) && !Directory.Exists(path))
                throw new FileNotFoundException($"Источник {path} не найден!");
        }

        _Clipdoard = array;
        _IsMove = true;
    }

    public void Paste(string path)
    {
        if (!ContainsData)
            throw new ArgumentNullException(nameof(path));

        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Путь {path} не найден.");

        foreach (var file in _Clipdoard)
            if (File.Exists(file))
            {
                try
                {
                    PasteFile(file, Path.Combine(path, new FileInfo(file).Name));
                }
                catch (InvalidOperationException ex)
                {
                    throw ex;
                }
            }
            else if (Directory.Exists(file))
            {
                try
                {
                    PasteDirectory(file, Path.Combine(path, new DirectoryInfo(file).Name));
                }
                catch (InvalidOperationException ex)
                {
                    throw ex;
                }
            }
            else
                throw new InvalidOperationException("Не удалось скопировать файлы!");
    }

    /// <summary>Вставка файла.</summary>
    /// <param name="source">Вставляемый файл.</param>
    /// <param name="dest">Новый файл.</param>
    /// <param name="isMove">Перемещение файла.</param>
    /// <exception cref="InvalidOperationException">Не удалось вставить файл.</exception>
    private void PasteFile(string source, string dest)
    {
        var file = new FileInfo(dest);
        var extansion = file.Extension;
        var name = Path.Combine(file.Directory!.FullName, Path.GetFileNameWithoutExtension(dest));

        for (int i = 2; File.Exists($"{name}{extansion}"); i++)
            name += " — копия";

        name += extansion;

        try
        {
            if (_IsMove) File.Move(source, name);
            else File.Copy(source, name);
        }
        catch
        {
            throw new InvalidOperationException($"Не удалось скопировать файл {source}");
        }
    }

    /// <summary>Вставка каталога.</summary>
    /// <param name="source">Вставляемый каталог.</param>
    /// <param name="dest">Новый каталог.</param>
    /// <exception cref="InvalidOperationException">Не удалось вставить каталог.</exception>
    private void PasteDirectory(string source, string dest)
    {
        if (source == dest) return;

        try
        {
            if (_IsMove) Directory.Move(source, dest);
            else CopyDirectory(source, dest);
        }
        catch
        {
            throw new InvalidOperationException($"Не удалось скопировать папку {source}");
        }
    }

    /// <summary>Копирование директории.</summary>
    /// <param name="source">Директория для копирования.</param>
    /// <param name="dest">новая диретория.</param>
    /// <exception cref="InvalidOperationException">Не удалось скопировать директорию.</exception>
    static void CopyDirectory(string source, string dest)
    {
        try
        {
            var directory = new DirectoryInfo(source);

            DirectoryInfo[] directories = directory.GetDirectories();

            Directory.CreateDirectory(dest);

            foreach (FileInfo file in directory.GetFiles())
            {
                string path = Path.Combine(dest, file.Name);
                file.CopyTo(path);
            }

            foreach (DirectoryInfo dir in directories)
            {
                string path = Path.Combine(dest, dir.Name);
                CopyDirectory(dir.FullName, path);
            }
        }
        catch
        {
            throw new InvalidOperationException($"Не удалось скопировать папку {source}");
        }
    }
}
