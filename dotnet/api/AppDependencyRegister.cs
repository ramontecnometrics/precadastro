using System;
using System.Linq;
using System.Reflection;

namespace api
{
    internal static class AppDependencyRegister
    {
        public static void Register(Assembly assembly, bool resolveProperties, Action<Type, Type> handler)
        {
            foreach (var t in assembly.GetTypes()
                .Where(t => !t.IsAbstract))
            {
                handler(t, t);
            }
        }

        public static void Register(Type dependency, Type implementation, bool resolveProperties, Action<Type, Type> handler)
        {
            handler(dependency, implementation);
        }

        public static void Register(Assembly assembly, string nameSpace, bool resolveProperties, Action<Type, Type> handler)
        {
            foreach (var t in assembly.GetTypes()
                .Where(t => !t.IsAbstract)
                .Where(t => t.Namespace == nameSpace))
            {
                handler(t, t);
            }
        }
    }
}