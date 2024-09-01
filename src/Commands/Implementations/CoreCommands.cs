using Microsoft.Xna.Framework;
using Ruby.Commands.Attributes;
using Ruby.Commands.Contexts;
using Ruby.Extensions;
using Ruby.Network.Comfortable.Packets;
using Ruby.Server;
using Ruby.Server.Players;
using Ruby.Server.Players.Punishments;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;
using Terraria.IO;
using Terraria.Net;

namespace Ruby.Commands.Implementations;

public static class CoreCommands
{
    [Command("help", "просмотреть список всех доступных команд")]
    [CommandSyntax("[страница]")]
    public static void HelpPage(CommandInvokeContext ctx, int page = 0)
    {
        List<string> lines = new List<string>(CommandsManager.Commands.Count);

        foreach (var cmd in CommandsManager.Commands.OrderBy(p => p.Data.Name))
        {   
            if (cmd.Data.Flags.HasFlag(CommandFlags.IngameOnly) && ctx.Player == null)
                continue;

            if (cmd.Data.RequiredPermission != null && ctx.Sender.HasPermission(cmd.Data.RequiredPermission) == false) 
                continue;

            lines.Add($"/{cmd.Data.Name}{(cmd.Data.Syntax == null ? "" : " " + string.Join(" ", cmd.Data.Syntax))} - {cmd.Data.Description}");
        }

        ctx.Sender.SendPage(lines, page, "Команды (страница: {0} из {1})", "Следующая страница: /help {2}");
    }

    [Command("plugins", "просмотреть список загруженных плагинов")]
    [CommandSyntax("[страница]")]
    public static void PluginsPage(CommandInvokeContext ctx, int page = 0)
    {
        List<string> lines = PluginsNode.LoadedPlugins.Select(p => p.Name).ToList();
        ctx.Sender.SendList(lines, page, "Плагины (страница: {0} из {1})", "Следующая страница: /plugins {3}");
    }

    [Command("reload", "перезагрузить плагины")]
    [CommandPermission("op.reload")]
    public static void PluginsReload(CommandInvokeContext ctx)
    {
        PluginsNode.UnloadPlugins();
        PluginsNode.LoadPlugins();

        ctx.Sender.SendSuccessMessage("Плагины перезагружены.");
    }

    [Command("reload-cfg", "перезагрузить конфиг сервера")]
    [CommandPermission("op.reload")]
    public static void ConfigReload(CommandInvokeContext ctx)
    {
        ServerConfiguration.ReloadConfiguration();

        ctx.Sender.SendSuccessMessage("Плагины перезагружены.");
    }

    [Command("spawn", "телепортироваться на точку спавна")]
    [CommandFlags(CommandFlags.IngameOnly)]
    public static void Spawn(CommandInvokeContext ctx)
    {
        ctx.Player?.Move(Main.spawnTileX, Main.spawnTileY);
        ctx.Sender.SendSuccessMessage("Телепортация на спавн прошла успешно.");
    }

    [Command("who", "просмотреть игроков онлайн")]
    [CommandSyntax("[страница]")]
    public static void Who(CommandInvokeContext ctx, int page = 0)
    {
        List<string> lines = PlayerTracker.Players.Where(p => p != null && p.Active).Select(p => p.Name).ToList();
        ctx.Sender.SendList(lines, page, $"Игроки на сервере (страница: {{0}} из {{1}}, онлайн: {PlayerTracker.OnlinePlayers} из {PlayerTracker.MaxSlots})", "Следующая страница: /who {3}");
    }

    [Command("op", "выдать права оператора игроку")]
    [CommandSyntax("<ник>")]
    [CommandPermission("op.management")]
    public static void Op(CommandInvokeContext ctx, RubyPlayer player)
    {
        player.SetOperator(true);

        ServerChat.BroadcastText($"{player.Name} теперь является оператором.", Color.Red);
    }

    [Command("deop", "отобрать права оператора у игрока")]
    [CommandSyntax("<ник>")]
    [CommandPermission("op.management")]
    public static void Deop(CommandInvokeContext ctx, RubyPlayer player)
    {
        player.SetOperator(false);

        ServerChat.BroadcastText($"{player.Name} теперь не является оператором.", Color.Red);
    }

