using System.Diagnostics;
using Ruby.Extensions;

namespace Ruby.Extensions;

internal class ExceptionCatcher<T> : IResultCatcher<T, ExceptionResult> where T : IExtension
{
    internal ExceptionCatcher(ProcessNotifier<T> notifier)
    {
        _notifier = notifier;
        _exceptions = new List<ExceptionResult>();
    }

    private ProcessNotifier<T> _notifier;
    private List<ExceptionResult> _exceptions;

    public void Announce()
    {
        int id = 0;

        foreach (ExceptionResult result in _exceptions)
            ModernConsole.WriteLine($"   $r$!bException {id++} ({result.ReloadableName}): {result.Result.ToString()}");
    }

    public void Catch(ExceptionResult result)
    {
        _notifier.Errors++;
        _exceptions.Add(result);
    }
}