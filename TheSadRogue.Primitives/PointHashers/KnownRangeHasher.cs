using System.Collections.Generic;

namespace SadRogue.Primitives.PointHashers
{
    /// <summary>
    /// NON-GENERALIZED hashing algorithm for <see cref="Point"/> instances that calculates a hash, assuming a known
    /// minimum and maximum extent of the Points being hashed.
    /// </summary>
    /// <remarks>
    /// This is NOT an efficient generalized hashing algorithm; it generates a hash code using the assumption
    /// of a "maximum range" of x/y values in which all the points given to it to hash fall.  It simply normalizes
    /// the points to a range of (0, 0), to (x_1, y_1), then hashes them via a simple call to
    /// <see cref="Point.ToIndex(int)"/>. The algorithm WILL still function on points outside of the min/max extents;
    /// however it will start to generate more collisions.
    ///
    /// This algorithm is nonetheless useful in some cases; the assumption that points will fall from
    /// (minX, minY) to some known max width/height is fairly common (as it could apply to any fixed-size grid).
    /// Since this algorithm can be significantly faster to compute than fully generalized algorithms, it can be
    /// useful for these cases.
    /// </remarks>
    public class KnownRangeHasher : EqualityComparer<Point>
    {
        /// <summary>
        /// Minimum x/y values that will generally occur in points hashed by this instance.
        /// </summary>
        public readonly Point MinExtent;

        /// <summary>
        /// Width of the area which encompasses points which will generally be hashed by this algorithm, starting at
        /// <see cref="MinExtent"/>.
        /// </summary>
        public readonly int BoundsWidth;

        /// <summary>
        /// Creates a new instance of the comparison/hashing algorithm implementation.
        /// </summary>
        /// <param name="minExtent">
        /// Point whose x and y values constitute the minimum x/y values that will
        /// generally be encountered in Point instances that are hashed by this algorithm.
        /// </param>
        /// <param name="maxExtent">
        /// Point whose x and y values constitute the maximum x/y values that will
        /// generally be encountered in Point instances that are hashed by this algorithm.
        /// </param>
        public KnownRangeHasher(Point minExtent, Point maxExtent)
        {
            MinExtent = minExtent;
            BoundsWidth = maxExtent.X - minExtent.X + 1;
        }

        /// <summary>
        /// Creates a new instance of the comparison/hashing algorithm implementation.
        /// </summary>
        /// <param name="bounds">
        /// Bounds encompassing the area which points generally hashed by this algorithm will reside within.
        /// </param>
        public KnownRangeHasher(Rectangle bounds)
        {
            MinExtent = bounds.MinExtent;
            BoundsWidth = bounds.Width;
        }

        /// <summary>
        /// Creates a new instance of the comparison/hashing algorithm implementation.
        /// </summary>
        /// <param name="minExtent">
        /// Point whose x and y values constitute the minimum x/y values that will
        /// generally be encountered in Point instances that are hashed by this algorithm.
        /// </param>
        /// <param name="boundsWidth">
        /// Width of the bounds that will generally encompass Points hashed by this hashing algorithm.
        /// </param>
        public KnownRangeHasher(Point minExtent, int boundsWidth)
        {
            MinExtent = minExtent;
            BoundsWidth = boundsWidth;
        }

        /// <inheritdoc/>
        public override bool Equals(Point x, Point y) => x.Equals(y);

        /// <inheritdoc/>
        public override int GetHashCode(Point p) => (p - MinExtent).ToIndex(BoundsWidth);
    }
}
