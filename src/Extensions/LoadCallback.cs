using System.Diagnostics;
using System.Reflection;

namespace Ruby.Extensions;

public delegate void LoadCallback<T>(Assembly assembly, T reloadable, Stopwatch timing) where T : IExtension;