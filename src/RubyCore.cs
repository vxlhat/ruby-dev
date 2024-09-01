using System.Reflection;
using System.Runtime.ExceptionServices;
using Ruby.Commands;
using Ruby.Extensions;
using Ruby.Network;
using Ruby.Permissions;
using Ruby.Server;
using Ruby.Server.Players;
using Ruby.Server.Players.Characters;
using Ruby.Storages;
using Terraria;
using Terraria.Initializers;
using Terraria.Localization;

namespace Ruby;

internal static class ServerBootstrapper
{
    internal static bool DebugMode;

    private static void Main(string[] args)
    {
        CallLogging.Initialize();

		AppDomain.CurrentDomain.AssemblyResolve += delegate(object? sender, ResolveEventArgs sargs)
		{
			string resourceName = new AssemblyName(sargs.Name).Name + ".dll";
            
            if (File.Exists($"deps/{resourceName}"))
                return Assembly.LoadFrom($"deps/{resourceName}");

            if (File.Exists($"deps/auto/{resourceName}"))
                return Assembly.LoadFrom($"deps/auto/{resourceName}");

			return null;
		};
        
        if (Directory.Exists("data") == false)
            Directory.CreateDirectory("data");

        InitializeServer(args);
        Lang.InitializeLegacyLocalization();
        
        ServerConfiguration.Initialize();

        NetworkRegulator.Initialize();

        PlayerTracker.Initialize();
        CharactersNode.Initialize();
        ServerChat.Initialize();
        PermissionsNode<RubyPlayer>.Register(new PlayerPermissionWorker());

        TileFix.Initialize();

        PacketHandlers.Initialize();

        CommandsManager.Initialize();

        ModulesNode.LoadModules();
        PluginsNode.LoadPlugins();

        StartServer();
    }

    internal static void InitializeServer(string[] args)
    {
        // 🌿🌿🌿🌿🌿🌿🌿🌿
        //     |
        //   \ | /    04:20
        //  __\|/__
        // 🌿🌿🌿🌿🌿🌿🌿🌿

        Thread.CurrentThread.Name = "Server Thread";
        //if (monoArgs) args = Utils.ConvertMonoArgsToDotNet(args);
        Program.LaunchParameters = Utils.ParseArguements(args);
        Program.SavePath = Path.Combine("data");
        ThreadPool.SetMinThreads(8, 8);
        Program.InitializeConsoleOutput();
        Program.SetupLogging();
        //Platform.Get<IWindowService>().SetQuickEditEnabled(false);

        Terraria.Main.dedServ = true;
        LanguageManager.Instance.SetLanguage(GameCulture.DefaultCulture);
    }

    internal static void StartServer()
    {
        using var main = new Main();
        LaunchInitializer.LoadParameters(main);
        main.DedServ();

        main.Run();
    }

}