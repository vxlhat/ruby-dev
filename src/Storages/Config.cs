using Newtonsoft.Json;

namespace Ruby.Storages;

public sealed class Config<T> where T : struct
{
    internal static string GetPath()
    {
        var dataName = typeof(T).Name;
        var folder = "data";

        return $"{folder}/{dataName}.json";
    }

    /// <summary>
    /// Creates new Config<T> instance. Use it if you are need to load or save data.
    /// </summary>
    /// <returns>Config<T> reference</returns>
    public static Config<T> New()
    {
        var config = new Config<T>(GetPath());
        return config;
    }

    /// <summary>
    /// Gets data of config.
    /// </summary>
    /// <returns>T</returns>
    public static T GetData()
    {
        return New().Load();
    }

    internal Config(string path)
    {
        _path = path;
    }

    private readonly string _path;

    public T Load()
    {
        if (File.Exists(_path))
        {
            T? deserialized = JsonConvert.DeserializeObject<T>(File.ReadAllText(_path));
            if (deserialized != null) return deserialized.Value;
        }

        Save(new T());
        return new T();
    }

    public void Save(T value)
    {
        var serializedValue = JsonConvert.SerializeObject(value, Formatting.Indented);
        File.WriteAllText(_path, serializedValue);
    }
}