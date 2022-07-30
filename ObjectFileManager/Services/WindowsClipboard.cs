using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using FileManager.Services;

namespace ObjectFileManager.Services;

/// <summary>Класс для работы с буфером обмена операционной системы.</summary>
public sealed class WindowsClipboard : IClipboard<string, string>
{
    /// <summary>Объект класса буфера обмена операционной системы.</summary>
    private static readonly WindowsClipboard _Clipboard = new WindowsClipboard();

    /// <summary>Объект класса буфера обмена операционной системы.</summary>
    public static WindowsClipboard Clipboard => _Clipboard;

    /// <summary>Инициализация объекта буфера обмена операционной системы.</summary>
    private WindowsClipboard() { }

    /// <summary>Проверка на содержание данных в буфере обмена.</summary>
    public bool ContainsData => System.Windows.Clipboard.ContainsFileDropList();
    /// <summary>Вырезание в буфер обмена.</summary>
    /// <param name="path">Путь вырезаемого элемента.</param>
    /// <exception cref="ArgumentNullException">Путь не инициализирован или пустой.</exception>
    /// <exception cref="FileNotFoundException">Источник не найден!</exception>
    public void Cut(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!File.Exists(path) && !Directory.Exists(path))
            throw new FileNotFoundException($"Источник {path} не найден!");

