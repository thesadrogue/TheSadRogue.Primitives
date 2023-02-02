using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SadRogue.Primitives
{
    /// <summary>
    /// A custom enumerator used to iterate over all positions within a rectangle efficiently.
    /// </summary>
    /// <remarks>
    /// This type is a struct, and as such is much more efficient when used in a foreach loop than a function returning
    /// IEnumerable&lt;Point&gt; by using "yield return".  This type does contain implement <see cref="IEnumerable{Point}"/>,
    /// so you can pass it to functions which require one (for example, System.LINQ).  However, this will have reduced
    /// performance due to boxing of the iterator.
    /// </remarks>
    public struct RectanglePositionsEnumerator : IEnumerator<Point>, IEnumerable<Point>
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

        object IEnumerator.Current => _current;

        /// <summary>
        /// Creates an enumerator which iterates over all positions in the given rectangle.
        /// </summary>
        /// <param name="positions">A rectangle containing the positions to iterate over.</param>
        public RectanglePositionsEnumerator(Rectangle positions)
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
        public RectanglePositionsEnumerator GetEnumerator() => this;

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
}
