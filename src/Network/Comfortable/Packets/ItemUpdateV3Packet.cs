// Terraria SDK CodeGen
// This code was generated by the program, and it's normally if there are warnings.


using Microsoft.Xna.Framework;

namespace Ruby.Network.Comfortable.Packets;

internal class ItemUpdateV3Packet : IPacket<ItemUpdateV3>
{
   public int PacketID => 148;
   public ItemUpdateV3 Deserialize(byte[] data)
   {
       using MemoryStream stream = new MemoryStream(data);
       using BinaryReader reader = new BinaryReader(stream);
       
       Int16 itemIndex = reader.ReadInt16();
       Vector2 position = reader.ReadVector2();
       Vector2 velocity = reader.ReadVector2();
       Int16 itemStack = reader.ReadInt16();
       Byte itemPrefix = reader.ReadByte();
       Byte ownIgnore = reader.ReadByte();
       Int16 itemType = reader.ReadInt16();
       Byte timeLeftCannotBeTakenByEnemies = reader.ReadByte();
       
       return new()
       {
           ItemIndex = itemIndex,
           Position = position,
           Velocity = velocity,
           ItemStack = itemStack,
           ItemPrefix = itemPrefix,
           OwnIgnore = ownIgnore,
           ItemType = itemType,
           TimeLeftCannotBeTakenByEnemies = timeLeftCannotBeTakenByEnemies,
       };
   }
   public byte[] Serialize(ItemUpdateV3 data)
   {
       return new PacketWriter().SetType(148)
           .PackInt16(data.ItemIndex)
           .PackVector2(data.Position)
           .PackVector2(data.Velocity)
           .PackInt16(data.ItemStack)
           .PackByte(data.ItemPrefix)
           .PackByte(data.OwnIgnore)
           .PackInt16(data.ItemType)
           .PackByte(data.TimeLeftCannotBeTakenByEnemies)
           .BuildPacket();
   }
}
 
public struct ItemUpdateV3
{
   public Int16 ItemIndex;
   public Vector2 Position;
   public Vector2 Velocity;
   public Int16 ItemStack;
   public Byte ItemPrefix;
   public Byte OwnIgnore;
   public Int16 ItemType;
   public Byte TimeLeftCannotBeTakenByEnemies;
}
