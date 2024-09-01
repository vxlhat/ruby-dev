// Terraria SDK CodeGen
// This code was generated by the program, and it's normally if there are warnings.


namespace Ruby.Network.Comfortable.Packets;

internal class ItemRemoveOwnerPacket : IPacket<ItemRemoveOwner>
{
   public int PacketID => 39;
   public ItemRemoveOwner Deserialize(byte[] data)
   {
       using MemoryStream stream = new MemoryStream(data);
       using BinaryReader reader = new BinaryReader(stream);
       
       Int16 itemIndex = reader.ReadInt16();
       
       return new()
       {
           ItemIndex = itemIndex,
       };
   }
   public byte[] Serialize(ItemRemoveOwner data)
   {
       return new PacketWriter().SetType(39)
           .PackInt16(data.ItemIndex)
           .BuildPacket();
   }
}
 
public struct ItemRemoveOwner
{
   public Int16 ItemIndex;
}
