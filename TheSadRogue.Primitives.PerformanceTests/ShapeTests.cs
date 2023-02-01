using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests;

/// <summary>
/// Alternate implementations of the shape functions which use a "plot" function instead of IEnumerable.
/// </summary>
internal static class PlotShapes
{
    public static void GetCircle(Point center, int radius, Action<Point> plot)
    {
        int xi = -radius, yi = 0, err = 2 - 2 * radius; /* II. Quadrant */
        do
        {
            plot(new Point(center.X - xi, center.Y + yi)); /*   I. Quadrant */
            plot(new Point(center.X - yi, center.Y - xi)); /*  II. Quadrant */
            plot(new Point(center.X + xi, center.Y - yi)); /* III. Quadrant */
            plot(new Point(center.X + yi, center.Y + xi)); /*  IV. Quadrant */
            radius = err;
            if (radius <= yi)
                err += ++yi * 2 + 1;           /* e_xy+e_y < 0 */

            if (radius > xi || err > yi)
                err += ++xi * 2 + 1; /* e_xy+e_x > 0 or no 2nd y-step */
        } while (xi < 0);
    }
}

internal static class YieldReturnEnumerableShapes
{
    public static IEnumerable<Point> GetCircle(Point center, int radius)
    {
        int xi = -radius, yi = 0, err = 2 - 2 * radius; /* II. Quadrant */
        do
        {
            yield return new Point(center.X - xi, center.Y + yi); /*   I. Quadrant */
            yield return new Point(center.X - yi, center.Y - xi); /*  II. Quadrant */
            yield return new Point(center.X + xi, center.Y - yi); /* III. Quadrant */
            yield return new Point(center.X + yi, center.Y + xi); /*  IV. Quadrant */
            radius = err;
            if (radius <= yi)
                err += ++yi * 2 + 1;           /* e_xy+e_y < 0 */

            if (radius > xi || err > yi)
                err += ++xi * 2 + 1; /* e_xy+e_x > 0 or no 2nd y-step */
        } while (xi < 0);
    }

    public static IEnumerable<Point> GetEllipse(int x0, int y0, int x1, int y1)
    {
        int a = Math.Abs(x1 - x0), b = Math.Abs(y1 - y0), b1 = b & 1; /* values of diameter */
        long dx = 4 * (1 - a) * b * b, dy = 4 * (b1 + 1) * a * a; /* error increment */
        long err = dx + dy + b1 * a * a, e2; /* error of 1.step */

        if (x0 > x1) { x0 = x1; x1 += a; } /* if called with swapped points */
        if (y0 > y1)
        {
            y0 = y1; /* .. exchange them */
        }

        y0 += (b + 1) / 2; y1 = y0 - b1;   /* starting pixel */
        a *= 8 * a; b1 = 8 * b * b;

        do
        {
            yield return new Point(x1, y0); /*   I. Quadrant */
            yield return new Point(x0, y0); /*  II. Quadrant */
            yield return new Point(x0, y1); /* III. Quadrant */
            yield return new Point(x1, y1); /*  IV. Quadrant */
            e2 = 2 * err;
            if (e2 <= dy) { y0++; y1--; err += dy += a; }  /* y step */
            if (e2 >= dx || 2 * err > dy) { x0++; x1--; err += dx += b1; } /* x step */
        } while (x0 <= x1);

        while (y0 - y1 < b)
        {  /* too early stop of flat ellipses a=1 */
            yield return new Point(x0 - 1, y0); /* -> finish tip of ellipse */
            yield return new Point(x1 + 1, y0++);
            yield return new Point(x0 - 1, y1);
            yield return new Point(x1 + 1, y1--);
        }
    }
}

public class CircleTests
{
    public static readonly Point Center = new (1, 1);

    [Params(5, 10, 25, 50)]
    public int Radius;

    #region Circle
    [Benchmark]
    public int Primitives()
    {
        int sum = 0;
        foreach (var point in Shapes.GetCircle(Center, Radius))
            sum += point.X + point.Y;

        return sum;
    }
    [Benchmark]
    public int PrimitivesToEnumerable()
    {
        int sum = 0;
        foreach (var point in Shapes.GetCircle(Center, Radius))
            sum += point.X + point.Y;

        return sum;
    }


    [Benchmark]
    public int Plot()
    {
        int sum = 0;

        void PlotFunc(Point point) => sum += point.X + point.Y;
        PlotShapes.GetCircle(Center, Radius, PlotFunc);

        return sum;
    }

    [Benchmark]
    public int YieldReturn()
    {
        int sum = 0;
        foreach (var point in YieldReturnEnumerableShapes.GetCircle(Center, Radius))
            sum += point.X + point.Y;

        return sum;
    }

    #endregion
}

public class EllipseTests
{
    [ParamsSource(nameof(TestCases))]
    public (Point f1, Point f2) Ellipse;

    [Benchmark]
    public int Primitives()
    {
        int sum = 0;
        foreach (var point in Shapes.GetEllipse(Ellipse.f1, Ellipse.f2))
            sum += point.X + point.Y;

        return sum;
    }

    [Benchmark]
    public int PrimitivesToEnumerable()
    {
        int sum = 0;
        foreach (var point in Shapes.GetEllipse(Ellipse.f1, Ellipse.f2))
            sum += point.X + point.Y;

        return sum;
    }

    [Benchmark]
    public int YieldReturn()
    {
        int sum = 0;
        foreach (var point in YieldReturnEnumerableShapes.GetEllipse(Ellipse.f1.X, Ellipse.f1.Y, Ellipse.f2.X, Ellipse.f2.Y))
            sum += point.X + point.Y;

        return sum;
    }

    public IEnumerable<(Point f1, Point f2)> TestCases()
    {
        yield return ((1, 1), (25, 50));
        yield return ((25, 50), (1, 1));
        yield return ((1, 1), (50, 1));
        yield return ((1, 1), (1, 25));
    }
}
