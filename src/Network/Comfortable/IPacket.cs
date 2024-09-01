namespace Ruby.Network.Comfortable;

public interface IPacket<T> where T : struct
{
    public int PacketID { get; }

    public T Deserialize(byte[] data);
    public byte[] Serialize(T data);
}