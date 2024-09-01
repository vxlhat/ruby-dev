using System.Reflection;
using Ruby.Server.Players;
using Ruby.Commands.Contexts;

namespace Ruby.Commands;

public sealed class StaticCommand : ICommand
{
    internal StaticCommand(bool pluginCommand, CommandData data, MethodInfo method)
    {
        Method = method;
        PluginCommand = pluginCommand;
        Data = data;
    }

    public MethodInfo Method { get; }

    public bool PluginCommand { get; }

    public CommandData Data { get; }

    public void Invoke(CommandInvokeContext ctx)
    {
        ParameterInfo[] methodParams = Method.GetParameters();
        object?[] invokeArgs = new object?[methodParams.Length];
        invokeArgs[0] = ctx;

        if (!CreateArguments(ctx.Sender, ref invokeArgs, ctx.Arguments.ToList(), methodParams))
            return;

        Method.Invoke(null, invokeArgs);
    }

    public bool CreateArguments(ICommandSender sender, ref object?[] param, List<string> arguments, ParameterInfo[] methodParams)
    {
        int enteredParams = arguments.Count;

        for (int i = 1; i < methodParams.Length; i++)
        {
            int fixedParamIndex = i;
            int fixedRawArgIndex = i - 1;

            ParameterInfo p = methodParams[i];

            if (i > enteredParams)
            {
                if (p.HasDefaultValue)
                {
                    param[fixedParamIndex] = p.DefaultValue;
                    continue;
                }

                sender.SendErrorMessage("Недостаточно аргументов для выполнения этой команды!");

                if (Data.Syntax != null)
                    sender.SendInfoMessage($"Синтаксис команды: /{Data.Name} {string.Join(" ", Data.Syntax)}");
                return false;
            }

            string? paramSyntax = (Data.Syntax != null && Data.Syntax.Length <= i) ? Data.Syntax[i - 1] : null;

            ParseResult result = CommandsManager.TryParse(p.ParameterType, arguments[fixedRawArgIndex], out object value);
            switch (result)
            {
                case ParseResult.ParserNotFound:
                    sender.SendErrorMessage($"Не удалось преобразовать аргумент '{arguments[fixedRawArgIndex]}' в тип '{p.ParameterType.Name}'.");
                    sender.SendErrorMessage("Обратитесь к техническому персоналу сервера для дальнешего решения проблемы.");
                    
                    return false;
                case ParseResult.InvalidArgument:
                    sender.SendErrorMessage($"Произошла ошибка в аргументе '{paramSyntax}', индекс аргумента: {i}");
                    if (p.ParameterType == typeof(RubyPlayer))
                        sender.SendErrorMessage("Игрок не найден или он не находится на сервере.");

                    else
                        sender.SendErrorMessage($"Не удалось преобразовать аргумент '{arguments[fixedRawArgIndex]}' в тип '{p.ParameterType.Name}'.");

                    return false;

                case ParseResult.Success:
                    param[fixedParamIndex] = value;
                    break;
            }
        }

        return true;
    }
}