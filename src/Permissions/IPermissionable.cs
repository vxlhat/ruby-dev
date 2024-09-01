namespace Ruby.Permissions;

public interface IPermissionable
{
    public string Name { get; }

    public bool HasPermission(string permission);
    public bool CanBuild(int x, int y, int? width = null, int? height = null);
}