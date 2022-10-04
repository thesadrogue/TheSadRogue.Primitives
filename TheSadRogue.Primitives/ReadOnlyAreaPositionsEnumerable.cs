using System.Runtime.CompilerServices;

namespace SadRogue.Primitives
{
    /// <summary>
    /// A custom enumerator used to iterate over all positions within an area with a foreach loop efficiently.
    /// </summary>
    /// <remarks>
    /// This type is a struct, and as such is much more efficient than using the otherwise equivalent type of
    /// IEnumerable&lt;Point&gt; with "yield return".
    /// </remarks>
    public struct ReadOnlyAreaPositionsEnumerable
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
        public ReadOnlyAreaPositionsEnumerable(IReadOnlyArea area)
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
        public ReadOnlyAreaPositionsEnumerable GetEnumerator() => this;
    }
}