    [Command("time freeze", "заморозить время")]
    [CommandPermission("world.time")]
    public static void TimeFreeze(CommandInvokeContext ctx)
    {
        CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled =
            !CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled;
            
        NetPacket packet = NetCreativePowersModule.PreparePacket(CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().PowerId, 0);
        NetManager.Instance.Broadcast(packet);
        ServerChat.BroadcastText($"{ctx.Sender.Name} {(CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled ? "заморозил" : "разморозил")} время.", Color.DarkSlateGray);
    }

    [Command("time day", "установить дневное время суток")]
    [CommandPermission("world.time")]
    public static void TimeDay(CommandInvokeContext ctx)
    {
        Main.SkipToTime(27000, setIsDayTime: true);
        ServerChat.BroadcastText($"{ctx.Sender.Name} установил дневное время суток.", Color.DarkSlateGray);
    }

    [Command("time night", "установить ночное время суток")]
    [CommandPermission("world.time")]
    public static void TimeNight(CommandInvokeContext ctx)
    {
        Main.SkipToTime(16200, setIsDayTime: false);
        ServerChat.BroadcastText($"{ctx.Sender.Name} установил ночное время суток.", Color.DarkSlateGray);
    }

    [Command("kick", "кикнуть игрока")]
    [CommandPermission("admin.punishments.kick")]
    public static void KickPlayer(CommandInvokeContext ctx, RubyPlayer player, string reason = "Причина не указана")
    {
        player.Kick("===== Вас кикнули с сервера! =====\nПричина: " + reason);
        ServerChat.BroadcastText($"{ctx.Sender.Name} кикнул игрока {player.Name}, по причине: {reason}.", Color.Red);
    }

    [Command("tp", "телепортироваться к игроку")]
    [CommandSyntax("<игрок>")]
    [CommandFlags(CommandFlags.IngameOnly)]
    [CommandPermission("admin.teleport")]
    public static void Teleport(CommandInvokeContext ctx, RubyPlayer player)
    {
        if (ctx.Player == null) return;

        ctx.Player.Move(player.TPlayer.position);
        ctx.Sender.SendSuccessMessage("Телепортация прошла успешно.");
    }

    [Command("tphere", "телепортировать игрока к себе")]
    [CommandSyntax("<игрок>")]
    [CommandFlags(CommandFlags.IngameOnly)]
    [CommandPermission("admin.teleport")]
    public static void TeleportHere(CommandInvokeContext ctx, RubyPlayer player)
    {
        if (ctx.Player == null) return;
        
        player.Move(ctx.Player.TPlayer.position);
        ctx.Sender.SendSuccessMessage("Телепортация прошла успешно.");
    }

    [Command("save", "сохранить мир")]
    [CommandPermission("op.save")]
    public static void SaveWorld(CommandInvokeContext ctx)
    {
        ctx.Sender.SendInfoMessage("Сохранение мира...");
        WorldFile.InternalSaveWorld(false, false);
        ctx.Sender.SendSuccessMessage("Мир сохранен!");
    }

    [Command("falldown", "'уронить' сервер")]
    [CommandPermission("op.falldown")]
    public static void Falldown(CommandInvokeContext ctx)
    {
        ctx.Sender.SendInfoMessage("Сохранение мира...");
        WorldFile.InternalSaveWorld(false, false);
        ctx.Sender.SendSuccessMessage("Мир сохранен!");

        foreach (RubyPlayer player in PlayerTracker.Players)
            if (player != null)
                player.Kick("Сервер в фаллдауне.");

        Environment.Exit(0);
    }

    [Command("debug-mode", "режим отладки")]
    [CommandPermission("op.falldown")]
    public static void DebugMode(CommandInvokeContext ctx)
    {
        ServerBootstrapper.DebugMode = !ServerBootstrapper.DebugMode;
        ctx.Sender.SendInfoMessage($"Режим отладки: {(ServerBootstrapper.DebugMode ? "включен" : "отключен")}");
    }
}