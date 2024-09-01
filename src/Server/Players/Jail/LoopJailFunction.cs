namespace Ruby.Server.Players.Jail;

public sealed class LoopJailFunction : IJailFunction
{
    public LoopJailFunction(RubyPlayer player, string givenBy, string reason, Func<RubyPlayer, bool> func)
    {
        Player = player;
        Function = func;
        IsEnabled = true;
        GivenBy = givenBy;
        Reason = reason;
    }

    public RubyPlayer Player { get; }
    public Func<RubyPlayer, bool> Function { get; }
    public bool IsEnabled { get; set; }
    public string GivenBy { get; }
    public string Reason { get; }

    public bool IsActive() => Function(Player);
}