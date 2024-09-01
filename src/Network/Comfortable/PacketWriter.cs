using Microsoft.Xna.Framework;

namespace Ruby.Network.Comfortable;

public sealed class PacketWriter
{
    internal MemoryStream packetStream;
    internal BinaryWriter packetBinaryWriter;

    public PacketWriter()
    {
        packetStream = new MemoryStream();
        packetBinaryWriter = new BinaryWriter(packetStream);
        packetBinaryWriter.BaseStream.Position = 3L;
    }

    public PacketWriter SetType(short type)
    {
        var position = packetBinaryWriter.BaseStream.Position;
        packetBinaryWriter.BaseStream.Position = 2L;
        packetBinaryWriter.Write(type);
        packetBinaryWriter.BaseStream.Position = position;
        return this;
    }

    public PacketWriter PackBoolean(bool flag)
    {
        packetBinaryWriter.Write(flag);
        return this;
    }

    public PacketWriter PackSByte(sbyte num)
    {
        packetBinaryWriter.Write(num);
        return this;
    }

    public PacketWriter PackByte(byte num)
    {
        packetBinaryWriter.Write(num);
        return this;
    }

    public PacketWriter PackInt16(short num)
    {
        packetBinaryWriter.Write(num);
        return this;
    }

    public PacketWriter PackUInt16(ushort num)
    {
        packetBinaryWriter.Write(num);
        return this;
    }

    public PacketWriter PackInt32(int num)
    {
        packetBinaryWriter.Write(num);
        return this;
    }

    public PacketWriter PackUInt32(uint num)
    {
        packetBinaryWriter.Write(num);
        return this;
    }

    public PacketWriter PackInt64(long num)
    {
        packetBinaryWriter.Write(num);
        return this;
    }

    public PacketWriter PackUInt64(ulong num)
    {
        packetBinaryWriter.Write(num);
        return this;
    }

    public PacketWriter PackSingle(float num)
    {
        packetBinaryWriter.Write(num);
        return this;
    }

    public PacketWriter PackString(string str)
    {
        packetBinaryWriter.Write(str);
        return this;
    }

    public PacketWriter PackColor(byte r, byte g, byte b)
    {
        packetBinaryWriter.Write(r);
        packetBinaryWriter.Write(g);
        packetBinaryWriter.Write(b);
        return this;
    }

    public PacketWriter PackColor(Color color)
    {
        packetBinaryWriter.Write(color.R);
        packetBinaryWriter.Write(color.G);
        packetBinaryWriter.Write(color.B);
        return this;
    }

    public PacketWriter PackVector2(float x, float y)
    {
        packetBinaryWriter.Write(x);
        packetBinaryWriter.Write(y);
        return this;
    }

    public PacketWriter PackVector2(Vector2 vector2)
    {
        packetBinaryWriter.Write(vector2.X);
        packetBinaryWriter.Write(vector2.Y);
        return this;
    }

    public PacketWriter PackPoint(int x, int y)
    {
        packetBinaryWriter.Write(x);
        packetBinaryWriter.Write(y);
        return this;
    }

    public PacketWriter PackPoint(Point point)
    {
        packetBinaryWriter.Write(point.X);
        packetBinaryWriter.Write(point.Y);
        return this;
    }

    public byte[] BuildPacket()
    {
        var position = packetBinaryWriter.BaseStream.Position;
        packetBinaryWriter.BaseStream.Position = 0L;
        packetBinaryWriter.Write((short)position);
        packetBinaryWriter.BaseStream.Position = position;
        return packetStream.ToArray();
    }
}