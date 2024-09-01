using System.Collections;

namespace Ruby.Server.Players.Punishments;

public sealed class PunishmentCollection<T> : IEnumerable<T> where T : PlayerPunishment
{
    internal PunishmentCollection(RubyPlayer basePlayer)
    {
        this.basePlayer = basePlayer;
        _punishments = new List<T>();
    }

    public IReadOnlyList<T> Punishments => _punishments.AsReadOnly(); 
    
    internal RubyPlayer basePlayer;

    private List<T> _punishments;

    internal void Update(List<T> punishments)
    {
        _punishments = punishments;
    }

    internal void TryAdd(T punishment)
    {
        if (punishment.CanBePunished(basePlayer))
            _punishments.Add(punishment);
    }

    public void Add(T punishment)
    {
        _punishments.Add(punishment);
    }

    public void Remove(T punishment)
    {
        _punishments.Remove(punishment);
    }

    public IEnumerator<T> GetEnumerator() => _punishments.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _punishments.GetEnumerator();
}