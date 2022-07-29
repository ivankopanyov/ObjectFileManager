using System;
using System.Windows;
using FileManager.Editor;
using ObjectFileManager.Services;
using ObjectFileManager.ViewModels;
using ObjectFileManager.Views;

namespace ObjectFileManager;

/// <summary>Interaction logic for App.xaml</summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var messageService = new MessageService();
        var dialogService = new DialogService();

        var action = (object obj) =>
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            if (obj.GetType() != typeof(FileEditor))
                throw new TypeAccessException(nameof(obj));

            var window = new EditorWindow();
            window.DataContext = new EditorViewModel((FileEditor)obj, new Action(window.Close), dialogService, messageService);
            window.ShowDialog();
        };
        dialogService.Register("editor", action);

        var mainWindow = new MainWindow()
        {
            DataContext = new MainViewModel(dialogService, messageService)
        };
        mainWindow.Show();
    }
}
