// Terraria SDK CodeGen
// This code was generated by the program, and it's normally if there are warnings.


namespace Ruby.Network.Comfortable.Packets;

internal class PlayerSwitchLoadoutPacket : IPacket<PlayerSwitchLoadout>
{
   public int PacketID => 147;
   public PlayerSwitchLoadout Deserialize(byte[] data)
   {
       using MemoryStream stream = new MemoryStream(data);
       using BinaryReader reader = new BinaryReader(stream);
       
       Byte playerIndex = reader.ReadByte();
       Byte loadoutIndex = reader.ReadByte();
       UInt16 accessoryVisiblity = reader.ReadUInt16();
       
       return new()
       {
           PlayerIndex = playerIndex,
           LoadoutIndex = loadoutIndex,
           AccessoryVisiblity = accessoryVisiblity,
       };
   }
   public byte[] Serialize(PlayerSwitchLoadout data)
   {
       return new PacketWriter().SetType(147)
           .PackByte(data.PlayerIndex)
           .PackByte(data.LoadoutIndex)
           .PackUInt16(data.AccessoryVisiblity)
           .BuildPacket();
   }
}
 
public struct PlayerSwitchLoadout
{
   public Byte PlayerIndex;
   public Byte LoadoutIndex;
   public UInt16 AccessoryVisiblity;
}
