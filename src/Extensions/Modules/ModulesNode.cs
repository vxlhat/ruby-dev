using Ruby.Commands;

namespace Ruby.Extensions;

public static class ModulesNode
{
    internal static List<RubyModule> LoadedModules = new List<RubyModule>();
    internal static ExtensionLoader<RubyModule> Loader = new ExtensionLoader<RubyModule>("modules", true);

    public static IReadOnlyList<RubyModule> Modules => LoadedModules.AsReadOnly();

    public static void LoadModules()
    {
        ModernConsole.WriteLine($"$!bLoading modules:");

        Loader.Load((asm, p, sw) => 
        {
            LoadedModules.Add(p);
            CommandsManager.InsertFrom(asm, false);
        });
    }
}