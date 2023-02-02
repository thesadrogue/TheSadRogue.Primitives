using System.Collections.Generic;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Extension methods for IReadOnlyArea.
    /// </summary>
    public static class ReadOnlyAreaExtensions
    {
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
        /// <param name="self"/>
        /// <returns>A custom enumerator that iterates over the positions in the area in the most efficient manner possible via a generic interface.</returns>
        public static ReadOnlyAreaPositionsEnumerator FastEnumerator(this IReadOnlyArea self)
            => new ReadOnlyAreaPositionsEnumerator(self);

        /// <summary>
        /// Returns all points that are on the border of the area, assuming the specified adjacency rule is used to determine adjacent cells
        /// for the sake of determining border.
        /// </summary>
        /// <remarks>
        /// Typically, you will want to use AdjacencyRule.EightWay as the rule; however AdjacencyRule.Cardinals is faster if you don't want
        /// border cells adjacent to a wall ONLY diagonally to be considered border cells.
        ///
        /// <example>
        /// Using AdjacencyRule.Cardinals, if "." and "x" are cells within the area and "#" are cells that are not within, X will NOT be considered
        /// a border:
        /// <code>
        /// # # # # # # # #
        /// # . . . . . # #
        /// # . . . . X . #
        /// # . . . . . . #
        /// # . . . . . . #
        /// # . . . . . . #
        /// # . . . . . . #
        /// # # # # # # # #
        /// </code>
        /// </example>
        ///
        /// <example>
        /// Using AdjacencyRule.EightWay, if "." and "x" are cells within the area and "#" are cells that are not within, X WILL be considered
        /// a border:
        /// <code>
        /// # # # # # # # #
        /// # . . . . . # #
        /// # . . . . X . #
        /// # . . . . . . #
        /// # . . . . . . #
        /// # . . . . . . #
        /// # . . . . . . #
        /// # # # # # # # #
        /// </code>
        /// </example>
        /// </remarks>
        ///
        /// <param name="area"/>
        /// <param name="rule">The AdjacencyRule to use for determining adjacency to cells which are outside of the area.</param>
        /// <returns>An enumerable of every point which is on the outer edge of the area specified.</returns>
        public static IEnumerable<Point> PerimeterPositions(this IReadOnlyArea area, AdjacencyRule rule)
        {
            foreach (var pos in area.FastEnumerator())
            {
                foreach (var dir in rule.DirectionsOfNeighborsCache)
                {
                    var neighbor = pos + dir;
                    if (!area.Contains(neighbor))
                    {
                        yield return pos;
                        break;
                    }
                }
            }
        }
    }
}
