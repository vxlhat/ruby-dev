using MonoMod.RuntimeDetour;

namespace Ruby.Hooks;

public abstract class RuntimeHook<TDelegate> where TDelegate : Delegate
{
    public abstract string Name { get; }
    
    protected List<TDelegate> _hooks = new List<TDelegate>();

    public bool Add(TDelegate hook)
    {
        if (_hooks.Contains(hook)) return false;

        _hooks.Add(hook);
        return true;
    }

    public bool Remove(TDelegate hook) => _hooks.Remove(hook);
}