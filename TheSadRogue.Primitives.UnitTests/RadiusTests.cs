using System.Linq;
using Xunit;
using XUnit.ValueTuples;
using SadRogue.Primitives;

namespace TheSadRogue.Primitives.UnitTests
{
    public class RadiusTests
    {
        #region Testdata
        public static Radius[] Radiuses => new Radius[] { Radius.SQUARE, Radius.CIRCLE, Radius.DIAMOND };

        public static (Radius.Types, Radius)[] TypeRadiusConversion => new (Radius.Types, Radius)[]
        { (Radius.Types.SQUARE, Radius.SQUARE), (Radius.Types.CIRCLE, Radius.CIRCLE), (Radius.Types.DIAMOND,Radius.DIAMOND) };

        public static (Radius, AdjacencyRule)[] AdjacencyRuleConversionValues => new (Radius, AdjacencyRule)[]
        { (Radius.SQUARE, AdjacencyRule.EIGHT_WAY), (Radius.CIRCLE, AdjacencyRule.EIGHT_WAY), (Radius.DIAMOND, AdjacencyRule.CARDINALS) };

        public static (Radius, Distance)[] DistanceConversionValues => new (Radius, Distance)[]
        { (Radius.SQUARE, Distance.CHEBYSHEV), (Radius.CIRCLE, Distance.EUCLIDEAN), (Radius.DIAMOND, Distance.MANHATTAN) };
        #endregion

        #region Equality/Inequality
        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void TestEquality(Radius rad)
        {
            var compareTo = rad;
            var allRads = Radiuses;
            Assert.True(rad == compareTo);

            Assert.Equal(1, allRads.Count(i => i == compareTo));
        }

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void TestInequality(Radius rad)
        {
            var compareTo = rad;
            var allRads = Radiuses;
            Assert.False(rad != compareTo);

            Assert.Equal(allRads.Length - 1, allRads.Count(i => i != compareTo));
        }

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void TestEqualityInqeualityOpposite(Radius compareRad)
        {
            var rads = Radiuses;

            foreach (var rad in rads)
                Assert.NotEqual(rad == compareRad, rad != compareRad);
        }

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void TestEqualityEquivalence(Radius compareRad)
        {
            var rads = Radiuses;

            foreach (var rad in rads)
            {
                Assert.Equal(rad == compareRad, rad.Equals(compareRad));
                Assert.Equal(rad == compareRad, rad.Equals((object)compareRad));
            }
        }
        #endregion

        #region DistanceTypeToDistanceConversion
        [Theory]
        [MemberDataTuple(nameof(TypeRadiusConversion))]
        public void RadiusTypeConversion(Radius.Types type, Radius expectedRad)
        {
            Radius rad = Radius.ToRadius(type);
            Assert.Equal(expectedRad, rad);
        }
        #endregion

        #region DistanceImplicitConversions
        [Theory]
        [MemberDataTuple(nameof(AdjacencyRuleConversionValues))]
        public void AdjacencyRuleConversion(Radius dist, AdjacencyRule expected)
        {
            Assert.Equal(expected, dist);
        }

        [Theory]
        [MemberDataTuple(nameof(DistanceConversionValues))]
        public void DistanceConversion(Radius rad, Distance expected)
        {
            Distance r = rad;
            Assert.Equal(expected, r);
        }
        #endregion

        #region PositionsInRadius
        // TODO: Test PositionsInRadius functions.
        #endregion
    }
}
