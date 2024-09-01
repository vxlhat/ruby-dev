namespace Ruby.Extensions;

internal class ProcessNotifier<TReloadable> where TReloadable : IExtension
{
    internal int Total;
    internal int Errors;
    internal int Warnings;

    private Dictionary<string, dynamic> _handlers = new Dictionary<string, dynamic>();

    internal void AddResultCatcher<T>(IResultCatcher<TReloadable, T> catcher)
    {
        _handlers.Add(typeof(T).Name, catcher);
    }

    internal void AddResult<T>(T result)
    {
        var catcher = (IResultCatcher<TReloadable, T>)_handlers[typeof(T).Name];
        catcher.Catch(result);
    }

    internal void Announce()
    {
        if (Total == 0) return;

        int errPercents = Math.Min((int)(Errors / (double)(Total / 50.0)), 50);
        int warnPercents = Math.Min((int)(Warnings / (double)(Total / 50.0)), 50);
        int totalPercents = Math.Min(50 - errPercents - warnPercents, 50);

        string percents = "$!b";

        for (int i = 0; i < errPercents; i++)
            percents += "$@r ";
        for (int i = 0; i < warnPercents; i++)
            percents += "$@y ";
        for (int i = 0; i < totalPercents; i++)
            percents += "$@g ";

        ModernConsole.WriteLine($"{percents}$!r");
        int success = Total - Errors;
        ModernConsole.WriteLine($"  $!bSuccessfully: {(success > 0 ? "$!b$g" : "$!b$r")}{success}");
        ModernConsole.WriteLine($"  $!bErrors: {(Errors > 0 ? "$!b$r" : "$!b$g")}{Errors}");
        ModernConsole.WriteLine($"  $!bWarnings: {(Warnings > 0 ? "$!b$y" : "$!b$g")}{Warnings}");

        foreach (var kvp in _handlers)
            kvp.Value.Announce();
    }
}