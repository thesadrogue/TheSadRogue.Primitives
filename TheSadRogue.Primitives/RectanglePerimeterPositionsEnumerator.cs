using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SadRogue.Primitives
{
    public struct RectanglePerimeterPositionsEnumerator : IEnumerator<Point>, IEnumerable<Point>
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

        object IEnumerator.Current => _current;

        private readonly Point _minExtent;
        private readonly Point _maxExtent;
        private int _changingValue;
        private int _state;

        /// <summary>
        /// Creates an enumerator which iterates over all positions on the outside edges of the given rectangle.
        /// </summary>
        /// <param name="rectangle">A rectangle defining the area to iterate over perimeter for.</param>
        public RectanglePerimeterPositionsEnumerator(Rectangle rectangle)
        {
            _minExtent = rectangle.MinExtent;
            _maxExtent = rectangle.MaxExtent;
            _current = Point.None;
            _state = 0;
            _changingValue = _minExtent.X;
        }

        /// <summary>
        /// Advances the iterator to the next position.
        /// </summary>
        /// <returns>True if the a new position on the outside of the rectangle was found; false otherwise.</returns>
        public bool MoveNext()
        {
            // for (int x = MinExtentX; x <= MaxExtentX; x++)
            //     yield return new Point(x, MinExtentY); // Minimum y-side perimeter
            //
            // // Start offset 1, since last loop returned the corner piece
            // for (int y = MinExtentY + 1; y <= MaxExtentY; y++)
            //     yield return new Point(MaxExtentX, y);
            //
            // // Again skip 1 because last loop returned the corner piece
            // for (int x = MaxExtentX - 1; x >= MinExtentX; x--)
            //     yield return new Point(x, MaxExtentY);
            //
            // // Skip 1 on both ends, because last loop returned one corner, first loop returned the other
            // for (int y = MaxExtentY - 1; y >= MinExtentY + 1; y--)
            //     yield return new Point(MinExtentX, y);

            // TODO: Eliminate property accesses?
            switch (_state)
            {
                case 0:
                    _current = new Point(_changingValue, _minExtent.Y);
                    _changingValue++;
                    if (_changingValue > _maxExtent.X)
                    {
                        _state++;
                        // Start offset 1, since last loop returned the corner piece
                        _changingValue = _minExtent.Y + 1;
                    }
                    return true;
                case 1:
                    _current = new Point(_maxExtent.X,_changingValue);
                    _changingValue++;
                    if (_changingValue > _maxExtent.Y)
                    {
                        _state++;
                        // Again skip 1 because last loop returned the corner piece
                        _changingValue = _maxExtent.X - 1;
                    }
                    return true;
                case 2:
                    _current = new Point(_changingValue, _maxExtent.Y);
                    _changingValue--;
                    if (_changingValue < _minExtent.X)
                    {
                        _state++;
                        // Skip 1 on both ends, because last loop returned one corner, first loop returned the other
                        _changingValue = _maxExtent.Y - 1;
                    }
                    return true;
                case 3:
                    _current = new Point(_minExtent.X, _changingValue);
                    _changingValue--;
                    if (_changingValue <= _minExtent.Y)
                        _state++;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns this enumerator.
        /// </summary>
        /// <returns>This enumerator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectanglePerimeterPositionsEnumerator GetEnumerator() => this;

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
