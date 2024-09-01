namespace Ruby.Server.Players.Characters;

public struct CharactersConfig
{
    public CharactersConfig()
    {
        PrehardmodeLife = 120;
        PrehardmodeMana = 40;
        PrehardmodeItems = new NetItem[]
        {
            new NetItem(Terraria.ID.ItemID.CopperShortsword, 1, 0),
            new NetItem(Terraria.ID.ItemID.CopperPickaxe, 1, 0),
            new NetItem(Terraria.ID.ItemID.CopperAxe, 1, 0),
            new NetItem(Terraria.ID.ItemID.CopperHammer, 1, 0),
            new NetItem(Terraria.ID.ItemID.Wood, 75, 0),
            new NetItem(Terraria.ID.ItemID.Torch, 25, 0),
            new NetItem(Terraria.ID.ItemID.Rope, 50, 0),
        };
    }

    public bool EnableSSC;
    public string? DatabaseName;

    public int PrehardmodeLife;
    public int PrehardmodeMana;
    public NetItem[] PrehardmodeItems;
}