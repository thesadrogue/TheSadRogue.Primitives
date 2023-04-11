using System.Collections.Generic;
using SadRogue.Primitives.PointHashers;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests.PointHashers
{
    public class KnownRangeHasherTests
    {
        #region Test Data
        private static readonly Point[] s_points = { (1, 2), (3, 4), (6, 5)};
        public static IEnumerable<(Point p1, Point p2)> PointPairs = s_points.Combinate(s_points);
        #endregion

        [Fact]
        public void HashCode()
        {
            var points = new Rectangle((1, 2), (17, 14));

            var hasher = new KnownRangeHasher(points.MinExtent, points.MaxExtent);
            var hasher2 = new KnownRangeHasher(points);

            foreach (var pos in points.Positions())
            {
                Assert.Equal((pos - points.MinExtent).ToIndex(points.Width), hasher.GetHashCode(pos));
                Assert.Equal(hasher.GetHashCode(pos), hasher2.GetHashCode(pos));
            }

        }

        [Theory]
        [MemberDataTuple(nameof(PointPairs))]
        public void Equality(Point p1, Point p2)
        {
            var hasher = new KnownRangeHasher((1, 2), (17, 14));
            Assert.Equal(p1.Matches(p2), hasher.Equals(p1, p2));
        }
    }
}
