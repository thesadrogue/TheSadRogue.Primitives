using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests
{
    public static class AreaPerimeterPositionsExtensions
    {
        public static IEnumerable<Point> PerimeterPositionsLinq(this IReadOnlyArea area, AdjacencyRule rule)
            => area.Where(pos => rule.Neighbors(pos).Any(n => !area.Contains(n)));

        public static IEnumerable<Point> PerimeterPositionsNeighborsFunc(this IReadOnlyArea area, AdjacencyRule rule)
        {
            foreach (var pos in area.FastEnumerator())
            {
                foreach (var neighbor in rule.Neighbors(pos))
                {
                    if (!area.Contains(neighbor))
                    {
                        yield return pos;
                        break;
                    }
                }
            }
        }
    }
    public class ReadOnlyAreaExtensions
    {
        [Params(10, 100, 200)]
        public int Size;

        private AdjacencyRule _rule;

        private SadRogue.Primitives.Area _area = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _area = new SadRogue.Primitives.Area(new SadRogue.Primitives.Rectangle(0, 0, Size, Size).Positions().ToEnumerable());
            _rule = AdjacencyRule.Cardinals;
        }

        [Benchmark]
        public int PerimeterPositions()
        {
            int sum = 0;
            foreach (var pos in _area.PerimeterPositions(_rule))
                sum += pos.X + pos.Y;

            return sum;
        }

        [Benchmark]
        public int PerimeterPositionsNeighborsFunc()
        {
            int sum = 0;
            foreach (var pos in _area.PerimeterPositionsNeighborsFunc(_rule))
                sum += pos.X + pos.Y;

            return sum;
        }

        [Benchmark]
        public int PerimeterPositionsLinq()
        {
            int sum = 0;
            foreach (var pos in _area.PerimeterPositionsLinq(_rule))
                sum += pos.X + pos.Y;

            return sum;
        }

    }
}
