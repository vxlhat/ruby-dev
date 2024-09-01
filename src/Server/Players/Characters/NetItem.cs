namespace Ruby.Server.Players.Characters;

public struct NetItem
{
    public NetItem(short itemId, short itemStack, byte itemPrefix)
    {
        ItemID = itemId;
        ItemStack = itemStack;
        ItemPrefix = itemPrefix;
    }

    public short ItemID;
    public short ItemStack;
    public byte ItemPrefix;

    public static bool operator ==(NetItem left, NetItem right)
    {
        if (left.ItemID == right.ItemID && left.ItemStack == right.ItemStack && left.ItemPrefix == right.ItemPrefix)
        {
            return true;
        }

        return false;
    }

    public static bool operator !=(NetItem left, NetItem right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}