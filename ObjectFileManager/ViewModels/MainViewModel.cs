using FileManager;
using ObjectFileManager.Commands;
using ObjectFileManager.Utilities;
using ObjectFileManager.ViewModels.Base;
using System.Windows.Input;

namespace ObjectFileManager.ViewModels;

public class MainViewModel : ViewModel
{
    private IMessageService _MessageService;

    private INavigator _Navigator;

    private string _PathLine;

    private string _Title;

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

    public ICommand ToBackCommand => new RelayCommand((obj) =>
    {
        _Navigator.ToBack(_MessageService);
        UpdateHeaders();
    },
    (obj) => _Navigator.BackExists);

    public ICommand ToForwardCommand => new RelayCommand((obj) =>
    {
        _Navigator.ToForward(_MessageService);
        UpdateHeaders();
    },
    (obj) => _Navigator.ForwardExists);

    public ICommand ToUpCommand => new RelayCommand((obj) =>
    {
        _Navigator.ToUp(_MessageService);
        UpdateHeaders();
    },
    (obj) => _Navigator.UpExists);

    public ICommand ToPathCommand => new RelayCommand((obj) =>
    {
        _Navigator.ToPath(PathLine, _MessageService);
        Title = _Navigator.CurrentName;
    });

    public MainViewModel()
    {
        _MessageService = new MessageService();
        _Navigator = OSNavigator.Navigator;
        UpdateHeaders();
    }

    private void UpdateHeaders()
    {
        PathLine = _Navigator.Current;
        Title = _Navigator.CurrentName;
    }
}
