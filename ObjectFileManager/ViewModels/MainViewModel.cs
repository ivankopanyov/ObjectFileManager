using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using FileManager;
using FileManager.Content;
using FileManager.Navigation;
using ObjectFileManager.Commands;
using ObjectFileManager.Services;
using ObjectFileManager.ViewModels.Base;
using ObjectFileManager.Views;

namespace ObjectFileManager.ViewModels;

public class MainViewModel : ViewModel
{

    private MessageService _MessageService = new MessageService();

    private FMLogic _FileManager;

    private string _PathLine;

    private string _Title;

    private ObservableCollection<Drive> _Drives;

    private ObservableCollection<CatalogItem> _Items;

    private CatalogItem _SelectedItem;

    private string _FilterText;

    public string PathLine
    {
        get => _PathLine;
        set { _PathLine = value; OnPropertyChanged(); }
    }
    public string Title
    {
        get => _Title;
        set { _Title = value; OnPropertyChanged(); }
    }

    public ObservableCollection<Drive> Drives
    {
        get => _Drives;
        set { _Drives = value; OnPropertyChanged(); }
    }

    public ObservableCollection<CatalogItem> Items
    {
        get => _Items;
        set { _Items = value; OnPropertyChanged(); }
    }

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

    public bool SelectedItemReadOnly
    {
        get => SelectedItem is not null ? SelectedItem.ReadOnly : false;
        set => _FileManager.ChangeAttribute(SelectedItem, FileAttributes.ReadOnly, value);
    }

    public bool SelectedItemHidden
    {
        get => SelectedItem is not null ? SelectedItem.Hidden : false;
        set => _FileManager.ChangeAttribute(SelectedItem, FileAttributes.Hidden, value);
    }

    public Visibility ShowInfo => SelectedItem is null ? Visibility.Hidden : Visibility.Visible;

    public ICommand ToBackCommand => new RelayCommand((obj) =>
    {
        Update(_FileManager.ChangeDirectory(NavigatorDirection.Back));
    },
    (obj) => _FileManager.BackExists);

    public ICommand ToForwardCommand => new RelayCommand((obj) =>
    {
        Update(_FileManager.ChangeDirectory(NavigatorDirection.Forward));
    },
    (obj) => _FileManager.ForwardExists);

    public ICommand ToUpCommand => new RelayCommand((obj) =>
    {
        Update(_FileManager.ChangeDirectory(NavigatorDirection.Up));
    },
    (obj) => _FileManager.UpExists);

    public ICommand ToPathCommand => new RelayCommand((obj) => Open(obj));

    public ICommand OpenCommand => new RelayCommand((obj) => Open(obj),
        (obj) => SelectedItem is not null);

    public ICommand CutCommand => new RelayCommand((obj) =>
        _FileManager.Cut(SelectedItem, WindowsClipboard.Clipboard),
        (obj) => SelectedItem is not null && !SelectedItem.ReadOnly);

    public ICommand CopyCommand => new RelayCommand((obj) =>
        _FileManager.Copy(SelectedItem, WindowsClipboard.Clipboard),
        (obj) => SelectedItem is not null);

    public ICommand PasteCommand => new RelayCommand((obj) =>
    {
        _FileManager.Paste(WindowsClipboard.Clipboard, SelectedItem is null ? _FileManager.CurrentDirectory : SelectedItem.FullName);
        Items = new(_FileManager.ItemsList);
    },
    (obj) => WindowsClipboard.Clipboard.ContainsFiles && (SelectedItem is null || 
        (SelectedItem.Type == CatalogItemType.Catalog && SelectedItem.Exists)));

    public ICommand RemoveCommand => new RelayCommand((obj) =>
    {
        _FileManager.Remove(SelectedItem);
        Items = new(_FileManager.ItemsList);
    },
    (obj) => SelectedItem is not null && !SelectedItem.ReadOnly);

    public ICommand CreateFileCommand => new RelayCommand((obj) =>
    {
        _FileManager.CreateFile();
        Items = new(_FileManager.ItemsList);
    });

    public ICommand CreateCatalogCommand => new RelayCommand((obj) =>
    {
        _FileManager.CreateCatalog();
        Items = new(_FileManager.ItemsList);
    });

    public ICommand ExitCommand => new RelayCommand((obj) => App.Current.Shutdown());

    public MainViewModel(DialogService dialogService) : base(dialogService)
    {
        _FileManager = new FMLogic(OSNavigator.Navigator, _MessageService);
        Drives = new(_FileManager.Drives);
        Update(_FileManager.CurrentDirectory);
    }

    private void Update(string path)
    {
        PathLine = path;
        Title = new DirectoryInfo(path).Name;
        Items = new(_FileManager.ItemsList);
    }

    private void Open(object obj)
    {
        if (obj is null || obj.GetType() != typeof(string))
            return;

        if (CatalogItem.GetItemType((string)obj) == CatalogItemType.File)
        {
            try
            {
                _DialogService.Update("editor", obj);
                _DialogService.ShowDialog("editor");
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