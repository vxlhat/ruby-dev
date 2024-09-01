using System.Diagnostics;

namespace Ruby.Extensions;

public class SkipResult : IExtensionResult<string>
{
    public SkipResult(string name, Stopwatch time, string result)
    {
        ReloadableName = name;
        Time = time;
        Result = result;
    }

    public string ReloadableName { get; }

    public Stopwatch Time { get; }

    public string Result { get; }
}