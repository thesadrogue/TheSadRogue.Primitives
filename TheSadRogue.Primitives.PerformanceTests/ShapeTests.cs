using System;
using System.Collections;
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
}

public class ShapeTests
{
    public static readonly Point Center = new (1, 1);

    [Params(5, 10, 25, 50)]
    public int Radius;

    [Benchmark]
    public int PrimitivesCircle()
    {
        int sum = 0;
        foreach (var point in ShapeAlgorithms.GetCircle(Center, Radius))
            sum += point.X + point.Y;

        return sum;
    }
    [Benchmark]
    public int PrimitivesCircleToEnumerable()
    {
        int sum = 0;
        foreach (var point in ShapeAlgorithms.GetCircle(Center, Radius).ToEnumerable())
            sum += point.X + point.Y;

        return sum;
    }


    [Benchmark]
    public int PlotCircle()
    {
        int sum = 0;

        void Plot(Point point) => sum += point.X + point.Y;
        PlotShapes.GetCircle(Center, Radius, Plot);

        return sum;
    }

    [Benchmark]
    public int YieldReturnCircle()
    {
        int sum = 0;
        foreach (var point in YieldReturnEnumerableShapes.GetCircle(Center, Radius))
            sum += point.X + point.Y;

        return sum;
    }
}
