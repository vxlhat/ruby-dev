namespace Ruby.Extensions;

public interface IExtension
{
    public string Name { get; }

    public void Load(LoadContext ctx);
    public void Unload();
}