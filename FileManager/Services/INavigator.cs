namespace FileManager.Services;

/// <summary>Навигатор.</summary>
public interface INavigator<T>
{
    /// <summary>Текущее положение.</summary>
    T Current { get; }

    /// <summary>Проверка наличия обратного значения.</summary>
    bool BackExists { get; }

    /// <summary>Проверка наличия следующего значения.</summary>
    bool ForwardExists { get; }

    /// <summary>Проверка наличия вышестоящего значения.</summary>
    bool UpExists { get; }

    /// <summary>Переход к указанному значению.</summary>
    /// <param name="value">Значение.</param>
    /// <param name="flag">Флаг.</param>
    void GoTo(T value, bool flag);

    /// <summary>Переход к вышестоящему значению.</summary>
    void GoToUp();

    /// <summary>Переход к обратному значению.</summary>
    void GoToBack();

    /// <summary>Переход к следущему значению.</summary>
    void GoToForward();
}
