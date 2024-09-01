using Microsoft.Xna.Framework;
using MongoDB.Bson.Serialization.Attributes;
using Ruby.Network.Comfortable.Models;
using Ruby.Storages;

namespace Ruby.Server.Players.Characters;

[BsonIgnoreExtraElements]
public sealed class PlayerCharacter : DataModel
{
    public PlayerCharacter(string name) : base(name)
    {
        Slots = new NetItem[350];
        MaxLife = 100;
        MaxMana = 200;
    }

    public NetItem[] Slots;
    public int MaxLife;
    public int MaxMana;
    public PlayerInfo1 Info1;
    public PlayerInfo2 Info2;
    public PlayerInfo3 Info3;
    public CharacterVisuals Visuals;

    public override void Save()
    {
        CharactersNode.Collection.Save(this);
    }

    public override void Remove()
    {
        CharactersNode.Collection.Remove(Name);
    }
}

public struct CharacterVisuals
{
    public byte SkinVariant;
    public byte Hair;
    public byte HairDye;
    public bool[] HideAccessories;
    public byte HideMisc;

    [BsonIgnore]
    public Color HairColor
    {
        get => new Color(DbHairColor.R, DbHairColor.G, DbHairColor.B);
        set => DbHairColor = new DbColor(value.R, value.G, value.B);
    }
    public DbColor DbHairColor;

    [BsonIgnore]
    public Color SkinColor
    {
        get => new Color(DbSkinColor.R, DbSkinColor.G, DbSkinColor.B);
        set => DbSkinColor = new DbColor(value.R, value.G, value.B);
    }
    public DbColor DbSkinColor;

    [BsonIgnore]
    public Color EyesColor
    {
        get => new Color(DbEyesColor.R, DbEyesColor.G, DbEyesColor.B);
        set => DbEyesColor = new DbColor(value.R, value.G, value.B);
    }
    public DbColor DbEyesColor;

    [BsonIgnore]
    public Color ShirtColor
    {
        get => new Color(DbShirtColor.R, DbShirtColor.G, DbShirtColor.B);
        set => DbShirtColor = new DbColor(value.R, value.G, value.B);
    }
    public DbColor DbShirtColor;

    [BsonIgnore]
    public Color UndershirtColor
    {
        get => new Color(DbUndershirtColor.R, DbUndershirtColor.G, DbUndershirtColor.B);
        set => DbUndershirtColor = new DbColor(value.R, value.G, value.B);
    }
    public DbColor DbUndershirtColor;

    [BsonIgnore]
    public Color PantsColor
    {
        get => new Color(DbPantsColor.R, DbPantsColor.G, DbPantsColor.B);
        set => DbPantsColor = new DbColor(value.R, value.G, value.B);
    }
    public DbColor DbPantsColor;

    [BsonIgnore]
    public Color ShoesColor
    {
        get => new Color(DbShoesColor.R, DbShoesColor.G, DbShoesColor.B);
        set => DbShoesColor = new DbColor(value.R, value.G, value.B);
    }
    public DbColor DbShoesColor;
}

public struct DbColor
{
    public DbColor(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }
    
    [BsonIgnore]
    public Color XNA => new Color(R, G, B);

    public byte R;
    public byte G;
    public byte B;
}