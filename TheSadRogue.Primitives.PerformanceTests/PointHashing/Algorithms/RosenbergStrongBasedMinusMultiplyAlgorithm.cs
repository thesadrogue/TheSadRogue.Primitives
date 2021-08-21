using System.Collections.Generic;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms
{
    /// <summary>
    /// Same as <see cref="RosenbergStrongBasedAlgorithm"/>, but with one multiply, in an effort
    /// to speed up the algorithm (but potentially at the cost of collisions)
    /// </summary>
    public sealed class RosenbergStrongBasedMinusMultiplyAlgorithm : EqualityComparer<Point>
    {
        public static readonly IEqualityComparer<Point> Instance = new RosenbergStrongBasedMinusMultiplyAlgorithm();

        public override bool Equals(Point x, Point y) => x.Equals(y);

        public override int GetHashCode(Point p)
        {
            int x = p.X + 3, y = p.Y + 3, n = (x >= y ? x * (x + 2) - y : y * y + x);
            return (int)((n ^ (uint)n >> 1 ^ 0xD1B54A35) * 0xC13FA9AB ^ 0x7F4A7C15);
        }
    }
}
