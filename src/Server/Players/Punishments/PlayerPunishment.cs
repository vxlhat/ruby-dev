using MongoDB.Bson.Serialization.Attributes;
using Ruby.Storages;

namespace Ruby.Server.Players.Punishments;

// dont forget about [BsonIgnoreExtraElements]
public abstract class PlayerPunishment : DataModel
{
    public PlayerPunishment(string id, DateTime time, string reason, string? username, string? ip, string? uuid) : base(id)
    {
        Time = time;
        Reason = reason;

        Username = username;
        IP = ip;
        UUID = uuid;
    }

    public DateTime Time { get; set; }
    public string Reason { get; set; }

    public string? Username { get; set; }
    public string? IP { get; set; }
    public string? UUID { get; set; }

    public bool IsExpired => Time < DateTime.UtcNow;

    public bool CanBePunished(RubyPlayer player)
    {
        return player.Name == Username || player.IP == IP || player.UUID == UUID;
    }

    public string GetExpirationText(string format = "{0}д. {1}ч. {2}мин. {3}с.")
    {
        TimeSpan span = Time - DateTime.UtcNow;
        return string.Format(format, span.Days, span.Hours, span.Minutes, span.Seconds);
    }
}