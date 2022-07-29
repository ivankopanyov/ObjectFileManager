namespace FileManager.Editor;

/// <summary>Редактор</summary>
/// <typeparam name="T">Тип редактируемого объекта.</typeparam>
public interface IEditor<T>
{
    /// <summary>Имя редактируемого объекта.</summary>
    string SourceName { get; }

    /// <summary>Значение редактируемого объекта.</summary>
    T Content { get; set; }

    /// <summary>Флаг изменения значения объекта.</summary>
    bool UpdateContent { get; }

    /// <summary>Сохранение изменений.</summary>
    void Save();
}
