namespace FileManager.Services;

public interface IMessageService
{
    void ShowError(string message);

    bool ShowYesNo(string message);

    MessageResult ShowYesNoCancel(string message);
}
