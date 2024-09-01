namespace Ruby.Extensions;

internal class WarnCatcher<T> : IResultCatcher<T, WarnResult> where T : IExtension
{
    internal WarnCatcher(ProcessNotifier<T> notifier)
    {
        _notifier = notifier;
        _warns = new List<WarnResult>();
    }

    private ProcessNotifier<T> _notifier;
    private List<WarnResult> _warns;

    public void Announce()
    {
        int id = 0;

        foreach (WarnResult reason in _warns)
            ModernConsole.WriteLine($"   $y$!bWarning {id++} ({reason.ReloadableName}): {reason.Result}");
    }

    public void Catch(WarnResult result)
    {
        _notifier.Warnings++;
        _warns.Add(result);
    }
}