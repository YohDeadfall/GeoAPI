using System;
using System.Reflection;
using System.Threading;

namespace GeoAPI.Benchmarks
{
    public static class UseLazyInitializer
    {
        private static IGeometryServices s_instance;
        private static object s_lock = new object();
        private static bool s_initialized;

        public static IGeometryServices Instance
        {
            get => Volatile.Read(ref s_instance) ?? LazyInitializer.EnsureInitialized(ref s_instance, ref s_initialized, ref s_lock, ReflectInstance);
            set => Volatile.Write(ref s_instance, value);
        }

        public static void Reset()
        {
            s_instance = null;
            s_initialized = false;
        }

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
