using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SadRogue.Primitives.PointHashers
{
    /// <summary>
    /// NON-GENERALIZED hashing algorithm for <see cref="Point"/> instances that calculates a hash, assuming a known
    /// maximum x-value.
    /// </summary>
    /// <remarks>
    /// This is NOT an efficient generalized hashing algorithm; it generates a hash code using the assumption
    /// of a "maximum width" that can occur in any point given to it.  It simply uses the <see cref="Point.ToIndex(int)"/>
    /// function to generate a hash.  The algorithm WILL still function on points outside of the max x-value; however
    /// it will simply start to generate collisions.
    ///
    /// This algorithm is nonetheless useful in some cases; the assumption that points will fall from
    /// (0, 0) to some known max width is fairly common (as it could apply to any fixed-size grid).  Since this
    /// algorithm can be significantly faster to compute than fully generalized algorithms, it can be useful for these
    /// cases.
    /// </remarks>
    [DataContract]
    public class KnownSizeHasher : EqualityComparer<Point>
    {
        /// <summary>
        /// Maximum x-value that is generally expected for points that this hashing algorithm will receive.
        /// </summary>
        [DataMember] public readonly int MaxXValue;

        /// <summary>
        /// Creates a new instance of the comparison/hashing algorithm implementation.
        /// </summary>
        /// <param name="maxXValue">
        /// Maximum x-value to assume for points that are hashed by this algorithm.
        /// It IS allowed to hash points outside this value, however doing so may generate
        /// collisions.
        /// </param>
        public KnownSizeHasher(int maxXValue)
        {
            MaxXValue = maxXValue;
        }

        /// <inheritdoc/>
        public override bool Equals(Point x, Point y) => x.Equals(y);

        /// <inheritdoc/>
        public override int GetHashCode(Point p) => p.ToIndex(MaxXValue);
    }
}
