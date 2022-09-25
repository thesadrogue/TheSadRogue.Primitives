using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace TheSadRogue.Primitives.PerformanceTests.GridViews
{

    public struct CustomPositionsEnumerable
    {
        private Point _current;

        public Point Current => _current;

        private readonly Rectangle _positions;

        public CustomPositionsEnumerable(Rectangle positions)
        {
            _positions = positions;

            _current = positions.MinExtent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            //var maxExtent = _positions.MaxExtent;
            if (_current.X < _positions.MaxExtent.X)
            {
                _current = new Point(_current.X + 1, _current.Y);
                return true;
            }
            else if (_current.Y < _positions.MaxExtent.Y)
            {
                _current = new Point(_positions.MinExtent.X, _current.Y + 1);
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CustomPositionsEnumerable GetEnumerator() => this;

        public IEnumerable<Point> ToEnumerable()
        {
            foreach (var point in this)
                yield return point;
        }
    }

    public static class CustomPositionsExtension
    {
        public static CustomPositionsEnumerable PositionsCustom<T>(this IGridView<T> gridView)
            => new CustomPositionsEnumerable(gridView.Bounds());
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
        public int ManualPositionsIteration()
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
        public int ManualPositionsIterationNoCachingWidthHeight()
        {
            int sum = 0;
            for (int y = 0; y < _gridView.Height; y++)
            for (int x = 0; x < _gridView.Width; x++)
                sum += x + y;

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
        public int CustomPositionsIteration()
        {
            int sum = 0;
            foreach (var pos in _gridView.PositionsCustom())
                sum += pos.X + pos.Y;

            return sum;
        }

        [Benchmark]
        public int CustomPositionsEnumerableIteration()
        {
            int sum = 0;
            foreach (var pos in _gridView.PositionsCustom().ToEnumerable())
                sum += pos.X + pos.Y;

            return sum;
        }
    }
}
