using System.Diagnostics;
using System.Reflection;

namespace Ruby.Extensions;

internal sealed class ExtensionLoader<T> where T : IExtension
{
    internal ExtensionLoader(string workingDirectory, bool notUnloadable)
    {
        _workingDirectory = workingDirectory;
        _notifier = new ProcessNotifier<T>();
        _notUnloadable = notUnloadable;
    }   

    private string _workingDirectory;
    private bool _notUnloadable;
    private ProcessNotifier<T> _notifier;
    private List<Action<LoadCallback<T>>> _loadActions = new List<Action<LoadCallback<T>>>();

    internal void Load(LoadCallback<T> callback)
    {
        _notifier.AddResultCatcher<ExceptionResult>(new ExceptionCatcher<T>(_notifier));
        _notifier.AddResultCatcher<WarnResult>(new WarnCatcher<T>(_notifier));
        _notifier.AddResultCatcher<SkipResult>(new SkipCatcher<T>(_notifier));

        if (Directory.Exists(_workingDirectory) == false)
            Directory.CreateDirectory(_workingDirectory);

        foreach (string file in Directory.EnumerateFiles(_workingDirectory, "*.dll"))
            LoadExtension(file, callback);

        foreach (var loadAction in _loadActions)
            loadAction(callback);

        _notifier.Announce();
    }

    internal void LoadExtension(string file, LoadCallback<T> callback)
    {
        if (_notifier == null)
            throw new InvalidOperationException("You should use Load(...) before LoadAssembly(...!");

        _notifier.Total++;
        Stopwatch sw = new Stopwatch();
        sw.Start();
        try
        {
            Assembly assembly = _notUnloadable ? Assembly.LoadFrom(file) : Assembly.Load(File.ReadAllBytes(file));

            _loadActions.Add((cb) => 
            {
                try
                {
                    T? instance = GetInstance(assembly);
                    if (instance == null)
                    {
                        ModernConsole.WriteLine($"$!d[$!r$gExtensionLoader$!r$!d]: $!r$rFailed to load $w$!i{file}$!r$r.");
                        sw.Stop();
                        return;
                    }

                    LoadContext ctx = new LoadContext(instance, _notifier, sw);

                    instance.Load(ctx);

                    callback(assembly, instance, sw);

                    ModernConsole.WriteLine($"$!d[$!r$gExtensionLoader$!r$!d]: $!rLoaded $g$!i{file}$!r in {sw.ElapsedMilliseconds}ms.");
                    sw.Stop();
                }
                catch (Exception ex)
                {
                    if (ex is SkipException) return;

                    sw.Stop();
                    _notifier.AddResult(new ExceptionResult(file, sw, ex));
                }
            });
        }
        catch (Exception ex)
        {
            if (ex is SkipException) return;

            sw.Stop();
            _notifier.AddResult(new ExceptionResult(file, sw, ex));
        }
    }

    private T? GetInstance(Assembly from)
    {
        var target = typeof(T);

        var pluginTypes = from.GetExportedTypes().Where((p) => p.IsSubclassOf(target)).ToList();
        foreach (var type in pluginTypes)
        {
            var createdInstance = (T?)Activator.CreateInstance(type);
            return createdInstance;
        }

        return (T?)(object?)null;
    }
}