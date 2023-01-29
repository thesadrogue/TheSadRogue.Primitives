using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests;

public struct BresenhamEnumerable
    {
        // Suppress warning stating to use auto-property because we want to guarantee micro-performance
        // characteristics.
        #pragma warning disable IDE0032 // Use auto property
        private Point _current;
        #pragma warning restore IDE0032 // Use auto property

        /// <summary>
        /// The current value for enumeration.
        /// </summary>
        public Point Current => _current;

        private int _startX;
        private int _startY;

        private int _numerator;
        private readonly int _shortest;
        private readonly int _longest;

        private readonly int _dx1;
        private readonly int _dy1;
        private readonly int _dx2;
        private readonly int _dy2;

        private int _idx;

        public BresenhamEnumerable(Point start, Point end)
        {
            _current = Point.None;

            (_startX, _startY) = start;

            int w = end.X - _startX;
            int h = end.Y - _startY;
            _dx1 = 0;
            _dy1 = 0;
            _dx2 = 0;
            _dy2 = 0;

            if (w < 0) _dx1 = -1;
            else if (w > 0) _dx1 = 1;
            if (h < 0) _dy1 = -1;
            else if (h > 0) _dy1 = 1;
            if (w < 0) _dx2 = -1;
            else if (w > 0) _dx2 = 1;
            _longest = Math.Abs(w);
            _shortest = Math.Abs(h);
            if (!(_longest > _shortest))
            {
                _longest = Math.Abs(h);
                _shortest = Math.Abs(w);
                if (h < 0) _dy2 = -1;
                else if (h > 0) _dy2 = 1;
                _dx2 = 0;
            }

            _numerator = _longest >> 1;

            _idx = 0;
        }

        public bool MoveNext()
        {
            /*
            for (int i = 0; i <= longest; i++)
            {
                yield return new Point(startX, startY);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    startX += dx1;
                    startY += dy1;
                }
                else
                {
                    startX += dx2;
                    startY += dy2;
                }
            }
            */

            if (_idx == _longest + 1)
                return false;

            _current = new Point(_startX, _startY);
            _numerator += _shortest;
            if (!(_numerator < _longest))
            {
                _numerator -= _longest;
                _startX += _dx1;
                _startY += _dy1;
            }
            else
            {
                _startX += _dx2;
                _startY += _dy2;
            }

            _idx++;
            return true;
        }

        public BresenhamEnumerable GetEnumerator() => this;
    }

static class BresenhamWithFunctionPointers
{
    public static void BresenhamFunc(Point start, Point end, Func<Point, bool> action)
        => BresenhamFunc(start.X, start.Y, end.X, end.Y, action);

    public static void BresenhamAction(Point start, Point end, Action<Point> action)
        => BresenhamAction(start.X, start.Y, end.X, end.Y, action);

    private static void BresenhamFunc(int startX, int startY, int endX, int endY, Func<Point, bool> action)
    {
        int w = endX - startX;
        int h = endY - startY;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1;
        else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1;
        else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1;
        else if (w > 0) dx2 = 1;
        int longest = Math.Abs(w);
        int shortest = Math.Abs(h);
        if (!(longest > shortest))
        {
            longest = Math.Abs(h);
            shortest = Math.Abs(w);
            if (h < 0) dy2 = -1;
            else if (h > 0) dy2 = 1;
            dx2 = 0;
        }

        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            if (!action(new Point(startX, startY)))
                return;
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                startX += dx1;
                startY += dy1;
            }
            else
            {
                startX += dx2;
                startY += dy2;
            }
        }
    }

    private static void BresenhamAction(int startX, int startY, int endX, int endY, Action<Point> action)
    {
        int w = endX - startX;
        int h = endY - startY;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1;
        else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1;
        else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1;
        else if (w > 0) dx2 = 1;
        int longest = Math.Abs(w);
        int shortest = Math.Abs(h);
        if (!(longest > shortest))
        {
            longest = Math.Abs(h);
            shortest = Math.Abs(w);
            if (h < 0) dy2 = -1;
            else if (h > 0) dy2 = 1;
            dx2 = 0;
        }

        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            action(new Point(startX, startY));
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                startX += dx1;
                startY += dy1;
            }
            else
            {
                startX += dx2;
                startY += dy2;
            }
        }
    }
}

public class LineReturnMethodTests
{
    [ParamsSource(nameof(TestCases))]
    public (Point start, Point end) LineToDraw;

    [Benchmark]
    public int BresenhamIEnumerable()
    {
        int sum = 0;
        foreach (var pos in Lines.GetLine(LineToDraw.start, LineToDraw.end, Lines.Algorithm.Bresenham))
            sum += pos.X + pos.Y;

        return sum;
    }

    [Benchmark]
    public int BresenhamFunc()
    {
        int sum = 0;

        bool Action(Point pos)
        {
            sum += pos.X + pos.Y;
            return true;
        }

        BresenhamWithFunctionPointers.BresenhamFunc(LineToDraw.start, LineToDraw.end, Action);

        return sum;
    }

    [Benchmark]
    public int BresenhamAction()
    {
        int sum = 0;

        void Action(Point pos)
        {
            sum += pos.X + pos.Y;
        }

        BresenhamWithFunctionPointers.BresenhamAction(LineToDraw.start, LineToDraw.end, Action);

        return sum;
    }

    [Benchmark]
    public int BresenhamCustomEnumerable()
    {
        int sum = 0;

        foreach (var pos in new BresenhamEnumerable(LineToDraw.start, LineToDraw.end))
            sum += pos.X + pos.Y;

        return sum;
    }

    public static IEnumerable<(Point start, Point end)> TestCases()
    {
        yield return ((1, 1), (25, 50));
        yield return ((1, 1), (50, 25));
        yield return ((25, 50), (1, 1));
        yield return ((50, 25), (1, 1));
        // These lines have manhattan distance == chebyshev distance,  These cases are useful to help prove that
        // Orthogonal is about as fast as the others, if measured proportional to the appropriate distance calculation
        // (eg. the length of the line)
        yield return ((1, 1), (50, 1));
        yield return ((1, 1), (1, 25));
    }


}
