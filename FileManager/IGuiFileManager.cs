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

    /// <summary>Элементы текущего каталога.</summary>
    CatalogItem[] ItemsList { get; }

    /// <summary>Системные диски.</summary>
    CIDrive[] Drives { get; }

    /// <summary>Проверка налиция текущего каталога.</summary>
    bool BackExists { get; }

    /// <summary>Проверка наличия следущего каталога.</summary>
    bool ForwardExists { get; }

    /// <summary>Проверка наличия родительского каталога.</summary>
    bool UpExists { get; }

    /// <summary>Изменение текущей директории.</summary>
    /// <param name="direction">Направление для перехода.</param>
    /// <returns>Новая директория.</returns>
    string ChangeDirectory(NavigatorDirection direction);

    /// <summary>Изменение текущей директории.</summary>
    /// <param name="path">Путь для перехода.</param>
    /// <returns>Новая директория.</returns>
    string ChangeDirectory(string path);

    /// <summary>Переименование элемента каталога.</summary>
    /// <param name="item">Элемент каталога для переименования.</param>
    /// <param name="name">Новое имя элемента каталога.</param>
    void Rename(CatalogItem item, string name);

    /// <summary>Изменение значения атрибута элемента каталога.</summary>
    /// <param name="item">Элемент каталога.</param>
    /// <param name="attribute">Изменяемый атрибут.</param>
    /// <param name="value">Новое значение изменяемого атрибута.</param>
    void ChangeAttribute(CatalogItem item, FileAttributes attribute, bool value);

    /// <summary>Вырезание элемента каталога в буфер обмена.</summary>
    /// <param name="item">Элемент каталога.</param>
    /// <param name="clipboard">Буфер обмена.</param>
    /// <exception cref="ArgumentNullException">Элемент каталога или буфер обмена не инициализирован.</exception>
    void Cut(CatalogItem item, IClipboard<string, string> clipboard);

    /// <summary>Копирование элемента каталога в буфер обмена.</summary>
    /// <param name="item">Элемент каталога.</param>
    /// <param name="clipboard">Буфер обмена.</param>
    void Copy(CatalogItem item, IClipboard<string, string> clipboard);

    /// <summary>Вставка элементов каталога из буфера обмена.</summary>
    /// <param name="clipboard">Буфер обмена.</param>
    /// <param name="path">Путь каталога для вставки.</param>
    void Paste(IClipboard<string, string> clipboard, string path);

    /// <summary>Удаление элемента каталога.</summary>
    /// <param name="item">Удаляемый элемент каталога.</param>
    void Remove(CatalogItem item);

    /// <summary>Получение элемента каталога по пути.</summary>
    /// <param name="path">Путь к элементу каталога.</param>
    /// <returns>Элемент каталога.</returns>
    CatalogItem GetCatalogItem(string path);

    /// <summary>Создание нового файла.</summary>
    /// <param name="fileName">Имя нового файла.</param>
    void CreateFile(string fileName);

    /// <summary>Создание нового каталога.</summary>
    /// <param name="catalogName">Полное имя нового каталога, включающее путь к каталогу.</param>
    void CreateCatalog(string catalogName);

    /// <summary>Поиск элементов в текущем каталоге.</summary>
    /// <param name="filter">Фильтр для поиска.</param>
    /// <param name="allDirectories">Поиск по все поддиректориям.</param>
    /// <returns>Найденные элементы.</returns>
    CatalogItem[] Find(string filter, bool allDirectories);
}
