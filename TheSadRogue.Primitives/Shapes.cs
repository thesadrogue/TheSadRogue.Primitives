using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SadRogue.Primitives
{
    /// <summary>
    /// A custom enumerator used to iterate over all positions on the outside of a circle with a foreach loop efficiently.
    /// Generally, you should use <see cref="Shapes.GetCircle"/> to get an instance of this.
    /// </summary>
    /// <remarks>
    /// This type is a struct, and as such is much more efficient than using the otherwise equivalent type of
    /// IEnumerable&lt;Point&gt; with "yield return".  The type does contain a function <see cref="ToEnumerable"/> which
    /// creates an IEnumerable&lt;Point&gt;, which can be convenient for allowing the returned positions to be used with
    /// LINQ; however using this function is not recommended in situations where runtime performance is a primary
    /// concern.
    /// </remarks>
    public struct CirclePositionsEnumerable
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

        /// <summary>
        /// Creates an enumerator which iterates over all positions on the outside of the given circle.
        /// </summary>
        /// <param name="center">Center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        public CirclePositionsEnumerable(Point center, int radius)
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

            return false;
        }

        /// <summary>
        /// Returns this enumerator.
        /// </summary>
        /// <returns>This enumerator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CirclePositionsEnumerable GetEnumerator() => this;

        /// <summary>
        /// Converts the result of the enumerable to a <see cref="IEnumerable{T}"/>, which can be useful if you need
        /// to use the result with LINQ.
        /// </summary>
        /// <remarks>
        /// Note that this function advances the state of the enumerator, evaluating it to its fullest extent.  Also
        /// note that it is NOT recommended to use this function in cases where performance is critical.
        /// </remarks>
        /// <returns>
        /// An IEnumerable&lt;Point&gt; which iterates over all positions on the outside of the circle specified to this
        /// enumerator.
        /// </returns>
        public IEnumerable<Point> ToEnumerable()
        {
            // This is a re-implementation of the circle algorithm implemented in MoveNext above, and is equivalent to:
            // foreach (var pos in this)
            //    yield return pos
            //
            // However, this is slightly faster.
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

    /// <summary>
    /// Provides implementations of various line-drawing algorithms which are useful for
    /// for generating lines on a 2D integer grid.
    /// </summary>
    public static class Shapes
    {
        /// <summary>
        /// Gets the points on the outside of a circle.
        /// </summary>
        /// <param name="center">Center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CirclePositionsEnumerable GetCircle(Point center, int radius)
            => new CirclePositionsEnumerable(center, radius);

        // TODO: Ellipse
    }
}
