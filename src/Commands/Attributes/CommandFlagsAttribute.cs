namespace Ruby.Commands.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CommandFlagsAttribute : Attribute
{
    public readonly CommandFlags Flags;

    public CommandFlagsAttribute(CommandFlags flags)
    {
        Flags = flags;
    }
}