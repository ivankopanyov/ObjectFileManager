using FileManager.Content;
using FileManager.Services;
using FileManager.Extension;

namespace FileManager;

/// <summary>Класс, описывающий логику работы файлового менеджера.</summary>
public class FileManagerLogic : IGuiFileManager, IConsoleFileManager
{
    /// <summary>Навигатор по директориям.</summary>
    private readonly INavigator<string> _Navigator;

    /// <summary>Системные диски.</summary>
    private readonly CIDrive[] _Drives;

    /// <summary>Сервис вывода сообщений в пользотельский интерфейс.</summary>
    public IMessageService MessageService { get; private set; }

    /// <summary>Текущая директория.</summary>
    public string CurrentDirectory => _Navigator.Current;

    /// <summary>Элементы текущей директории.</summary>
    public DirectoryItem[] ItemsList
    {
        get 
        {
            try
            {
                return DirectoryItem.GetDirectoryItems(_Navigator.Current);
            }
            catch (Exception ex)
            {
                if (MessageService is not null)
                    MessageService.ShowError(ex.Message);
                return new DirectoryItem[0];
            }
        }
    }

    /// <summary>Системные диски.</summary>
    public CIDrive[] Drives => _Drives;

    /// <summary>Проверка налиция текущей директории.</summary>
    public bool BackExists => _Navigator.BackExists;

    /// <summary>Проверка наличия следущей директории.</summary>
    public bool ForwardExists => _Navigator.ForwardExists;

    /// <summary>Проверка наличия родительской директории.</summary>
    public bool UpExists => _Navigator.UpExists;

    /// <summary>Инициализация объекта файлового менеджера.</summary>
    /// <param name="navigator">Навигатор по директориям.</param>
    /// <param name="messageService">Сервис вывода сообщений в пользотельский интерфейс.</param>
    /// <exception cref="ArgumentNullException">Навигатор не инициализирован.</exception>
    public FileManagerLogic(INavigator<string> navigator, IMessageService messageService)
    { 
        if (navigator is null)
            throw new ArgumentNullException(nameof(navigator));

        _Navigator = navigator;
        MessageService = messageService; 
        _Drives = CIDrive.GetDrives();
    }

    /// <summary>Изменение текущей директории.</summary>
    /// <param name="direction">Направление для перехода.</param>
    /// <returns>Новая директория.</returns>
    public string ChangeDirectory(NavigatorDirection direction)
    {
        try
        {
            switch (direction)
            {
                case NavigatorDirection.Up: _Navigator.GoToUp(); break;
                case NavigatorDirection.Back: _Navigator.GoToBack(); break;
                case NavigatorDirection.Forward: _Navigator.GoToForward(); break;
            }
        }
        catch (Exception ex)
        {
            if (MessageService is not null)
                MessageService.ShowError(ex.Message);
        }
        
        return _Navigator.Current;
    }

    /// <summary>Изменение текущей директории.</summary>
    /// <param name="path">Путь для перехода.</param>
    /// <returns>Новая директория.</returns>
    /// <exception cref="ArgumentNullException">Путь не инициализирован.</exception>
    public string ChangeDirectory(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            if (MessageService is not null)
                MessageService.ShowError("Путь не указан!");
            throw new ArgumentNullException(nameof(path));
        }

        try
        {
            _Navigator.GoTo(path, true);
        }
        catch (Exception ex)
        {
            if (MessageService is not null)
                MessageService.ShowError(ex.Message);
        }

