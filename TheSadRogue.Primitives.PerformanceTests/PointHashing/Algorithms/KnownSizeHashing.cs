using System.Collections.Generic;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms
{
    /// <summary>
    /// NON-GENERALIZED hashing algorithm that calculates a hash, assuming a known maximum width.
    /// </summary>
    /// <remarks>
    /// This is NOT and efficient generalized hashing algorithm; it generates a hash code using the assumption
    /// of a "maximum width" that can occur in any point given.  It simply uses the <see cref="Point.ToIndex(int)"/>
    /// function to generate a hash.  The algorithm WILL still function on points outside of the max width; however
    /// it will simply start to generate more collisions.
    ///
    /// This algorithm is nonetheless useful to performance test, because the assumption that points will fall from
    /// (0, 0) to some known max width is fairly common (as it could apply to any fixed-size grid).  Since this
    /// algorithm can be significantly faster to compute than fully generalized algorithms, it can be useful for these
    /// cases, and having a measure of its relative performance is useful.
    /// </remarks>
    public sealed class KnownSizeHashing : IEqualityComparer<Point>
    {
        private readonly int _maxWidth;

        public KnownSizeHashing(int maxWidth)
        {
            _maxWidth = maxWidth;
        }

        public bool Equals(Point x, Point y) => x.Equals(y);

        public int GetHashCode(Point p) => p.ToIndex(_maxWidth);
    }
}
