namespace FileManager;

public interface INavigator
{
    string Current { get; }

    string CurrentName { get; }

    string Back { get; }

    bool BackExists { get; }

    string Forward { get; }

    bool ForwardExists { get; }

    string Up { get; }

    bool UpExists { get; }

    bool ToPath(string path, IMessageService messageService = null!);

    void ToUp(IMessageService messageService = null!);

    void ToBack(IMessageService messageService = null!);

    void ToForward(IMessageService messageService = null!);

    void ClearBack();

    void ClearForward();
}
