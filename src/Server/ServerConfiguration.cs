using Ruby.Network;
using Ruby.Storages;

namespace Ruby.Server;

public class ServerConfiguration
{
    public static Configuration Current { get; private set; } = new Configuration();
    internal static Config<Configuration> ConfigManager { get; } = Config<Configuration>.New();

    internal static void Initialize()
    {
        bool shutdownServer = File.Exists("data/Configuration.json") == false;

        ConfigManager.Save(Current = ConfigManager.Load());

        if (shutdownServer)
        {
            ModernConsole.WriteLine("$!b$rYour server is not configured.");
            ModernConsole.WriteLine("$!b$yOpen 'data/Configuration.json' and edit as you want.");
            
            while (true) Console.ReadLine();
        }
    }

    public static void ReloadConfiguration()
    {
        Current = ConfigManager.Load();
    }

    public static void UpdateConfiguration(Configuration config)
    {
        ConfigManager.Save(Current = config);
    }

    public struct Configuration
    {
        public Configuration()
        {
            MaxPlayers = 255;
            Operators = new List<string>();

            MongoDbUri = "mongodb://localhost:27017";
            MongoDbName = "RubyDev";

            EnableServerName = true;
            ServerName = "Ruby Development Version";
        }

        public byte MaxPlayers;
        public List<string> Operators;
        public string MongoDbUri;
        public string MongoDbName;
        public bool EnableServerName;
        public string ServerName;
    }
}