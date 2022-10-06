using System.Collections.Generic;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Extension methods for IReadOnlyArea.
    /// </summary>
    public static class ReadOnlyAreaExtensions
    {
        /// <summary>
        /// Returns an enumerator which can be used to iterate over the positions in this area with a foreach loop
        /// in the most efficient manner possible.
        /// </summary>
        /// <remarks>
        /// The enumerator returned will use the area's indexer to iterate over the positions (like you might a list),
        /// if the area's <see cref="IReadOnlyArea.UseIndexEnumeration"/> is true.  Otherwise, it uses the typical IEnumerator
        /// implementation for that area.
        ///
        /// This may be significantly faster than the typical IEnumerable/IEnumerator usage for implementations which have
        /// <see cref="IReadOnlyArea.UseIndexEnumeration"/> set to true; however it won't have much benefit otherwise.
        /// </remarks>
        /// <param name="self"/>
        /// <returns>A custom enumerator that iterates over the positions in the area in the most efficient manner possible.</returns>
        public static ReadOnlyAreaPositionsEnumerable FastEnumerator(this IReadOnlyArea self)
            => new ReadOnlyAreaPositionsEnumerable(self);

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
        /// <param name="area"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
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
