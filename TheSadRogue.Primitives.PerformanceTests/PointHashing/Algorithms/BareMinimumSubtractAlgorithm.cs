using System.Collections.Generic;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms
{
    /// <summary>
    /// A variation of <see cref="BareMinimumAlgorithm"/> which uses subtraction instead of addition.
    /// </summary>
    public sealed class BareMinimumSubtractAlgorithm : EqualityComparer<Point>
    {
        public static readonly IEqualityComparer<Point> Instance = new BareMinimumSubtractAlgorithm();

        public override bool Equals(Point x, Point y) => x.Equals(y);

        public override int GetHashCode(Point p)
        {
            uint x = (uint)p.X, y = (uint)p.Y;
            return (int)(x - (y << 16 | y >> 16));
        }
    }
}
