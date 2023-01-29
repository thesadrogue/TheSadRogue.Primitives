using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests;

internal static class YieldReturnEnumerableLines
{
    private const int ModifierX = 0x7fff;
        private const int ModifierY = 0x7fff;

        public static IEnumerable<Point> Bresenham(int startX, int startY, int endX, int endY)
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
        }

        public static IEnumerable<Point> DDA(int startX, int startY, int endX, int endY)
        {
            int dx = endX - startX;
            int dy = endY - startY;

            int nx = Math.Abs(dx);
            int ny = Math.Abs(dy);

            // Calculate octant value
            int octant = (dy < 0 ? 4 : 0) | (dx < 0 ? 2 : 0) | (ny > nx ? 1 : 0);
            int fraction = 0;
            int mn = Math.Max(nx, ny);

            int move;

            if (mn == 0)
            {
                yield return new Point(startX, startY);
                yield break;
            }

            if (ny == 0)
            {
                if (dx > 0)
                    for (int x = startX; x <= endX; x++)
                        yield return new Point(x, startY);
                else
                    for (int x = startX; x >= endX; x--)
                        yield return new Point(x, startY);

                yield break;
            }

            if (nx == 0)
            {
                if (dy > 0)
                    for (int y = startY; y <= endY; y++)
                        yield return new Point(startX, y);
                else
                    for (int y = startY; y >= endY; y--)
                        yield return new Point(startX, y);

                yield break;
            }

            switch (octant)
            {
                case 0: // +x, +y
                    move = (ny << 16) / nx;
                    for (int primary = startX; primary <= endX; primary++, fraction += move)
                        yield return new Point(primary, startY + ((fraction + ModifierY) >> 16));
                    break;

                case 1:
                    move = (nx << 16) / ny;
                    for (int primary = startY; primary <= endY; primary++, fraction += move)
                        yield return new Point(startX + ((fraction + ModifierX) >> 16), primary);
                    break;

                case 2: // -x, +y
                    move = (ny << 16) / nx;
                    for (int primary = startX; primary >= endX; primary--, fraction += move)
                        yield return new Point(primary, startY + ((fraction + ModifierY) >> 16));
                    break;

                case 3:
                    move = (nx << 16) / ny;
                    for (int primary = startY; primary <= endY; primary++, fraction += move)
                        yield return new Point(startX - ((fraction + ModifierX) >> 16), primary);
                    break;

                case 6: // -x, -y
                    move = (ny << 16) / nx;
                    for (int primary = startX; primary >= endX; primary--, fraction += move)
                        yield return new Point(primary, startY - ((fraction + ModifierY) >> 16));
                    break;

                case 7:
                    move = (nx << 16) / ny;
                    for (int primary = startY; primary >= endY; primary--, fraction += move)
                        yield return new Point(startX - ((fraction + ModifierX) >> 16), primary);
                    break;

                case 4: // +x, -y
                    move = (ny << 16) / nx;
                    for (int primary = startX; primary <= endX; primary++, fraction += move)
                        yield return new Point(primary, startY - ((fraction + ModifierY) >> 16));
                    break;

                case 5:
                    move = (nx << 16) / ny;
                    for (int primary = startY; primary >= endY; primary--, fraction += move)
                        yield return new Point(startX + ((fraction + ModifierX) >> 16), primary);
                    break;
            }
        }

        public static IEnumerable<Point> Ortho(int startX, int startY, int endX, int endY)
        {
            int dx = endX - startX;
            int dy = endY - startY;

            int nx = Math.Abs(dx);
            int ny = Math.Abs(dy);

            int signX = dx > 0 ? 1 : -1;
            int signY = dy > 0 ? 1 : -1;

            int workX = startX;
            int workY = startY;

            yield return new Point(startX, startY);

            for (int ix = 0, iy = 0; ix < nx || iy < ny;)
            {
                // Optimized version of `if ((0.5 + ix) / nx < (0.5 + iy) / ny)`
                if ((1 + ix + ix) * ny < (1 + iy + iy) * nx)
                {
                    workX += signX;
                    ix++;
                }
                else
                {
                    workY += signY;
                    iy++;
                }

                yield return new Point(workX, workY);
            }
        }
}

internal static class FunctionPointerLines
{
    public static void BresenhamFunc(int startX, int startY, int endX, int endY, Func<Point, bool> action)
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
}

public class LineReturnMethodTests
{
    [ParamsSource(nameof(TestCases))]
    public (Point start, Point end) LineToDraw;

    [Benchmark]
    public int BresenhamPrimitivesToEnumerable()
    {
        int sum = 0;
        // ReSharper disable once RedundantArgumentDefaultValue
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

        FunctionPointerLines.BresenhamFunc(LineToDraw.start.X, LineToDraw.start.Y, LineToDraw.end.X, LineToDraw.end.Y, Action);

        return sum;
    }

    [Benchmark]
    public int BresenhamPrimitives()
    {
        int sum = 0;

        foreach (var pos in Lines.GetBresenhamLine(LineToDraw.start, LineToDraw.end))
            sum += pos.X + pos.Y;

        return sum;
    }

    [Benchmark]
    public int BresenhamYieldReturn()
    {
        int sum = 0;

        foreach (var pos in YieldReturnEnumerableLines.Bresenham(LineToDraw.start.X, LineToDraw.start.Y, LineToDraw.end.X, LineToDraw.end.Y))
            sum += pos.X + pos.Y;

        return sum;
    }

    [Benchmark]
    public int DDAPrimitivesToEnumerable()
    {
        int sum = 0;
        foreach (var pos in Lines.GetLine(LineToDraw.start, LineToDraw.end, Lines.Algorithm.DDA))
            sum += pos.X + pos.Y;

        return sum;
    }

    [Benchmark]
    public int DDAPrimitives()
    {
        int sum = 0;

        foreach (var pos in Lines.GetDDALine(LineToDraw.start, LineToDraw.end))
            sum += pos.X + pos.Y;

        return sum;
    }

    [Benchmark]
    public int DDAYieldReturn()
    {
        int sum = 0;

        foreach (var pos in YieldReturnEnumerableLines.DDA(LineToDraw.start.X, LineToDraw.start.Y, LineToDraw.end.X, LineToDraw.end.Y))
            sum += pos.X + pos.Y;

        return sum;
    }

    [Benchmark]
    public int OrthogonalPrimitivesToEnumerable()
    {
        int sum = 0;
        foreach (var pos in Lines.GetLine(LineToDraw.start, LineToDraw.end, Lines.Algorithm.Orthogonal))
            sum += pos.X + pos.Y;

        return sum;
    }

    [Benchmark]
    public int OrthogonalPrimitives()
    {
        int sum = 0;

        foreach (var pos in Lines.GetOrthogonalLine(LineToDraw.start, LineToDraw.end))
            sum += pos.X + pos.Y;

        return sum;
    }

    [Benchmark]
    public int OrthogonalYieldReturn()
    {
        int sum = 0;

        foreach (var pos in YieldReturnEnumerableLines.Ortho(LineToDraw.start.X, LineToDraw.start.Y, LineToDraw.end.X, LineToDraw.end.Y))
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
