using System.Collections.Generic;
using SadRogue.Primitives.PointHashers;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests.PointHashers
{
    public class KnownSizeHasherTests
    {
        #region Test Data
        private static readonly Point[] _points = { (1, 2), (3, 4), (6, 5)};
        public static IEnumerable<(Point p1, Point p2)> PointPairs = _points.Combinate(_points);
        #endregion

        [Fact]
        public void HashCode()
        {
            int width = 17;
            int height = 14;
            var hasher = new KnownSizeHasher(width);
            foreach (var pos in new Rectangle(0, 0, width, height).Positions())
                Assert.Equal(pos.ToIndex(width), hasher.GetHashCode(pos));
        }

        [Theory]
        [MemberDataTuple(nameof(PointPairs))]
        public void Equality(Point p1, Point p2)
        {
            var hasher = new KnownSizeHasher(6);
            Assert.Equal(p1.Matches(p2), hasher.Equals(p1, p2));
        }
    }
}
