namespace Ruby.Commands;

public struct CommandData
{
    public CommandData(string name, string description, string[]? syntax, CommandFlags flags, string? requiredPermission)
    {
        Name = name;
        Description = description;
        Syntax = syntax;
        Flags = flags;
        RequiredPermission = requiredPermission;
    }

    public string Name;
    public string Description;
    public string[]? Syntax;
    public CommandFlags Flags;
    public string? RequiredPermission;
}