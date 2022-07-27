using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using FileManager.Memory;

namespace ObjectFileManager.Utilities;

public class WindowsClipboard : IClipboard
{
    private static WindowsClipboard _Clipboard = new WindowsClipboard();

    public static WindowsClipboard Clipboard => _Clipboard;

    private WindowsClipboard() { }

    public bool ContainsFiles => System.Windows.Clipboard.ContainsFileDropList();

    public void Cut(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!File.Exists(path) && !Directory.Exists(path))
            throw new FileNotFoundException($"Источник {path} не найден!");

        CutProcess(new() { path });
    }

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

    public void Copy(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));

        if (!File.Exists(path) && !Directory.Exists(path))
            throw new FileNotFoundException($"Источник {path} не найден!");

        Clear();
        System.Windows.Clipboard.SetFileDropList(new() { path });
    }

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

    public void Paste(string path)
    {
        if (!ContainsFiles)
            throw new ArgumentNullException(nameof(path));

        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Путь {path} не найден.");

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

    public void Clear() => System.Windows.Clipboard.Clear();

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
