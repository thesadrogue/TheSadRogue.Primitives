using System.Collections.Generic;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms
{
    /// <summary>
    /// Algorithm using the exact Cantor pairing function.
    /// This does not provide special treatment for negative inputs.
    /// </summary>
    public sealed class CantorPureAlgorithm : EqualityComparer<Point>
    {
        public static readonly IEqualityComparer<Point> Instance = new CantorPureAlgorithm();

        public override bool Equals(Point x, Point y) => x.Equals(y);

        public override int GetHashCode(Point p)
        {
            int x = p.X, y = p.Y;
            return y + ((x + y) * (x + y + 1) >> 1);
        }
    }
}
