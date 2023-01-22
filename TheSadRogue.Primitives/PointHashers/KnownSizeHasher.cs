using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SadRogue.Primitives.PointHashers
{
    /// <summary>
    /// NON-GENERALIZED hashing algorithm for <see cref="Point"/> instances that calculates a hash, assuming points will
    /// fall between (0, 0) and some known maximum width.
    /// </summary>
    /// <remarks>
    /// This is NOT an efficient generalized hashing algorithm; it generates a hash code using the assumption
    /// that any points given to it fall between (0, 0) and the (width - 1, arbitrary_y).  It simply uses the
    /// <see cref="Point.ToIndex(int)"/> function to generate a hash.  The algorithm WILL still function on points
    /// outside of the assumed bounds; however it will simply start to generate collisions.
    ///
    /// This algorithm is nonetheless useful in some cases; the assumption that points will fall from
    /// (0, 0) to some known max x-value is fairly common (as it could apply to any fixed-size grid).  Since this
    /// algorithm can be significantly faster to compute than fully generalized algorithms, it can be useful for these
    /// cases.
    /// </remarks>
    [DataContract]
    public class KnownSizeHasher : EqualityComparer<Point>
    {
        /// <summary>
        /// The width of the area points are assumed to fall in.
        /// </summary>
        [DataMember] public readonly int BoundsWidth;

        /// <summary>
        /// Creates a new instance of the comparison/hashing algorithm implementation.
        /// </summary>
        /// <param name="boundsWidth">
        /// The width of the area points are assumed to fall in.  Points are assumed to fall between (0, 0)
        /// and (boundsWidth - 1).  It WILL hash points outside this range, however doing so may generate
        /// collisions.
        /// </param>
        public KnownSizeHasher(int boundsWidth)
        {
            BoundsWidth = boundsWidth;
        }

        /// <inheritdoc/>
        public override bool Equals(Point x, Point y) => x.Equals(y);

        /// <inheritdoc/>
        public override int GetHashCode(Point p) => p.ToIndex(BoundsWidth);
    }
}
