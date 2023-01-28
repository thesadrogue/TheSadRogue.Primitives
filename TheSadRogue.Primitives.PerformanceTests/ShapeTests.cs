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
        => Circle(center.X, center.Y, radius, plot);

    private static void Circle(int centerX, int centerY, int radius, Action<Point> plot)
    {
        int xi = -radius, yi = 0, err = 2 - 2 * radius; /* II. Quadrant */
        do
        {
            plot(new Point(centerX - xi, centerY + yi)); /*   I. Quadrant */
            plot(new Point(centerX - yi, centerY - xi)); /*  II. Quadrant */
            plot(new Point(centerX + xi, centerY - yi)); /* III. Quadrant */
            plot(new Point(centerX + yi, centerY + xi)); /*  IV. Quadrant */
            radius = err;
            if (radius <= yi)
                err += ++yi * 2 + 1;           /* e_xy+e_y < 0 */

            if (radius > xi || err > yi)
                err += ++xi * 2 + 1; /* e_xy+e_x > 0 or no 2nd y-step */
        } while (xi < 0);
    }
}

interface ICustomEnumerable
{
    public bool MoveNext();
    public Point Current { get; }
    public ICustomEnumerable GetEnumerator() => this;
}

internal struct CircleEnumerable : ICustomEnumerable
{
    private readonly Point _center;
    private int _radius;
    private int _xi;
    private int _yi;
    private int _err;

    // Suppress warning stating to use auto-property because we want to guarantee micro-performance
    // characteristics.
    #pragma warning disable IDE0032 // Use auto property
    private Point _current;
    #pragma warning restore IDE0032 // Use auto property

    /// <summary>
    /// The current value for enumeration.
    /// </summary>
    public Point Current => _current;

    private int _state;

    public CircleEnumerable(Point center, int radius)
    {
        // int xi = -radius, yi = 0, err = 2 - 2 * radius
        _center = center;
        _radius = radius;
        _xi = -radius;
        _yi = 0;
        _err = 2 - 2 * radius;
        _current = new Point(center.X - _xi, center.Y + _yi);
        _state = 1;
    }

    public bool MoveNext()
    {
        switch (_state)
        {
            case 0: /*   I. Quadrant */
                _current = new Point(_center.X - _xi, _center.Y + _yi);
                _state++;
                return true;
            case 1: /*  II. Quadrant */
                _current = new Point(_center.X - _yi, _center.Y - _xi);
                _state++;
                return true;
            case 2: /* III. Quadrant */
                _current = new Point(_center.X + _xi, _center.Y - _yi);
                _state++;
                return true;
            case 3: /*  IV. Quadrant */
                _current = new Point(_center.X + _yi, _center.Y + _xi);
                _radius = _err;
                if (_radius <= _yi)
                    _err += ++_yi * 2 + 1;           /* e_xy+e_y < 0 */

                if (_radius > _xi || _err > _yi)
                    _err += ++_xi * 2 + 1; /* e_xy+e_x > 0 or no 2nd y-step */

                _state = 0;
                return _xi < 0;
        }

        return false;
    }

    public CircleEnumerable GetEnumerator() => this;

    public IEnumerable<Point> ToEnumerable()
    {
        // Equivalent to doing "yield return" on a foreach loop over this iterator, but faster.
        do
        {
            yield return new Point(_center.X - _xi, _center.Y + _yi); /*   I. Quadrant */
            yield return new Point(_center.X - _yi, _center.Y - _xi); /*  II. Quadrant */
            yield return new Point(_center.X + _xi, _center.Y - _yi); /* III. Quadrant */
            yield return new Point(_center.X + _yi, _center.Y + _xi); /*  IV. Quadrant */
            _radius = _err;
            if (_radius <= _yi)
                _err += ++_yi * 2 + 1;           /* e_xy+e_y < 0 */

            if (_radius > _xi || _err > _yi)
                _err += ++_xi * 2 + 1; /* e_xy+e_x > 0 or no 2nd y-step */

        } while (_xi < 0);
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
        foreach (var point in Shapes.GetCircle(Center, Radius))
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
    public int CustomEnumeratorCircle()
    {
        int sum = 0;
        foreach (var point in new CircleEnumerable(Center, Radius))
            sum += point.X + point.Y;

        return sum;
    }

    [Benchmark]
    public int CustomEnumeratorCircleToEnumerable()
    {
        int sum = 0;
        IEnumerable<Point> it = new CircleEnumerable(Center, Radius).ToEnumerable();
        foreach (var point in it)
            sum += point.X + point.Y;

        return sum;
    }

    [Benchmark]
    public int CustomEnumeratorCircleViaInterface()
    {
        int sum = 0;
        ICustomEnumerable it = new CircleEnumerable(Center, Radius);
        foreach (var point in it)
            sum += point.X + point.Y;

        return sum;
    }
}
