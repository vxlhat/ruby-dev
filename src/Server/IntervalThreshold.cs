namespace Ruby.Server;

public class IntervalThreshold : IThreshold
{
    public IntervalThreshold(int maxCounters, bool alwaysReset)
    {
        _alwaysReset = alwaysReset;
        _counters = new DateTime[maxCounters];
        _intervals = new int[maxCounters];
    }

    private DateTime[] _counters;
    private int[] _intervals;
    private bool _alwaysReset;
    private bool _disposed;

    public bool Fire(int index)
    {
        if (_disposed) throw new ObjectDisposedException("Counter is disposed.");

        if (_intervals[index] == 0) return false;
        
        if (_counters[index].AddMilliseconds(_intervals[index]) > DateTime.UtcNow)
        {
            if (_alwaysReset)
                _counters[index] = DateTime.UtcNow;

            return true;
        }

        _counters[index] = DateTime.UtcNow;

        return false;
    }

    public void Setup(int index, int interval)
    {
        if (_disposed) throw new ObjectDisposedException("Counter is disposed.");

        _intervals[index] = interval;
    }

    public void Dispose()
    {
        _disposed = true;
        _counters = new DateTime[0];
        _intervals = new int[0];
    }
}