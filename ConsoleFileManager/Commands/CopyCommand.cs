﻿using ConsoleFileManager.Commands.Base;
using FileManager;

namespace ConsoleFileManager.Commands;

/// <summary>Класс, описывающий команду копирования файла или директории.</summary>
public class CopyCommand : Command
{
    /// <summary>Объект логики файлового менеджера.</summary>
    private readonly IConsoleFileManager _FileManager;

    /// <summary>Примеры использования команды.</summary>
    private readonly string[] _Examples = new[]
    {
        @"C:\dir_name\file_name C:\dir_name\new_file_name",
        "\"..\\dir name\" \"new dir name\"",
        "C:\\dir_name\\file_name \"..\\dir name\\new_file_name\""
    };

    /// <summary>Описание команды.</summary>
    public override string Description => "Копирование файла или директории.";

    /// <summary>Примеры использования команды.</summary>
    public override string[] Examples => _Examples;

    /// <summary>Инициализация объекта команды копирования файла или директории.</summary>
    /// <param name="fileManager">Объект логики файлового менеджера.</param>
    /// <exception cref="ArgumentNullException">Объект файлового менеджера не инициализирован.</exception>
    public CopyCommand(IConsoleFileManager fileManager)
    {
        if (fileManager is null)
            throw new ArgumentNullException(nameof(fileManager));

        _FileManager = fileManager;
    }

    /// <summary>Выполнение команды копирования файла или директории.</summary>
    /// <param name="args">Значения параметров команды.</param>
    public override void Execute(params string[] args)
    {
        if (args is null || args.Length < 3)
        {
            _FileManager.MessageService.ShowError("Не указаны параметры команды!");
            return;
        }

        var argsLine = string.Join(' ', args, 1, args.Length - 1);

        if (argsLine.Contains('"'))
        { 
            var currentArgs = argsLine.Split('"', StringSplitOptions.RemoveEmptyEntries);
            if (currentArgs.Length != 2 || string.IsNullOrWhiteSpace(currentArgs[0]) || string.IsNullOrWhiteSpace(currentArgs[1]))
            {
                _FileManager.MessageService.ShowError("Не корректно указаны параметры команды!");
            }

            _FileManager.Copy(currentArgs[0].Trim(), currentArgs[1].Trim());
            return;
        }

        if (string.IsNullOrWhiteSpace(args[1]) || string.IsNullOrWhiteSpace(args[2]))
        {
            _FileManager.MessageService.ShowError("Не корректно указаны параметры команды!");
            return;
        }

        _FileManager.Copy(args[1].Trim(), args[2].Trim());
    }
}
