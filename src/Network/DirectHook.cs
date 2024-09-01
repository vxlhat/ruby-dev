using Ruby.Network.Primitive;

namespace Ruby.Network;

public static class DirectHook<TPacket>
{
    static DirectHook()
    {
        Type packetType = typeof(TPacket);
        if (packetType != typeof(IncomingPacket) && packetType != typeof(OutcomingPacket))
            throw new InvalidOperationException("Cannot use DirectHook<TPacket> because type of that packet is not supported -> Use IncomingPacket or OutcomingPacket or IncomingModule");

        Binds = new List<PacketHandlerDelegate<TPacket>>();
    }

    internal static List<PacketHandlerDelegate<TPacket>>? Binds;

    public static bool Add(PacketHandlerDelegate<TPacket> packetBind)
    {
        if (Binds == null)
        {
            Binds = new List<PacketHandlerDelegate<TPacket>>()
            {
                packetBind
            };

            return true;
        }

        if (Binds.Contains(packetBind) == true)
            return false;

        Binds?.Add(packetBind);
        return true;
    }

    public static bool Remove(byte packetId, PacketHandlerDelegate<TPacket> packetBind)
    {
        if (Binds == null)
            return true;

        return Binds.Remove(packetBind);
    }

    internal static void Reset()
    {
        Binds = new List<PacketHandlerDelegate<TPacket>>();
    }
}