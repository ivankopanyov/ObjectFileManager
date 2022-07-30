namespace ConsoleFileManager.Commands.Base;

public abstract class Command
{
    public string KeyWord { get; private set; }

    public string Description { get; set; }

    public abstract void Execute(params string[] args);

    public Command(string keyWord)
    {
        if (string.IsNullOrWhiteSpace(keyWord))
            throw new ArgumentNullException(nameof(keyWord));

        KeyWord = keyWord;
    }
}
