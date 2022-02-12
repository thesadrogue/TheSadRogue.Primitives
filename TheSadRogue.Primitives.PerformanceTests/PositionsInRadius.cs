using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests
{
    /// <summary>
    /// Benchmarks for the Radius.PositionsInRadius functions.
    /// </summary>
    public class PositionsInRadius
    {
        [Params(10, 50, 100, 200)]
        public int MapSize;

        [Params(5, 10, 25)]
        public int Radius;

        [ParamsAllValues]
        public Radius.Types RadiusShapeType;

        private Radius _radiusShape;
        private RadiusLocationContext _existingContext = null!;
        private Point _center;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _radiusShape = RadiusShapeType;
            _center = new Point(MapSize / 2, MapSize / 2);
            _existingContext = new RadiusLocationContext(_center, Radius);
        }

        [Benchmark]
        public int IteratePositionsNewContext()
        {
            int sum = 0;
            foreach (var pos in _radiusShape.PositionsInRadius(_center, Radius))
                sum += pos.ToIndex(MapSize);
            return sum;
        }

        [Benchmark]
        public int IteratePositionsExistingContext()
        {
            int sum = 0;
            foreach (var pos in _radiusShape.PositionsInRadius(_existingContext))
                sum += pos.ToIndex(MapSize);
            return sum;
        }
    }
}
