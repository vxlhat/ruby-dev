using Ruby.Server.Players;

namespace Ruby.Network;

public delegate void PacketHandlerDelegate<TPacket>(RubyPlayer? target, TPacket packet, ref bool handled);