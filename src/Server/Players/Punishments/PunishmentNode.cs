using Ruby.Storages;

namespace Ruby.Server.Players.Punishments;

public static class PunishmentNode<T> where T : PlayerPunishment
{
    static PunishmentNode()
    {
        ServerHooks.PlayerConnected.Add((index) => Players[index] = new(PlayerTracker.Players[index]));
    }

    internal static PunishmentCollection<T>[] Players = new PunishmentCollection<T>[256];

    public static MongoCollection<T> Collection { get; } = MongoDatabase.Main.Get<T>();

    public static void AddPunishment(T punishment)
    {
        Collection.Save(punishment);

        ModernConsole.WriteLine($"$!d[$!r$bPunishment$!r$!d<$!r$c{typeof(T)}$!r$!d>$!r$!d]: $!rAdded punishment for player $!b$c{punishment.Username}$!r:");
        ModernConsole.WriteLine($"      $!d-> Reason: $!r$!i$c{punishment.Reason}$!r");
        ModernConsole.WriteLine($"      $!d-> Expiration: $!r$!i$c{punishment.GetExpirationText("{0}d. {1}h. {2}min. {3}s.")}$!r");

        foreach (var collection in Players)
            if (collection != null && punishment.CanBePunished(collection.basePlayer)) 
                collection.Add(punishment);
    }

    public static void RemovePunishment(T punishment)
    {
        Collection.Save(punishment);

        ModernConsole.WriteLine($"$!d[$!r$bPunishment$!r$!d<$!r$c{typeof(T)}$!r$!d>$!r$!d]: $!rRemoved punishment for player $!b$c{punishment.Username}$!r:");
        ModernConsole.WriteLine($"      $!d-> Reason: $!r$!i$c{punishment.Reason}$!r");

        foreach (var collection in Players)
            if (collection != null)
                collection.Remove(punishment);
    }
}