        return _Navigator.Current;
    }

    /// <summary>Переименование элемента директории.</summary>
    /// <param name="item">Элемент директории для переименования.</param>
    /// <param name="name">Новое имя элемента директории.</param>
    /// <exception cref="ArgumentNullException">Элемент директории не инициализирован, 
    /// или имя не инициализировано или пустое.</exception>
    /// <exception cref="InvalidOperationException">Не удалось переименовать элемент.</exception>
    public void Rename(DirectoryItem item, string name)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));

        if (string.IsNullOrWhiteSpace(name)) return;

        try
        {
            item.Name = name;
        }
        catch (Exception ex)
        {
            if (MessageService is not null)
                MessageService.ShowError(ex.Message);

            throw new InvalidOperationException(ex.Message);
        }
    }

    /// <summary>Изменение значения атрибута элемента директории.</summary>
    /// <param name="item">Элемент директории.</param>
    /// <param name="attribute">Изменяемый атрибут.</param>
    /// <param name="value">Новое значение изменяемого атрибута.</param>
    /// <exception cref="ArgumentNullException">Элемент директории не инициализирован.</exception>
    /// <exception cref="ArgumentException">Изенение указанного атрибута не поддерживается.</exception>
    public void ChangeAttribute(DirectoryItem item, FileAttributes attribute, bool value)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));

        try
        {
            switch (attribute)
            {
                case FileAttributes.ReadOnly: item.ReadOnly = value; break;
                case FileAttributes.Hidden: item.Hidden = value; break;
                default: throw new ArgumentException($"Изменение аттрибута {attribute} не поддерживается!", nameof(attribute));
            }
        }
        catch (InvalidOperationException ex)
        {
            if (MessageService is not null)
                MessageService.ShowError(ex.Message);
        }
    }

    /// <summary>Вырезание элемента директории в буфер обмена.</summary>
    /// <param name="item">Элемент директории.</param>
    /// <param name="clipboard">Буфер обмена.</param>
    /// <exception cref="ArgumentNullException">Элемент директории или буфер обмена не инициализирован.</exception>
    public void Cut(DirectoryItem item, IClipboard<string, string> clipboard)
    {
        if (clipboard is null)
            throw new ArgumentNullException(nameof(clipboard));

        if (item is null)
            throw new ArgumentNullException(nameof(item));

        if (!item.Exists)
        {
            if (MessageService is not null)
                MessageService.ShowError($"Источник {item.FullName} не найден!");
            return;
        }

        item.Cut(clipboard);
    }

    /// <summary>Копирование элемента директории в буфер обмена.</summary>
    /// <param name="item">Элемент директории.</param>
    /// <param name="clipboard">Буфер обмена.</param>
    /// <exception cref="ArgumentNullException">Элемент директории или буфер обмена не инициализирован.</exception>
    public void Copy(DirectoryItem item, IClipboard<string, string> clipboard)
    {
        if (clipboard is null)
            throw new ArgumentNullException(nameof(clipboard));

        if (item is null)
            throw new ArgumentNullException(nameof(item));

        if (!item.Exists)
        {
            if (MessageService is not null)
                MessageService.ShowError($"Источник {item.FullName} не найден!");
            return;
        }

        item.Copy(clipboard);
    }

    /// <summary>Копирование элемента директории.</summary>
    /// <param name="source">Путь к копируемому элементу.</param>
    /// <param name="dest">Путь для копирования.</param>
    /// <exception cref="ArgumentNullException">Параметр не инициализирован или пустой.</exception>
    public void Copy(string source, string dest)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (dest is null)
            throw new ArgumentNullException(nameof(dest));

        if (!Path.IsPathRooted(source))
            source = Path.GetFullPath(Path.Combine(CurrentDirectory, source));

        if (!Path.IsPathRooted(dest))
            dest = Path.GetFullPath(Path.Combine(CurrentDirectory, dest));

        if (!File.Exists(source) && !Directory.Exists(source))
        {
            MessageService.ShowError($"Файл {source} не найден!");
            return;
        }

        if (File.Exists(dest) || Directory.Exists(dest))
        {
            MessageService.ShowError($"Файл {dest} уже существует!");
            return;
        }

        try
        {
            if (File.Exists(source))
            {
                File.Copy(source, dest);
                MessageService.ShowOk($"Файл {source} успешно скопирован!");
            }
            else
            {
                new DirectoryInfo(source).Copy(dest);
                MessageService.ShowOk($"Директория {source} успешно скопирована!");
            }
        }
        catch
        {
            MessageService.ShowError($"Не удалось скопировать файл {source}");
            return;
        }
    }

    /// <summary>Перемещение элемента директории.</summary>
    /// <param name="source">Путь к перемещаемому элементу.</param>
    /// <param name="dest">Путь для перемещения.</param>
    /// <exception cref="ArgumentNullException">Параметр не инициализирован или пустой.</exception>
    public void Move(string source, string dest)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (dest is null)
            throw new ArgumentNullException(nameof(dest));

        if (!Path.IsPathRooted(source))
            source = Path.GetFullPath(Path.Combine(CurrentDirectory, source));

        if (!Path.IsPathRooted(dest))
            dest = Path.GetFullPath(Path.Combine(CurrentDirectory, dest));

        if (!File.Exists(source) && !Directory.Exists(source))
        {
            MessageService.ShowError($"Файл {source} не найден!");
            return;
        }

        if (File.Exists(dest) || Directory.Exists(dest))
        {
            MessageService.ShowError($"Файл {dest} уже существует!");
            return;
        }

        try
        {
            if (File.Exists(source))
            {
                File.Move(source, dest);
                MessageService.ShowOk($"Файл {source} успешно перемещен!");
            }
            else
            {
                Directory.Move(source, dest);
                MessageService.ShowOk($"Директория {source} успешно перемещена!");
            }
        }
        catch
        {
            MessageService.ShowError($"Не удалось переместить файл {source}");
            return;
        }
    }

    /// <summary>Вставка элементов директории из буфера обмена.</summary>
    /// <param name="clipboard">Буфер обмена.</param>
    /// <param name="path">Путь директории для вставки.</param>
    /// <exception cref="ArgumentNullException">Путь или буфер обмена не инициализирован.</exception>
    public void Paste(IClipboard<string, string> clipboard, string path)
    {
        if (clipboard is null)
            throw new ArgumentNullException(nameof(clipboard));

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        try
        {
            clipboard.Paste(path);
        }
        catch (Exception ex)
        {
            if (MessageService is not null)
                MessageService.ShowError(ex.Message);
        }
    }

    /// <summary>Удаление элемента директории.</summary>
    /// <param name="item">Удаляемый элемент директории.</param>
    public void Remove(DirectoryItem item)
    {
        if (MessageService is not null && !MessageService.ShowYesNo($"Вы уверены, что хотите удалить {item.Name}?"))
            return;

        try
        {
            item.Remove();
        }
        catch (UnauthorizedAccessException ex)
        {
            if (MessageService is not null)
                MessageService.ShowError(ex.Message);
            return;
        }

        if (MessageService is not null)
            MessageService.ShowOk($"Файл {item.FullName} успешно удален!");
    }

    /// <summary>Получение элемента директории по пути.</summary>
    /// <param name="path">Путь к элементу директории.</param>
    /// <returns>Элемент директории.</returns>
    public DirectoryItem GetDirectoryItem(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            if (MessageService is not null)
                MessageService.ShowError("Не указан путь!");
            return null!;
        }

        if (!Path.IsPathRooted(path))
            path = Path.GetFullPath(Path.Combine(CurrentDirectory, path));

        if (DirectoryItem.GetItemType(path) == DirectoryItemType.None)
        {
            if (MessageService is not null)
                MessageService.ShowError($"Файл {path} не найден!");
            return null!;
        }

        return DirectoryItem.GetDirectoryItem(path);
    }

    /// <summary>Создание нового файла.</summary>
    /// <param name="fileName">Полное имя нового файла, включающее путь к файлу.</param>
    /// <exception cref="ArgumentNullException">Имя не инциализировано или пустое.</exception>
    public void CreateFile(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentNullException(nameof(fileName));

        if (!Path.IsPathRooted(fileName))
            fileName = Path.GetFullPath(Path.Combine(CurrentDirectory, fileName));

        var parent = Directory.GetParent(fileName);
        if (parent is null || !parent.Exists)
        {
            if (MessageService is not null)
                MessageService.ShowError("Директория файла не найдена.");

            return;
        }

        if (File.Exists(fileName) || Directory.Exists(fileName))
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            var exstansion = Path.GetExtension(fileName);

            var newName = name;

            for (int i = 2; File.Exists($"{newName}{exstansion}") || Directory.Exists($"{newName}{exstansion}"); i++)
                newName = $"{name} ({i})";

            fileName = $"{newName}{exstansion}";
        }

        try
        {
            File.Create(fileName);

            if (MessageService is not null)
                MessageService.ShowOk($"Файл {fileName} успешно создан!");
        }
        catch
        {
            if (MessageService is not null)
                MessageService.ShowError("Не удалось создать файл!");
        }
    }

    /// <summary>Создание новой директории.</summary>
    /// <param name="dirName">Полное имя новой директории, включающее путь к директории.</param>
    /// <exception cref="ArgumentNullException">Имя не инциализировано или пустое.</exception>
    public void CreateDirectory(string dirName)
    {
        if (string.IsNullOrWhiteSpace(dirName))
            throw new ArgumentNullException(nameof(dirName));

        if (!Path.IsPathRooted(dirName))
            dirName = Path.GetFullPath(Path.Combine(CurrentDirectory, dirName));

        var parent = Directory.GetParent(dirName);
        if (parent is null || !parent.Exists)
        {
            if (MessageService is not null)
                MessageService.ShowError("Родительская директория не найдена.");

            return;
        }

        if (File.Exists(dirName) || Directory.Exists(dirName))
        {
            var newName = dirName;

            for (int i = 2; File.Exists(newName) || Directory.Exists(newName); i++)
                newName = $"{dirName} ({i})";

            dirName = newName;
        }

        try
        {
            Directory.CreateDirectory(dirName);

            if (MessageService is not null)
                MessageService.ShowOk($"Директория {dirName} успешно создана!!");
        }
        catch
        {
            if (MessageService is not null)
                MessageService.ShowError("Не удалось создать директорию!");
        }
    }

    /// <summary>Поиск элементов в текущей директории.</summary>
    /// <param name="filter">Фильтр для поиска.</param>
    /// <param name="allDirectories">Поиск по все поддиректориям.</param>
    /// <returns>Найденные элементы.</returns>
    /// <exception cref="ArgumentNullException">Фильтр для поиска не инициализирован или пустой.</exception>
    public DirectoryItem[] Find(string filter, bool allDirectories)
    {
        if (string.IsNullOrWhiteSpace(filter))
            throw new ArgumentNullException(nameof(filter));

        try
        {
            return DirectoryItem.FindDirectoryItems(CurrentDirectory, filter, allDirectories);
        }
        catch (InvalidOperationException ex)
        {
            if (MessageService is not null)
                MessageService.ShowError(ex.Message);
            throw ex;
        }
    }
}
