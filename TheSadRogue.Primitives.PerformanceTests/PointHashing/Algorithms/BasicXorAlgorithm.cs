using System.Collections.Generic;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms
{
    /// <summary>
    /// Algorithm using the very simple xor function commonly used in example code and as a default go-to.
    ///
    /// This is quite fast but generates a _ton_ of collisions and thus isn't viable for use; it's meant mostly as
    /// a baseline for how collisions affect performance.
    /// </summary>
    public sealed class BasicXorAlgorithm : EqualityComparer<Point>
    {
        public static readonly IEqualityComparer<Point> Instance = new BasicXorAlgorithm();

        public override bool Equals(Point x, Point y) => x.Equals(y);

        public override int GetHashCode(Point p) => p.X ^ p.Y;
    }
}
