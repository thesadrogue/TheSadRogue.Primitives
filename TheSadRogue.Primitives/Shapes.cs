using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SadRogue.Primitives
{
    /// <summary>
    /// A custom enumerator used to iterate over all positions on the outside of a circle efficiently.
    ///
    /// Generally, you should use <see cref="Shapes.GetCircle"/> to get an instance of this, rather than creating one
    /// yourself.
    /// </summary>
    /// <remarks>
    /// This type is a struct, and as such is much more efficient when used in a foreach loop than a function returning
    /// IEnumerable&lt;Point&gt; by using "yield return".  This type does contain implement <see cref="IEnumerable{Point}"/>,
    /// so you can pass it to functions which require one (for example, System.LINQ).  However, this will have reduced
    /// performance due to boxing of the iterator.
    /// </remarks>
    public struct CirclePositionsEnumerator : IEnumerator<Point>, IEnumerable<Point>
    {
        private readonly Point _center;
        private int _radius;
        private int _xi;
        private int _yi;
        private int _err;
        private bool _terminate;

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

        object IEnumerator.Current => _current;

        /// <summary>
        /// Creates an enumerator which iterates over all positions on the outside of the given circle.
        /// </summary>
        /// <param name="center">Center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        public CirclePositionsEnumerator(Point center, int radius)
        {
            _center = center;
            _radius = radius;
            _xi = -radius;
            _yi = 0;
            _err = 2 - 2 * radius;
            _current = Point.None;
            _state = 0;
            _terminate = false;
        }

        /// <summary>
        /// Advances the iterator to the next position.
        /// </summary>
        /// <returns>True if the a new position on the outside of the circle exists; false otherwise.</returns>
        public bool MoveNext()
        {
            switch (_state)
            {
                case 0: /*   I. Quadrant */
                    if (_terminate)
                        return false;

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
                    _terminate = _xi >= 0;
                    return true;
            }

            // Unreachable
            return false;
        }

        /// <summary>
        /// Returns this enumerator.
        /// </summary>
        /// <returns>This enumerator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CirclePositionsEnumerator GetEnumerator() => this;

        /// <summary>
        /// Obsolete.
        /// </summary>
        /// <returns/>
        [Obsolete(
            "This method is obsolete; this structure itself implements IEnumerable directly and provides equivalent behavior, so you should no longer call this function.")]
        public IEnumerable<Point> ToEnumerable() => this;

        // Explicitly implemented to ensure we prefer the non-boxing versions where possible
        #region Explicit Interface Implementations
        /// <summary>
        /// This iterator does not support resetting.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        void IEnumerator.Reset() => throw new NotSupportedException();
        IEnumerator<Point> IEnumerable<Point>.GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        void IDisposable.Dispose()
        { }
        #endregion
    }

    /// <summary>
    /// A custom enumerator used to iterate over all positions on the outside of an ellipse efficiently.
    ///
    /// Generally, you should use <see cref="Shapes.GetEllipse"/> to get an instance of this, rather than creating one
    /// yourself.
    /// </summary>
    /// <remarks>
    /// This type is a struct, and as such is much more efficient when used in a foreach loop than a function returning
    /// IEnumerable&lt;Point&gt; by using "yield return".  This type does contain implement <see cref="IEnumerable{Point}"/>,
    /// so you can pass it to functions which require one (for example, System.LINQ).  However, this will have reduced
    /// performance due to boxing of the iterator.
    /// </remarks>
    public struct EllipsePositionsEnumerator : IEnumerator<Point>, IEnumerable<Point>
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

        private long _dx;
        private long _dy;
        private readonly int _a;
        private readonly int _b;
        private readonly int _b1;
        private int _x0;
        private int _y0;
        private int _x1;
        private int _y1;
        private int _state;
        private bool _terminate;
        private long _err;

        object IEnumerator.Current => _current;

        /// <summary>
        /// Creates an enumerator which iterates over all positions on the outside of the given ellipse.
        /// </summary>
        /// <param name="f1">The first focus point of the ellipse.</param>
        /// <param name="f2">The second focus point of the ellipse.</param>
        public EllipsePositionsEnumerator(Point f1, Point f2)
        {
            (_x0, _y0) = f1;
            (_x1, _y1) = f2;

            _current = Point.None;
            _state = 0;
            _terminate = false;

            _a = Math.Abs(_x1 - _x0);
            _b = Math.Abs(_y1 - _y0);
            _b1 = _b & 1; /* values of diameter */

            /* error increment */
            _dx = 4 * (1 - _a) * _b * _b;
            _dy = 4 * (_b1 + 1) * _a * _a;

            _err = _dx + _dy + _b1 * _a * _a; /* error of 1.step */

            if (_x0 > _x1) { _x0 = _x1; _x1 += _a; } /* if called with swapped points */
            if (_y0 > _y1)
                _y0 = _y1; /* .. exchange them */

            _y0 += (_b + 1) / 2; _y1 = _y0 - _b1;   /* starting pixel */
            _a *= 8 * _a; _b1 = 8 * _b * _b;
        }

        /// <summary>
        /// Advances the iterator to the next position.
        /// </summary>
        /// <returns>True if the a new position on the outside of the ellipse exists; false otherwise.</returns>
        public bool MoveNext()
        {
            switch (_state)
            {
                case 0: /*   I. Quadrant */
                    _current = new Point(_x1, _y0);
                    _state++;
                    return true;
                case 1: /*  II. Quadrant */
                    _current = new Point(_x0, _y0);
                    _state++;
                    return true;
                case 2: /* III. Quadrant */
                    _current = new Point(_x0, _y1);
                    _state++;
                    return true;
                case 3: /*  IV. Quadrant */
                    _current = new Point(_x1, _y1);
                    long e2 = 2 * _err;
                    if (e2 <= _dy) { _y0++; _y1--; _err += _dy += _a; }  /* y step */
                    if (e2 >= _dx || 2 * _err > _dy) { _x0++; _x1--; _err += _dx += _b1; } /* x step */

                    _state = _x0 <= _x1 ? 0 : 4;
                    if (_state == 4)
                        _terminate = _y0 - _y1 >= _b;
                    return true;
                case 4: /* cases 4-7, too early stop of flat ellipses a=1 */
                    if (_terminate)
                        return false;

                    _current = new Point(_x0 - 1, _y0);
                    _state++;
                    return true;
                case 5:
                    _current = new Point(_x1 + 1, _y0++);
                    _state++;
                    return true;
                case 6:
                    _current = new Point(_x0 - 1, _y1);
                    _state++;
                    return true;
                case 7:
                    _current = new Point(_x1 + 1, _y1--);
                    _state = 4;
                    _terminate = _y0 - _y1 >= _b;
                    return true;
            }

            // Unreachable
            return false;
        }

        /// <summary>
        /// Returns this enumerator.
        /// </summary>
        /// <returns>This enumerator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EllipsePositionsEnumerator GetEnumerator() => this;

        /// <summary>
        /// Obsolete.
        /// </summary>
        /// <returns/>
        [Obsolete(
            "This method is obsolete; this structure itself implements IEnumerable directly and provides equivalent behavior, so you should no longer call this function.")]
        public IEnumerable<Point> ToEnumerable() => this;

        // Explicitly implemented to ensure we prefer the non-boxing versions where possible
        #region Explicit Interface Implementations
        /// <summary>
        /// This iterator does not support resetting.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        void IEnumerator.Reset() => throw new NotSupportedException();
        IEnumerator<Point> IEnumerable<Point>.GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        void IDisposable.Dispose()
        { }
        #endregion
    }

    /// <summary>
    /// Provides implementations of various shape algorithms which are useful for for generating points representing
    /// shapes on a 2D integer grid.
    /// </summary>
    public static class Shapes
    {
        /// <summary>
        /// Gets the points on the outside of a circle.
        /// </summary>
        /// <remarks>
        /// This function returns a custom iterator which is very fast when used in a foreach loop.
        /// If you need an IEnumerable to use with LINQ or other code, the returned struct does implement that interface;
        /// however note that iterating over it this way will not perform as well as iterating directly over this object.
        /// </remarks>
        /// <param name="center">Center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CirclePositionsEnumerator GetCircle(Point center, int radius)
            => new CirclePositionsEnumerator(center, radius);

        /// <summary>
        /// Gets the points on the outside of an ellipse.
        /// </summary>
        /// <remarks>
        /// This function returns a custom iterator which is very fast when used in a foreach loop.
        /// If you need an IEnumerable to use with LINQ or other code, the returned struct does implement that interface;
        /// however note that iterating over it this way will not perform as well as iterating directly over this object.
        /// </remarks>
        /// <param name="f1">The first focus point of the ellipse.</param>
        /// <param name="f2">The second focus point of the ellipse.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EllipsePositionsEnumerator GetEllipse(Point f1, Point f2)
            => new EllipsePositionsEnumerator(f1, f2);
    }
}
