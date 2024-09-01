namespace Ruby.Extensions;

public abstract class RubyPlugin : IExtension
{
    public abstract string Name { get; }

    public abstract void Load(LoadContext ctx);

    public abstract void Unload();
}