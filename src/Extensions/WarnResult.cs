using System.Diagnostics;

namespace Ruby.Extensions;

public class WarnResult : IExtensionResult<string>
{
    public WarnResult(string name, Stopwatch time, string result)
    {
        ReloadableName = name;
        Time = time;
        Result = result;
    }

    public string ReloadableName { get; }

    public Stopwatch Time { get; }

    public string Result { get; }
}