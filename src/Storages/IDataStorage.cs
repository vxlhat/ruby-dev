using System.Linq.Expressions;

namespace Ruby.Storages;

public interface IDataStorage<TModel> where TModel : DataModel
{
    public IEnumerable<TModel> FindAll();

    public IEnumerable<TModel> FindAll(Expression<Func<TModel, bool>> predicate);

    public TModel? Find(string name);

    public TModel? Find(Expression<Func<TModel, bool>>  predicate);

    public void Save(TModel model);

    public bool Remove(string name);

    public bool Remove(Expression<Func<TModel, bool>> predicate);
}