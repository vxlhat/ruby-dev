using Ruby.Commands;

namespace Ruby.Extensions;

public static class PluginsNode
{
    internal static List<RubyPlugin> LoadedPlugins = new List<RubyPlugin>();
    public static IReadOnlyList<RubyPlugin> Plugins => LoadedPlugins.AsReadOnly();

    public static void LoadPlugins()
    {
        ModernConsole.WriteLine($"$!bLoading plugins:");

        var loader = new ExtensionLoader<RubyPlugin>("plugins", false);
        loader.Load((asm, p, sw) => 
        {
            LoadedPlugins.Add(p);
            CommandsManager.InsertFrom(asm, true);
        });
    }
    public static void UnloadPlugins()
    {
        CommandsManager.UnloadPluginCommands();

        foreach (RubyPlugin plugin in LoadedPlugins)
        {
            plugin.Unload();
        }

        LoadedPlugins.Clear();
    }
}