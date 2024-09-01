using Ruby.Network.Primitive;

namespace Ruby.Network;

public static class PrimitiveHook<TPacket>
{
    static PrimitiveHook()
    {
        Type packetType = typeof(TPacket);
        if (packetType != typeof(IncomingPacket) && packetType != typeof(OutcomingPacket) && packetType != typeof(IncomingModule))
            throw new InvalidOperationException("Cannot use PrimitiveHook<TPacket> because type of that packet is not supported -> Use IncomingPacket or OutcomingPacket or IncomingModule");

        Hijacks = new PacketHandlerDelegate<TPacket>?[255];
        Binds = new List<PacketHandlerDelegate<TPacket>>?[255];
    }

    internal static PacketHandlerDelegate<TPacket>?[] Hijacks;
    internal static List<PacketHandlerDelegate<TPacket>>?[] Binds;

    public static void Hijack(byte packetId, PacketHandlerDelegate<TPacket>? packetBind)
    {
        Hijacks[packetId] = packetBind;
    }

    public static bool Add(byte packetId, PacketHandlerDelegate<TPacket> packetBind)
    {
        if (packetId < 0 || packetId > Binds.Length - 1)
            return false;

        if (Binds[packetId] == null)
        {
            Binds[packetId] = new List<PacketHandlerDelegate<TPacket>>()
            {
                packetBind
            };

            return true;
        }

        if (Binds[packetId]?.Contains(packetBind) == true)
            return false;

        Binds[packetId]?.Add(packetBind);
        return true;
    }

    public static bool Remove(byte packetId, PacketHandlerDelegate<TPacket> packetBind)
    {
        if (packetId < 0 || packetId > Binds.Length - 1)
            return false;

        if (Binds[packetId] == null)
            return true;

        return Binds[packetId]?.Remove(packetBind) ?? false;
    }

    internal static void Reset()
    {
        Hijacks = new PacketHandlerDelegate<TPacket>?[255];
        Binds = new List<PacketHandlerDelegate<TPacket>>?[255];
    }
}