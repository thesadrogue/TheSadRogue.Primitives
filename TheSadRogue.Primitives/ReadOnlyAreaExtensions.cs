using System.Collections.Generic;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Extension methods for IReadOnlyArea.
    /// </summary>
    public static class ReadOnlyAreaExtensions
    {
        /// <summary>
        /// Yields the area's positions as an <see cref="IEnumerable{T}"/>, which can be useful if you need
        /// to use the result with LINQ.
        /// </summary>
        /// <remarks>
        /// Note that it is NOT recommended to use this function in cases where performance is critical.
        /// </remarks>
        /// <returns>
        /// An IEnumerable&lt;Point&gt; which iterates over all positions within the area.
        /// </returns>
        public static IEnumerable<Point> ToEnumerable(this IReadOnlyArea self)
            => new ReadOnlyAreaPostionsEnumerable(self).ToEnumerable();
    }
}
