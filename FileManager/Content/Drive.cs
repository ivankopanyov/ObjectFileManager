namespace FileManager.Content;

public class Drive
{
    private DriveInfo _Drive;

    public string Name => _Drive.Name;

    public long TotalSize => _Drive.IsReady ? _Drive.TotalSize : 1;

    public long FillSize => _Drive.IsReady ? _Drive.TotalSize - _Drive.TotalFreeSpace : 0;

    public long FreeSize => _Drive.IsReady ? _Drive.TotalFreeSpace : 0;

    public Drive(DriveInfo drive) => _Drive = drive;

    public static Drive[] GetDrives()
    {
        try
        {
            var drives = DriveInfo.GetDrives();
            var result = new Drive[drives.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = new Drive(drives[i]);
            return result;
        }
        catch
        {
            return new Drive[0];
        }
    }
}
