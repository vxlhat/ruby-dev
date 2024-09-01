using System.Collections;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Xna.Framework;
using Ruby.Commands;
using Ruby.Network;
using Ruby.Network.Comfortable;
using Ruby.Network.Comfortable.Packets;
using Ruby.Permissions;
using Ruby.Server.Player.Punishments;
using Ruby.Server.Players.Characters;
using Ruby.Server.Players.Jail;
using Ruby.Server.Players.Punishments;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Net.Sockets;

namespace Ruby.Server.Players;

public partial class RubyPlayer : IPermissionable, IJailable, IPunishable, ICommandSender
{
    internal RubyPlayer(int index)
    {
        Index = index;
        Name = "";
        UUID = "";

        IP = Socket.GetRemoteAddress()?.ToString()?.Split(':')[0] ?? "0.0.0.0";

        _jails = new Dictionary<string, IJailFunction>();
    }

    public int Index { get; }
    public bool Active => TPlayer.active;
    public bool Connected => Socket.IsConnected();
    public bool WasGreeted { get; internal set; }

    public DateTime? CharacterReseted { get; internal set; }

    public ISocket Socket => Netplay.Clients[Index].Socket;
    public RemoteClient Client => Netplay.Clients[Index];
    public Terraria.Player TPlayer => Main.player[Index];

    public string Name { get; private set; }
    public string IP { get; set; }
    public string UUID { get; set; }

    public ICharacter? Character { get; set; }

#region Player methods

    public void SetOperator(bool value)
    {
        if (value == IsOperator) return;

        ServerConfiguration.Configuration configuration = ServerConfiguration.Current;

        if (value == true) configuration.Operators.Add(Name);
        else if (value == false) configuration.Operators.Remove(Name);

        ServerConfiguration.UpdateConfiguration(configuration);
    }   

    public void Respawn()
    {
        TPlayer.Spawn(PlayerSpawnContext.ReviveFromDeath);

        SendSdkPacket(new PlayerSpawn()
        {
            PlayerIndex = (byte)Index,
            SpawnX = -1,
            SpawnY = -1,
            DeathsPvE = (short)TPlayer.numberOfDeathsPVE,
            DeathsPvP = (short)TPlayer.numberOfDeathsPVP
        });
    }

    public void SetName(string name, bool broadcast)
    {
        Name = name;
        TPlayer.name = name;

        if (broadcast)
            NetMessage.SendData(4, -1, -1, NetworkText.Empty, Index);
    }

	public void Move(Vector2 destination)
	{
		NetMessage.SendTileSquare(Index, (int)(destination.X / 16f), (int)(destination.Y / 16f), 15, 0, 0);
		TPlayer.Teleport(destination, 0, 1);
		NetMessage.SendData(65, -1, -1, NetworkText.Empty, 0, (float)Index, destination.X, destination.Y, 1, 0, 0);
	}

	public void Move(int x, int y)
	{
		NetMessage.SendTileSquare(Index, x, y, 15, 0, 0);
		TPlayer.Teleport(new(x * 16, y * 16), 0, 1);
		NetMessage.SendData(65, -1, -1, NetworkText.Empty, 0, (float)Index, x * 16, y * 16, 1, 0, 0);
	}

    public void Kick(string reason)
    {
        NetMessage.BootPlayer(Index, new NetworkText(reason, NetworkText.Mode.Literal));
    }

    public void Kick(PlayerBan ban)
    {
        string kickReason = $"===== Вы забанены на этом сервере! =====\nПричина: {ban.Reason}\nБан пройдет через: {ban.GetExpirationText()}";
        Kick(kickReason);
    }

    public bool CheckBansByName()
    {
        IEnumerable<PlayerBan> bans = PunishmentNode<PlayerBan>.Collection
            .FindAll(p => p.Username == Name);
        
        return KickByBans(bans);
    }

    public bool CheckBansByIP()
    {
        IEnumerable<PlayerBan> bans = PunishmentNode<PlayerBan>.Collection
            .FindAll(p => p.IP == IP);
        
        return KickByBans(bans);
    }


    public bool CheckBansByUUID()
    {
        IEnumerable<PlayerBan> bans = PunishmentNode<PlayerBan>.Collection
            .FindAll(p => p.UUID == UUID);
        
        return KickByBans(bans);
    }

    public bool KickByBans(IEnumerable<PlayerBan> bans)
    {
        foreach (PlayerBan ban in bans)
        {
            if (ban.IsExpired == false)
                Kick(ban);
            return true;
        }

        return false;
    }

