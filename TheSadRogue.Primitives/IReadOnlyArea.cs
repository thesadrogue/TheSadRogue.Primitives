using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SadRogue.Primitives
{
    public struct ReadOnlyAreaPostionsEnumerable
    {
        private readonly IReadOnlyArea _area;

        private readonly int _count;
        private int _currentIdx;
        public Point Current => _area[_currentIdx];

        public ReadOnlyAreaPostionsEnumerable(IReadOnlyArea area)
        {
            _area = area;
            _currentIdx = -1;
            _count = area.Count;
        }

        public bool MoveNext()
        {
            _currentIdx++;
            return _currentIdx < _count;
        }

        /// <summary>
        /// Returns this enumerator.
        /// </summary>
        /// <returns>This enumerator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyAreaPostionsEnumerable GetEnumerator() => this;

        public IEnumerable<Point> ToEnumerable()
        {
            int count = _area.Count;
            for (int i = 0; i < count; i++)
                yield return _area[i];
        }
    }
    /// <summary>
    /// Read-only interface for an arbitrary 2D area.
    /// </summary>
    public interface IReadOnlyArea : IMatchable<IReadOnlyArea>, IEnumerable<Point>
    {
        /// <summary>
        /// Smallest possible rectangle that encompasses every position in the area.
        /// </summary>
        Rectangle Bounds { get; }

        /// <summary>
        /// Number of (unique) positions in the current area.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns positions from the area in the same fashion you would via a list.
        /// </summary>
        /// <param name="index">Index of list to retrieve.</param>
        Point this[int index] { get; }

        /// <summary>
        /// Returns whether or not the given area is completely contained within the current one.
        /// </summary>
        /// <param name="area">Area to check.</param>
        /// <returns>
        /// True if the given area is completely contained within the current one, false otherwise.
        /// </returns>
        bool Contains(IReadOnlyArea area);

        /// <summary>
        /// Determines whether or not the given position is considered within the area or not.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>True if the specified position is within the area, false otherwise.</returns>
        bool Contains(Point position);

        /// <summary>
        /// Determines whether or not the given position is considered within the area or not.
        /// </summary>
        /// <param name="positionX">X-value of the position to check.</param>
        /// <param name="positionY">X-value of the position to check.</param>
        /// <returns>True if the specified position is within the area, false otherwise.</returns>
        bool Contains(int positionX, int positionY);

        /// <summary>
        /// Returns whether or not the given map area intersects the current one. If you intend to
        /// determine/use the exact intersection based on this return value, it is best to instead
        /// call <see cref="Area.GetIntersection(IReadOnlyArea, IReadOnlyArea)"/>, and check the number
        /// of positions in the result (0 if no intersection).
        /// </summary>
        /// <param name="area">The area to check.</param>
        /// <returns>True if the given area intersects the current one, false otherwise.</returns>
        bool Intersects(IReadOnlyArea area);
    }
}
