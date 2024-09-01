using Ruby.Network;
using Ruby.Network.Comfortable.Packets;
using Ruby.Storages;
using Terraria;
using Terraria.ID;

using Timer = System.Timers.Timer;

namespace Ruby.Server.Players.Characters;

public static class CharactersNode
{
    internal static CharactersConfig Config = Config<CharactersConfig>.GetData();
    internal static MongoDatabase Database = new MongoDatabase(ServerConfiguration.Current.MongoDbUri, Config.DatabaseName ?? ServerConfiguration.Current.MongoDbName);
    internal static CharacterVisuals?[] TempVisuals = new CharacterVisuals?[256];

    public static MongoCollection<PlayerCharacter> Collection { get; } = Database.Get<PlayerCharacter>();

    private static Timer SaveTimer = new Timer(1000)
    {
        AutoReset = true,
        Enabled = true
    };

    internal static void Initialize()
    {
        Main.ServerSideCharacter = Config.EnableSSC;

        if (Config.EnableSSC == false) return;

        ComfortableHook<PlayerSlot>.Add(OnPlayerSlot);
        ComfortableHook<PlayerMana>.Add(OnPlayerMana);
        ComfortableHook<PlayerLife>.Add(OnPlayerLife);
        ComfortableHook<PlayerInfo>.Add(OnPlayerInfo);

        ServerHooks.PlayerGreet.Add(OnPlayerGreet);
        ServerHooks.PlayerDisconnected.Add(OnPlayerDisconnected);

        SaveTimer.Elapsed += SaveSSC;
    }

    private static void SaveSSC(object? sender, System.Timers.ElapsedEventArgs e)
    {
        foreach (RubyPlayer player in PlayerTracker.Players)
            if (player != null && player.Active)
                player.Character?.TrySave();
    }

    private static void OnPlayerGreet(int index)
    {
        RubyPlayer player = PlayerTracker.Players[index];

        PlayerCharacter? character = CharactersNode.Collection.Find(player.Name);
        if (character == null)
        {
            player.Kick(":[");
            return;
        }

        // guarantee
        Task.Run(() => 
        {
            player.CharacterReseted = DateTime.UtcNow;
            player.Character?.ProcessGreet();

            player.Character = new ServersideCharacter(player, character);
            player.Character.ProcessGreet();

            Thread.Sleep(1000);
            player.Character.SetLife(character.MaxLife, character.MaxLife, false);
            player.Character.SetMana(character.MaxMana, character.MaxMana, false);
        });
    }

    private static void OnPlayerSlot(RubyPlayer? target, PlayerSlot packet, ref bool handled)
    {
        if (target == null)
            return;

        if (target.Character?.ReceiveSlot(packet.SlotIndex, 
            new NetItem(packet.ItemType, packet.ItemStack, packet.ItemPrefix)) == true)
        {
            handled = true;
        }
    }

    private static void OnPlayerMana(RubyPlayer? target, PlayerMana packet, ref bool handled)
    {
        if (target == null || handled)
            return;

        if (target.Character?.ReceiveMana(packet.Mana, packet.ManaMax) == true)
        {
            handled = true;
        }

        if (target.TPlayer.statManaMax == packet.ManaMax) return;

        ModernConsole.WriteLine($"$!d[$!r$bSSC$$!r$!d]: $!r$!g{target.Name}$!r $!d[$!r$b{target.Character?.GetType().Name ?? "NULL"}$!d]$!r -> life change request from {target.TPlayer.statManaMax} to {packet.ManaMax}; handled=$!b{handled}$!r.");
    }

    private static void OnPlayerLife(RubyPlayer? target, PlayerLife packet, ref bool handled)
    {
        if (target == null || handled)
            return;

        if (target.Character?.ReceiveLife(packet.Life, packet.LifeMax) == true)
        {
            handled = true;
        }

        if (target.TPlayer.statLifeMax == packet.LifeMax) return;

        ModernConsole.WriteLine($"$!d[$!r$bSSC$$!r$!d]: $!r$!g{target.Name}$!r $!d[$!r$b{target.Character?.GetType().Name ?? "NULL"}$!d]$!r -> life change request from {target.TPlayer.statLifeMax} to {packet.LifeMax}; handled=$!b{handled}$!r.");
    }

