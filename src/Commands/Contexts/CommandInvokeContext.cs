using Ruby.Server.Players;

namespace Ruby.Commands.Contexts;

public class CommandInvokeContext
{
    public CommandInvokeContext(ICommandSender session, RubyPlayer? player, IReadOnlyList<string> args)
    {
        Sender = session;
        Arguments = args;
        Player = player;
    }

    public ICommandSender Sender { get; }
    public RubyPlayer? Player { get; }
    public IReadOnlyList<string> Arguments { get; }

}