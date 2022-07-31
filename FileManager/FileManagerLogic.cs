using FileManager.Content;
using FileManager.Services;
using FileManager.Extension;

namespace FileManager;

/// <summary>Класс, описывающий логику работы файлового менеджера.</summary>
public class FileManagerLogic : IGuiFileManager, IConsoleFileManager
{
    /// <summary>Навигатор по каталогам.</summary>
    private readonly INavigator<string> _Navigator;

    /// <summary>Системные диски.</summary>
    private readonly CIDrive[] _Drives;

    /// <summary>Сервис вывода сообщений в пользотельский интерфейс.</summary>
    public IMessageService MessageService { get; private set; }

    /// <summary>Текущая директория.</summary>
    public string CurrentDirectory => _Navigator.Current;

    /// <summary>Элементы текущего каталога.</summary>
    public CatalogItem[] ItemsList
    {
        get 
        {
            try
            {
                return CatalogItem.GetCatalogItems(_Navigator.Current);
            }
            catch (Exception ex)
            {
                if (MessageService is not null)
                    MessageService.ShowError(ex.Message);
                return new CatalogItem[0];
            }
        }
    }

    /// <summary>Системные диски.</summary>
    public CIDrive[] Drives => _Drives;

    /// <summary>Проверка налиция текущего каталога.</summary>
    public bool BackExists => _Navigator.BackExists;

    /// <summary>Проверка наличия следущего каталога.</summary>
    public bool ForwardExists => _Navigator.ForwardExists;

    /// <summary>Проверка наличия родительского каталога.</summary>
    public bool UpExists => _Navigator.UpExists;

    /// <summary>Инициализация объекта файлового менеджера.</summary>
    /// <param name="navigator">Навигатор по каталогам.</param>
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

    /// <summary>Переименование элемента каталога.</summary>
    /// <param name="item">Элемент каталога для переименования.</param>
    /// <param name="name">Новое имя элемента каталога.</param>
    /// <exception cref="ArgumentNullException">Элемент каталога не инициализирован, 
    /// или имя не инициализировано или пустое.</exception>
    /// <exception cref="InvalidOperationException">Не удалось переименовать элемент.</exception>
    public void Rename(CatalogItem item, string name)
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

    /// <summary>Изменение значения атрибута элемента каталога.</summary>
    /// <param name="item">Элемент каталога.</param>
    /// <param name="attribute">Изменяемый атрибут.</param>
    /// <param name="value">Новое значение изменяемого атрибута.</param>
    /// <exception cref="ArgumentNullException">Элемент каталога не инициализирован.</exception>
    /// <exception cref="ArgumentException">Изенение указанного атрибута не поддерживается.</exception>
    public void ChangeAttribute(CatalogItem item, FileAttributes attribute, bool value)
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

    /// <summary>Вырезание элемента каталога в буфер обмена.</summary>
    /// <param name="item">Элемент каталога.</param>
    /// <param name="clipboard">Буфер обмена.</param>
    /// <exception cref="ArgumentNullException">Элемент каталога или буфер обмена не инициализирован.</exception>
    public void Cut(CatalogItem item, IClipboard<string, string> clipboard)
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

    /// <summary>Копирование элемента каталога в буфер обмена.</summary>
    /// <param name="item">Элемент каталога.</param>
    /// <param name="clipboard">Буфер обмена.</param>
    /// <exception cref="ArgumentNullException">Элемент каталога или буфер обмена не инициализирован.</exception>
    public void Copy(CatalogItem item, IClipboard<string, string> clipboard)
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

    /// <summary>Копирование элемента каталога.</summary>
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
                MessageService.ShowOk($"Папка {source} успешно скопирована!");
            }
        }
        catch
        {
            MessageService.ShowError($"Не удалось скопировать файл {source}");
            return;
        }
    }

    /// <summary>Перемещение элемента каталога.</summary>
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
                MessageService.ShowOk($"Папка {source} успешно перемещена!");
            }
        }
        catch
        {
            MessageService.ShowError($"Не удалось переместить файл {source}");
            return;
        }
    }

    /// <summary>Вставка элементов каталога из буфера обмена.</summary>
    /// <param name="clipboard">Буфер обмена.</param>
    /// <param name="path">Путь каталога для вставки.</param>
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

    /// <summary>Удаление элемента каталога.</summary>
    /// <param name="item">Удаляемый элемент каталога.</param>
    public void Remove(CatalogItem item)
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

    /// <summary>Получение элемента каталога по пути.</summary>
    /// <param name="path">Путь к элементу каталога.</param>
    /// <returns>Элемент каталога.</returns>
    public CatalogItem GetCatalogItem(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            if (MessageService is not null)
                MessageService.ShowError("Не указан путь!");
            return null!;
        }

        if (!Path.IsPathRooted(path))
            path = Path.GetFullPath(Path.Combine(CurrentDirectory, path));

        if (CatalogItem.GetItemType(path) == CatalogItemType.None)
        {
            if (MessageService is not null)
                MessageService.ShowError($"Файл {path} не найден!");
            return null!;
        }

        return CatalogItem.GetCatalogItem(path);
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

    /// <summary>Создание нового каталога.</summary>
    /// <param name="catalogName">Полное имя нового каталога, включающее путь к каталогу.</param>
    /// <exception cref="ArgumentNullException">Имя не инциализировано или пустое.</exception>
    public void CreateCatalog(string catalogName)
    {
        if (string.IsNullOrWhiteSpace(catalogName))
            throw new ArgumentNullException(nameof(catalogName));

        if (!Path.IsPathRooted(catalogName))
            catalogName = Path.GetFullPath(Path.Combine(CurrentDirectory, catalogName));

        var parent = Directory.GetParent(catalogName);
        if (parent is null || !parent.Exists)
        {
            if (MessageService is not null)
                MessageService.ShowError("Родительская директория не найдена.");

            return;
        }

        if (File.Exists(catalogName) || Directory.Exists(catalogName))
        {
            var newName = catalogName;

            for (int i = 2; File.Exists(newName) || Directory.Exists(newName); i++)
                newName = $"{catalogName} ({i})";

            catalogName = newName;
        }

        try
        {
            Directory.CreateDirectory(catalogName);

            if (MessageService is not null)
                MessageService.ShowOk($"Папка {catalogName} успешно создана!!");
        }
        catch
        {
            if (MessageService is not null)
                MessageService.ShowError("Не удалось создать папку!");
        }
    }

    /// <summary>Поиск элементов в текущем каталоге.</summary>
    /// <param name="filter">Фильтр для поиска.</param>
    /// <param name="allDirectories">Поиск по все поддиректориям.</param>
    /// <returns>Найденные элементы.</returns>
    /// <exception cref="ArgumentNullException">Фильтр для поиска не инициализирован или пустой.</exception>
    public CatalogItem[] Find(string filter, bool allDirectories)
    {
        if (string.IsNullOrWhiteSpace(filter))
            throw new ArgumentNullException(nameof(filter));

        try
        {
            return CatalogItem.FindCatalogItems(CurrentDirectory, filter, allDirectories);
        }
        catch (InvalidOperationException ex)
        {
            if (MessageService is not null)
                MessageService.ShowError(ex.Message);
            throw ex;
        }
    }
}
