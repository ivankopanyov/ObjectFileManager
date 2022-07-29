using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using FileManager;
using FileManager.Content;
using FileManager.Editor;
using FileManager.Services;
using ObjectFileManager.Commands;
using ObjectFileManager.Services;
using ObjectFileManager.ViewModels.Base;

namespace ObjectFileManager.ViewModels;

/// <summary>Класс, описывающий ViewModel главного окна.</summary>
public class MainViewModel : ViewModel
{
    /// <summary>Логика работы файлового менеджера.</summary>
    private readonly FileManagerLogic _FileManager;

    /// <summary>Текущий путь.</summary>
    private string _PathLine;

    /// <summary>Текущий заголовок.</summary>
    private string _Title;

    /// <summary>Коллекция системных дисков.</summary>
    private ObservableCollection<CIDrive> _Drives;

    /// <summary>Коллекция элементов текущего каталога.</summary>
    private ObservableCollection<CatalogItem> _Items;

    /// <summary>Выбранный каталог</summary>
    private CatalogItem _SelectedItem;

    /// <summary>Фильтр для поиска.</summary>
    private string _FilterText;

    /// <summary>Текущий путь.</summary>
    public string PathLine
    {
        get => _PathLine;
        set { _PathLine = value; OnPropertyChanged(); }
    }

    /// <summary>Текущий заголовок.</summary>
    public string Title
    {
        get => _Title;
        set { _Title = value; OnPropertyChanged(); }
    }

    /// <summary>Коллекция системных дисков.</summary>
    public ObservableCollection<CIDrive> Drives
    {
        get => _Drives;
        set { _Drives = value; OnPropertyChanged(); }
    }

    /// <summary>Коллекция элементов текущего каталога.</summary>
    public ObservableCollection<CatalogItem> Items
    {
        get => _Items;
        set { _Items = value; OnPropertyChanged(); }
    }

    /// <summary>Выбранный элемент.</summary>
    public CatalogItem SelectedItem
    {
        get => _SelectedItem;
        set
        {
            _SelectedItem = value;
            OnPropertyChanged();
            OnPropertyChanged("SelectedItemName");
            OnPropertyChanged("SelectedItemReadOnly");
            OnPropertyChanged("SelectedItemHidden");
            OnPropertyChanged("ShowInfo");
        }
    }

    /// <summary>Фильтр для поиска.</summary>
    public string FilterText
    {
        get => _FilterText;
        set
        {
            _FilterText = value;

            if (string.IsNullOrWhiteSpace(_FilterText))
            {
                Items = new(_FileManager.ItemsList);
                return;
            }

            try
            {
                Items = new(_FileManager.Find(_FilterText, true));
            }
            catch
            {
                FilterText = string.Empty;
                return;
            }
        }
    }

    /// <summary>Имя выбранного элемента.</summary>
    public string SelectedItemName
    {
        get => SelectedItem is not null ? SelectedItem.Name : null!;
        set
        {
            try
            {
                _FileManager.Rename(SelectedItem, value);
            }
            catch
            {
                return;
            }
            Items = new(_Items);
            OnPropertyChanged("SelectedItem");
        }
    }

    /// <summary>Значение атрибута "Только для чтения" выбранного элемента.</summary>
    public bool SelectedItemReadOnly
    {
        get => SelectedItem is not null ? SelectedItem.ReadOnly : false;
        set => _FileManager.ChangeAttribute(SelectedItem, FileAttributes.ReadOnly, value);
    }

    /// <summary>Значение атрибута "Скрытый" выбранного элемента.</summary>
    public bool SelectedItemHidden
    {
        get => SelectedItem is not null ? SelectedItem.Hidden : false;
        set => _FileManager.ChangeAttribute(SelectedItem, FileAttributes.Hidden, value);
    }

    /// <summary>Отображение информации о выбранном каталоге.</summary>
    public Visibility ShowInfo => SelectedItem is null ? Visibility.Hidden : Visibility.Visible;

    /// <summary>Команда перехода к предыдущей директории.</summary>
    public ICommand ToBackCommand => new Command((obj) =>
    {
        Update(_FileManager.ChangeDirectory(NavigatorDirection.Back));
    },
    (obj) => _FileManager.BackExists);

