using System.Collections.Generic;
using JetBrains.Annotations;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Read-only interface for an arbitrary 2D area.
    /// </summary>
    [PublicAPI]
    public interface IReadOnlyArea : IMatchable<IReadOnlyArea>, IEnumerable<Point>
    {
        /// <summary>
        /// Whether or not it is more efficient for this implementation to use enumeration by index,
        /// rather than generic IEnumerable, when iterating over positions using <see cref="ReadOnlyAreaPositionsEnumerator"/>.
        /// </summary>
        /// <remarks>
        /// Set this to true if your indexer implementation scales well (constant time), and is relatively fast.  Implementations with
        /// more complex indexers should set this to false.
        ///
        /// The default interface implementation returns false, in order to preserve backwards compatibility with previous versions.
        ///
        /// If you set this to false, your IEnumerable.GetEnumerator() implementations must NOT call return a ReadOnlyAreaPositionsEnumerator,
        /// as this will create an infinite loop.
        /// </remarks>
        public bool UseIndexEnumeration => false;
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
        /// call <see cref="Area.GetIntersection"/>, and check the number
        /// of positions in the result (0 if no intersection).
        /// </summary>
        /// <param name="area">The area to check.</param>
        /// <returns>True if the given area intersects the current one, false otherwise.</returns>
        bool Intersects(IReadOnlyArea area);

        /// <summary>
        /// Returns an enumerator which can be used to iterate over the positions in this area in the most efficient
        /// manner possible via a generic interface.
        /// </summary>
        /// <remarks>
        /// The enumerator returned will use the area's indexer to iterate over the positions (like you might a list),
        /// if the area's <see cref="IReadOnlyArea.UseIndexEnumeration"/> is true.  Otherwise, it uses the typical IEnumerator
        /// implementation for that area.
        ///
        /// This may be significantly faster than the typical IEnumerable/IEnumerator usage for implementations which have
        /// <see cref="IReadOnlyArea.UseIndexEnumeration"/> set to true; however it won't have much benefit otherwise.
        ///
        /// If you have a value of a concrete type rather than an interface, and the GetEnumerator implementation for that
        /// given type is particularly fast or a non-boxed type (like <see cref="Area"/>, you will probably get faster performance
        /// out of that than by using this; however this will provide better performance if you are working with an interface
        /// and thus don't know the type of area.  Use cases for this function are generally for iteration via IReadOnlyArea.
        ///
        /// </remarks>
        /// <returns>A custom enumerator that iterates over the positions in the area in the most efficient manner possible via a generic interface.</returns>
        new ReadOnlyAreaPositionsEnumerator GetEnumerator() => new ReadOnlyAreaPositionsEnumerator(this);
    }
}
