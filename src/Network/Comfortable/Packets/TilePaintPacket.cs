// Terraria SDK CodeGen
// This code was generated by the program, and it's normally if there are warnings.


namespace Ruby.Network.Comfortable.Packets;

internal class TilePaintPacket : IPacket<TilePaint>
{
   public int PacketID => 63;
   public TilePaint Deserialize(byte[] data)
   {
       using MemoryStream stream = new MemoryStream(data);
       using BinaryReader reader = new BinaryReader(stream);
       
       Int16 x = reader.ReadInt16();
       Int16 y = reader.ReadInt16();
       Byte paintId = reader.ReadByte();
       Byte coatId = reader.ReadByte();
       
       return new()
       {
           X = x,
           Y = y,
           PaintId = paintId,
           CoatId = coatId,
       };
   }
   public byte[] Serialize(TilePaint data)
   {
       return new PacketWriter().SetType(63)
           .PackInt16(data.X)
           .PackInt16(data.Y)
           .PackByte(data.PaintId)
           .PackByte(data.CoatId)
           .BuildPacket();
   }
}
 
public struct TilePaint
{
   public Int16 X;
   public Int16 Y;
   public Byte PaintId;
   public Byte CoatId;
}
