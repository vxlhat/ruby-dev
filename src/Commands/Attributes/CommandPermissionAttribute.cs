namespace Ruby.Commands.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CommandPermissionAttribute : Attribute
{
    public readonly string RequiredPermissions;

    public CommandPermissionAttribute(string requiredGroup)
    {
        RequiredPermissions = requiredGroup;
    }
}