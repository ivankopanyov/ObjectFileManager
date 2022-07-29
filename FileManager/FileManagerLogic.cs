using FileManager.Content;
using FileManager.Services;

namespace FileManager;

/// <summary>Класс, описывающий логику работы файлового менеджера.</summary>
public class FileManagerLogic
{
    /// <summary>Навигатор по каталогам.</summary>
    private readonly INavigator<string> _Navigator;

    /// <summary>Сервис вывода сообщений в пользотельский интерфейс.</summary>
    private readonly IMessageService _MessageService;

    /// <summary>Системные диски.</summary>
    private readonly CIDrive[] _Drives;

    /// <summary>Элементы текущего каталога.</summary>
    private CatalogItem[] _ItemsList;

    /// <summary>Навигатор по каталогам.</summary>
    public INavigator<string> Navigator => _Navigator;

    /// <summary>Текущая директория.</summary>
    public string CurrentDirectory => _Navigator.Current;

    /// <summary>Элементы текущего каталога.</summary>
    public CatalogItem[] ItemsList => _ItemsList;

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
        _MessageService = messageService; 
        _Drives = CIDrive.GetDrives();
        UpdateItems();
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

            UpdateItems();
        }
        catch (Exception ex)
        {
            if (_MessageService is not null)
                _MessageService.ShowError(ex.Message);
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
            if (_MessageService is not null)
                _MessageService.ShowError("Путь не указан!");
            throw new ArgumentNullException(nameof(path));
        }

        try
        {
            _Navigator.GoTo(path, true);
            UpdateItems();
        }
        catch (Exception ex)
        {
            if (_MessageService is not null)
                _MessageService.ShowError(ex.Message);
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
            if (_MessageService is not null)
                _MessageService.ShowError(ex.Message);

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
            if (_MessageService is not null)
                _MessageService.ShowError(ex.Message);
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
            if (_MessageService is not null)
                _MessageService.ShowError($"Источник {item.FullName} не найден!");
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
            if (_MessageService is not null)
                _MessageService.ShowError($"Источник {item.FullName} не найден!");
            return;
        }

        item.Copy(clipboard);
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
        catch (InvalidOperationException ex)
        {
            _MessageService.ShowError(ex.Message);
        }

        UpdateItems();
    }

    /// <summary>Удаление элемента каталога.</summary>
    /// <param name="item">Удаляемый элемент каталога.</param>
    public void Remove(CatalogItem item)
    {
        if (_MessageService is not null && !_MessageService.ShowYesNo($"Вы уверены, что хотите удалить {item.Name}?"))
            return;

        try
        {
            item.Remove();
        }
        catch (UnauthorizedAccessException ex)
        {
            if (_MessageService is not null)
                _MessageService.ShowError(ex.Message);
            return;
        }

        UpdateItems();
    }

    /// <summary>Создание нового файла в текущем каталоге.</summary>
    public void CreateFile()
    {
        try
        {
            CatalogItem.CreateFile(_Navigator.Current);
        }
        catch (InvalidOperationException ex)
        {
            if (_MessageService is not null)
                _MessageService.ShowError(ex.Message);
        }

        UpdateItems();
    }

    /// <summary>Создание подкаталога в текущем каталоге.</summary>
    public void CreateCatalog()
    {
        try
        {
            CatalogItem.CreateCatalog(_Navigator.Current);
        }
        catch (InvalidOperationException ex)
        {
            if (_MessageService is not null)
                _MessageService.ShowError(ex.Message);
        }

        UpdateItems();
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
            if (_MessageService is not null)
                _MessageService.ShowError(ex.Message);
            throw ex;
        }
    }

    /// <summary>Обновление списка элементов текущего каталога.</summary>
    private void UpdateItems()
    {
        try
        {
            _ItemsList = CatalogItem.GetCatalogItems(_Navigator.Current);
        }
        catch (UnauthorizedAccessException ex)
        {
            if (_MessageService is not null)
                _MessageService.ShowError(ex.Message);

            _ItemsList = new CatalogItem[0];
        }
    }
}