        CutProcess(new() { path });
    }

    /// <summary>Вырезание нескольких элементов в буфер обмена.</summary>
    /// <param name="paths">Перечисление путей вырезаемых объектов.</param>
    /// <exception cref="ArgumentNullException">Перечисление путей не инициализировано или пустое.</exception>
    /// <exception cref="ArgumentException">Перечисление содержит не инициалированный или пустой элемент.</exception>
    /// <exception cref="FileNotFoundException">Источник не найден.</exception>
    public void Cut(IEnumerable<string> paths)
    {
        if (paths is null)
            throw new ArgumentNullException(nameof(paths));

        StringCollection collection = new();

        foreach (var path in paths)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Перечисление содержит пустой элемент.");
            else if (!File.Exists(path) && !Directory.Exists(path))
                throw new FileNotFoundException($"Источник {path} не найден!");

            collection.Add(path);
        }

        if (collection.Count == 0)
            throw new ArgumentNullException(nameof(paths));

        CutProcess(collection);
    }

    /// <summary>Копирование в буфер обмена.</summary>
    /// <param name="path">Путь копируемого элемента.</param>
    /// <exception cref="ArgumentNullException">Путь не инициализирован или пустой.</exception>
    /// <exception cref="FileNotFoundException">Источник не найден!</exception>
    public void Copy(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!File.Exists(path) && !Directory.Exists(path))
            throw new FileNotFoundException($"Источник {path} не найден!");

        Clear();
        System.Windows.Clipboard.SetFileDropList(new() { path });
    }

    /// <summary>Копирование нескольких элементов в буфер обмена.</summary>
    /// <param name="paths">Перечисление путей копируемых объектов.</param>
    /// <exception cref="ArgumentNullException">Перечисление путей не инициализировано или пустое.</exception>
    /// <exception cref="ArgumentException">Перечисление содержит не инициалированный или пустой элемент.</exception>
    /// <exception cref="FileNotFoundException">Источник не найден.</exception>
    public void Copy(IEnumerable<string> paths)
    {
        if (paths is null)
            throw new ArgumentNullException(nameof(paths));

        StringCollection collection = new();

        foreach (var path in paths)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Перечисление содержит пустой элемент.");
            else if (!File.Exists(path) && !Directory.Exists(path))
                throw new FileNotFoundException($"Источник {path} не найден!");

            collection.Add(path);
        }

        if (collection.Count == 0)
            throw new ArgumentNullException(nameof(paths));

        Clear();
        System.Windows.Clipboard.SetFileDropList(collection);
    }

    /// <summary>Вставка элементов из буфера обмена.</summary>
    /// <param name="path">Путь к каталогу для вставки.</param>
    /// <exception cref="ArgumentNullException">Путь не инициализирован или пустой.</exception>
    /// <exception cref="DirectoryNotFoundException">Путь не найден.</exception>
    /// <exception cref="InvalidOperationException">Не удалось скопировать файлы.</exception>
    public void Paste(string path)
    {
        if (string.IsNullOrEmpty(path))
            new ArgumentNullException(nameof(path));

        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Путь {path} не найден.");

        if (!ContainsData)
            throw new InvalidOperationException("Буфер обмена пуст.");

        var files = System.Windows.Clipboard.GetFileDropList();

        bool move = false;

        var dataDropEffect = System.Windows.Clipboard.GetData("Preferred DropEffect");
        if (dataDropEffect != null)
        {
            using var dropEffect = (MemoryStream)dataDropEffect;
            byte[] moveEffect = new byte[4];
            dropEffect.Read(moveEffect, 0, moveEffect.Length);
            move = BitConverter.ToInt32(moveEffect, 0) == 2;
        }

        foreach (var file in files)
            if (File.Exists(file))
            {
                try
                {
                    PasteFile(file, Path.Combine(path, new FileInfo(file).Name), move);
                }
                catch (InvalidOperationException ex)
                {
                    throw ex;
                }
            }
            else if (Directory.Exists(file))
            {
                try
                {
                    PasteDirectory(file, Path.Combine(path, new DirectoryInfo(file).Name), move);
                }
                catch (InvalidOperationException ex)
                {
                    throw ex;
                }
            }
            else
                throw new InvalidOperationException("Не удалось скопировать файлы!");

        if (move) Clear();
    }

    /// <summary>Очистка буфера обмена.</summary>
    public void Clear() => System.Windows.Clipboard.Clear();

    /// <summary>Процесс вырезания элементов.</summary>
    /// <param name="items">Коллекция путей к вырезаемым элементам.</param>
    private void CutProcess(StringCollection items)
    {
        var moveEffect = new byte[] { 2, 0, 0, 0 };
        using var dropEffect = new MemoryStream();
        dropEffect.Write(moveEffect, 0, moveEffect.Length);

        var data = new DataObject();
        data.SetFileDropList(items);
        data.SetData("Preferred DropEffect", dropEffect);

        Clear();
        System.Windows.Clipboard.SetDataObject(data, true);
    }

    /// <summary>Вставка файла.</summary>
    /// <param name="source">Вставляемый файл.</param>
    /// <param name="dest">Новый файл.</param>
    /// <param name="isMove">Перемещение файла.</param>
    /// <exception cref="InvalidOperationException">Не удалось вставить файл.</exception>
    private void PasteFile(string source, string dest, bool isMove)
    {
        var file = new FileInfo(dest);
        var extansion = file.Extension;
        var name = Path.Combine(file.Directory!.FullName, Path.GetFileNameWithoutExtension(dest));

        for (int i = 2; File.Exists($"{name}{extansion}"); i++)
            name += " — копия";

        name += extansion;

        try
        {
            if (isMove) File.Move(source, name);
            else File.Copy(source, name);
        }
        catch
        {
            throw new InvalidOperationException($"Не удалось скопировать файл {source}");
        }
    }

    /// <summary>Вставка каталога.</summary>
    /// <param name="source">Вставляемый каталог.</param>
    /// <param name="dest">Новый каталог.</param>
    /// <param name="isMove">Перемещение каталога.</param>
    /// <exception cref="InvalidOperationException">Не удалось вставить каталог.</exception>
    private void PasteDirectory(string source, string dest, bool isMove)
    {
        if (source == dest) return;

        try
        {
            if (isMove) Directory.Move(source, dest);
            else CopyDirectory(source, dest);
        }
        catch
        {
            throw new InvalidOperationException($"Не удалось скопировать папку {source}");
        }
    }

    /// <summary>Копирование директории.</summary>
    /// <param name="source">Директория для копирования.</param>
    /// <param name="dest">новая диретория.</param>
    /// <exception cref="InvalidOperationException">Не удалось скопировать директорию.</exception>
    static void CopyDirectory(string source, string dest)
    {
        try
        {
            var directory = new DirectoryInfo(source);

            DirectoryInfo[] directories = directory.GetDirectories();

            Directory.CreateDirectory(dest);

            foreach (FileInfo file in directory.GetFiles())
            {
                string path = Path.Combine(dest, file.Name);
                file.CopyTo(path);
            }

            foreach (DirectoryInfo dir in directories)
            {
                string path = Path.Combine(dest, dir.Name);
                CopyDirectory(dir.FullName, path);
            }
        }
        catch
        {
            throw new InvalidOperationException($"Не удалось скопировать папку {source}");
        }
    }
}
