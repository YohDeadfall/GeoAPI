using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Reflection;

namespace GeoAPI.Benchmarks
{
    public class Program
    {
        static void Main(string[] args)
            => BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
    }

    public class AutoInitialized
    {
        [Benchmark]
        public IGeometryServices Locks()
        {
            UseLocks.Reset();
            return UseLocks.Instance;
        }

        [Benchmark]
        public IGeometryServices PoolRequest39()
        {
            UsePoolRequest39.Reset();
            return UsePoolRequest39.Instance;
        }

        [Benchmark]
        public IGeometryServices Lazy()
        {
            UseLazy.Reset();
            return UseLazy.Instance;
        }

        [Benchmark]
        public IGeometryServices LazyInitializer()
        {
            UseLazyInitializer.Reset();
            return UseLazyInitializer.Instance;
        }
    }

    public class UserInitialized
    {
        private static readonly DummyGeometryServices s_dummy = new DummyGeometryServices();

        [GlobalSetup]
        public void Setup()
        {
            UseLocks.Instance = s_dummy;
            UsePoolRequest39.Instance = s_dummy;
            UseLazy.Instance = s_dummy;
            UseLazyInitializer.Instance = s_dummy;
        }

        [Benchmark]
        public IGeometryServices Locks()
            => UseLocks.Instance;

        [Benchmark]
        public IGeometryServices PoolRequest39()
            => UsePoolRequest39.Instance;

        [Benchmark]
        public IGeometryServices Lazy()
            => UseLazy.Instance;

        [Benchmark]
        public IGeometryServices LazyInitializer()
            => UseLazyInitializer.Instance;
    }
}
