using System;
#if COMPAT_BOOTSTRAP_USING_REFLECTION && HAS_SYSTEM_APPDOMAIN_GETASSEMBLIES && HAS_SYSTEM_REFLECTION_ASSEMBLY_GETEXPORTEDTYPES
using System.Reflection;
using System.Reflection.Emit;
#endif

namespace GeoAPI
{
    /// <summary>
    /// Static class that provides access to a  <see cref="IGeometryServices"/> class.
    /// </summary>
    public static class GeometryServiceProvider
    {
        private static volatile IGeometryServices _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets or sets the <see cref="IGeometryServices"/> instance.
        /// </summary>
        public static IGeometryServices Instance
        {
            get => _instance ?? InitializeInstance();
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                lock (_lock)
                    _instance = value;
            }
        }

        private static IGeometryServices InitializeInstance()
        {
#if COMPAT_BOOTSTRAP_USING_REFLECTION && HAS_SYSTEM_APPDOMAIN_GETASSEMBLIES && HAS_SYSTEM_REFLECTION_ASSEMBLY_GETEXPORTEDTYPES
            lock (_lock)
            {
                var instance = _instance;
                if (instance != null) return instance;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.GlobalAssemblyCache && assembly.CodeBase == Assembly.GetExecutingAssembly().CodeBase)
                        continue;

                    var assemblyType = assembly.GetType();
                    if (assemblyType == typeof(AssemblyBuilder) ||
                        assemblyType.FullName == "System.Reflection.Emit.InternalAssemblyBuilder")
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
                        {
                            if (constructor.IsPublic && constructor.GetParameters().Length == 0)
                                return _instance = (IGeometryServices)Activator.CreateInstance(type);
                        }
                    }
                }
            }
#endif
            throw new InvalidOperationException("Cannot use GeometryServiceProvider without an assigned IGeometryServices class");
        }
    }
}
