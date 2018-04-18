using System;
using System.Reflection;

namespace GeoAPI.Benchmarks
{
    public static class UseLocks
    {
        private static volatile IGeometryServices s_instance;
        private static readonly object s_lock = new object();

        public static IGeometryServices Instance
        {
            get
            {
                lock (s_lock)
                {
                    return s_instance ?? (s_instance = ReflectInstance());
                }
            }
            set
            {
                lock (s_lock)
                {
                    s_instance = value;
                }
            }
        }

        public static void Reset()
            => s_instance = null;

        private static IGeometryServices ReflectInstance()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GlobalAssemblyCache && assembly.CodeBase == Assembly.GetExecutingAssembly().CodeBase)
                    continue;

                var assemblyType = assembly.GetType().FullName;
                if (assemblyType == "System.Reflection.Emit.AssemblyBuilder" ||
                    assemblyType == "System.Reflection.Emit.InternalAssemblyBuilder")
                    continue;

                Type[] types;

                try
                {
                    types = assembly.GetExportedTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types;
                }
                catch (Exception)
                {
                    continue;
                }

                var requiredType = typeof(IGeometryServices);
                foreach (var type in types)
                {
                    if (type.IsNotPublic || type.IsInterface || type.IsAbstract || !requiredType.IsAssignableFrom(type))
                        continue;

                    foreach (var constructor in type.GetConstructors())
                        if (constructor.IsPublic && constructor.GetParameters().Length == 0)
                        {
                            return (IGeometryServices)Activator.CreateInstance(type);
                        }
                }
            }
            throw new InvalidOperationException("Cannot use GeometryServiceProvider without an assigned IGeometryServices class");
        }
    }
}
