namespace ConsoleFileManager.Commands.Base;

/// <summary>Класс, описывающий команду.</summary>
public abstract class Command
{
    /// <summary>Ключевое слово для поиска команды.</summary>
    public string KeyWord { get; private set; }

    /// <summary>Описание команды.</summary>
    public abstract string Description { get; }

    /// <summary>Выполнение команды.</summary>
    /// <param name="args">Значения параметров команды.</param>
    public abstract void Execute(params string[] args);

    /// <summary>Инициализация объекта команды.</summary>
    /// <param name="keyWord">Ключевое слово для поиска команды.</param>
    /// <exception cref="ArgumentNullException">Ключевое слово не инициализировано или пустое.</exception>
    public Command(string keyWord)
    {
        if (string.IsNullOrWhiteSpace(keyWord))
            throw new ArgumentNullException(nameof(keyWord));

        KeyWord = keyWord;
    }
}
