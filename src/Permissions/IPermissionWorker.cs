namespace Ruby.Permissions;

public interface IPermissionWorker<T>
{
    public PermissionAccess HasPermission(T target, string permission);
    public PermissionAccess HasBuildPermission(T target, int x, int y, int? width = null, int? height = null);
}