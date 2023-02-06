using System.Collections.Generic;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms
{
    /// <summary>
    /// Algorithm using the a very simple hashing function that is surprisingly good at avoiding collisions at x/y
    /// less than 65536.
    /// </summary>
    public sealed class SimpleShiftAlgorithm : EqualityComparer<Point>
    {
        public static readonly IEqualityComparer<Point> Instance = new SimpleShiftAlgorithm();

        public override bool Equals(Point x, Point y) => x.Equals(y);

        public override int GetHashCode(Point p)
        {
            return p.X + (p.Y << 16);
        }
    }
}
