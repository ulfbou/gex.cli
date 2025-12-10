using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gex.Core;

public static class ModuleLoader
{
    public static IEnumerable<ICommandModule> DiscoverModules()
    {
        var modules = new List<ICommandModule>();
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in asm.GetTypes().Where(t => typeof(ICommandModule).IsAssignableFrom(t) && !t.IsAbstract))
            {
                if (Activator.CreateInstance(type) is ICommandModule module)
                    modules.Add(module);
            }
        }
        return modules;
    }
}
