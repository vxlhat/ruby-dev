namespace Ruby.Extensions;

internal class SkipCatcher<T> : IResultCatcher<T, SkipResult> where T : IExtension
{
    internal SkipCatcher(ProcessNotifier<T> notifier)
    {
        _notifier = notifier;
        _reasons = new List<SkipResult>();
    }

    private ProcessNotifier<T> _notifier;
    private List<SkipResult> _reasons;

    public void Announce()
    {
        int id = 0;

        foreach (SkipResult reason in _reasons)
            ModernConsole.WriteLine($"   $b$!bSkip {id++} ({reason.ReloadableName}): {reason.Result}");
    }

    public void Catch(SkipResult result)
    {
        _notifier.Errors++;
        _reasons.Add(result);
    }
}