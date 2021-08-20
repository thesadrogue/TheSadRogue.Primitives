using System.Collections.Generic;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms
{
    /// <summary>
    /// Algorithm based on the Rosenberg-Strong algorithm.
    /// </summary>
    public sealed class RosenbergStrongBasedAlgorithm : IEqualityComparer<Point>
    {
        public static readonly IEqualityComparer<Point> Instance = new RosenbergStrongBasedAlgorithm();

        public bool Equals(Point x, Point y) => x.Equals(y);

        public int GetHashCode(Point p)
        {
            int x = p.X + 3, y = p.Y + 3, n = (x >= y ? x * (x + 2) - y : y * y + x);
            return (int)(((n ^ (uint)n >> 1 ^ 0xD1B54A35) * 0xC13FA9AB ^ 0x7F4A7C15) * 0x91E10DA3);
        }
    }
}
