using Ruby.Network.Comfortable.Packets;

namespace Ruby.Server.Players.Characters;

public interface ICharacter
{
    public string Name { get; }

    public bool AbleToChange { get; }
    public bool SentCharacter { get; }

    public void ProcessGreet();

    public bool ReceiveSlot(int slot, NetItem item);
    public bool ReceiveInfo(PlayerInfo info);
    public bool ReceiveLife(int current, int max);
    public bool ReceiveMana(int current, int max);

    public void SetSlot(int slot, NetItem item, bool quiet);
    public void SetInfo(PlayerInfo info, bool quiet);
    public void SetLife(int current, int max, bool quiet);
    public void SetMana(int current, int max, bool quiet);

    public void TrySave();
}