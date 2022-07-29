using ObjectFileManager.Services;
using ObjectFileManager.ViewModels;
using ObjectFileManager.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ObjectFileManager;

/// <summary>Interaction logic for App.xaml</summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var dialogService = new DialogService();
        var window = new EditorWindow();
        window.DataContext = new EditorViewModel(new Action(window.Close), dialogService);
        dialogService.Register("editor", window);

        var mainWindow = new MainWindow()
        {
            DataContext = new MainViewModel(dialogService)
        };
        mainWindow.Show();
    }
}
