using Ruby.Network.Comfortable.Packets;
using Ruby.Server;
using Ruby.Server.Players;
using Ruby.Server.Players.Punishments;
using Terraria;

namespace Ruby.Network;

internal static class PacketHandlers
{
    internal static void Initialize()
    {
        ComfortableHook<ConnectRequest>.Add(OnConnectRequest);
        ComfortableHook<PlayerUUID>.Add(OnPlayerUUID);
        ComfortableHook<PlayerInfo>.Add(OnPlayerInfo);
    }

    private static void OnConnectRequest(RubyPlayer? target, ConnectRequest packet, ref bool handled)
    {
        handled = true;
        if (target == null) return;

        if (target.CheckBansByIP())
            return; 

        target.SendSdkPacket<PlayerIndex>(new PlayerIndex()
        {
            PlayerId = (byte)target.Index
        });

		Netplay.Clients[target.Index].State = 1;
    }

    private static void OnPlayerUUID(RubyPlayer? target, PlayerUUID packet, ref bool handled)
    {
        if (target == null) return;

        if (Guid.TryParse(packet.Uuid, out Guid guid) == false)
        {
            target.Kick("Кажется, у вас поврежден UUID.");
            return;
        }

        target.UUID = packet.Uuid;

        if (target.CheckBansByUUID())
            return;
    }

    private static void OnPlayerInfo(RubyPlayer? target, PlayerInfo packet, ref bool handled)
    {
        if (target == null) return;

        if (packet.Name.Trim(' ') == "")
        {
            target.Kick("Пустой никнейм.");
            handled = true;
            return;
        }
        
        if (target.Name != "") return;

        target.SetName(packet.Name, false);

        if (target.CheckBansByName())
            return; 
    }
}