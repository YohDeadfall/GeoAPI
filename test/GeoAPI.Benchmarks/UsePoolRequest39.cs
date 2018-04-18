using System;
using System.Reflection;
using System.Threading;

namespace GeoAPI.Benchmarks
{
    public static class UsePoolRequest39
    {
        internal static volatile IGeometryServices s_instance;
        private static readonly object s_lock = new object();

        public static IGeometryServices Instance
        {
            get => s_instance ?? InitializeInstance();
            set => s_instance = value;
        }

        public static void Reset()
            => s_instance = null;

        private static IGeometryServices InitializeInstance()
        {
            lock (s_lock)
            {
                var instance = s_instance;
                if (instance != null) return instance;
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
                                Interlocked.CompareExchange(ref s_instance, (IGeometryServices)Activator.CreateInstance(type), null);
                                return s_instance;
                            }
                    }
                }
            }
            throw new InvalidOperationException("Cannot use GeometryServiceProvider without an assigned IGeometryServices class");
        }
    }
}
