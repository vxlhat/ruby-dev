using Microsoft.Xna.Framework;
using Ruby.Network;
using Ruby.Network.Comfortable;
using Ruby.Server.Players.Jail;
using Terraria;

namespace Ruby.Server.Players;

public static class PlayerTracker
{
    public static readonly RubyPlayer[] Players = new RubyPlayer[256];
    public static int OnlinePlayers => Players.Count(p => p != null && p.Active);
    public static int MaxSlots = Main.maxNetPlayers;

    internal static void Initialize()
    {
        ServerHooks.PlayerConnected.Add(OnPlayerConnected);
    }

    private static void OnPlayerConnected(int index)
    {
        var player = new RubyPlayer(index);
        Players[index] = player;

        player.Jail(new LoopJailFunction(player, "not_authenticated", "Не подключен полностью",
            p => p.Active == false || p.Name == "" || p.UUID == ""));
    }

    public static void BroadcastText(string text, Color color)
    {
        byte[] buffer = new PacketWriter().SetType(82)
                .PackUInt16(1)
                .PackByte(255)
                .PackByte(0)
                .PackString(text)
                .PackColor(color)
                .BuildPacket();

        BroadcastRawPacket(buffer);
    }

    public static void BroadcastSdkPacket<TData>(TData data) where TData : struct
    {
        byte[] buffer = ComfortableHook<TData>.Packet.Serialize(data);
        BroadcastRawPacket(buffer);
    }

    public static void BroadcastRawPacket(byte[] buffer)
    {
        foreach (var plr in Players)
            if (plr != null && plr.Socket.IsConnected())
                plr.SendRawPacket(buffer);
    }

    public static void SetMaxSlots(int maxSlots)
    {
        Main.maxNetPlayers = maxSlots;
    }
}