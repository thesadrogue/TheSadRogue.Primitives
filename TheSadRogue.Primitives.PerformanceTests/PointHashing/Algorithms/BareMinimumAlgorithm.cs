using System.Collections.Generic;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms
{
    /// <summary>
    /// Algorithm using the simplest function I could think of.
    /// </summary>
    public sealed class BareMinimumAlgorithm : EqualityComparer<Point>
    {
        public static readonly IEqualityComparer<Point> Instance = new BareMinimumAlgorithm();

        public override bool Equals(Point x, Point y) => x.Equals(y);

        public override int GetHashCode(Point p)
        {
            uint x = (uint)p.X, y = (uint)p.Y;
            return (int)(x + (y << 16 | y >> 16));
        }
    }
}
