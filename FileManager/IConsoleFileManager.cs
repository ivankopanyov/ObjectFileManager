using FileManager.Content;
using FileManager.Services;

namespace FileManager;

public interface IConsoleFileManager
{
    /// <summary>Сервис вывода сообщений в пользотельский интерфейс.</summary>
    IMessageService MessageService { get; }

    /// <summary>Текущая директория.</summary>
    string CurrentDirectory { get; }

    /// <summary>Элементы текущей директории.</summary>
    DirectoryItem[] ItemsList { get; }

    /// <summary>Изменение текущей директории.</summary>
    /// <param name="path">Путь для перехода.</param>
    /// <returns>Новая директория.</returns>
    string ChangeDirectory(string path);

    /// <summary>Копирование элемента директории.</summary>
    /// <param name="source">Путь к копируемому элементу.</param>
    /// <param name="dest">Путь для копирования.</param>
    void Copy(string source, string dest);

    /// <summary>Перемещение элемента директории.</summary>
    /// <param name="source">Путь к перемещаемому элементу.</param>
    /// <param name="dest">Путь для перемещения.</param>
    void Move(string source, string dest);

    /// <summary>Удаление элемента директории.</summary>
    /// <param name="item">Удаляемый элемент директории.</param>
    void Remove(DirectoryItem item);

    /// <summary>Создание нового файла.</summary>
    /// <param name="fileName">Имя нового файла.</param>
    void CreateFile(string fileName);

    /// <summary>Создание новой директории.</summary>
    /// <param name="dirName">Имя новой директории.</param>
    void CreateDirectory(string dirName);
}
