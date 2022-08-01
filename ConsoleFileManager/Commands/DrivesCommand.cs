using ConsoleFileManager.Commands.Base;
using FileManager;
using FileManager.Content;
using System.Globalization;
using System.Text;

namespace ConsoleFileManager.Commands;

/// <summary>Класс, описывающий команду получения списка дисков.</summary>
public class DrivesCommand : Command
{
    /// <summary>Объект логики файлового менеджера.</summary>
    private readonly IConsoleFileManager _FileManager;

    /// <summary>Примеры использования команды.</summary>
    private readonly string[] _Examples = new[] { string.Empty };

    /// <summary>Культура формата объема диска.</summary>
    private readonly CultureInfo _Ru = CultureInfo.CreateSpecificCulture("ru-RU");

    /// <summary>Описание команды.</summary>
    public override string Description => "Список дисков.";

    /// <summary>Примеры использования команды.</summary>
    public override string[] Examples => _Examples;

    /// <summary>Инициализация объекта команды получения списка дисков.</summary>
    /// <param name="fileManager">Объект логики файлового менеджера.</param>
    /// <exception cref="ArgumentNullException">Объект файлового менеджера не инициализирован.</exception>
    public DrivesCommand(IConsoleFileManager fileManager)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    /// <summary>Выполнение команды получения списка дисков.</summary>
    /// <param name="args">Значения параметров команды.</param>
    public override void Execute(params string[] args)
    {
        var stringBuilder = new StringBuilder();

        foreach (var drive in CIDrive.GetDrives())
        {
            var total = ((drive.TotalSize ?? 0) / 1000).ToString("N0", _Ru);

            stringBuilder.AppendLine($"Диск {drive.Name}\r\n\tОбъем: {drive.TotalSize ?? 0} KB\r\n\tСвободно: {drive.FreeSize ?? 0} KB\n");
        }

        _FileManager.MessageService.ShowOk(stringBuilder.ToString());
    }
}
