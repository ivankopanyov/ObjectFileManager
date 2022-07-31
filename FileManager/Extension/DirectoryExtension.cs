namespace FileManager.Extension;

/// <summary>Расширение класса DirectoryInfo.</summary>
public static class DirectoryInfoExtension
{
    /// <summary>Копирование директории.</summary>
    /// <param name="directory">Копируемая директория.</param>
    /// <param name="dest">Новая директория.</param>
    /// <exception cref="DirectoryNotFoundException">Директория не найдена.</exception>
    /// <exception cref="InvalidOperationException">Не удалось скопировать директорию.</exception>
    public static void Copy(this DirectoryInfo directory, string dest)
    {
        if (!directory.Exists)
            throw new DirectoryNotFoundException($"Директория {directory.FullName} не найдена!");

        try
        {
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
                new DirectoryInfo(dir.FullName).Copy(path);
            }
        }
        catch
        {
            throw new InvalidOperationException($"Не удалось скопировать директорию {directory}");
        }
    }
}
