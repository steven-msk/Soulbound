using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Implementations {

    public static List<Type> GetAllImplementationsOf<T>() {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assemblies => assemblies.GetTypes())
            .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .ToList();
    }
}
