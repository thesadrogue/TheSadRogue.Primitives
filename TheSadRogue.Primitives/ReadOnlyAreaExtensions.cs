using System;
using System.Collections.Generic;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Extension methods for IReadOnlyArea.
    /// </summary>
    public static class ReadOnlyAreaExtensions
    {
        /// <summary>
        /// Obsolete.
        /// </summary>
        /// <returns/>
        [Obsolete(
            "This method is obsolete; IReadOnlyArea implements GetEnumerator and provides equivalent behavior, so you should no longer call this function; instead just use your area in a foreach loop directly.")]
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
            foreach (var pos in area)
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
