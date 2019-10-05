using System.Linq;
using SadRogue.Primitives;
using Xunit;
using XUnit.ValueTuples;

namespace TheSadRogue.Primitives.UnitTests
{
    public class RadiusTests
    {
        #region Testdata
        public static Radius[] Radiuses => new Radius[] { Radius.Square, Radius.Circle, Radius.Diamond };

        public static (Radius.Types, Radius)[] TypeRadiusConversion => new (Radius.Types, Radius)[]
        { (Radius.Types.Square, Radius.Square), (Radius.Types.Circle, Radius.Circle), (Radius.Types.Diamond,Radius.Diamond) };

        public static (Radius, AdjacencyRule)[] AdjacencyRuleConversionValues => new (Radius, AdjacencyRule)[]
        { (Radius.Square, AdjacencyRule.EightWay), (Radius.Circle, AdjacencyRule.EightWay), (Radius.Diamond, AdjacencyRule.Cardinals) };

        public static (Radius, Distance)[] DistanceConversionValues => new (Radius, Distance)[]
        { (Radius.Square, Distance.Chebyshev), (Radius.Circle, Distance.Euclidean), (Radius.Diamond, Distance.Manhattan) };
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
            {
                Assert.NotEqual(rad == compareRad, rad != compareRad);
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(Radiuses))]
        public void TestEqualityEquivalence(Radius compareRad)
        {
            Radius[] rads = Radiuses;

            foreach (Radius rad in rads)
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
        // TODO: Test PositionsInRadius functions.
        #endregion
    }
}
