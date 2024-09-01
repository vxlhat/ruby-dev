using MongoDB.Bson.Serialization.Attributes;

namespace Ruby.Server.Players.Punishments;

[BsonIgnoreExtraElements]
public class PlayerBan : PlayerPunishment
{
    public PlayerBan(string id, DateTime time, string reason, string? username, string? ip, string? uuid) : base(id, time, reason, username, ip, uuid)
    {
    }

    public override void Save()
    {
        PunishmentNode<PlayerBan>.Collection.Save(this);
    }
    public override void Remove()
    {
        PunishmentNode<PlayerBan>.Collection.Remove(Name);
    }
}