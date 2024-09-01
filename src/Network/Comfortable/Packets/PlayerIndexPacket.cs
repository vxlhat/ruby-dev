// Terraria SDK CodeGen
// This code was generated by the program, and it's normally if there are warnings.


namespace Ruby.Network.Comfortable.Packets;

internal class PlayerIndexPacket : IPacket<PlayerIndex>
{
   public int PacketID => 3;

   public PlayerIndex Deserialize(byte[] data)
   {
       using MemoryStream stream = new MemoryStream(data);
       using BinaryReader reader = new BinaryReader(stream);
       
       Byte playerId = reader.ReadByte();
       Boolean unknown = reader.ReadBoolean();
       
       return new()
       {
           PlayerId = playerId,
           Unknown = unknown,
       };
   }
   public byte[] Serialize(PlayerIndex data)
   {
       return new PacketWriter().SetType(3)
           .PackByte(data.PlayerId)
           .PackBoolean(data.Unknown)
           .BuildPacket();
   }
}
 
public struct PlayerIndex
{
   public Byte PlayerId;
   public Boolean Unknown;
}
