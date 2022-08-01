using System;
using System.Windows;
using FileManager;
using FileManager.Editor;
using FileManager.Services;
using ObjectFileManager.Services;
using ObjectFileManager.ViewModels;
using ObjectFileManager.Views;

namespace ObjectFileManager;

/// <summary>Interaction logic for App.xaml</summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var messageService = new MessageBoxService()
        { 
            IgnoreOk = true
        };
        var windowService = new WindowService();

        var action = (object obj) =>
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            if (obj.GetType() != typeof(FileEditor))
                throw new TypeAccessException(nameof(obj));

            var window = new EditorWindow();
            window.DataContext = new EditorViewModel((FileEditor)obj, new Action(window.Close), windowService, messageService);
            window.ShowDialog();
        };
        windowService.Register("editor", action);

        var fileManager = new FileManagerLogic(OSNavigator.Navigator, messageService);

        var mainWindow = new MainWindow()
        {
            DataContext = new MainViewModel(fileManager, windowService)
        };
        mainWindow.Show();
    }
}
