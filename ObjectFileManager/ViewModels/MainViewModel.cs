using FileManager;
using ObjectFileManager.Commands;
using ObjectFileManager.Utilities;
using ObjectFileManager.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ObjectFileManager.ViewModels;

public class MainViewModel : ViewModel
{
    private IMessageService _MessageService;

    private INavigator _Navigator;

    private string _PathLine;

    private string _Title;

    private ObservableCollection<Drive> _Drives; 
    
    private ObservableCollection<CatalogItem> _Items;

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

    public ICommand ToPathCommand => new RelayCommand((obj) =>
    {
        if (obj.GetType() != typeof(string)) return;

        var path = (string)obj;
        var type = CatalogItem.GetItemType(path);

        if (type != CatalogItemType.File && _Navigator.ToPath((string)obj, _MessageService))
        {
            Update();
            return;
        }
    });

    public ICommand ExitCommand => new RelayCommand((obj) => App.Current.Shutdown());

    public MainViewModel()
    {
        _MessageService = new MessageService();
        _Navigator = OSNavigator.Navigator;
        Drives = new(Drive.GetDrives());
        Update();
    }

    private void Update()
    {
        PathLine = _Navigator.Current;
        Title = _Navigator.CurrentName;
        var items = Catalog.GetCatalogItems(_Navigator.Current, _MessageService);
        Items = new(items);
    }
}
