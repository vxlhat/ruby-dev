using System.Diagnostics;

namespace Ruby.Extensions;

public sealed class LoadContext
{
    internal LoadContext(IExtension reloadable, dynamic notifier, Stopwatch sw)
    {
        _reloadable = reloadable;
        _notifier = notifier;
        _sw = sw;
    }

    private IExtension _reloadable;
    private dynamic _notifier;
    private Stopwatch _sw;

    public void Skip(string reason)
    {
        _sw.Stop();
        _notifier.AddResult(new SkipResult(_reloadable.Name, _sw, reason));

        throw new SkipException();
    }

    public void Warn(string reason)
    {
        _notifier.AddResult(new WarnResult(_reloadable.Name, _sw, reason));
    }
}