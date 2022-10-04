using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests
{
    /// <summary>
    /// Benchmarks for <see cref="SadRogue.Primitives.Area"/>.
    /// </summary>
    public class Area
    {
        [Params(10, 100, 200)]
        public int Size;

        private SadRogue.Primitives.Area _area = null!;
        private IReadOnlyArea _areaInterface = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _area = new SadRogue.Primitives.Area(new SadRogue.Primitives.Rectangle(0, 0, Size, Size).Positions().ToEnumerable());
            _areaInterface = _area;
        }

        [Benchmark]
        public int BenchmarkEnumerable()
        {
            int sum = 0;
            foreach (var pos in _areaInterface)
                sum += pos.X + pos.Y;

            return sum;
        }

        [Benchmark]
        public int BenchmarkEnumerableAsConcrete()
        {
            int sum = 0;
            foreach (var pos in _area)
                sum += pos.X + pos.Y;

            return sum;
        }
    }
}
