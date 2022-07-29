using FileManager.Content;
using FileManager.Services;

namespace FileManager;

public class FileManagerLogic
{
    private readonly INavigator _Navigator;

    private readonly IMessageService _MessageService;

    private readonly CIDrive[] _Drives;

    private CatalogItem[] _ItemsList;

    public INavigator Navigator => _Navigator;

    public string CurrentDirectory => _Navigator.CurrentDirectory;

    public CatalogItem[] ItemsList => _ItemsList;

    public CIDrive[] Drives => _Drives;

    public bool BackExists => _Navigator.BackExists;

    public bool ForwardExists => _Navigator.ForwardExists;

    public bool UpExists => _Navigator.UpExists;

    public FileManagerLogic(INavigator navigator, IMessageService messageService)
    { 
        if (navigator is null)
            throw new ArgumentNullException(nameof(navigator));

        _Navigator = navigator;
        _MessageService = messageService; 
        _Drives = CIDrive.GetDrives();
        UpdateItems();
    }

    public string ChangeDirectory(NavigatorDirection direction)
    {
        try
        {
            switch (direction)
            {
                case NavigatorDirection.Up: _Navigator.ToUp(); break;
                case NavigatorDirection.Back: _Navigator.ToBack(); break;
                case NavigatorDirection.Forward: _Navigator.ToForward(); break;
            }

            UpdateItems();
        }
        catch (Exception ex)
        {
            if (_MessageService is not null)
                _MessageService.ShowError(ex.Message);
        }
        
        return _Navigator.CurrentDirectory;
    }

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
            _Navigator.ToPath(path, true);
            UpdateItems();
        }
        catch (Exception ex)
        {
            if (_MessageService is not null)
                _MessageService.ShowError(ex.Message);
        }

        return _Navigator.CurrentDirectory;
    }

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

    public void CreateFile()
    {
        try
        {
            CatalogItem.CreateFile(_Navigator.CurrentDirectory);
        }
        catch (InvalidOperationException ex)
        {
            if (_MessageService is not null)
                _MessageService.ShowError(ex.Message);
        }

        UpdateItems();
    }

    public void CreateCatalog()
    {
        try
        {
            CatalogItem.CreateCatalog(_Navigator.CurrentDirectory);
        }
        catch (InvalidOperationException ex)
        {
            if (_MessageService is not null)
                _MessageService.ShowError(ex.Message);
        }

        UpdateItems();
    }

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

    private void UpdateItems()
    {
        try
        {
            _ItemsList = CatalogItem.GetCatalogItems(_Navigator.CurrentDirectory);
        }
        catch (UnauthorizedAccessException ex)
        {
            if (_MessageService is not null)
                _MessageService.ShowError(ex.Message);

            _ItemsList = new CatalogItem[0];
        }
    }
}
