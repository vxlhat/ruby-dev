using Ruby.Network.Comfortable.Models;
using Ruby.Network.Comfortable.Packets;
using Terraria;
using Terraria.Localization;

namespace Ruby.Server.Players.Characters;

public sealed class ServersideCharacter : ICharacter
{
    public ServersideCharacter(RubyPlayer player, PlayerCharacter character)
    {
        this.player = player;
        this.character = character;
    }

    public string Name => "Serverside";

    public bool AbleToChange => player.CharacterReseted == null || player.CharacterReseted.Value.AddSeconds(3) < DateTime.UtcNow;

    public bool SentCharacter => true;

    public readonly RubyPlayer player;
    public PlayerCharacter character;

    public void ProcessGreet()
    {
        SetInfo(new PlayerInfo()
        {
            Info1 = character.Info1,
            Info2 = character.Info2,
            Info3 = character.Info3,
        }, false);

        SetLife(character.MaxLife, character.MaxLife, false);
        SetMana(character.MaxMana, character.MaxMana, false);

        for (int i = 0; i < character.Slots.Length; i++)
        {
            if (i > 58 && i <= 88)
                character.Slots[i] = character.Slots[260 + 30 * player.TPlayer.CurrentLoadoutIndex + (i - 59)];
        
            SetSlot(i, character.Slots[i], false);
        }
    }

    public bool ReceiveInfo(PlayerInfo packet)
    {
        SetInfo(packet, true);
        return false;
    }

    public bool ReceiveLife(int current, int max)
    {
        if (AbleToChange == false)
        {
            current = character.MaxLife;
            max = character.MaxLife;
        }

        SetLife(current, max, AbleToChange);
        return !AbleToChange;
    }

    public bool ReceiveMana(int current, int max)
    {
        if (AbleToChange == false)
        {
            current = character.MaxMana;
            max = character.MaxMana;
        }

        SetMana(current, max, AbleToChange);
        return !AbleToChange;
    }

    public bool ReceiveSlot(int slot, NetItem item)
    {
        SetSlot(slot, item, true);
        return false;
    }

    public void SetInfo(PlayerInfo packet, bool quiet)
    {
        character.Info1 = packet.Info1;
        character.Info2 = packet.Info2;
        character.Info3 = packet.Info3;

        character.Save();

        if (quiet) return;

        player.TPlayer.skinVariant = character.Visuals.SkinVariant;
        player.TPlayer.hair = character.Visuals.Hair;
        player.TPlayer.hairDye = character.Visuals.HairDye;
        player.TPlayer.hideMisc = character.Visuals.HideMisc;
        player.TPlayer.hideVisibleAccessory = character.Visuals.HideAccessories ?? new bool[10];
        player.TPlayer.hairColor = character.Visuals.HairColor;
        player.TPlayer.skinColor = character.Visuals.SkinColor;
        player.TPlayer.eyeColor = character.Visuals.EyesColor;
        player.TPlayer.shirtColor = character.Visuals.ShirtColor;
        player.TPlayer.underShirtColor = character.Visuals.UndershirtColor;
        player.TPlayer.pantsColor = character.Visuals.PantsColor;
        player.TPlayer.shoeColor = character.Visuals.ShoesColor;

        player.TPlayer.extraAccessory = Main.hardMode;

        player.TPlayer.UsingBiomeTorches = character.Info2.HasFlag(PlayerInfo2.UsingBiomeTorches);
        player.TPlayer.happyFunTorchTime = character.Info2.HasFlag(PlayerInfo2.HappyTorchTime);
        player.TPlayer.unlockedBiomeTorches = character.Info2.HasFlag(PlayerInfo2.UnlockedBiomeTorches);
        player.TPlayer.unlockedSuperCart = character.Info2.HasFlag(PlayerInfo2.UnlockedSuperCart);
        player.TPlayer.enabledSuperCart = character.Info2.HasFlag(PlayerInfo2.EnabledSuperCart);

        player.TPlayer.usedAegisCrystal = character.Info3.HasFlag(PlayerInfo3.UsedAegisCrystal);
        player.TPlayer.usedAegisFruit = character.Info3.HasFlag(PlayerInfo3.UsedAegisFruit);
        player.TPlayer.usedArcaneCrystal = character.Info3.HasFlag(PlayerInfo3.UsedArcaneCrystal);
        player.TPlayer.usedGalaxyPearl = character.Info3.HasFlag(PlayerInfo3.UsedGalaxyPearl);
        player.TPlayer.usedGummyWorm = character.Info3.HasFlag(PlayerInfo3.UsedGummyWorm);
        player.TPlayer.usedAmbrosia = character.Info3.HasFlag(PlayerInfo3.UsedAmbrosia);
        player.TPlayer.ateArtisanBread = character.Info3.HasFlag(PlayerInfo3.AteArtisanBread);

        try
        {
            NetMessage.SendData(4, -1, -1, NetworkText.Empty, player.Index);
        }
        catch { }
    }

    public void SetLife(int current, int max, bool quiet)
    {
        if (character.MaxLife != max)
        {
            character.MaxLife = max;
            character.Save();
        }

        if (quiet) return;

        player.TPlayer.statLifeMax = character.MaxLife;
        player.TPlayer.statLifeMax2 = character.MaxLife;
        NetMessage.SendData(16, -1, -1, NetworkText.Empty, player.Index);
    }

    public void SetMana(int current, int max, bool quiet)
    {
        if (character.MaxMana != max)
        {
            character.MaxMana = max;
            character.Save();
        }

        if (quiet) return;
        
        player.TPlayer.statManaMax = character.MaxMana;
        player.TPlayer.statManaMax2 = character.MaxMana;
        NetMessage.SendData(42, -1, -1, NetworkText.Empty, player.Index);
    }

    public void SetSlot(int slot, NetItem item, bool quiet)
    {
        int fixedSlot = 260 + 30 * player.TPlayer.CurrentLoadoutIndex + (slot - 59);

        if (slot >= 59 && slot <= 88 && character.Slots[fixedSlot] != item)
        {
            character.Slots[fixedSlot] = item;
            //character.Save();
        }
        else if (character.Slots[slot] != item)
        {
            character.Slots[slot] = item;
            //character.Save();
        }

        if (quiet) return;

        CharactersNode.TerrarifySlot(player, item, slot);

        PlayerTracker.BroadcastSdkPacket(new PlayerSlot()
        {
            PlayerIndex = (byte)player.Index,
            SlotIndex = (short)slot,
            ItemType = item.ItemID,
            ItemStack = item.ItemStack,
            ItemPrefix = item.ItemPrefix
        });
    }

    public void TrySave()
    {
        character.Save();
    }
}