namespace Ruby.Commands;

[Flags]
public enum CommandFlags
{
    None,
    Hidden,
    IngameOnly,
    NoLogging
}