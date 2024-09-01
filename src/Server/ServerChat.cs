using Microsoft.Xna.Framework;
using Ruby.Commands;
using Ruby.Network;
using Ruby.Network.Primitive;
using Ruby.Server.Players;
using Terraria.GameContent.NetModules;
using Terraria.Localization;
using Terraria.Net;

namespace Ruby.Server;

public static class ServerChat
{
    internal static void Initialize()
    {
        PrimitiveHook<IncomingModule>.Add(1, OnChatModule);
    }

    public static void BroadcastText(string text, Color color)
    {
        var packet = NetTextModule.SerializeServerMessage(NetworkText.FromLiteral(text), color);
        NetManager.Instance.Broadcast(packet, -1);
    }

    private static void OnChatModule(RubyPlayer? target, IncomingModule packet, ref bool handled)
    {
        if (target == null) return;

        var reader = packet.GetReader();

        string chatCommand = reader.ReadString();
        string chatText = reader.ReadString();
        
        string message = chatCommand == "Say" ? chatText : $"/{chatCommand.ToLower()} {chatText}";

        if (message.StartsWith("/"))
        {
            CommandsManager.RunCommand(target, target, message);
            handled = true;
            return;
        }
    }
}