using System.Reflection;
using Ruby.Server.Players;
using Ruby.Commands.Attributes;
using Ruby.Commands.Contexts;

namespace Ruby.Commands;

public static class CommandsManager
{
    internal static List<ICommand> Commands = new List<ICommand>();
    public static IReadOnlyList<ICommand> RegisteredCommands => Commands.AsReadOnly();

    internal static Dictionary<Type, Func<string, object>> Parsers = new();

    internal static void Initialize()
    {
        Parsers.Add(typeof(string), (arg) => arg);
        Parsers.Add(typeof(bool), (arg) =>
        {
            if (bool.TryParse(arg, out var result))
                return result;

            return new InvalidParameterValue(arg);
        });
        Parsers.Add(typeof(byte), (arg) =>
        {
            if (byte.TryParse(arg, out var result))
                return result;

            return new InvalidParameterValue(arg);
        });
        Parsers.Add(typeof(int), (arg) =>
        {
            if (int.TryParse(arg, out var result))
                return result;

            return new InvalidParameterValue(arg);
        });
        Parsers.Add(typeof(float), (arg) =>
        {
            if (float.TryParse(arg, out var result))
                return result;

            return new InvalidParameterValue(arg);
        });
        Parsers.Add(typeof(short), (arg) =>
        {
            if (short.TryParse(arg, out var result))
                return result;

            return new InvalidParameterValue(arg);
        });
        Parsers.Add(typeof(long), (arg) =>
        {
            if (long.TryParse(arg, out var result))
                return result;

            return new InvalidParameterValue(arg);
        });
        Parsers.Add(typeof(ulong), (arg) =>
        {
            if (ulong.TryParse(arg, out var result))
                return result;

            return new InvalidParameterValue(arg);
        });
        Parsers.Add(typeof(RubyPlayer), (arg) =>
        {
            var plr = PlayerTracker.Players.FirstOrDefault((p) => p != null && p.Active && p.Name == arg);
            if (plr != null)
                return plr;

            plr = PlayerTracker.Players.FirstOrDefault((p) => p != null && p.Active && p.Name?.ToLower() == arg.ToLower());
            if (plr != null)
                return plr;

            plr = PlayerTracker.Players.FirstOrDefault((p) => p != null && p.Active && p.Name?.ToLower().StartsWith(arg) == true);
            if (plr != null)
                return plr;

            return new InvalidParameterValue(arg);
        });
        InsertFrom(typeof(CommandsManager).Assembly, false);
    }

    public static ICommand? FindCommand(string name)
    {
        if (name.StartsWith("/"))
            name = name.Substring(1);

        List<ICommand> cmds = Commands;

        foreach (ICommand cmd in cmds)
            if (cmd.NameEquals(name))
                return cmd;

        return null;
    }

    public static void RunCommand(RubyPlayer? player, ICommandSender sender, string text)
    {
        ICommand? cmd = FindCommand(text);

        if (cmd == null)
        {
            sender.SendErrorMessage("Команда не найдена. Введите /help для просмотра всех доступных вам команд.");
            return;
        }

        var args = SplitArguments(text).Skip(cmd.Data.Name.Split(' ').Length).ToList();

        if (cmd.Data.Flags.HasFlag(CommandFlags.NoLogging) == false)
            ModernConsole.WriteLine($"$bICommandSender $!r$!b{sender.Name} $!r$!d($!r$!b{(player == null ? "$bnon in-game" : "$rRubyPlayer")}$!r$!d) $!r$!d-> $!r$b{cmd.Data.Name} $!r$c{string.Join(" ", args.Select(p => $"'{p}'"))}");

        if (cmd.Data.Flags.HasFlag(CommandFlags.IngameOnly) && player == null)
        {
            sender.SendErrorMessage("Команду можно выполнить только в игре.");
            return;
        }

        if (cmd.Data.RequiredPermission != null && sender.HasPermission(cmd.Data.RequiredPermission) == false)
        {
            sender.SendErrorMessage("У вас недостаточно прав для выполнения команды.");
            return;
        }


        try
        {
            CommandInvokeContext ctx = new CommandInvokeContext(sender, player, args);
            cmd.Invoke(ctx);
        }
        catch (Exception ex)
        {
            sender.SendErrorMessage("Произошла ошибка при выполнении этой команды.");
            sender.SendErrorMessage("Пожалуйста, обратитесь к техническому персоналу сервера для дальнейшего решения этой проблемы.");

            ModernConsole.WriteLine("$!b$rError in handling command:");
            ModernConsole.WriteLine($"$!b$r{ex}$!r");
        }
    }


    public static ParseResult TryParse(Type type, string input, out object value)
    {
        if (!Parsers.ContainsKey(type))
        {
            ModernConsole.WriteLine($"$!dFailed to parse type $!r$r$!b{type.FullName}$!r");
            value = new InvalidParameterValue(input);
            return ParseResult.ParserNotFound;
        }

        value = Parsers[type](input);
        if (value is InvalidParameterValue)
            return ParseResult.InvalidArgument;

        return ParseResult.Success;
    }

    internal static void InsertFrom(Assembly assembly, bool isPlugin)
    {
        foreach (Type type in assembly.GetExportedTypes())
            foreach (MethodInfo m in type.GetMethods())
            {
                CommandAttribute? cmdAttr = m.GetCustomAttribute<CommandAttribute>();
                if (cmdAttr == null) continue;

                ParameterInfo[] parameters = m.GetParameters();
                if (parameters.Length < 1 || parameters[0].ParameterType != typeof(CommandInvokeContext))
                {
                    Console.WriteLine($"Cannot load command {m.Name} from {type.FullName} -> invalid method arguments.");
                    return;
                }

                string[]? syntax = m.GetCustomAttribute<CommandSyntaxAttribute>()?.Syntax ?? null;
                CommandFlags flags = m.GetCustomAttribute<CommandFlagsAttribute>()?.Flags ?? CommandFlags.None;
                string? requiredPermission = m.GetCustomAttribute<CommandPermissionAttribute>()?.RequiredPermissions ?? null;

                CommandData cmdData = new CommandData(cmdAttr.Name, cmdAttr.Description, syntax, flags, requiredPermission);
                ICommand command = new StaticCommand(isPlugin, cmdData, m);

                Console.WriteLine($"Registered command '{command.Data.Name}' from {type.FullName}");
                Commands.Add(command);
            }

        Commands = new(Commands.OrderByDescending(p => p.Data.Name.Length));
    }

    internal static void UnloadPluginCommands()
    {
        Commands.RemoveAll((p) => p.PluginCommand);
        Commands = new(Commands.OrderByDescending(p => p.Data.Name.Length));
    }

    public static List<string> SplitArguments(string text)
    {
        List<string> args = new List<string>();
        args.Add("");
        int index = 0;

        bool blockSpace = false;
        bool ignoreFormat = false;
        foreach (char c in text)
        {
            if (c == '"' && !ignoreFormat)
            {
                blockSpace = !blockSpace;
                ignoreFormat = false;
            }
            else if (c == ' ' && !ignoreFormat && !blockSpace)
            {
                args.Add("");
                index++;
                ignoreFormat = false;
            }
            else if (c == '\\' && !ignoreFormat) ignoreFormat = true;
            else
            {
                args[index] += c;
            }
        }

        return args;
    }
}