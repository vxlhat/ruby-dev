using System.Formats.Tar;
using System.Reflection;
using Ruby.Network.Primitive;
using Ruby.Server;
using Ruby.Server.Players;
using Ruby.Server.Players.Jail;
using Ruby.Server.Players.Punishments;
using Terraria;
using Terraria.Localization;

namespace Ruby.Network;

public static class NetworkRegulator
{   
    #region Initialize
    internal static void Initialize()
    {
        InitializeHooks();
        InitializeComfortableHooks();
    }

    internal static void InitializeHooks()
    {
        On.Terraria.NetMessage.SendData += OnSendData;
        On.Terraria.MessageBuffer.GetData += OnGetData;
    }

    internal static void InitializeComfortableHooks()
    {
        int counter = 0;

        foreach (Type packetType in typeof(NetworkRegulator).Assembly.GetTypes())
        {
            if (packetType.Namespace != "Ruby.Network.Comfortable.Packets")
                continue;

            Type? interfaceType = packetType.GetInterface("IPacket`1");
            if (interfaceType == null) continue;

            Type genericArg = interfaceType.GetGenericArguments()[0];
            Type hookType = typeof(ComfortableHook<>).MakeGenericType(genericArg);
            hookType.GetMethod("Setup", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, new object?[]
            {
                Activator.CreateInstance(packetType)
            });

            counter++;
        }

        ModernConsole.WriteLine($"$!bNetworkRegulator: $!rRegistered {counter} comfortable packets.");
    }
    #endregion

    private static void OnGetData(On.Terraria.MessageBuffer.orig_GetData orig, Terraria.MessageBuffer self, int start, int length, out int messageType)
    {
        messageType = NetMessage.buffer[self.whoAmI].readBuffer[start];

        if (messageType == (byte)PacketTypes.SyncCavernMonsterType)
            return;

        if (length > 999) return;

        RubyPlayer target = PlayerTracker.Players[self.whoAmI];

        bool handled = false;

        try
        {
            IncomingPacket packet = new IncomingPacket((byte)messageType, (byte)self.whoAmI, start + 1, length - 1);
            
            var directBinds = DirectHook<IncomingPacket>.Binds;
            if (directBinds != null)
            {
                foreach (var bindDelegate in directBinds)
                {
                    try
                    {
                        bindDelegate(target, packet, ref handled);
                    }
                    catch (Exception ex)
                    {
                        ModernConsole.WriteLine($"$!d[$!r$rDirectHook$!r$!d<$!r$cIncomingPacket$!r$!d>$!r$!d]: $!r$rError in handling $a{messageType}$!r: {ex.ToString()}");
                    }
                }
            }

            #region Module handling
            if (messageType == 82) // netmodule
            {
                ushort moduleId = packet.GetReader().ReadUInt16();

                IncomingModule module = new IncomingModule(moduleId, (byte)self.whoAmI, start + 3, length - 3);

                var moduleBinds = PrimitiveHook<IncomingModule>.Binds[moduleId];
                if (moduleBinds != null)
                {
                    foreach (var bindDelegate in moduleBinds)
                    {
                        try
                        {
                            bindDelegate(target, module, ref handled);
                        }
                        catch (Exception ex)
                        {
                            ModernConsole.WriteLine($"$!d[$!r$rPrimitiveHook$!r$!d<$!r$cIncomingModule$!r$!d>$!r$!d]: $!r$rError in handling $c{moduleId}$!r: {ex.ToString()}");
                        }
                    }
                }

                if (handled) return;

                var moduleHijack = PrimitiveHook<IncomingModule>.Hijacks[moduleId];
                if (moduleHijack != null)
                    moduleHijack(target, module, ref handled);
                else 
                    orig(self, start, length, out messageType);
                return;
            }
            #endregion

            #region Packet handling
            var binds = PrimitiveHook<IncomingPacket>.Binds[messageType];
            if (binds != null)
            {
                foreach (var bindDelegate in binds)
                {
                    try
                    {
                        bindDelegate(target, packet, ref handled);
                    }
                    catch (Exception ex)
                    {
                        ModernConsole.WriteLine($"$!d[$!r$rPrimitiveHook$!r$!d<$!r$cIncomingPacket$!r$!d>$!r$!d]: $!r$rError in handling $a{messageType}$!r: {ex.ToString()}");
                    }
                }
            }

            if (handled) return;

            var hijack = PrimitiveHook<IncomingPacket>.Hijacks[messageType];
            if (hijack != null)
                hijack(target, packet, ref handled);
            else 
                orig(self, start, length, out messageType);
            #endregion
        }
        catch (Exception ex)
        {
            ModernConsole.WriteLine($"$!d[$!r$rPrimitiveHook$!r$!d<$!r$cIncomingPacket$!r$!d>$!r$!d]: $!r$rError in handling bind $c{messageType}$!r: {ex.ToString()}");

            return;
        }
    }

    private static void OnSendData(On.Terraria.NetMessage.orig_SendData orig, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
    {
        try
        {
            OutcomingPacket packet = new OutcomingPacket(msgType, remoteClient, ignoreClient, text, number, number2, number3, number4, number5, number6, number7);
            RubyPlayer? target = remoteClient == -1 ? null : PlayerTracker.Players[remoteClient];

            bool handled = false;

            var binds = PrimitiveHook<OutcomingPacket>.Binds[msgType];
            if (binds != null)
            {
                foreach (var bindDelegate in binds)
                {
                    try
                    {
                        bindDelegate(target, packet, ref handled);
                    }
                    catch (Exception ex)
                    {
                        ModernConsole.WriteLine($"$!d[$!r$rPrimitiveHook$!r$!d<$!r$cOutcomingPacket$!r$!d>$!r$!d]: $!r$rError in handling bind $a{msgType}$!r: {ex.ToString()}");
                    }
                }
            }
            
            if (handled) return;

            var hijack = PrimitiveHook<OutcomingPacket>.Hijacks[msgType];
            if (hijack != null)
                hijack(target, packet, ref handled);
            else
                orig(msgType, remoteClient, ignoreClient, text, number, number2, number3, number4, number5, number6, number7);
        }
        catch(Exception ex)
        {
            ModernConsole.WriteLine($"$!d[$!r$rPrimitiveHook$!r$!d<$!r$cOutcomingPacket$!r$!d>$!r$!d]: $!r$rError in handling $c{msgType}$!r: {ex.ToString()}");
        }

    }
}