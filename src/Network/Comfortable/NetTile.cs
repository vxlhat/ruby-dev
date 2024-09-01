using Terraria;

namespace Ruby.Network.Comfortable.Structures;

public struct NetTile
{
    public NetTile(BinaryReader reader)
    {
        var bitsBytes = (BitsByte)reader.ReadByte();
        var bitsBytes2 = (BitsByte)reader.ReadByte();
        var bitsBytes3 = (BitsByte)reader.ReadByte();

        Active = bitsBytes[0];

        if (bitsBytes[4])
            Wire = true;

        if (bitsBytes[5])
            HalfBrick = true;

        if (bitsBytes[6])
            Actuator = true;

        if (bitsBytes[7])
            Inactive = true;

        Wire2 = bitsBytes2[0];
        Wire3 = bitsBytes2[1];

        if (bitsBytes2[2])
            TileColor = reader.ReadByte();
        
        if (bitsBytes2[3])
            WallColor = reader.ReadByte();

        if (Active)
        {
            TileID = reader.ReadUInt16();
            if (Main.tileFrameImportant[TileID])
            {
                FrameX = reader.ReadInt16();
                FrameY = reader.ReadInt16();
            }

            if (bitsBytes2[4])
                Slope++;

            if (bitsBytes2[5])
                Slope += 2;

            if (bitsBytes2[6])
                Slope += 4;
        }

        if (bitsBytes[3])
        {
            Liquid = reader.ReadByte();
            LiquidID = reader.ReadByte();
        }

        Wire4 = bitsBytes2[7];

        Fullbright = bitsBytes3[0];
        FullbrightWall = bitsBytes3[1];
        Invisible = bitsBytes3[2];
        InvisibleWall = bitsBytes3[3];
        
        if (bitsBytes[2]) Wall = reader.ReadUInt16();

    }
    
    public void Serialize(ref PacketWriter packet)
    {
        BitsByte bitsByte1 = new BitsByte();
        bitsByte1[0] = Active;
        bitsByte1[2] = Wall > 0;
        bitsByte1[3] = Liquid > 0;
        bitsByte1[4] = Wire;
        bitsByte1[5] = HalfBrick;
        bitsByte1[6] = Actuator;
        bitsByte1[7] = Inactive;

        BitsByte bitsByte2 = new BitsByte();
        bitsByte2[0] = Wire2;
        bitsByte2[1] = Wire3;
        bitsByte2[2] = Active && TileColor > 0;
        bitsByte2[3] = Wall > 0 && WallColor > 0;
        bitsByte2 = (byte)((byte)bitsByte2 + (byte)(Slope << 4));
        bitsByte2[7] = Wire4;

        BitsByte bitsByte3 = new BitsByte();
        bitsByte3[0] = Fullbright;
        bitsByte3[1] = FullbrightWall;
        bitsByte3[2] = Invisible;
        bitsByte3[3] = InvisibleWall;

        packet = packet.PackByte((byte)bitsByte1)
            .PackByte((byte)bitsByte2)
            .PackByte((byte)bitsByte3);

        if (bitsByte2[2])
            packet = packet.PackByte((byte)TileColor);

        if (bitsByte2[3])
            packet = packet.PackByte((byte)WallColor);

        if (Active)
        {
            packet = packet.PackUInt16(TileID);
            if (Main.tileFrameImportant[TileID])
            {
                packet = packet.PackInt16(FrameX)
                    .PackInt16(FrameY);
            }
        }

        if (Wall > 0)
            packet = packet.PackUInt16(Wall);

        if (Liquid > 0)
        {
            packet = packet.PackByte(Liquid)
                .PackByte(LiquidID);
        }
    }

    public bool Active;
    public ushort TileID;
    public short FrameX;
    public short FrameY;
    public ushort Wall;
    public byte Liquid;
    public byte LiquidID;
    public bool Wire;
    public bool Wire2;
    public bool Wire3;
    public bool Wire4;
    public bool Inactive;
    public bool HalfBrick;
    public bool Actuator;
    public byte TileColor;
    public byte WallColor;
    public byte Slope;
    public bool Fullbright;
    public bool FullbrightWall;
    public bool Invisible;
    public bool InvisibleWall;
}