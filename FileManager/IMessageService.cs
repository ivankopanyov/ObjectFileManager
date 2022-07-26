namespace FileManager;

public interface IMessageService
{
    void ShowError(string message);

    bool ShowYesNo(string message);

    bool ShowOkCancel(string message);
}
