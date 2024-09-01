using System.Linq.Expressions;
using MongoDB.Driver;

namespace Ruby.Storages;

public sealed class MongoCollection<TModel> : IDataStorage<TModel> where TModel : DataModel
{
    internal MongoCollection(MongoDatabase db)
    {
        Database = db;
        InternalCollection = Database.InternalDb.GetCollection<TModel>(CollectionName);
    }

    public string CollectionName => $"{typeof(TModel).Name}Collection";
    public MongoDatabase Database { get; }
    public IMongoCollection<TModel> InternalCollection { get; }

    public TModel? Find(string name)
    {
        CallLogging.DatabaseLog($"MODEL [{typeof(TModel).Name}]: find '{name}'");

        return Find(m => m.Name == name);
    }

    public TModel? Find(Expression<Func<TModel, bool>> predicate)
    {   
        var found = InternalCollection.Find(predicate);
        if (found.Any() == false) return null;
        return found.First();
    }

    public IEnumerable<TModel> FindAll()
    {
        return FindAll(m => true);
    }

    public IEnumerable<TModel> FindAll(Expression<Func<TModel, bool>> predicate)
    {
        CallLogging.DatabaseLog($"MODEL [{typeof(TModel).Name}]: find all");

        var found = InternalCollection.Find(predicate);
        return found.ToEnumerable();
    }

    public bool Remove(string name)
    {
        CallLogging.DatabaseLog($"MODEL [{typeof(TModel).Name}]: remove '{name}'");

        return InternalCollection.DeleteOne(m => m.Name == name).DeletedCount > 0;
    }

    public bool Remove(Expression<Func<TModel, bool>> predicate)
    {
        CallLogging.DatabaseLog($"MODEL [{typeof(TModel).Name}]: remove many");

        return InternalCollection.DeleteMany(predicate).DeletedCount > 0;
    }

    public void Save(TModel model)
    {
        CallLogging.DatabaseLog($"MODEL [{typeof(TModel).Name}]: save {model.Name}");

        if (Find(model.Name) != null)
            InternalCollection.ReplaceOne(m => m.Name == model.Name, model);
        else
            InternalCollection.InsertOne(model);
    }
}