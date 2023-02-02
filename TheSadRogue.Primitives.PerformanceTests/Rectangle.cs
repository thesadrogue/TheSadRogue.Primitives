using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests
{
    public static class RectangleTestingExtensions
    {
        public static IEnumerable<SadRogue.Primitives.Rectangle> BisectHorizontallyNativeEnumerator(this SadRogue.Primitives.Rectangle rectangle)
        {
            int startX = rectangle.MinExtentX;
            int stopY = rectangle.MaxExtentY;
            int startY = rectangle.MinExtentY;
            int stopX = rectangle.MaxExtentX;
            int bisection = (startY + stopY) / 2;

            yield return new SadRogue.Primitives.Rectangle(new Point(startX, startY), new Point(stopX, bisection));
            yield return new SadRogue.Primitives.Rectangle(new Point(startX, bisection + 1), new Point(stopX, stopY));
        }
    }

    /// <summary>
    /// Benchmarks for <see cref="SadRogue.Primitives.Rectangle"/>.
    /// </summary>
    public class Rectangle
    {
        [Params(10, 50, 100)]
        public int Width;
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
        public int BisectHorizontallyNativeEnumerable()
        {
            int sum = 0;
            foreach (var rect in _rectangle.BisectHorizontallyNativeEnumerator())
                sum += rect.Width + rect.Height;

            return sum;
        }
        #endregion
    }
}
