namespace FileManager.Content;

/// <summary>Класс, описывающий системный диск.</summary>
public class CIDrive
{
    /// <summary>Системный диск, описываемый текущим классом.</summary>
    private readonly DriveInfo _Drive;

    /// <summary>Имя системного диска.</summary>
    public string Name => _Drive.Name;
    
    /// <summary>Размер системного диска.</summary>
    public long TotalSize => _Drive.IsReady ? _Drive.TotalSize : 1;

    /// <summary>Занятое пространство на системном диске.</summary>
    public long FillSize => _Drive.IsReady ? _Drive.TotalSize - _Drive.TotalFreeSpace : 0;

    /// <summary>Свободное пространство на системном диске.</summary>
    public long FreeSize => _Drive.IsReady ? _Drive.TotalFreeSpace : 0;

    /// <summary>Инициализация объекта системного диска.</summary>
    /// <param name="drive">Системный диск, описываемый текущим классом.</param>
    public CIDrive(DriveInfo drive) => _Drive = drive;

    /// <summary>Получение всех системных дисков.</summary>
    /// <returns>Системные диски.</returns>
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
