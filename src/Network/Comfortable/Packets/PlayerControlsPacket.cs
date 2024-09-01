// Terraria SDK CodeGen
// That code was generated by program and this is normally if there is warnings.


using Microsoft.Xna.Framework;
using Ruby.Network.Comfortable.Models;

namespace Ruby.Network.Comfortable.Packets;

internal class PlayerControlsPacket : IPacket<PlayerControls>
{
   public int PacketID => 13;
   public PlayerControls Deserialize(byte[] data)
   {
       using MemoryStream stream = new MemoryStream(data);
       using BinaryReader reader = new BinaryReader(stream);
       
       Byte playerIndex = reader.ReadByte();
       PlayerControls1 controls1 = reader.Read<PlayerControls1>();
       PlayerControls2 controls2 = reader.Read<PlayerControls2>();
       PlayerControls3 controls3 = reader.Read<PlayerControls3>();
       PlayerControls4 controls4 = reader.Read<PlayerControls4>();
       Byte selectedItem = reader.ReadByte();
       Vector2 position = reader.ReadVector2();

       return new()
       {
           PlayerIndex = playerIndex,
           Controls1 = controls1,
           Controls2 = controls2,
           Controls3 = controls3,
           Controls4 = controls4,
           SelectedItem = selectedItem,
           Position = position,
       };
   }
   public byte[] Serialize(PlayerControls data)
   {
       var packet =  new PacketWriter().SetType(13)
           .PackByte(data.PlayerIndex)
           .PackByte((byte)data.Controls1)
           .PackByte((byte)data.Controls2)
           .PackByte((byte)data.Controls3)
           .PackByte((byte)data.Controls4)
           .PackByte(data.SelectedItem)
           .PackVector2(data.Position);

        return packet.BuildPacket();
   }
}
 
public struct PlayerControls
{
   public Byte PlayerIndex;
   public PlayerControls1 Controls1;
   public PlayerControls2 Controls2;
   public PlayerControls3 Controls3;
   public PlayerControls4 Controls4;
   public Byte SelectedItem;
   public Vector2 Position;
   public Vector2 Velocity;
   public Vector2 PotionOfReturnOriginal;
   public Vector2 PotionOfReturnHome;
}
