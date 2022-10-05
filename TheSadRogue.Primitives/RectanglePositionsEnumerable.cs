using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SadRogue.Primitives
{
    /// <summary>
    /// A custom enumerator used to iterate over all positions within a rectangle with a foreach loop efficiently.
    /// </summary>
    /// <remarks>
    /// This type is a struct, and as such is much more efficient than using the otherwise equivalent type of
    /// IEnumerable&lt;Point&gt; with "yield return".  The type does contain a function <see cref="ToEnumerable"/> which
    /// creates an IEnumerable&lt;Point&gt;, which can be convenient for allowing the returned positions to be used with
    /// LINQ; however using this function is not recommended in situations where runtime performance is a primary
    /// concern.
    /// </remarks>
    public struct RectanglePositionsEnumerable
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

        private readonly Rectangle _positions;

        /// <summary>
        /// Creates an enumerator which iterates over all positions in the given rectangle.
        /// </summary>
        /// <param name="positions">A rectangle containing the positions to iterate over.</param>
        public RectanglePositionsEnumerable(Rectangle positions)
        {
            _positions = positions;

            _current = (_positions.Width * _positions.Height > 0) ? positions.MinExtent - new Point(1, 0) : _positions.MaxExtent;
        }

        /// <summary>
        /// Advances the iterator to the next position.
        /// </summary>
        /// <returns>True if the a position within the rectangle was found; false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (_current.X + 1 <= _positions.MaxExtent.X)
            {
                _current = new Point(_current.X + 1, _current.Y);
                return true;
            }
            else if (_current.Y + 1 <= _positions.MaxExtent.Y)
            {
                _current = new Point(_positions.MinExtent.X, _current.Y + 1);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns this enumerator.
        /// </summary>
        /// <returns>This enumerator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectanglePositionsEnumerable GetEnumerator() => this;

        /// <summary>
        /// Converts the result of the enumerable to a <see cref="IEnumerable{T}"/>, which can be useful if you need
        /// to use the result with LINQ.
        /// </summary>
        /// <remarks>
        /// Note that this function advances the state of the enumerator, evaluating it to its fullest extent.  Also
        /// note that it is NOT recommended to use this function in cases where performance is critical.
        /// </remarks>
        /// <returns>
        /// An IEnumerable&lt;Point&gt; which iterates over all positions within the rectangle specified to this
        /// enumerator.
        /// </returns>
        public IEnumerable<Point> ToEnumerable()
        {
            // This is equivalent of:
            // foreach (var pos in this)
            //    yield return pos
            //
            // However, this is slightly faster.
            var maxExtent = _positions.MaxExtent;

            for (int y = _positions.MinExtentY; y <= maxExtent.Y; y++)
                for (int x = _positions.MinExtentX; x <= maxExtent.X; x++)
                    yield return new Point(x, y);
        }
    }
}
