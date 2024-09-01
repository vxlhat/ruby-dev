using Ruby.Server.Players.Punishments;

namespace Ruby.Server.Player.Punishments;

public interface IPunishable
{
    public PunishmentCollection<T> GetPunishments<T>() where T : PlayerPunishment;

    public bool AnyPunishment<T>() where T : PlayerPunishment;
}