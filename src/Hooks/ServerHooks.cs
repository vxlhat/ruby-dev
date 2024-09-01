#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

using Terraria;
using Ruby.Hooks;
using System.Net;
using Terraria.Utilities;
using Terraria.Localization;
using Ruby.Network;
using Terraria.GameContent.Events;
using On.Terraria.Chat;
using Microsoft.Xna.Framework;
using Ruby.Server;
using Terraria.GameContent.Skies;
using Terraria.GameInput;
using System.Collections;
using Terraria.Graphics.Capture;
using Terraria.GameContent;
using Terraria.Net.Sockets;
using Terraria.DataStructures;
using Ruby.Server.Players;

namespace Ruby;

public static class ServerHooks
{

#region Server init

    public static ServerInitializeHook ServerInitialize { get; } = new ServerInitializeHook();

    public delegate void OnServerInitialize();
    public class ServerInitializeHook : RuntimeHook<OnServerInitialize>
    {
        public override string Name => "ServerInitialize";
        
        internal ServerInitializeHook()
        {
            On.Terraria.Netplay.InitializeServer += PatchedHook;
        }

        private void PatchedHook(On.Terraria.Netplay.orig_InitializeServer orig)
        {
            Netplay.Connection.ResetSpecialFlags();
            Netplay.ResetNetDiag();
            if (Main.rand == null)
            {
                Main.rand = new UnifiedRandom((int)DateTime.Now.Ticks);
            }
            Main.myPlayer = 255;
            Netplay.ServerIP = IPAddress.Any;
            Main.menuMode = 14;
            Main.statusText = Lang.menu[8].Value;
            Main.netMode = 2;
            Netplay.Disconnect = false;
            for (int i = 0; i < 256; i++)
            {
                Netplay.Clients[i] = new RemoteClient();
                Netplay.Clients[i].Reset();
                Netplay.Clients[i].Id = i;
                Netplay.Clients[i].ReadBuffer = new byte[1024];
            }
            Netplay.TcpListener = new RemadeTcpSocket();
            if (!Netplay.Disconnect)
            {
                if (!Netplay.StartListening())
                {
                    Main.statusText = Language.GetTextValue("Error.TriedToRunServerTwice");
                    Netplay.SaveOnServerExit = false;
                    Netplay.Disconnect = true;
                }
                Main.statusText = Language.GetTextValue("CLI.ServerStarted");
            }

            Main.maxNetPlayers = ServerConfiguration.Current.MaxPlayers;

            _hooks.ForEach(p => p());
        }
    }

#endregion

#region Player connected

    public static PlayerConnectedHook PlayerConnected { get; } = new PlayerConnectedHook();

    public delegate void OnPlayerConnected(int index);
    public class PlayerConnectedHook : RuntimeHook<OnPlayerConnected>
    {
        public override string Name => "PlayerConnected";
        
        internal PlayerConnectedHook()
        {
            On.Terraria.Netplay.OnConnectionAccepted += PatchedHook;
        }

        private void PatchedHook(On.Terraria.Netplay.orig_OnConnectionAccepted orig, ISocket client)
        {
            var index = Netplay.FindNextOpenClientSlot();
            if (index != -1)
            {
                Netplay.Clients[index].Reset();
                Netplay.Clients[index].Socket = client;
                _hooks.ForEach(p => p(index));
            }
            else
            {
                lock (Netplay.fullBuffer)
                {
                    Netplay.KickClient(client, NetworkText.FromKey("CLI.ServerIsFull"));
                }
            }

            if (Netplay.FindNextOpenClientSlot() == -1)
            {
                Netplay.StopListening();
                Netplay.IsListening = false;
            }
        }
    }

#endregion

#region Player disconnected

    public static PlayerDisconnectedHook PlayerDisconnected { get; } = new PlayerDisconnectedHook();

    public delegate void OnPlayerDisconnected(int index);
    public class PlayerDisconnectedHook : RuntimeHook<OnPlayerDisconnected>
    {
        public override string Name => "PlayerDisconnected";
        
        internal PlayerDisconnectedHook()
        {
            On.Terraria.NetMessage.SyncDisconnectedPlayer += PatchedHook;
        }

        private void PatchedHook(On.Terraria.NetMessage.orig_SyncDisconnectedPlayer orig, int plr)
        {
            NetMessage.SyncOnePlayer(plr, -1, plr);
            NetMessage.EnsureLocalPlayerIsPresent();

            _hooks.ForEach(p => p(plr));
        }
    }

#endregion

#region Player greet

    public static PlayerGreetHook PlayerGreet { get; } = new PlayerGreetHook();

    public delegate void OnPlayerGreet(int index);
    public class PlayerGreetHook : RuntimeHook<OnPlayerGreet>
    {
        public override string Name => "PlayerGreet";
        
        internal PlayerGreetHook()
        {
            On.Terraria.NetMessage.greetPlayer += PatchedHook;
        }

        private void PatchedHook(On.Terraria.NetMessage.orig_greetPlayer orig, int plr)
        {
            PlayerTracker.Players[plr].WasGreeted = true;
            _hooks.ForEach(p => p(plr));
        }
    }

#endregion

#region Chat broadcast

    public static ChatBroadcastHook ChatBroadcast { get; } = new ChatBroadcastHook();

    public delegate void OnChatBroadcast(ref bool ignore, byte messageAuthor, NetworkText text, Color color, int excludedPlayer);
    public class ChatBroadcastHook : RuntimeHook<OnChatBroadcast>
    {
        public override string Name => "ChatBroadcast";
        
        internal ChatBroadcastHook()
        {
            On.Terraria.Chat.ChatHelper.BroadcastChatMessageAs += PatchedHook;
        }

        private void PatchedHook(ChatHelper.orig_BroadcastChatMessageAs orig, byte messageAuthor, NetworkText text, Color color, int excludedPlayer)
        {
            bool ignore = false;
            _hooks.ForEach(p => p(ref ignore, messageAuthor, text, color, excludedPlayer));

            if (ignore) return;

            orig(messageAuthor, text, color, excludedPlayer);
        }
    }

#endregion

#region Npc Spawn

    public static NpcSpawnHook NpcSpawn { get; } = new NpcSpawnHook();

    public delegate void OnNpcSpawn(ref bool ignore, IEntitySource source, int x, int y, int type, int start, float ai0, float ai1, float ai2, float ai3, int target);
    public class NpcSpawnHook : RuntimeHook<OnNpcSpawn>
    {
        public override string Name => "NpcSpawn";
        
        internal NpcSpawnHook()
        {
            On.Terraria.NPC.NewNPC += PatchedHook;
        }

        private int PatchedHook(On.Terraria.NPC.orig_NewNPC orig, IEntitySource source, int X, int Y, int Type, int Start, float ai0, float ai1, float ai2, float ai3, int Target)
        {
            bool ignore = false;
            _hooks.ForEach(p => p(ref ignore, source, X, Y, Type, Start, ai0, ai1, ai2, ai3, Target));

            return orig(source, X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
        }
    }

#endregion

}
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.