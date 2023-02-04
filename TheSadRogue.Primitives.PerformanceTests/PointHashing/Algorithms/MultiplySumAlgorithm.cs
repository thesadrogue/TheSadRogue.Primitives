using System.Collections.Generic;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms
{
    /// <summary>
    /// Algorithm that multiplies each component of the point by a different number, then adds them all up.
    /// </summary>
    public sealed class MultiplySumAlgorithm : EqualityComparer<Point>
    {
        public static readonly IEqualityComparer<Point> Instance = new MultiplySumAlgorithm();

        public override bool Equals(Point x, Point y) => x.Equals(y);

        public override int GetHashCode(Point p)
        {
            // The multipliers are the top 32 bits of constants used by Martin Roberts,
            // http://extremelearning.com.au/unreasonable-effectiveness-of-quasirandom-sequences/
            return -1052792407 * p.X + -1847521883 * p.Y;
        }
    }
}