    public void GiveItem(int type, int stack, int prefix)
	{
		int num = Item.NewItem(new EntitySource_DebugCommand(), (int)TPlayer.position.X, (int)TPlayer.position.Y, TPlayer.width, TPlayer.height, type, stack, true, prefix, true, false);
		Main.item[num].playerIndexTheItemIsReservedFor = Index;
		SendPacket(21, null, num, 1f, 0f, 0f, 0);
		SendPacket(22, null, num, 0f, 0f, 0f, 0);
	}

    public void SendText(string text, Color color)
    {
        byte[] buffer = new PacketWriter().SetType(82)
                .PackUInt16(1)
                .PackByte(255)
                .PackByte(0)
                .PackString(text)
                .PackColor(color)
                .BuildPacket();

        SendRawPacket(buffer);
    }

    public void SendSdkPacket<TData>(TData data) where TData : struct
    {
        byte[] buffer = ComfortableHook<TData>.Packet.Serialize(data);
        SendRawPacket(buffer);
    }

    public void SendRawPacket(byte[] buffer)
    {
        Socket.AsyncSend(buffer, 0, buffer.Length, Client.ServerWriteCallBack);
    }

    public void SendPacket(int packetId, NetworkText? text = null, int number1 = 0, float number2 = 0, float number3 = 0, float number4 = 0, int number5 = 0, int number6 = 0, int number7 = 0)
    {
        NetMessage.SendData(packetId, Index, -1, text ?? NetworkText.Empty, number1, number2, number3, number4, number5, number6, number7);
    }
#endregion

#region IPermissionable
    public bool IsOperator => ServerConfiguration.Current.Operators.Contains(Name);

    public bool HasPermission(string permission)
    {
        if (IsJailed) return false;

        return PermissionsNode<RubyPlayer>.HasPermission(this, permission) == PermissionAccess.HasPermission;
    }

    public bool CanBuild(int x, int y, int? width = null, int? height = null)
    {
        if (IsJailed) return false;

        return PermissionsNode<RubyPlayer>.HasBuildPermission(this, x, y, width, height) == PermissionAccess.HasPermission;
    }
#endregion

#region IJailable

    private Dictionary<string, IJailFunction> _jails;

    public bool IsJailed
    {
        get
        {
            var jails = _jails;
            return jails.Any(p => p.Value.IsEnabled && p.Value.IsActive());
        }
    }
    public IEnumerable<IJailFunction> ActiveJails
    {
        get
        {
            var jails = _jails;
            return jails.Values.Where(p => p.IsEnabled && p.IsActive()).AsEnumerable();
        }
    }

    public void Jail(IJailFunction jail)
    {
        if (_jails.ContainsKey(jail.GivenBy))
            _jails[jail.GivenBy] = jail;
        else
            _jails.Add(jail.GivenBy, jail);
    }

    public void Unjail(IJailFunction jail)
    {
         _jails.Remove(jail.GivenBy);
    }
#endregion

#region IPunishable
    public PunishmentCollection<T> GetPunishments<T>() where T : PlayerPunishment
    {
        if (PunishmentNode<T>.Players[Index] == null)
            PunishmentNode<T>.Players[Index] = new(this);

        PunishmentCollection<T> collection = PunishmentNode<T>.Players[Index];
        return collection;
    }

    public bool AnyPunishment<T>() where T : PlayerPunishment
    {
        PunishmentCollection<T> collection = GetPunishments<T>();
        foreach (var punishment in collection)
            if (punishment.IsExpired == false)
                return true;

        return false;
    }
    #endregion

#region Command sender
    public void SendBasicMessage(string text) => SendText("[c/787878:Rubydev •] " + text, new(57, 179, 77));

    public void SendSuccessMessage(string text) => SendText("[c/787878:Rubydev •] " + text, new(84, 255, 90));

    public void SendInfoMessage(string text) => SendText("[c/787878:Rubydev •] " + text, new(255, 207, 84));

    public void SendErrorMessage(string text) => SendText("[c/787878:Rubydev •] " + text, new(255, 84, 84));

    public void SendWarningMessage(string text) => SendText("[c/787878:Rubydev •] " + text, new(255, 155, 84));

    public void SendList(List<string> items, int page, string headerFormat, string nextPageFormat) => TextUtils.SendList(this, items, page, headerFormat, nextPageFormat);

    public void SendPage(List<string> lines, int page, string headerFormat, string nextPageFormat)=> TextUtils.SendPage(this, headerFormat, lines, page, nextPageFormat);
#endregion

}