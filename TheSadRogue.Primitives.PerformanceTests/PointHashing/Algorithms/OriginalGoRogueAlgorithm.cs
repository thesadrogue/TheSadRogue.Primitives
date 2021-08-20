using System.Collections.Generic;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.PerformanceTests.PointHashing.Algorithms
{
    /// <summary>
    /// IEqualityComparer using the method originally used in GoRogue 2 (currently the same as what is used
    /// in the primitives library).
    /// </summary>
    public sealed class OriginalGoRogueAlgorithm : EqualityComparer<Point>
    {
        public static readonly IEqualityComparer<Point> Instance = new OriginalGoRogueAlgorithm();

        public override bool Equals(Point x, Point y) => x.Equals(y);

        public override int GetHashCode(Point p)
        {
            // Intentional overflow on both of these, part of hash-code generation
            int x2 = (int)(0x9E3779B9 * p.X), y2 = 0x632BE5AB * p.Y;
            return (int)(((uint)(x2 ^ y2) >> ((x2 & 7) + (y2 & 7))) * 0x85157AF5);
        }
    }
}
