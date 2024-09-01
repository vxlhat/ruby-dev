// Terraria SDK CodeGen
// This code was generated by the program, and it's normally if there are warnings.


namespace Ruby.Network.Comfortable.Packets;

internal class PlayerNpcStatsUpdatePacket : IPacket<PlayerNpcStatsUpdate>
{
   public int PacketID => 76;
   public PlayerNpcStatsUpdate Deserialize(byte[] data)
   {
       using MemoryStream stream = new MemoryStream(data);
       using BinaryReader reader = new BinaryReader(stream);
       
       Int32 anglerQuestsFinished = reader.ReadInt32();
       Int32 golferScoreAccumulated = reader.ReadInt32();
       
       return new()
       {
           AnglerQuestsFinished = anglerQuestsFinished,
           GolferScoreAccumulated = golferScoreAccumulated,
       };
   }
   public byte[] Serialize(PlayerNpcStatsUpdate data)
   {
       return new PacketWriter().SetType(76)
           .PackInt32(data.AnglerQuestsFinished)
           .PackInt32(data.GolferScoreAccumulated)
           .BuildPacket();
   }
}
 
public struct PlayerNpcStatsUpdate
{
   public Int32 AnglerQuestsFinished;
   public Int32 GolferScoreAccumulated;
}
