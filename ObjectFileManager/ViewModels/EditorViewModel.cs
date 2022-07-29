using System;
using System.Windows.Input;
using FileManager.Editor;
using FileManager.Services;
using ObjectFileManager.Commands;
using ObjectFileManager.Services;
using ObjectFileManager.ViewModels.Base;

namespace ObjectFileManager.ViewModels;

public class EditorViewModel : ViewModel
{
    protected readonly Action _Close;

    private IEditor<string> _FileEditor;

    public string FileContent
    {
        get => _FileEditor is null ? string.Empty : _FileEditor.Content;
        set
        {
            _FileEditor.Content = value;
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

    public string Title => _FileEditor.SourceName;

    public EditorViewModel(IEditor<string> editor, Action close, IDialogService<object> dialogService, IMessageService messageService) : 
        base(dialogService, messageService) 
    {
        if (close is null)
            throw new ArgumentNullException(nameof(close));

        if (editor is null)
            throw new ArgumentNullException(nameof(editor));

        try
        {
            _FileEditor = editor;
            OnPropertyChanged("FileContent");
            OnPropertyChanged("SymbolsCount");
            OnPropertyChanged("LinesCount");
        }
        catch (InvalidOperationException ex)
        {
            throw ex;
        }

        _Close = close;
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
