using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SadRogue.Primitives
{
    /// <summary>
    /// A custom enumerator used to iterate over all positions within an area with a foreach loop efficiently.
    /// </summary>
    /// <remarks>
    /// This type is a struct, and will either use an indexer-based enumeration method, or a standard IEnumerator, depending on
    /// the area's <see cref="IReadOnlyArea.UseIndexEnumeration"/> value.  Therefore, it will provide the quickest way of iterating
    /// over positions in an area with a for-each loop.
    ///
    /// If you have a value of a concrete type rather than an interface, and the GetEnumerator implementation for that
    /// given type is particularly fast or a non-boxed type (like <see cref="Area"/>, you will probably get faster performance
    /// out of that than by using this; however this will provide better performance if you are working with an interface
    /// and thus don't know the type of area.  Use cases for this class are generally for iteration via IReadOnlyArea.
    /// </remarks>
    public struct ReadOnlyAreaPositionsEnumerator : IEnumerator<Point>, IEnumerable<Point>
    {
        private readonly IReadOnlyArea _area;
        private readonly bool _useIndexEnumeration;

        private readonly IEnumerator<Point>? _enumerator;

        private readonly int _count;
        private int _currentIdx;

        /// <summary>
        /// The current value for enumeration.
        /// </summary>
        public Point Current => _useIndexEnumeration ? _area[_currentIdx] : _enumerator!.Current;

        object IEnumerator.Current => _useIndexEnumeration ? _area[_currentIdx] : _enumerator!.Current;

        /// <summary>
        /// Creates an enumerator which iterates over all positions in the given area.
        /// </summary>
        /// <param name="area">A read-only area containing the positions to iterate over.</param>
        public ReadOnlyAreaPositionsEnumerator(IReadOnlyArea area)
        {
            _area = area;
            _useIndexEnumeration = _area.UseIndexEnumeration;

            if (_useIndexEnumeration)
            {
                _currentIdx = -1;
                _count = _area.Count;
                _enumerator = null;
            }
            else
            {
                _currentIdx = _count = 0;
                _enumerator = ((IEnumerable<Point>)_area).GetEnumerator();
            }
        }

        /// <summary>
        /// Advances the iterator to the next position.
        /// </summary>
        /// <returns>True if the a position within the area was found; false otherwise.</returns>
        public bool MoveNext()
        {
            if (_useIndexEnumeration)
            {
                _currentIdx++;
                return _currentIdx < _count;
            }
            else
                return _enumerator!.MoveNext();
        }

        /// <summary>
        /// Returns this enumerator.
        /// </summary>
        /// <returns>This enumerator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyAreaPositionsEnumerator GetEnumerator() => this;

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