    private static void OnPlayerInfo(RubyPlayer? target, PlayerInfo packet, ref bool handled)
    {
        if (target == null)
            return;

        if (target.Character == null)
        {
            ModernConsole.WriteLine($"$!d[$!r$bSSC$$!r$!d]: $!r$!g{packet.Name}$!r -> $ycharacter is not initialized. Initializing now...");
            PlayerCharacter? character = Collection.Find(packet.Name);
            if (character == null)
            {
                ModernConsole.WriteLine($"$!d[$!r$bSSC$$!r$!d]: $!r$!g{packet.Name}$!r -> $rcreating new character...");
                
                bool[] accsHide = new bool[10];
                for (var i = 0; i < accsHide.Length; i++)
                    accsHide[i] = (packet.AccessoryVisiblity & (1 << i)) != 0;

                int maxlife = Config.PrehardmodeLife;
                int maxmana = Config.PrehardmodeMana;
                NetItem[] items = Config.PrehardmodeItems;

                character = new PlayerCharacter(packet.Name)
                {
                    MaxLife = maxlife,
                    MaxMana = maxmana,
                    
                    Visuals = new CharacterVisuals()
                    {
                        EyesColor = packet.EyeColor,
                        HairColor = packet.HairColor,
                        SkinColor = packet.SkinColor,
                        PantsColor = packet.PantsColor,
                        ShirtColor = packet.ShirtColor,
                        ShoesColor = packet.ShoesColor,
                        UndershirtColor = packet.UnderShirtColor,
                        Hair = packet.Hair,
                        HairDye = packet.HairDye,
                        HideAccessories = accsHide,
                        HideMisc = packet.HideMisc,
                        SkinVariant = packet.SkinVariant
                    }
                };

                Array.Copy(items, character.Slots, items.Length);

                character.Save();
            }

            ModernConsole.WriteLine($"$!d[$!r$bSSC$$!r$!d]: $!r$!g{packet.Name}$!r -> $gcharacter successfully initialized.");
            target.Character = new ReadonlyCharacter(target, character);
        }

        if (target.Character?.ReceiveInfo(packet) == true)
        {
            handled = true;
        }
    }
    private static void OnPlayerDisconnected(int index)
    {
        PlayerTracker.Players[index].Character?.TrySave();
        PlayerTracker.Players[index].Character = null;
    }

    public static Item TerrarifySlot(RubyPlayer player, NetItem netItem, int slot)
    {
        var item = new Item();
        item.SetDefaults(netItem.ItemID);
        item.stack = netItem.ItemStack;
        item.prefix = netItem.ItemPrefix;

        if (slot >= PlayerItemSlotID.Loadout3_Dye_0)
            player.TPlayer.Loadouts[2].Dye[slot - PlayerItemSlotID.Loadout3_Dye_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout3_Armor_0)
            player.TPlayer.Loadouts[2].Armor[slot - PlayerItemSlotID.Loadout3_Armor_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout2_Dye_0)
            player.TPlayer.Loadouts[1].Dye[slot - PlayerItemSlotID.Loadout2_Dye_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout2_Armor_0)
            player.TPlayer.Loadouts[1].Armor[slot - PlayerItemSlotID.Loadout2_Armor_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout1_Dye_0)
            player.TPlayer.Loadouts[0].Dye[slot - PlayerItemSlotID.Loadout1_Dye_0] = item;
        else if (slot >= PlayerItemSlotID.Loadout1_Armor_0)
            player.TPlayer.Loadouts[0].Armor[slot - PlayerItemSlotID.Loadout1_Armor_0] = item;
        else if (slot >= PlayerItemSlotID.Bank4_0)
            player.TPlayer.bank4.item[slot - PlayerItemSlotID.Bank4_0] = item;
        else if (slot >= PlayerItemSlotID.Bank3_0)
            player.TPlayer.bank3.item[slot - PlayerItemSlotID.Bank3_0] = item;
        else if (slot >= PlayerItemSlotID.TrashItem)
            player.TPlayer.trashItem = item;
        else if (slot >= PlayerItemSlotID.Bank2_0)
            player.TPlayer.bank2.item[slot - PlayerItemSlotID.Bank2_0] = item;
        else if (slot >= PlayerItemSlotID.Bank1_0)
            player.TPlayer.bank.item[slot - PlayerItemSlotID.Bank1_0] = item;
        else if (slot >= PlayerItemSlotID.MiscDye0)
            player.TPlayer.miscDyes[slot - PlayerItemSlotID.MiscDye0] = item;
        else if (slot >= PlayerItemSlotID.Misc0)
            player.TPlayer.miscEquips[slot - PlayerItemSlotID.Misc0] = item;
        else if (slot >= PlayerItemSlotID.Dye0)
            player.TPlayer.dye[slot - PlayerItemSlotID.Dye0] = item;
        else if (slot >= PlayerItemSlotID.Armor0)
            player.TPlayer.armor[slot - PlayerItemSlotID.Armor0] = item;
        else
            player.TPlayer.inventory[slot - PlayerItemSlotID.Inventory0] = item;

        return item;
    }
}