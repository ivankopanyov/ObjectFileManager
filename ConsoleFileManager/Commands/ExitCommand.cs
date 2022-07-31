using ConsoleFileManager.Commands.Base;
using FileManager.Services;
using System.Text;

namespace ConsoleFileManager.Commands;

/// <summary>Класс, описывающий команду выхода из приложения.</summary>
public class ExitCommand : Command
{
    /// <summary>Объект логики консольного файлового менеджера.</summary>
    private readonly ConsoleFileManagerLogic _FileManager;

    /// <summary>Примеры использования команды.</summary>
    private readonly string[] _Examples = new[] { string.Empty };

    /// <summary>Описание команды.</summary>
    public override string Description => "Выход из приложения.";

    /// <summary>Примеры использования команды.</summary>
    public override string[] Examples => _Examples;

    /// <summary>Инициализация объекта команды выхода из приложения.</summary>
    /// <param name="fileManager">Объект логики консольного файлового менеджера.</param>
    /// <exception cref="ArgumentNullException">Объект файлового менеджера не инициализирован.</exception>
    public ExitCommand(ConsoleFileManagerLogic fileManager)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    /// <summary>Выполнение команды выхода из приложения.</summary>
    /// <param name="args">Значения параметров команды.</param>
    public override void Execute(params string[] args) => _FileManager.Stop();
}
