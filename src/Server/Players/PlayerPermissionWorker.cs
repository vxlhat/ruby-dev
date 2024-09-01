using Ruby.Permissions;

namespace Ruby.Server.Players;

public class PlayerPermissionWorker : IPermissionWorker<RubyPlayer>
{
    public PermissionAccess HasBuildPermission(RubyPlayer target, int x, int y, int? width = null, int? height = null)
    {
        if (target.IsOperator) return PermissionAccess.HasPermission;

        return PermissionAccess.None;
    }

    public PermissionAccess HasPermission(RubyPlayer target, string permission)
    {
        if (target.IsOperator) return PermissionAccess.HasPermission;

        return PermissionAccess.None;
    }
}