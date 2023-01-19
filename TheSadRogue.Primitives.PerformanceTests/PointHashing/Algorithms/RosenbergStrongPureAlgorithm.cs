using System.Collections.Generic;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms
{
    /// <summary>
    /// Algorithm using the exact Rosenberg-Strong pairing function.
    /// This does not provide special treatment for negative inputs.
    /// </summary>
    public sealed class RosenbergStrongPureAlgorithm : EqualityComparer<Point>
    {
        public static readonly IEqualityComparer<Point> Instance = new RosenbergStrongPureAlgorithm();

        public override bool Equals(Point x, Point y) => x.Equals(y);

        public override int GetHashCode(Point p)
        {
            int x = p.X, y = p.Y;
            return (x >= y ? x * (x + 2) - y : y * y + x);
        }
    }
}
