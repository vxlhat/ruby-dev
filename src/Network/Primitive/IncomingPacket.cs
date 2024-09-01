using Terraria;

namespace Ruby.Network.Primitive;

public struct IncomingPacket
{
    public IncomingPacket(byte packetId, byte sender, int start, int length)
    {
        PacketID = packetId;
        Sender = sender;
        Start = start;
        Length = length;
    }
    
    public BinaryReader GetReader()
    {
        if (_reader == null || _stream == null)
        {
            _stream = new MemoryStream(NetMessage.buffer[Sender].readBuffer);
            _reader = new BinaryReader(_stream);
        }

        _reader.BaseStream.Position = Start;
        return _reader;
    }
    
    internal void DisposeReader()
    {
        _reader?.Dispose();
        _reader?.BaseStream.Dispose();
    }

    private MemoryStream? _stream;
    private BinaryReader? _reader;

    public byte PacketID;
    public byte Sender;
    public int Start;
    public int Length;
}