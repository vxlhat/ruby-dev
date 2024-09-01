using Timer = System.Timers.Timer;

namespace Ruby.Server;

public class CounterThreshold : IThreshold
{
    internal static Timer ResetTimer = new Timer(1000)
    {
        AutoReset = true,
        Enabled = true
    };

    public CounterThreshold(int maxCounters)
    {
        _counters = new int[maxCounters];
        _maxValues = new int[maxCounters];

        ResetTimer.Elapsed += OnReset;
    }

    private int[] _counters;
    private int[] _maxValues;
    private bool _disposed;

    public bool Fire(int index)
    {
        if (_disposed) throw new ObjectDisposedException("Counter is disposed.");

        if (_maxValues[index] == 0) return false;
        
        if (_counters[index]++ > _maxValues[index])
            return true;

        return false;
    }

    public void Setup(int index, int max)
    {
        if (_disposed) throw new ObjectDisposedException("Counter is disposed.");

        _maxValues[index] = max;
    }

    private void OnReset(object? sender, System.Timers.ElapsedEventArgs e)
    {
        for (int i = 0; i < _counters.Length; i++)
            _counters[i] = 0;
    }

    public void Dispose()
    {
        _disposed = true;
        _counters = new int[0];
        _maxValues = new int[0];

        ResetTimer.Elapsed -= OnReset;
    }
}