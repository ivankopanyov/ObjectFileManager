using FileManager.Content;
using FileManager.Services;

namespace FileManager;

/// <summary>Логика рфботы фалового менеджера для графического интерфейса.</summary>
public interface IGuiFileManager
{
    /// <summary>Сервис вывода сообщений в пользотельский интерфейс.</summary>
    IMessageService MessageService { get; }

    /// <summary>Текущая директория.</summary>
    string CurrentDirectory { get; }

    /// <summary>Элементы текущей директории.</summary>
    DirectoryItem[] ItemsList { get; }

    /// <summary>Диски.</summary>
    CIDrive[] Drives { get; }

    /// <summary>Проверка налиция текущей директории.</summary>
    bool BackExists { get; }

    /// <summary>Проверка наличия следущей директории.</summary>
    bool ForwardExists { get; }

    /// <summary>Проверка наличия родительской директории.</summary>
    bool UpExists { get; }

    /// <summary>Изменение текущей директории.</summary>
    /// <param name="direction">Направление для перехода.</param>
    /// <returns>Новая директория.</returns>
    string ChangeDirectory(NavigatorDirection direction);

    /// <summary>Изменение текущей директории.</summary>
    /// <param name="path">Путь для перехода.</param>
    /// <returns>Новая директория.</returns>
    string ChangeDirectory(string path);

    /// <summary>Переименование элемента директории.</summary>
    /// <param name="item">Элемент директории для переименования.</param>
    /// <param name="name">Новое имя элемента директории.</param>
    void Rename(DirectoryItem item, string name);

    /// <summary>Изменение значения атрибута элемента директории.</summary>
    /// <param name="item">Элемент директории.</param>
    /// <param name="attribute">Изменяемый атрибут.</param>
    /// <param name="value">Новое значение изменяемого атрибута.</param>
    void ChangeAttribute(DirectoryItem item, FileAttributes attribute, bool value);

    /// <summary>Вырезание элемента директории в буфер обмена.</summary>
    /// <param name="item">Элемент директории.</param>
    /// <param name="clipboard">Буфер обмена.</param>
    /// <exception cref="ArgumentNullException">Элемент директории или буфер обмена не инициализирован.</exception>
    void Cut(DirectoryItem item, IClipboard<string, string> clipboard);

    /// <summary>Копирование элемента директории в буфер обмена.</summary>
    /// <param name="item">Элемент директории.</param>
    /// <param name="clipboard">Буфер обмена.</param>
    void Copy(DirectoryItem item, IClipboard<string, string> clipboard);

    /// <summary>Вставка элементов директории из буфера обмена.</summary>
    /// <param name="clipboard">Буфер обмена.</param>
    /// <param name="path">Путь директории для вставки.</param>
    void Paste(IClipboard<string, string> clipboard, string path);

    /// <summary>Удаление элемента директории.</summary>
    /// <param name="item">Удаляемый элемент директории.</param>
    void Remove(DirectoryItem item);

    /// <summary>Получение элемента директории по пути.</summary>
    /// <param name="path">Путь к элементу директории.</param>
    /// <returns>Элемент директории.</returns>
    DirectoryItem GetDirectoryItem(string path);

    /// <summary>Создание нового файла.</summary>
    /// <param name="fileName">Имя нового файла.</param>
    void CreateFile(string fileName);

    /// <summary>Создание новой директории.</summary>
    /// <param name="dirName">Полное имя новой директории, включающее путь к директории.</param>
    void CreateDirectory(string dirName);

    /// <summary>Поиск элементов в текущей директории.</summary>
    /// <param name="filter">Фильтр для поиска.</param>
    /// <param name="allDirectories">Поиск по все поддиректориям.</param>
    /// <returns>Найденные элементы.</returns>
    DirectoryItem[] Find(string filter, bool allDirectories);
}
