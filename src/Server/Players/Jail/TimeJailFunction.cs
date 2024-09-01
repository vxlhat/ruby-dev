namespace Ruby.Server.Players.Jail;

public sealed class TimeJailFunction : IJailFunction
{
    public TimeJailFunction(RubyPlayer player, string givenBy, string reason, DateTime time)
    {
        Player = player;
        Time = time;
        IsEnabled = true;
        Reason = reason;
        GivenBy = givenBy;
    }

    public RubyPlayer Player { get; }
    public DateTime Time { get; }
    public bool IsEnabled { get; set; }
    public string GivenBy { get; }
    public string Reason { get; }

    public bool IsActive() => Time > DateTime.UtcNow;
}