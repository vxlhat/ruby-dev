using Ruby.Network.Comfortable;
using Ruby.Network.Primitive;
using Ruby.Server.Players;
using Terraria;

namespace Ruby.Network;

public static class ComfortableHook<TData> where TData : struct
{
    static ComfortableHook()
    {
        Binds = new List<PacketHandlerDelegate<TData>>();
    }

    internal static List<PacketHandlerDelegate<TData>> Binds;
    internal static IPacket<TData>? PacketReference;

    public static IPacket<TData> Packet => PacketReference ?? throw new InvalidOperationException($"Packet {typeof(TData).FullName} is not supported.");

    internal static void Setup(IPacket<TData> packet)
    {
        PacketReference = packet;
        PrimitiveHook<IncomingPacket>.Add((byte)packet.PacketID, OnPacketReceived);
    }

    private static void OnPacketReceived(RubyPlayer? target, IncomingPacket packet, ref bool handled)
    {
        if (Binds.Count == 0) return;

        byte[] data = new byte[packet.Length];
        Buffer.BlockCopy(NetMessage.buffer[packet.Sender].readBuffer, packet.Start, data, 0, packet.Length);

        TData packetData = Packet.Deserialize(data);
        foreach (var bind in Binds)
        {
            try
            {
                bind(target, packetData, ref handled);
            }
            catch (Exception ex)
            {
                ModernConsole.WriteLine($"$!d[$!r$rComfortableHook$!r$!d<$!r$c{typeof(TData)}$!r$!d>$!r$!d]: $!r$rError in handling: {ex.ToString()}");
            }
        }
    }

    public static bool Add(PacketHandlerDelegate<TData> packetBind)
    {
        if (Binds.Contains(packetBind) == true)
            return false;

        Binds.Add(packetBind);
        return true;
    }

    public static bool Remove(PacketHandlerDelegate<TData> packetBind)
    {
        return Binds.Remove(packetBind);
    }

    internal static void Reset()
    {
        Binds = new List<PacketHandlerDelegate<TData>>();
    }
}