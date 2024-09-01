namespace Ruby.Server.Players.Jail;

public interface IJailable
{
    public bool IsJailed { get; }
    public IEnumerable<IJailFunction> ActiveJails { get; }

    public void Jail(IJailFunction jail);
    public void Unjail(IJailFunction jail);
}