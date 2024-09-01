using System.Diagnostics;

namespace Ruby.Extensions;

public interface IExtensionResult<T>
{
    public string ReloadableName { get; }
    public Stopwatch Time { get; }
    public T Result { get; }
}