namespace Ruby.Server.Players.Jail;

public interface IJailFunction
{
    public RubyPlayer Player { get; }
    public bool IsEnabled { get; set; }
    public string GivenBy { get; }
    public string Reason { get; }

    public bool IsActive();
}