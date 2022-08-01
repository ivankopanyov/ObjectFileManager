namespace FileManager.Services;

/// <summary>Буфер обмена.</summary>
/// <typeparam name="TValue">Тип данных, содержащихся в буфере обмена.</typeparam>
/// <typeparam name="TDest">Тип объекта, получающего данные из буфера обмена.</typeparam>
public interface IClipboard<TValue, TDest>
{
    /// <summary>Проверка на содержание данных в буфере обмена.</summary>
    bool ContainsData { get; }

    /// <summary>Вырезание в буфер обмена.</summary>
    /// <param name="value">Значение вырезаемого объекта.</param>
    void Cut(TValue value);

    /// <summary>Вырезание значение нескольких объектов в буфер обмена.</summary>
    /// <param name="values">Перечисление значений вырезаемых объектов.</param>
    void Cut(IEnumerable<TValue> values);

    /// <summary>Копирование в буфер обмена.</summary>
    /// <param name="value">Значение копируемого объекта.</param>
    void Copy(TValue value);

    /// <summary>Копирование значение нескольких объектов в буфер обмена.</summary>
    /// <param name="values">Перечисление значений копируемых объектов.</param>
    void Copy(IEnumerable<TValue> values);

    /// <summary>Вставка значений из буфера обмена.</summary>
    /// <param name="dest">Объект для вставки.</param>
    void Paste(TDest dest);

    /// <summary>Очистка буфера обмена.</summary>
    void Clear();
}

