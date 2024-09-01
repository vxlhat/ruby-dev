using Terraria;

namespace Ruby.Network.Primitive;

public struct IncomingModule
{
    public IncomingModule(ushort moduleId, byte sender, int start, int length)
    {
        ModuleID = moduleId;
        Sender = sender;
        ModuleStart = start;
        Length = length;
    }

    public BinaryReader GetReader()
    {
        if (_reader == null || _stream == null)
        {
            _stream = new MemoryStream(NetMessage.buffer[Sender].readBuffer);
            _reader = new BinaryReader(_stream);
        }

        _reader.BaseStream.Position = ModuleStart;
        return _reader;
    }
    
    internal void DisposeReader()
    {
        _reader?.Dispose();
        _reader?.BaseStream.Dispose();
    }

    private MemoryStream? _stream;
    private BinaryReader? _reader;

    public ushort ModuleID;
    public byte Sender;
    public int ModuleStart;
    public int Length;
}