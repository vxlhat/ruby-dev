using Ruby.Commands;

internal static class CommandExtensions
{
    internal static bool NameEquals(this ICommand command, string text)
    {
        string[] textLines = text.Split(' ');
        string[] cmdLines = command.Data.Name.Split(' ');

        if (cmdLines.Length > textLines.Length)
            return false;

        for (int i = 0 ; i < cmdLines.Length; i++)
            if (cmdLines[i] != textLines[i])
                return false;

        return true;
    }
}