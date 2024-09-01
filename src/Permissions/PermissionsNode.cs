using Ruby.Server.Players;
using Ruby.Storages;

namespace Ruby.Permissions;

public static class PermissionsNode<T> where T : IPermissionable
{
    internal static List<IPermissionWorker<T>> Workers = new List<IPermissionWorker<T>>();
    public static PermissionAccess HasPermission(T target, string permission)
    {
        return HandleResult((worker) => worker.HasPermission(target, permission));
    }

    public static PermissionAccess HasBuildPermission(T target, int x, int y, int? width = null, int? height = null)
    {
        return HandleResult((worker) => worker.HasBuildPermission(target, x, y, width, height));
    }

    internal static PermissionAccess HandleResult(Func<IPermissionWorker<T>, PermissionAccess> invokeFunc)
    {
        PermissionAccess access = PermissionAccess.None;
        foreach (var worker in Workers)
        {
            var result = invokeFunc(worker);
            if (result == PermissionAccess.HasPermission)
                access = result;

            if (result == PermissionAccess.Blocked)
                return result;
        }

        return access;
    }

    public static bool Register(IPermissionWorker<T> worker)
    {
        if (Workers.Contains(worker)) return false;

        Workers.Add(worker);
        return true;
    }

    public static bool Unregister(IPermissionWorker<T> worker)
    {
        return Workers.Remove(worker);
    }
}