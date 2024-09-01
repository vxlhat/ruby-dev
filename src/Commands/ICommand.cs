using Ruby.Commands.Contexts;

namespace Ruby.Commands;

public interface ICommand
{
    public bool PluginCommand { get; }
    public CommandData Data { get; }

    public void Invoke(CommandInvokeContext ctx);
}