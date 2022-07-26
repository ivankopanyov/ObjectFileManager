using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using FileManager;

namespace ObjectFileManager.Utilities;

public class WindowsClipboard : IClipboard
{
    private static WindowsClipboard _Clipboard = new WindowsClipboard();

    public static WindowsClipboard Clipboard => _Clipboard;

    private WindowsClipboard() { }

    public bool ContainsItems => System.Windows.Clipboard.ContainsFileDropList();

    public void Cut(CatalogItem item)
    {
        if (item is null) return;

        Cut(new StringCollection() { item.FullName });
    }

    public void Cut(IEnumerable<CatalogItem> items)
    {
        if (items is null) return;

        var list = new StringCollection();

        foreach (var item in items) 
            list.Add(item.FullName);

        Cut(list);
    }

    private void Cut(StringCollection items)
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

    public void Copy(CatalogItem item)
    {
        if (item is null) return;
        var list = new StringCollection() { item.FullName };
        Clear();
        System.Windows.Clipboard.SetFileDropList(list);
    }

    public void Copy(IEnumerable<CatalogItem> items)
    {
        if (items is null) return;
        var list = new StringCollection();

        foreach (var item in items)
            list.Add(item.FullName);

        Clear();
        System.Windows.Clipboard.SetFileDropList(list);
    }

    public void Paste(string path, IMessageService messageService = null!)
    {
        var items = System.Windows.Clipboard.GetFileDropList();
        if (items == null || items.Count == 0) return;

        bool move = false;

        var dataDropEffect = System.Windows.Clipboard.GetData("Preferred DropEffect");
        if (dataDropEffect != null)
        {
            MemoryStream dropEffect = (MemoryStream)dataDropEffect;
            byte[] moveEffect = new byte[4];
            dropEffect.Read(moveEffect, 0, moveEffect.Length);
            move = BitConverter.ToInt32(moveEffect, 0) == 2;
        }

        StringCollection list = new();

        foreach (var item in items)
            if (File.Exists(item))
            {
                if (!PasteFile(item, Path.Combine(path, new FileInfo(item).Name), move) && messageService is not null)
                    messageService.ShowError($"Не удалось скопировать файл {item}.");
            }
            else if (Directory.Exists(item))
            {
                if (!PasteDirectory(item, Path.Combine(path, new DirectoryInfo(item).Name), move) && messageService is not null)
                    messageService.ShowError($"Не удалось скопировать директорию {item}.");
            }

        if (move) Clear();
    }

    public void Clear() => System.Windows.Clipboard.Clear();

    private bool PasteFile(string source, string dest, bool isMove)
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

            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool PasteDirectory(string source, string dest, bool isMove)
    {
        if (source == dest) return true;

        try
        {
            if (isMove) Directory.Move(source, dest);
            else CopyDirectory(source, dest);

            return true;
        }
        catch
        {
            return false;
        }
    }

    static bool CopyDirectory(string source, string dest)
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

            return true;
        }
        catch
        {
            return false;
        }
    }
}
