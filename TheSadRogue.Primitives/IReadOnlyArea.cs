using System.Collections.Generic;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Read-only interface for an arbitrary 2D area.
    /// </summary>
    public interface IReadOnlyArea : IMatchable<IReadOnlyArea>
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
        /// List of all (unique) positions in the current area.
        /// </summary>
        IReadOnlyList<Point> Positions { get; }

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
