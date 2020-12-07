using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Represents an arbitrarily-shaped 2D area. Stores and provides access to a list of each
    /// unique position in the area.
    /// </summary>
    [DataContract]
    public class Area : IReadOnlyArea, IEnumerable<Point>
    {
        private readonly HashSet<Point> _positionsSet;

        [DataMember] private readonly List<Point> _positions;

        private int _left, _top, _bottom, _right;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Area()
        {
            _left = int.MaxValue;
            _top = int.MaxValue;

            _right = 0;
            _bottom = 0;

            _positions = new List<Point>();
            _positionsSet = new HashSet<Point>();
        }

        /// <summary>
        /// Constructor that takes initial points to add to the area.
        /// </summary>
        /// <param name="initialPoints">Initial points to add to the area.</param>
        public Area(IEnumerable<Point> initialPoints)
            : this()
            => Add(initialPoints);

        /// <summary>
        /// Constructor that takes initial points to add to the area.
        /// </summary>
        /// <param name="initialPoints">Initial points to add to the area.</param>
        public Area(params Point[] initialPoints)
            : this((IEnumerable<Point>)initialPoints)
        { }

        /// <summary>
        /// Smallest possible rectangle that encompasses every position in the area.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                if (_right < _left)
                    return Rectangle.Empty;

                return new Rectangle(_left, _top, _right - _left + 1, _bottom - _top + 1);
            }
        }

        /// <summary>
        /// Number of (unique) positions in the area.
        /// </summary>
        public int Count => _positions.Count;

        /// <summary>
        /// List of all (unique) positions in the area.
        /// </summary>
        public IReadOnlyList<Point> Positions => _positions.AsReadOnly();

        /// <summary>
        /// Gets an area containing all positions in <paramref name="area1"/>, minus those that are in
        /// <paramref name="area2"/>.
        /// </summary>
        /// <param name="area1"/>
        /// <param name="area2"/>
        /// <returns>A area with exactly those positions in <paramref name="area1"/> that are NOT in
        /// <paramref name="area2"/>.</returns>
        public static Area GetDifference(IReadOnlyArea area1, IReadOnlyArea area2)
        {
            Area retVal = new Area();

            foreach (Point pos in area1.Positions)
            {
                if (area2.Contains(pos))
                    continue;

                retVal.Add(pos);
            }

            return retVal;
        }

        /// <summary>
        /// Gets an area containing exactly those positions contained in both of the given areas.
        /// </summary>
        /// <param name="area1"/>
        /// <param name="area2"/>
        /// <returns>An area containing exactly those positions contained in both of the given areas.</returns>
        public static Area GetIntersection(IReadOnlyArea area1, IReadOnlyArea area2)
        {
            Area retVal = new Area();

            if (!area1.Bounds.Intersects(area2.Bounds))
                return retVal;

            if (area1.Count > area2.Count)
                Swap(ref area1, ref area2);

            foreach (Point pos in area1.Positions)
                if (area2.Contains(pos))
                    retVal.Add(pos);

            return retVal;
        }

        /// <summary>
        /// Gets an area containing every position in one or both given areas.
        /// </summary>
        /// <param name="area1"/>
        /// <param name="area2"/>
        /// <returns>An area containing only those positions in one or both of the given areas.</returns>
        public static Area GetUnion(IReadOnlyArea area1, IReadOnlyArea area2)
        {
            Area retVal = new Area();

            retVal.Add(area1);
            retVal.Add(area2);

            return retVal;
        }

        /// <summary>
        /// Creates an area with the positions all shifted by the given vector.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs">Vector) to add to each position in <paramref name="lhs"/>.</param>
        /// <returns>
        /// An area with the positions all translated by the given amount in x and y directions.
        /// </returns>
        public static Area operator +(Area lhs, Point rhs)
        {
            Area retVal = new Area();

            foreach (Point pos in lhs.Positions)
                retVal.Add(pos + rhs);

            return retVal;
        }

        /// <summary>
        /// Compares for equality. Returns true if the two areas contain exactly the same points.
        /// </summary>
        /// <param name="other"/>
        /// <returns>True if the areas contain exactly the same points, false otherwise.</returns>
        public bool Matches(IReadOnlyArea? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            // Quick checks that can short-circuit a function that would otherwise require looping over all points
            if (Count != other.Count)
                return false;

            if (Bounds != other.Bounds)
                return false;

            foreach (Point pos in Positions)
                if (!other.Contains(pos))
                    return false;

            return true;
        }

        /// <summary>
        /// Adds the given position to the list of points within the area if it is not already in the
        /// list, or does nothing otherwise.
        /// </summary>
        /// <remarks>
        /// Because the class uses a hash set internally to determine what points have already been added,
        /// this is an average case O(1) operation.
        /// </remarks>
        /// <param name="position">The position to add.</param>
        public void Add(Point position)
        {
            if (!_positionsSet.Add(position))
                return;

            _positions.Add(position);

            // Update bounds
            if (position.X > _right)
                _right = position.X;

            if (position.X < _left)
                _left = position.X;

            if (position.Y > _bottom)
                _bottom = position.Y;

            if (position.Y < _top)
                _top = position.Y;
        }

        /// <summary>
        /// Adds the given position to the list of points within the area if it is not already in the
        /// list, or does nothing otherwise.
        /// </summary>
        /// <remarks>
        /// Because the class uses a hash set internally to determine what points have already been added,
        /// this is an average case O(1) operation.
        /// </remarks>
        /// <param name="positionX">X-value of the position to add.</param>
        /// <param name="positionY">Y-value of the position to add.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int positionX, int positionY) => Add(new Point(positionX, positionY));

        /// <summary>
        /// Adds the given positions to the list of points within the area if they are not already in
        /// the list.
        /// </summary>
        /// <param name="positions">Positions to add to the list.</param>
        public void Add(IEnumerable<Point> positions)
        {
            foreach (Point pos in positions)
                Add(pos);
        }

        /// <summary>
        /// Adds all positions in the given rectangle to the area, if they are not already present.
        /// </summary>
        /// <param name="rectangle">Rectangle indicating which points to add.</param>
        public void Add(Rectangle rectangle)
        {
            foreach (Point pos in rectangle.Positions())
                Add(pos);
        }

        /// <summary>
        /// Adds all positions in the given map area to this one.
        /// </summary>
        /// <param name="area">Area containing positions to add.</param>
        public void Add(IReadOnlyArea area)
        {
            foreach (Point pos in area.Positions)
                Add(pos);
        }

        /// <summary>
        /// Determines whether or not the given position is within the area or not.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>True if the specified position is within the area, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Point position) => _positionsSet.Contains(position);

        /// <summary>
        /// Determines whether or not the given position is within the area or not.
        /// </summary>
        /// <param name="positionX">X-value of the position to check.</param>
        /// <param name="positionY">Y-value of the position to check.</param>
        /// <returns>True if the specified position is within the area, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(int positionX, int positionY) => Contains(new Point(positionX, positionY));

        /// <summary>
        /// Returns whether or not the given area is completely contained within the current one.
        /// </summary>
        /// <param name="area">Area to check.</param>
        /// <returns>
        /// True if the given area is completely contained within the current one, false otherwise.
        /// </returns>
        public bool Contains(IReadOnlyArea area)
        {
            if (!Bounds.Contains(area.Bounds))
                return false;

            foreach (Point pos in area.Positions)
                if (!Contains(pos))
                    return false;

            return true;
        }

        /// <summary>
        /// Returns whether or not the given map area intersects the current one. If you intend to
        /// determine/use the exact intersection based on this return value, it is best to instead
        /// call the <see cref="Area.GetIntersection(IReadOnlyArea, IReadOnlyArea)"/>, and
        /// check the number of positions in the result (0 if no intersection).
        /// </summary>
        /// <param name="area">The area to check.</param>
        /// <returns>True if the given map area intersects the current one, false otherwise.</returns>
        public bool Intersects(IReadOnlyArea area)
        {
            if (!area.Bounds.Intersects(Bounds))
                return false;

            if (Count <= area.Count)
            {
                foreach (Point pos in Positions)
                    if (area.Contains(pos))
                        return true;

                return false;
            }

            foreach (Point pos in area.Positions)
                if (Contains(pos))
                    return true;

            return false;
        }

        /// <summary>
        /// Removes the given position specified from the area. Particularly when the remove operation
        /// operation changes the bounds, this operation can be expensive, so if you must do multiple
        /// remove operations, it would be best to group them into 1 using <see cref="Remove(IEnumerable{Point})"/>.
        /// </summary>
        /// <param name="position">The position to remove.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(Point position) => Remove(YieldPoint(position));

        /// <summary>
        /// Removes the given position specified from the area. Particularly when the remove operation
        /// operation changes the bounds, this operation can be expensive, so if you must do multiple
        /// remove operations, it would be best to group them into 1 using <see cref="Remove(IEnumerable{Point})"/>.
        /// </summary>
        /// <param name="positionX">X-value of the position to remove.</param>
        /// <param name="positionY">Y-value of the position to remove.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(int positionX, int positionY) => Remove(YieldPoint(new Point(positionX, positionY)));

        /// <summary>
        /// Removes positions for which the given predicate returns true from the area.
        /// </summary>
        /// <param name="predicate">Predicate returning true for positions that should be removed.</param>
        public void Remove(Func<Point, bool> predicate)
        {
            bool recalculateBounds = false;

            foreach (Point pos in _positions.Where(predicate))
                if (_positionsSet.Remove(pos))
                    if (pos.X == _left || pos.X == _right || pos.Y == _top || pos.Y == _bottom)
                        recalculateBounds = true;

            _positions.RemoveAll(c => predicate(c));


            if (recalculateBounds)
                RecalculateBounds();
        }

        /// <summary>
        /// Removes the given positions from the specified area.
        /// </summary>
        /// <param name="positions">Positions to remove.</param>
        public void Remove(HashSet<Point> positions)
        {
            bool recalculateBounds = false;
            foreach (Point pos in positions)
                if (_positionsSet.Remove(pos))
                    if (pos.X == _left || pos.X == _right || pos.Y == _top || pos.Y == _bottom)
                        recalculateBounds = true;

            _positions.RemoveAll(positions.Contains);

            if (recalculateBounds)
                RecalculateBounds();
        }

        /// <summary>
        /// Removes the given positions from the specified area.
        /// </summary>
        /// <param name="positions">Positions to remove.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(IEnumerable<Point> positions)
        {
            if (positions is HashSet<Point> set)
                Remove(set);
            else
                Remove(new HashSet<Point>(positions));
        }

        /// <summary>
        /// Removes all positions in the given map area from this one.
        /// </summary>
        /// <param name="area">Area containing positions to remove.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(IReadOnlyArea area) => Remove(area.Positions);

        /// <summary>
        /// Removes all positions in the given rectangle from this area.
        /// </summary>
        /// <param name="rectangle">Rectangle containing positions to remove.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(Rectangle rectangle) => Remove(rectangle.Positions());

        /// <summary>
        /// Returns the string of each position in the area, in a square-bracket enclosed list,
        /// eg. [(1, 2), (3, 4), (5, 6)].
        /// </summary>
        /// <returns>A string representation of those coordinates in the area.</returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder("[");
            bool first = true;
            foreach (Point item in _positions)
            {
                if (first)
                    first = false;
                else
                    result.Append(", ");

                result.Append(item.ToString());
            }

            result.Append("]");

            return result.ToString();
        }

        /// <summary>
        /// Returns an enumerator that iterates through all positions in the Area, in the order they were added.
        /// </summary>
        /// <returns>An enumerator that iterates through all the positions in the Area, in the order they were added.</returns>
        public IEnumerator<Point> GetEnumerator() => _positions.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through all positions in the Area, in the order they were added.
        /// </summary>
        /// <returns>An enumerator that iterates through all the positions in the Area, in the order they were added.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_positions).GetEnumerator();

        private void RecalculateBounds()
        {
            int leftLocal = int.MaxValue, topLocal = int.MaxValue;
            int rightLocal = int.MinValue, bottomLocal = int.MinValue;

            // Find new bounds
            foreach ((int x, int y) in _positions)
            {
                if (x > rightLocal)
                    rightLocal = x;

                if (x < leftLocal)
                    leftLocal = x;

                if (y > bottomLocal)
                    bottomLocal = y;

                if (y < topLocal)
                    topLocal = y;
            }

            _left = leftLocal;
            _right = rightLocal;
            _top = topLocal;
            _bottom = bottomLocal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap(ref IReadOnlyArea lhs, ref IReadOnlyArea rhs)
        {
            IReadOnlyArea temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        private static IEnumerable<Point> YieldPoint(Point item)
        {
            yield return item;
        }
    }
}
