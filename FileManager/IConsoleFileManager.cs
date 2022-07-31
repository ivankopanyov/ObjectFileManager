using FileManager.Content;
using FileManager.Services;

namespace FileManager;

public interface IConsoleFileManager
{
    /// <summary>Сервис вывода сообщений в пользотельский интерфейс.</summary>
    IMessageService MessageService { get; }

    /// <summary>Текущая директория.</summary>
    string CurrentDirectory { get; }

    /// <summary>Элементы текущего каталога.</summary>
    CatalogItem[] ItemsList { get; }

    /// <summary>Изменение текущей директории.</summary>
    /// <param name="path">Путь для перехода.</param>
    /// <returns>Новая директория.</returns>
    string ChangeDirectory(string path);

    /// <summary>Копирование элемента каталога.</summary>
    /// <param name="source">Путь к копируемому элементу.</param>
    /// <param name="dest">Путь для копирования.</param>
    void Copy(string source, string dest);

    /// <summary>Перемещение элемента каталога.</summary>
    /// <param name="source">Путь к перемещаемому элементу.</param>
    /// <param name="dest">Путь для перемещения.</param>
    void Move(string source, string dest);

    /// <summary>Удаление элемента каталога.</summary>
    /// <param name="item">Удаляемый элемент каталога.</param>
    void Remove(CatalogItem item);

    /// <summary>Создание нового файла.</summary>
    /// <param name="fileName">Имя нового файла.</param>
    void CreateFile(string fileName);

    /// <summary>Создание нового каталога.</summary>
    /// <param name="catalogName">Имя нового каталога.</param>
    void CreateCatalog(string catalogName);
}
