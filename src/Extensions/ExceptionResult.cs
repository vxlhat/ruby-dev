using System.Diagnostics;

namespace Ruby.Extensions;

public class ExceptionResult : IExtensionResult<Exception>
{
    public ExceptionResult(string name, Stopwatch time, Exception result)
    {
        ReloadableName = name;
        Time = time;
        Result = result;
    }

    public string ReloadableName { get; }

    public Stopwatch Time { get; }

    public Exception Result { get; }
}