using System.Collections.Concurrent;

namespace Ruby.Storages;

internal static class CallLogging
{
    internal static BlockingCollection<string> DatabaseLogs = new BlockingCollection<string>();

    internal static void Initialize()
    {
        if (Directory.Exists("debug") == false)
            Directory.CreateDirectory("debug");

        Task.Run(() => {
            while (true)
            {
                try
                {
                    string text = "";
                    while (DatabaseLogs.Count > 0 && DatabaseLogs.Count < 50)
                        text += $"TIME [{DateTime.UtcNow.ToString("HH:mm:ss:fff")}] {DatabaseLogs.Take()}\n";
                    
                    File.AppendAllText("debug/database.log", text);
                }
                catch {}
            }
        });
    }

    internal static void DatabaseLog(string text)
    {
        if (ServerBootstrapper.DebugMode == false)
            return;

        DatabaseLogs.Add(text);
    }
}