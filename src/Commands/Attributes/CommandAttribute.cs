namespace Ruby.Commands.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public readonly string Name;
    public readonly string Description;

    public CommandAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}