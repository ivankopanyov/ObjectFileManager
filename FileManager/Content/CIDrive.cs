namespace FileManager.Content;

/// <summary>Класс, описывающий диск.</summary>
public class CIDrive
{
    /// <summary>Диск, описываемый текущим классом.</summary>
    private readonly DriveInfo _Drive;

    /// <summary>Имя диска.</summary>
    public string Name => _Drive.Name;
    
    /// <summary>Размер диска.</summary>
    public long? TotalSize => _Drive.IsReady ? _Drive.TotalSize / 1000 : null;

    /// <summary>Занятое пространство на диске.</summary>
    public long? FillSize => _Drive.IsReady ? (_Drive.TotalSize - _Drive.TotalFreeSpace) / 1000 : null;

    /// <summary>Свободное пространство на диске.</summary>
    public long? FreeSize => _Drive.IsReady ? _Drive.TotalFreeSpace / 1000 : null;

    /// <summary>Инициализация объекта диска.</summary>
    /// <param name="drive">Диск, описываемый текущим классом.</param>
    public CIDrive(DriveInfo drive) => _Drive = drive;

    /// <summary>Получение всех дисков.</summary>
    /// <returns>Диски.</returns>
    public static CIDrive[] GetDrives()
    {
        try
        {
            var drives = DriveInfo.GetDrives();
            var result = new CIDrive[drives.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = new CIDrive(drives[i]);
            return result;
        }
        catch
        {
            return new CIDrive[0];
        }
    }
}
