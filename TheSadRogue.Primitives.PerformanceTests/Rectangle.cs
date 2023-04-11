using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using JetBrains.Annotations;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests
{
    public static class RectangleTestingExtensions
    {
        public static IEnumerable<SadRogue.Primitives.Rectangle> BisectHorizontallyNoCustomIterator(this SadRogue.Primitives.Rectangle rectangle)
        {
            int startX = rectangle.MinExtentX;
            int stopY = rectangle.MaxExtentY;
            int startY = rectangle.MinExtentY;
            int stopX = rectangle.MaxExtentX;
            int bisection = (startY + stopY) / 2;

            yield return new SadRogue.Primitives.Rectangle(new Point(startX, startY), new Point(stopX, bisection));
            yield return new SadRogue.Primitives.Rectangle(new Point(startX, bisection + 1), new Point(stopX, stopY));
        }

        public static IEnumerable<Point> PerimeterPositionsNoCustomIterator(this SadRogue.Primitives.Rectangle self)
        {
            for (int x = self.MinExtentX; x <= self.MaxExtentX; x++)
                yield return new Point(x, self.MinExtentY); // Minimum y-side perimeter

            // Start offset 1, since last loop returned the corner piece
            for (int y = self.MinExtentY + 1; y <= self.MaxExtentY; y++)
                yield return new Point(self.MaxExtentX, y);

            // Again skip 1 because last loop returned the corner piece
            for (int x = self.MaxExtentX - 1; x >= self.MinExtentX; x--)
                yield return new Point(x, self.MaxExtentY);

            // Skip 1 on both ends, because last loop returned one corner, first loop returned the other
            for (int y = self.MaxExtentY - 1; y >= self.MinExtentY + 1; y--)
                yield return new Point(self.MinExtentX, y);
        }

        public static IEnumerable<Point> PerimeterPositionsNoCustomIteratorCachedEnds(this SadRogue.Primitives.Rectangle self)
        {
            int minX = self.MinExtentX;
            int minY = self.MinExtentY;
            int maxX = self.MaxExtentX;
            int maxY = self.MaxExtentY;
            for (int x = minX; x <= maxX; x++)
                yield return new Point(x, minY); // Minimum y-side perimeter

            // Start offset 1, since last loop returned the corner piece
            for (int y = minY + 1; y <= maxY; y++)
                yield return new Point(maxX, y);

            // Again skip 1 because last loop returned the corner piece
            for (int x = maxX - 1; x >= minX; x--)
                yield return new Point(x, maxY);

            // Skip 1 on both ends, because last loop returned one corner, first loop returned the other
            for (int y = maxY - 1; y >= minY + 1; y--)
                yield return new Point(minX, y);
        }
    }

    /// <summary>
    /// Benchmarks for <see cref="SadRogue.Primitives.Rectangle"/>.
    /// </summary>
    public class Rectangle
    {
        [UsedImplicitly]
        [Params(10, 50, 100)]
        public int Width;

        [UsedImplicitly]
        [Params(10, 50, 100)]
        public int Height;

        private SadRogue.Primitives.Rectangle _rectangle;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _rectangle = new SadRogue.Primitives.Rectangle(0, 0, Width, Height);
        }

        #region Library Implementations

        [Benchmark]
        public int Bisect()
        {
            var result = _rectangle.Bisect();
            int sum = result.Rect1.Width + result.Rect1.Height;
            sum += result.Rect2.Width + result.Rect2.Height;

            return sum;
        }

        [Benchmark]
        public int BisectVertically()
        {
            var result = _rectangle.BisectVertically();
            int sum = result.Rect1.Width + result.Rect1.Height;
            sum += result.Rect2.Width + result.Rect2.Height;

            return sum;
        }

        [Benchmark]
        public int BisectHorizontally()
        {
            var result = _rectangle.BisectHorizontally();
            int sum = result.Rect1.Width + result.Rect1.Height;
            sum += result.Rect2.Width + result.Rect2.Height;

            return sum;
        }

        [Benchmark]
        public int BisectRecursive()
        {
            int sum = 0;
            foreach (var rect in _rectangle.BisectRecursive(3))
                sum += rect.Width + rect.Height;

            return sum;
        }


        [Benchmark]
        public int BisectToEnumerable()
        {
            int sum = 0;
            foreach (var rect in _rectangle.Bisect())
                sum += rect.Width + rect.Height;

            return sum;
        }

        [Benchmark]
        public int BisectVerticallyToEnumerable()
        {
            int sum = 0;
            foreach (var rect in _rectangle.BisectVertically())
                sum += rect.Width + rect.Height;

            return sum;
        }

        [Benchmark]
        public int BisectHorizontallyToEnumerable()
        {
            int sum = 0;
            foreach (var rect in _rectangle.BisectHorizontally())
                sum += rect.Width + rect.Height;

            return sum;
        }
        #endregion

        #region IEnumerable Implementations

        [Benchmark]
        public int BisectHorizontallyNoCustomIterator()
        {
            int sum = 0;
            foreach (var rect in _rectangle.BisectHorizontallyNoCustomIterator())
                sum += rect.Width + rect.Height;

            return sum;
        }

        [Benchmark]
        public int PerimeterPositions()
        {
            int sum = 0;
            foreach (var pos in _rectangle.PerimeterPositions())
                sum += pos.X + pos.Y;

            return sum;
        }

        [Benchmark]
        public int PerimeterPositionsNoCustomIterator()
        {
            int sum = 0;
            foreach (var pos in _rectangle.PerimeterPositionsNoCustomIterator())
                sum += pos.X + pos.Y;

            return sum;
        }

        [Benchmark]
        public int PerimeterPositionsNoCustomIteratorCachedEnds()
        {
            int sum = 0;
            foreach (var pos in _rectangle.PerimeterPositionsNoCustomIteratorCachedEnds())
                sum += pos.X + pos.Y;

            return sum;
        }
        #endregion
    }
}
