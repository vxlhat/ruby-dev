// Terraria SDK CodeGen
// This code was generated by the program, and it's normally if there are warnings.


namespace Ruby.Network.Comfortable.Packets;

internal class PlayerZonePacket : IPacket<PlayerZone>
{
   public int PacketID => 36;
   public PlayerZone Deserialize(byte[] data)
   {
       using MemoryStream stream = new MemoryStream(data);
       using BinaryReader reader = new BinaryReader(stream);
       
       Byte zone1 = reader.ReadByte();
       Byte zone2 = reader.ReadByte();
       Byte zone3 = reader.ReadByte();
       Byte zone4 = reader.ReadByte();
       Byte zone5 = reader.ReadByte();
       
       return new()
       {
           Zone1 = zone1,
           Zone2 = zone2,
           Zone3 = zone3,
           Zone4 = zone4,
           Zone5 = zone5,
       };
   }
   public byte[] Serialize(PlayerZone data)
   {
       return new PacketWriter().SetType(36)
           .PackByte(data.Zone1)
           .PackByte(data.Zone2)
           .PackByte(data.Zone3)
           .PackByte(data.Zone4)
           .PackByte(data.Zone5)
           .BuildPacket();
   }
}
 
public struct PlayerZone
{
   public Byte Zone1;
   public Byte Zone2;
   public Byte Zone3;
   public Byte Zone4;
   public Byte Zone5;
}
