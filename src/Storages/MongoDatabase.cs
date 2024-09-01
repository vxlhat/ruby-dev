using MongoDB.Driver;
using Ruby.Server;

namespace Ruby.Storages;

public sealed class MongoDatabase
{
    public static MongoDatabase Main { get; } = new MongoDatabase(ServerConfiguration.Current.MongoDbUri, ServerConfiguration.Current.MongoDbName);

    public MongoDatabase(string uri, string name)
    {
        Uri = uri;
        Name = name;
        
        Client = new MongoClient(uri);
        InternalDb = Client.GetDatabase(name);
    }

    public string Uri { get; }
    public string Name { get; }

    public MongoClient Client { get; }
    public IMongoDatabase InternalDb { get; }
    
    public MongoCollection<TModel> Get<TModel>() where TModel : DataModel
    {
        MongoCollection<TModel> storage = new(this);
        return storage;
    }

}