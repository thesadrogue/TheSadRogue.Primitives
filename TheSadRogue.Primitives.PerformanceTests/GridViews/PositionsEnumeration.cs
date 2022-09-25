using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Reports;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace TheSadRogue.Primitives.PerformanceTests.GridViews
{
    public static class EnumerablePositionsExtension
    {
        public static IEnumerable<Point> PositionsIEnumerable<T>(this IGridView<T> gridView)
        {
            for (int y = 0; y < gridView.Height; y++)
                for (int x = 0; x < gridView.Width; x++)
                    yield return new Point(x, y);
        }

        public static IEnumerable<Point> PositionsIEnumerableCacheWidthHeight<T>(this IGridView<T> gridView)
        {
            int width = gridView.Width;
            int height = gridView.Height;

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    yield return new Point(x, y);
        }
    }

    public class PositionsEnumeration
    {
        [Params(10, 100, 200)]
        public int Size;

        private IGridView<bool> _gridView = null!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _gridView = new ArrayView<bool>(Size, Size);
        }

        [Benchmark]
        public int ManualPositionsIterationNormal()
        {
            int sum = 0;
            for (int y = 0; y < _gridView.Height; y++)
                for (int x = 0; x < _gridView.Width; x++)
                    sum += x + y;

            return sum;
        }

        [Benchmark]
        public int ManualPositionsIterationCacheWidthHeight()
        {
            int sum = 0;
            int height = _gridView.Height;
            int width = _gridView.Width;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    sum += x + y;

            return sum;
        }

        [Benchmark]
        public int ManualPositionsIterationCountNormal()
        {
            int sum = 0;
            for (int i = 0; i < _gridView.Count; i++)
            {
                var pos = Point.FromIndex(i, _gridView.Width);
                sum += pos.X + pos.Y;
            }
                

            return sum;
        }

        [Benchmark]
        public int ManualPositionsIterationCountCacheCountAndWidth()
        {
            int count = _gridView.Count;
            int width = _gridView.Width;
            int sum = 0;
            for (int i = 0; i < count; i++)
            {
                var pos = Point.FromIndex(i, width);
                sum += pos.X + pos.Y;
            }

            return sum;
        }

        [Benchmark]
        public int PositionsIteration()
        {
            int sum = 0;
            foreach (var pos in _gridView.Positions())
                sum += pos.X + pos.Y;

            return sum;
        }

        [Benchmark]
        public int OldEnumerablePositionsIterationNormal()
        {
            int sum = 0;
            foreach (var pos in _gridView.PositionsIEnumerable())
                sum += pos.X + pos.Y;

            return sum;
        }

        [Benchmark]
        public int OldEnumerablePositionsIterationCacheWidthHeight()
        {
            int sum = 0;
            foreach (var pos in _gridView.PositionsIEnumerableCacheWidthHeight())
                sum += pos.X + pos.Y;

            return sum;
        }

        [Benchmark]
        public int PositionsToEnumerableIteration()
        {
            int sum = 0;
            foreach (var pos in _gridView.Positions().ToEnumerable())
                sum += pos.X + pos.Y;

            return sum;
        }
    }
}
