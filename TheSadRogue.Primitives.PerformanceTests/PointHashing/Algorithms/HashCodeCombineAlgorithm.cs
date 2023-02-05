using System;
using System.Collections.Generic;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms
{
    /// <summary>
    /// Algorithm using the HashSet.Combine function built into C#.
    /// </summary>
    public sealed class HashCodeCombineAlgorithm : EqualityComparer<Point>
    {
        public static readonly IEqualityComparer<Point> Instance = new HashCodeCombineAlgorithm();

        public override bool Equals(Point x, Point y) => x.Equals(y);

        public override int GetHashCode(Point p) => HashCode.Combine(p.X, p.Y);
    }
}
