namespace Ruby.Extensions;

public abstract class RubyModule : IExtension
{
    public abstract string Name { get; }

    public abstract void Load(LoadContext ctx);

    public void Unload(){}
}