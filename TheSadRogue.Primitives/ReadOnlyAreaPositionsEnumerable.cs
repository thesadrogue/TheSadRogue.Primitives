using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SadRogue.Primitives
{
    /// <summary>
    /// A custom enumerator used to iterate over all positions within an area with a foreach loop efficiently.
    /// </summary>
    /// <remarks>
    /// This type is a struct, and as such is much more efficient than using the otherwise equivalent type of
    /// IEnumerable&lt;Point&gt; with "yield return".  The type does contain a function <see cref="ToEnumerable"/> which
    /// creates an IEnumerable&lt;Point&gt;, which can be convenient for allowing the returned positions to be used with
    /// LINQ; however using this function is not recommended in situations where runtime performance is a primary
    /// concern.
    /// </remarks>
    public struct ReadOnlyAreaPostionsEnumerable
    {
        private readonly IReadOnlyArea _area;

        private readonly int _count;
        private int _currentIdx;

        /// <summary>
        /// The current value for enumeration.
        /// </summary>
        public Point Current => _area[_currentIdx];

        /// <summary>
        /// Creates an enumerator which iterates over all positions in the given area.
        /// </summary>
        /// <param name="area">A read-only area containing the positions to iterate over.</param>
        public ReadOnlyAreaPostionsEnumerable(IReadOnlyArea area)
        {
            _area = area;
            _currentIdx = -1;
            _count = area.Count;
        }

        /// <summary>
        /// Advances the iterator to the next position.
        /// </summary>
        /// <returns>True if the a position within the area was found; false otherwise.</returns>
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

        /// <summary>
        /// Converts the result of the enumerable to a <see cref="IEnumerable{T}"/>, which can be useful if you need
        /// to use the result with LINQ.
        /// </summary>
        /// <remarks>
        /// Note that this function advances the state of the enumerator, evaluating it to its fullest extent.  Also
        /// note that it is NOT recommended to use this function in cases where performance is critical.
        /// </remarks>
        /// <returns>
        /// An IEnumerable&lt;Point&gt; which iterates over all positions within the area specified to this
        /// enumerator.
        /// </returns>
        public IEnumerable<Point> ToEnumerable()
        {
            int count = _area.Count;
            for (int i = 0; i < count; i++)
                yield return _area[i];
        }
    }
}
