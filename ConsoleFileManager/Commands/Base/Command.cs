namespace ConsoleFileManager.Commands.Base;

/// <summary>Класс, описывающий команду.</summary>
public abstract class Command
{
    /// <summary>Описание команды.</summary>
    public abstract string Description { get; }

    /// <summary>Примеры использования команды.</summary>
    public abstract string[] Examples { get; }

    /// <summary>Выполнение команды.</summary>
    /// <param name="args">Значения параметров команды.</param>
    public abstract void Execute(params string[] args);
}
