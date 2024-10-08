// Terraria SDK CodeGen
// This code was generated by the program, and it's normally if there are warnings.


namespace Ruby.Network.Comfortable.Packets;

internal class TileEntityPlaceV2Packet : IPacket<TileEntityPlaceV2>
{
   public int PacketID => 87;
   public TileEntityPlaceV2 Deserialize(byte[] data)
   {
       using MemoryStream stream = new MemoryStream(data);
       using BinaryReader reader = new BinaryReader(stream);
       
       Int16 x = reader.ReadInt16();
       Int16 y = reader.ReadInt16();
       Byte type = reader.ReadByte();
       
       return new()
       {
           X = x,
           Y = y,
           Type = type,
       };
   }
   public byte[] Serialize(TileEntityPlaceV2 data)
   {
       return new PacketWriter().SetType(87)
           .PackInt16(data.X)
           .PackInt16(data.Y)
           .PackByte(data.Type)
           .BuildPacket();
   }
}
 
public struct TileEntityPlaceV2
{
   public Int16 X;
   public Int16 Y;
   public Byte Type;
}
