using System;
using System.IO;
using System.Windows.Input;
using FileManager.Editor;
using FileManager.Information;
using ObjectFileManager.Commands;
using ObjectFileManager.Services;
using ObjectFileManager.ViewModels.Base;

namespace ObjectFileManager.ViewModels;

public class EditorViewModel : DialogViewModel
{
    private FileEditor _FileEditor;

    private IMessageService _MessageService = new MessageService();

    private string _FilePath;

    public string FilePath
    { 
        get => _FilePath;
        set 
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(FilePath));

            if (!File.Exists(value))
                throw new FileNotFoundException($"Файл {value} не найден!");

            Title = Path.GetFileName(value);

            try
            {
                _FileEditor = new FileEditor(value);
                OnPropertyChanged("FileContent");
                OnPropertyChanged("SymbolsCount");
                OnPropertyChanged("LinesCount");
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
        }
    }

    public string FileContent
    {
        get => _FileEditor is null ? string.Empty : _FileEditor.FileContent;
        set
        {
            _FileEditor.FileContent = value;
            OnPropertyChanged("SymbolsCount");
            OnPropertyChanged("LinesCount");
            OnPropertyChanged();
        }
    }

    public int SymbolsCount => FileContent.Length;

    public int LinesCount
    {
        get
        {
            var counter = 1;
            foreach (var symbol in FileContent)
                if (symbol == '\n') counter++;

            return counter;
        }
    }

    public string Title { get; private set; }

    public EditorViewModel(Action close, DialogService dialogService) : base(null!, close, dialogService) { }

    public override void UpdateData(object data)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data));

        if (data.GetType() != typeof(string))
            throw new ArgumentException("Некорректный тип данных.");

        var path = (string)data;

        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(FilePath));

        if (!File.Exists(path))
            throw new FileNotFoundException($"Файл {path} не найден!");

        Title = Path.GetFileName(path);

        try
        {
            _FileEditor = new FileEditor(path);
            OnPropertyChanged("FileContent");
            OnPropertyChanged("SymbolsCount");
            OnPropertyChanged("LinesCount");
        }
        catch (InvalidOperationException ex)
        {
            throw ex;
        }
    }

    public ICommand SaveCommand => new RelayCommand((obj) => 
    {
        try
        {
            _FileEditor.Save();
        }
        catch (InvalidOperationException ex)
        {
            _MessageService.ShowError(ex.Message);
        }
    },
    (obj) => _FileEditor.UpdateContent);

    public ICommand CloseCommand => new RelayCommand((obj) => 
    {
        if (_FileEditor.UpdateContent)
        {
            switch (_MessageService.ShowYesNoCancel("Сохранить изменения?"))
            { 
                case MessageResult.Yes:
                    try
                    {
                        _FileEditor.Save();
                    }
                    catch (InvalidOperationException ex)
                    {
                        _MessageService.ShowError(ex.Message);
                        return;
                    }
                    break;

                case MessageResult.No:
                    break;

                case MessageResult.Cancel:
                    return;
            }
        }

        _Close();
    });
}
