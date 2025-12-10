using System;

namespace Gex.Core;

public static class BinderPrototype
{
    public static T Bind<T>(ParseResult result) where T : new()
    {
        var obj = new T();
        var type = typeof(T);
        foreach (var prop in type.GetProperties())
        {
            var name = prop.Name.ToLowerInvariant();
            var val = result.GetValue(name);
            if (val == null) continue;
            try
            {
                var converted = Convert.ChangeType(val, prop.PropertyType);
                prop.SetValue(obj, converted);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(Diagnostics.FormatError($"Failed to bind property '{prop.Name}': {ex.Message}", 0));
            }
        }
        return obj;
    }
}
