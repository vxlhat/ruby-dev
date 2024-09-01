// Terraria SDK CodeGen
// This code was generated by the program, and it's normally if there are warnings.


using Microsoft.Xna.Framework;

namespace Ruby.Network.Comfortable.Packets;

internal class CombatTextValuePacket : IPacket<CombatTextValue>
{
   public int PacketID => 81;
   public CombatTextValue Deserialize(byte[] data)
   {
       using MemoryStream stream = new MemoryStream(data);
       using BinaryReader reader = new BinaryReader(stream);
       
       Single x = reader.ReadSingle();
       Single y = reader.ReadSingle();
       Color color = reader.ReadColor();
       Int32 amount = reader.ReadInt32();
       
       return new()
       {
           X = x,
           Y = y,
           Color = color,
           Amount = amount,
       };
   }
   public byte[] Serialize(CombatTextValue data)
   {
       return new PacketWriter().SetType(81)
           .PackSingle(data.X)
           .PackSingle(data.Y)
           .PackColor(data.Color)
           .PackInt32(data.Amount)
           .BuildPacket();
   }
}
 
public struct CombatTextValue
{
   public Single X;
   public Single Y;
   public Color Color;
   public Int32 Amount;
}
