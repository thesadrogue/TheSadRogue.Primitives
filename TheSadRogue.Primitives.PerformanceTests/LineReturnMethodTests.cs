using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests;

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

        BresenhamWithFunctionPointers.BresenhamFunc(LineToDraw.start, LineToDraw.end, pos =>
        {
            sum += pos.X + pos.Y;
            return true;
        });

        return sum;
    }

    [Benchmark]
    public int BresenhamFuncLocalFunction()
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

        BresenhamWithFunctionPointers.BresenhamAction(LineToDraw.start, LineToDraw.end, pos => sum += pos.X + pos.Y);

        return sum;
    }

    [Benchmark]
    public int BresenhamActionLocalFunction()
    {
        int sum = 0;

        void Action(Point pos)
        {
            sum += pos.X + pos.Y;
        }

        BresenhamWithFunctionPointers.BresenhamAction(LineToDraw.start, LineToDraw.end, Action);

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
