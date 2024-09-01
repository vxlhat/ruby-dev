using Ruby.Network.Comfortable.Models;
using Ruby.Network.Comfortable.Packets;
using Terraria;

namespace Ruby.Server.Players.Characters;

public sealed class ReadonlyCharacter : ICharacter
{
    public ReadonlyCharacter(RubyPlayer player, PlayerCharacter character)
    {
        this.player = player;
        this.character = character;
    }

    public string Name => "Readonly";

    public bool AbleToChange => true;

    public bool SentCharacter => true;

    public readonly RubyPlayer player;
    public PlayerCharacter character;

    public void ProcessGreet()
    {
        NetMessage.SendData(7, player.Index);

        ReceiveInfo(default);
        ReceiveLife(0, 0);
        ReceiveMana(0, 0);

        for (int i = 0; i < character.Slots.Length; i++)
        {
            if (i > 58 && i <= 88)
                character.Slots[i] = character.Slots[260 + 30 * player.TPlayer.CurrentLoadoutIndex + (i - 59)];
        
            ReceiveSlot(i, default);
        }
    }

    public bool ReceiveInfo(PlayerInfo info)
    {
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

        player.TPlayer.difficulty = 0;
        player.TPlayer.extraAccessory = character.Info1.HasFlag(PlayerInfo1.ExtraAccessory);

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

        NetMessage.SendData(4, -1, -1, Terraria.Localization.NetworkText.Empty, player.Index);
        return true;
    }

    public bool ReceiveLife(int current, int max)
    {
        player.TPlayer.statLife = character.MaxLife;
        player.TPlayer.statLifeMax = character.MaxLife;
        player.TPlayer.statLifeMax2 = character.MaxLife;
        NetMessage.SendData(16, -1, -1, Terraria.Localization.NetworkText.Empty, player.Index);
        return true;
    }

    public bool ReceiveMana(int current, int max)
    {
        player.TPlayer.statLife = character.MaxLife;
        player.TPlayer.statLifeMax = character.MaxLife;
        player.TPlayer.statLifeMax2 = character.MaxLife;
        NetMessage.SendData(16, -1, -1, Terraria.Localization.NetworkText.Empty, player.Index);
        return true;
    }

    public bool ReceiveSlot(int slot, NetItem item)
    {
        item = character.Slots[slot];
        
        // TerrarifySlot(player, netItem, slot);

        PlayerTracker.BroadcastSdkPacket(new PlayerSlot()
        {
            PlayerIndex = (byte)player.Index,
            SlotIndex = (short)slot,
            ItemType = (short)item.ItemID,
            ItemStack = (short)item.ItemStack,
            ItemPrefix = item.ItemPrefix
        });
        return true;
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
    {
    }
}