using System;
using System.Windows.Input;
using FileManager.Editor;
using FileManager.Services;
using ObjectFileManager.Commands;
using ObjectFileManager.Services;
using ObjectFileManager.ViewModels.Base;

namespace ObjectFileManager.ViewModels;

/// <summary>Класс, описывающий ViewModel окна редактора.</summary>
public class EditorViewModel : ViewModel
{
    /// <summary>Закрытие окна.</summary>
    protected readonly Action _Close;

    /// <summary>Редактор.</summary>
    private IEditor<string> _FileEditor;

    /// <summary>Содержимое редактируемого файла.</summary>
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

    /// <summary>Колличество символов в содержимом файла.</summary>
    public int SymbolsCount => FileContent.Length;

    /// <summary>Колличество cnhjr в содержимом файла.</summary>
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

    /// <summary>Заголовок окна.</summary>
    public string Title => _FileEditor.SourceName;

    /// <summary>Инициализация объекта.</summary>
    /// <param name="editor">Редактор.</param>
    /// <param name="close">Зактрытие окна.</param>
    /// <param name="windowService">Сервис работы с окнами.</param>
    /// <param name="messageService">Сервис сообщение.</param>
    /// <exception cref="ArgumentNullException">Параметр не инициализирован.</exception>
    public EditorViewModel(IEditor<string> editor, Action close, IWindowService<object> windowService, IMessageService messageService) : 
        base(windowService, messageService) 
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

    /// <summary>Команда сохраниния содержимого файла.</summary>
    public ICommand SaveCommand => new Command((obj) => 
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

    /// <summary>Команда закрытия окна редактора.</summary>
    public ICommand CloseCommand => new Command((obj) => 
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
