using System.Linq;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class RadiusTests
    {
        #region Testdata

        public static Radius[] Radiuses => new[] { Radius.Square, Radius.Circle, Radius.Diamond };

        public static (Radius.Types, Radius)[] TypeRadiusConversion => new[]
        {
            (Radius.Types.Square, Radius.Square), (Radius.Types.Circle, Radius.Circle),
            (Radius.Types.Diamond, Radius.Diamond)
        };

        public static (Radius, AdjacencyRule)[] AdjacencyRuleConversionValues => new[]
        {
            (Radius.Square, AdjacencyRule.EightWay), (Radius.Circle, AdjacencyRule.EightWay),
            (Radius.Diamond, AdjacencyRule.Cardinals)
        };

        public static (Radius, Distance)[] DistanceConversionValues => new[]
        {
            (Radius.Square, (Distance) Distance.Chebyshev), (Radius.Circle, Distance.Euclidean),
            (Radius.Diamond, Distance.Manhattan)
        };

        #endregion

        #region Equality/Inequality

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void TestEquality(Radius rad)
        {
            Radius compareTo = rad;
            Radius[] allRads = Radiuses;
            Assert.True(rad == compareTo);

            Assert.Equal(1, allRads.Count(i => i == compareTo));
        }

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void TestInequality(Radius rad)
        {
            Radius compareTo = rad;
            Radius[] allRads = Radiuses;
            Assert.False(rad != compareTo);

            Assert.Equal(allRads.Length - 1, allRads.Count(i => i != compareTo));
        }

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void TestEqualityInqeualityOpposite(Radius compareRad)
        {
            Radius[] rads = Radiuses;

            foreach (Radius rad in rads)
                Assert.NotEqual(rad == compareRad, rad != compareRad);
        }

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void TestEqualityEquivalence(Radius compareRad)
        {
            Radius[] rads = Radiuses;

            foreach (Radius rad in rads)
            {
                Assert.Equal(rad == compareRad, rad.Equals(compareRad));
                Assert.Equal(rad == compareRad, rad.Matches(compareRad));
                Assert.Equal(rad == compareRad, rad.Equals((object)compareRad));
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void TestGetHashCode(Radius compareRad)
        {
            Radius[] rads = Radiuses;

            foreach (Radius rad in rads)
            {
                if (compareRad.Matches(rad))
                    Assert.Equal(compareRad.GetHashCode(), rad.GetHashCode());
            }
        }

        #endregion

        #region DistanceTypeToDistanceConversion

        [Theory]
        [MemberDataTuple(nameof(TypeRadiusConversion))]
        public void RadiusTypeConversion(Radius.Types type, Radius expectedRad)
        {
            Radius rad = type;
            Assert.Equal(expectedRad, rad);
        }

        #endregion

        #region DistanceImplicitConversions

        [Theory]
        [MemberDataTuple(nameof(AdjacencyRuleConversionValues))]
        public void AdjacencyRuleConversion(Radius dist, AdjacencyRule expected) => Assert.Equal(expected, dist);

        [Theory]
        [MemberDataTuple(nameof(DistanceConversionValues))]
        public void DistanceConversion(Radius rad, Distance expected)
        {
            Distance r = rad;
            Assert.Equal(expected, r);
        }

        #endregion

        #region PositionsInRadius

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void TestRadiusUnbounded(Radius shape)
        {
            Rectangle area = (1, 2, 55, 43);
            Point center = (25, 20);
            int radius = 10;

            // ReSharper disable once RedundantCast
            Distance dist = (Distance)shape;

            var positions = shape.PositionsInRadius(center, radius).ToList();
            var positionsHash = positions.ToHashSet();

            // No duplicates
            Assert.Equal(positionsHash.Count, positions.Count);

            // Sanity check; positions should be within the original area we're comparing against to ensure we check all points
            // by iterating over area
            Assert.All(positions, pos => Assert.True(area.Contains(pos)));

            // Positions returned should be exactly the ones within the radius
            var positionsHashExpected =
                area.Positions().ToEnumerable().Where(pos => dist.Calculate(pos, center) <= radius).ToHashSet();
            Assert.Equal(positionsHashExpected, positionsHash);
        }

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void TestRadiusBounded(Radius shape)
        {
            Rectangle bounds = (1, 2, 55, 43);
            // From here to bounds is < radius in terms of distance
            Point center = (5, 7);
            int radius = 10;

            // ReSharper disable once RedundantCast
            Distance dist = (Distance)shape;

            var positions = shape.PositionsInRadius(center, radius, bounds).ToList();
            var positionsHash = positions.ToHashSet();

            // No duplicates
            Assert.Equal(positionsHash.Count, positions.Count);

            // Sanity check; positions should be within the original area we're comparing against since that was the bounds
            Assert.All(positions, pos => Assert.True(bounds.Contains(pos)));

            // Positions returned should be exactly the ones within the radius
            var positionsHashExpected =
                bounds.Positions().ToEnumerable().Where(pos => dist.Calculate(pos, center) <= radius).ToHashSet();
            Assert.Equal(positionsHashExpected, positionsHash);
        }

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void RadiusXYPointEquivalenceBounded(Radius shape)
        {
            Point center = (25, 20);
            int radius = 10;

            // Return values should not change with context
            var expected = shape.PositionsInRadius(center, radius).ToArray();
            var actual = shape.PositionsInRadius(center.X, center.Y, radius).ToArray();

            TestUtils.AssertElementEquals(expected, actual);
        }

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void RadiusXYPointEquivalenceUnbounded(Radius shape)
        {
            Rectangle bounds = (1, 2, 55, 43);
            // From here to bounds is < radius in terms of distance
            Point center = (5, 7);
            int radius = 10;

            // Return values should not change with context
            var expected = shape.PositionsInRadius(center, radius, bounds).ToArray();
            var actual = shape.PositionsInRadius(center.X, center.Y, radius, bounds).ToArray();

            TestUtils.AssertElementEquals(expected, actual);
        }

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void TestRadiusWithContextBounded(Radius shape)
        {
            Point center = (25, 20);
            int radius = 10;

            var context = new RadiusLocationContext(center, radius);

            // Return values should not change with context
            var expected = shape.PositionsInRadius(center, radius).ToArray();
            var actual = shape.PositionsInRadius(context).ToArray();

            TestUtils.AssertElementEquals(expected, actual);

            // Re-use the context to ensure re-initialization happens properly
            actual = shape.PositionsInRadius(context).ToArray();
            TestUtils.AssertElementEquals(expected, actual);
        }

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void TestRadiusWithContextUnbounded(Radius shape)
        {
            Rectangle bounds = (1, 2, 55, 43);
            // From here to bounds is < radius in terms of distance
            Point center = (5, 7);
            int radius = 10;

            var context = new RadiusLocationContext(center, radius, bounds);

            // Return values should not change with context
            var expected = shape.PositionsInRadius(center, radius, bounds).ToArray();
            var actual = shape.PositionsInRadius(context).ToArray();

            TestUtils.AssertElementEquals(expected, actual);

            // Re-use the context to ensure re-initialization happens properly
            actual = shape.PositionsInRadius(context).ToArray();
            TestUtils.AssertElementEquals(expected, actual);
        }

        #endregion
    }
}
