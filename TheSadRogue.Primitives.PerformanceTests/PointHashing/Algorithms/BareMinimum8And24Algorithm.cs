using System.Collections.Generic;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms
{
    /// <summary>
    /// A variation of <see cref="BareMinimumAlgorithm"/> that shifts by different values.
    /// </summary>
    public sealed class BareMinimum8And24Algorithm : EqualityComparer<Point>
    {
        public static readonly IEqualityComparer<Point> Instance = new BareMinimum8And24Algorithm();

        public override bool Equals(Point x, Point y) => x.Equals(y);

        public override int GetHashCode(Point p)
        {
            uint x = (uint)p.X, y = (uint)p.Y;
            return (int)(x + (y << 8 | y >> 24));
        }
    }
}
