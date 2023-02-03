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
        private SadRogue.Primitives.Area _area2 = null!;
        private IReadOnlyArea _areaInterface = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _area = new SadRogue.Primitives.Area(new SadRogue.Primitives.Rectangle(0, 0, Size, Size).Positions());
            _areaInterface = _area;

            _area2 = new SadRogue.Primitives.Area(new SadRogue.Primitives.Rectangle(0, 0, Size / 2, Size / 2).Positions());
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

        [Benchmark]
        public int BenchmarkFastEnumerable()
        {
            int sum = 0;
            foreach (var pos in new ReadOnlyAreaPositionsEnumerator(_areaInterface))
                sum += pos.X + pos.Y;

            return sum;
        }

        [Benchmark]
        public int BenchmarkFastEnumerableAsConcrete()
        {
            int sum = 0;
            foreach (var pos in new ReadOnlyAreaPositionsEnumerator(_area))
                sum += pos.X + pos.Y;

            return sum;
        }

        [Benchmark]
        public int BenchmarkManualFor()
        {
            int sum = 0;
            for (int i = 0; i < _areaInterface.Count; i++)
            {
                var pos = _areaInterface[i];
                sum += pos.X + pos.Y;
            }

            return sum;
        }

        [Benchmark]
        public int BenchmarkManualForCachedCount()
        {
            int sum = 0;
            int count = _areaInterface.Count;
            for (int i = 0; i < count; i++)
            {
                var pos = _areaInterface[i];
                sum += pos.X + pos.Y;
            }

            return sum;
        }

        [Benchmark]
        public int BenchmarkManualForAsConcrete()
        {
            int sum = 0;
            for (int i = 0; i < _area.Count; i++)
            {
                var pos = _area[i];
                sum += pos.X + pos.Y;
            }

            return sum;
        }

        [Benchmark]
        public int BenchmarkManualForAsConcreteCachedCount()
        {
            int sum = 0;
            int count = _area.Count;
            for (int i = 0; i < count; i++)
            {
                var pos = _area[i];
                sum += pos.X + pos.Y;
            }

            return sum;
        }

        [Benchmark]
        public SadRogue.Primitives.Area Intersection()
            => SadRogue.Primitives.Area.GetIntersection(_area, _area2);

        [Benchmark]
        public SadRogue.Primitives.Area Difference()
            => SadRogue.Primitives.Area.GetDifference(_area, _area2);

        [Benchmark]
        public SadRogue.Primitives.Area Union()
            => SadRogue.Primitives.Area.GetUnion(_area, _area2);

        [Benchmark]
        public bool ContainsArea()
            => _area.Contains(_area2);


    }
}
