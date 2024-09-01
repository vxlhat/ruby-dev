namespace Ruby.Extensions;

public sealed class ReloadableContainer<T> where T : IExtension
{
    internal ReloadableContainer(string name, T reloadable, bool isLoaded)
    {
        Name = name;
        Reloadable = reloadable;
        IsLoaded = isLoaded;
    }
    
    public string Name { get; }
    public T Reloadable { get; }
    public bool IsLoaded { get; }
}