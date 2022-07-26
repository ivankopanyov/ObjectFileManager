using FileManager;
using ObjectFileManager.Commands;
using ObjectFileManager.Utilities;
using ObjectFileManager.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ObjectFileManager.ViewModels;

public class MainViewModel : ViewModel
{
    private IMessageService _MessageService;

    private INavigator _Navigator;

    private IClipboard _Clipboard;

    private string _PathLine;

    private string _Title;

    private ObservableCollection<Drive> _Drives; 
    
    private ObservableCollection<CatalogItem> _Items;

    private CatalogItem _SelectedItem;

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

    public string SelectedItemName
    {
        get => SelectedItem is not null ? SelectedItem.Name : null!;
        set 
        { 
            SelectedItem.Name = value;
            Items = new(_Items);
            OnPropertyChanged("SelectedItem");
        }
    }

    public bool SelectedItemReadOnly
    {
        get => SelectedItem is not null ? SelectedItem.ReadOnly : false;
        set { SelectedItem.ReadOnly = value; }
    }

    public bool SelectedItemHidden
    {
        get => SelectedItem is not null ? SelectedItem.Hidden : false;
        set { SelectedItem.Hidden = value; }
    }

    public Visibility ShowInfo => SelectedItem is null ? Visibility.Hidden : Visibility.Visible;

    public ICommand ToBackCommand => new RelayCommand((obj) =>
    {
        _Navigator.ToBack(_MessageService);
        Update();
    },
    (obj) => _Navigator.BackExists);

    public ICommand ToForwardCommand => new RelayCommand((obj) =>
    {
        _Navigator.ToForward(_MessageService);
        Update();
    },
    (obj) => _Navigator.ForwardExists);

    public ICommand ToUpCommand => new RelayCommand((obj) =>
    {
        _Navigator.ToUp(_MessageService);
        Update();
    },
    (obj) => _Navigator.UpExists);

    public ICommand ToPathCommand => new RelayCommand((obj) => Open(obj));

    public ICommand OpenCommand => new RelayCommand((obj) => Open(obj),
        (obj) => SelectedItem is not null);

    public ICommand CutCommand => new RelayCommand((obj) => _Clipboard.Cut(SelectedItem),
        (obj) => SelectedItem is not null && !SelectedItem.ReadOnly);

    public ICommand CopyCommand => new RelayCommand((obj) => _Clipboard.Copy(SelectedItem),
        (obj) => SelectedItem is not null); 
    
    public ICommand PasteCommand => new RelayCommand((obj) =>
    {
        _Clipboard.Paste(SelectedItem is null ? _Navigator.Current : SelectedItem.FullName, _MessageService);
        Update();
    },
    (obj) => SelectedItem is null || SelectedItem.GetType() == typeof(CICatalog));

    public ICommand RemoveCommand => new RelayCommand((obj) =>
    {
        if (!_MessageService.ShowYesNo($"Вы уверены, что хотите удалить {SelectedItem.Name}?"))
            return;

        if (SelectedItem.Remove()) Update();
    },
    (obj) => SelectedItem is not null && !SelectedItem.ReadOnly);

    public ICommand CreateFileCommand => new RelayCommand((obj) =>
    {
        if (CatalogItem.CreateFile(_Navigator.Current))
            Update();
    });

    public ICommand CreateCatalogCommand => new RelayCommand((obj) =>
    {
        if (CatalogItem.CreateCatalog(_Navigator.Current))
            Update();
    });

    public ICommand ExitCommand => new RelayCommand((obj) => App.Current.Shutdown());

    public MainViewModel()
    {
        _MessageService = new MessageService();
        _Navigator = OSNavigator.Navigator;
        _Clipboard = WindowsClipboard.Clipboard;
        Drives = new(Drive.GetDrives());
        Update();
    }

    private void Update()
    {
        PathLine = _Navigator.Current;
        Title = _Navigator.CurrentName;
        var items = CatalogItem.GetCatalogItems(_Navigator.Current, _MessageService);
        Items = new(items);
    }

    private void Open(object obj)
    {
        if (obj is null || obj.GetType() != typeof(string)) return;

        var path = (string)obj;
        var type = CatalogItem.GetItemType(path);

        if (type != CatalogItemType.File && _Navigator.ToPath((string)obj, _MessageService))
        {
            Update();
            return;
        }
    }
}
