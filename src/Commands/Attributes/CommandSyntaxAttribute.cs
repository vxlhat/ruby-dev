namespace Ruby.Commands.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CommandSyntaxAttribute : Attribute
{
    public readonly string[]? Syntax;

    public CommandSyntaxAttribute(params string[] syntax)
    {
        Syntax = syntax;
    }
}