    /// <summary>Команда перехода к следущей директории.</summary>
    public ICommand ToForwardCommand => new Command((obj) =>
    {
        Update(_FileManager.ChangeDirectory(NavigatorDirection.Forward));
    },
    (obj) => _FileManager.ForwardExists);

    /// <summary>Команда перехода к родительской директории.</summary>
    public ICommand ToUpCommand => new Command((obj) =>
    {
        Update(_FileManager.ChangeDirectory(NavigatorDirection.Up));
    },
    (obj) => _FileManager.UpExists);

    /// <summary>Команда перехода к указанной директории.</summary>
    public ICommand ToPathCommand => new Command((obj) => Open(obj));

    /// <summary>Команда открытия выбранного элемента.</summary>
    public ICommand OpenCommand => new Command((obj) => Open(obj),
        (obj) => SelectedItem is not null);

    /// <summary>Команда вырезания выбранного элемента в буфер обмена операционной системы.</summary>
    public ICommand CutCommand => new Command((obj) =>
        _FileManager.Cut(SelectedItem, WindowsClipboard.Clipboard),
        (obj) => SelectedItem is not null && !SelectedItem.ReadOnly);

    /// <summary>Команда копирования выбранного элемента в буфер обмена операционной системы.</summary>
    public ICommand CopyCommand => new Command((obj) =>
        _FileManager.Copy(SelectedItem, WindowsClipboard.Clipboard),
        (obj) => SelectedItem is not null);

    /// <summary>Команда вставки элементов из буфера обмена операционной системы
    /// в текущий или выбранный каталог.</summary>
    public ICommand PasteCommand => new Command((obj) =>
    {
        _FileManager.Paste(WindowsClipboard.Clipboard, SelectedItem is null ? _FileManager.CurrentDirectory : SelectedItem.FullName);
        Items = new(_FileManager.ItemsList);
    },
    (obj) => WindowsClipboard.Clipboard.ContainsData && (SelectedItem is null || 
        (SelectedItem.Type == CatalogItemType.Catalog && SelectedItem.Exists)));

    /// <summary>Команда удаления выбранного элемента.</summary>
    public ICommand RemoveCommand => new Command((obj) =>
    {
        _FileManager.Remove(SelectedItem);
        Items = new(_FileManager.ItemsList);
    },
    (obj) => SelectedItem is not null && !SelectedItem.ReadOnly);

    /// <summary>Команда создания нового файла в текущем каталоге.</summary>
    public ICommand CreateFileCommand => new Command((obj) =>
    {
        _FileManager.CreateFile();
        Items = new(_FileManager.ItemsList);
    });

    /// <summary>Команда создания подкаталога в текущем каталоге.</summary>
    public ICommand CreateCatalogCommand => new Command((obj) =>
    {
        _FileManager.CreateCatalog();
        Items = new(_FileManager.ItemsList);
    });

    /// <summary>Команда выхода из приложения.</summary>
    public ICommand ExitCommand => new Command((obj) => App.Current.Shutdown());

    /// <summary>Инициализация объекта.</summary>
    /// <param name="windowService">Сервис работы с окнами.</param>
    /// <param name="messageService">Сервис сообщений.</param>
    public MainViewModel(IWindowService<object> windowService, IMessageService messageService) : base(windowService, messageService)
    {
        _FileManager = new FileManagerLogic(OSNavigator.Navigator, messageService);
        Drives = new(_FileManager.Drives);
        Update(_FileManager.CurrentDirectory);
    }

    /// <summary>Обновление заголовков и списка элементов текущего каталога.</summary>
    /// <param name="path">Путь к текущему каталогу.</param>
    private void Update(string path)
    {
        PathLine = path;
        Title = new DirectoryInfo(path).Name;
        Items = new(_FileManager.ItemsList);
    }

    /// <summary>Открыте выбранного элемента.</summary>
    /// <param name="obj">Путь к выбранному элементу.</param>
    private void Open(object obj)
    {
        if (obj is null || obj.GetType() != typeof(string))
            return;

        if (CatalogItem.GetItemType((string)obj) == CatalogItemType.File)
        {
            try
            {
                _WindowService.ShowWindow("editor", new FileEditor((string)obj));
            }
            catch (Exception ex)
            {
                _MessageService.ShowError(ex.Message);
            }

            return;
        }

        Update(_FileManager.ChangeDirectory((string)obj));
    }
}