using Ruby.Network.Comfortable.Packets;

namespace Ruby.Server.Players.Characters;

public sealed class ClientsideCharacter : ICharacter
{
    public string Name => "Clientside";

    public bool AbleToChange => true;

    public bool SentCharacter => true;

    public void ProcessGreet()
    {

    }

    public bool ReceiveInfo(PlayerInfo info)
    {
        return false;
    }

    public bool ReceiveLife(int current, int max)
    {
        return false;
    }

    public bool ReceiveMana(int current, int max)
    {
        return false;
    }

    public bool ReceiveSlot(int slot, NetItem item)
    {
        return false;
    }

    public void SetInfo(PlayerInfo info, bool quiet)
    {
    }

    public void SetLife(int current, int max, bool quiet)
    {
    }

    public void SetMana(int current, int max, bool quiet)
    {
    }

    public void SetSlot(int slot, NetItem item, bool quiet)
    {
    }

    public void TrySave()
    {}
}