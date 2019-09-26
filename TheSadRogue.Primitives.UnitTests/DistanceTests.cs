using System.Linq;
using SadRogue.Primitives;
using Xunit;
using XUnit.ValueTuples;

namespace TheSadRogue.Primitives.UnitTests
{
    public class DistanceTests
    {
        #region Testdata
        public static Distance[] Distances => new Distance[] { Distance.Manhattan, Distance.Chebyshev, Distance.Euclidean };

        public static (Distance.Types, Distance)[] TypeDistanceConversion => new (Distance.Types, Distance)[]
        { (Distance.Types.Chebyshev, Distance.Chebyshev), (Distance.Types.Euclidean, Distance.Euclidean), (Distance.Types.Manhattan, Distance.Manhattan) };

        public static (Distance, AdjacencyRule)[] AdjacencyRuleConversionValues => new (Distance, AdjacencyRule)[]
        { (Distance.Chebyshev, AdjacencyRule.EightWay), (Distance.Euclidean, AdjacencyRule.EightWay), (Distance.Manhattan, AdjacencyRule.Cardinals) };

        public static (Distance, Radius)[] RadiusConversionValues => new (Distance, Radius)[]
        { (Distance.Chebyshev, Radius.Square), (Distance.Euclidean, Radius.Circle), (Distance.Manhattan, Radius.Diamond) };
        #endregion

        #region Equality/Inequality
        [Theory]
        [MemberDataEnumerable(nameof(Distances))]
        public void TestEquality(Distance dist)
        {
            Distance compareTo = dist;
            Distance[] allDists = Distances;
            Assert.True(dist == compareTo);

            Assert.Equal(1, allDists.Count(i => i == compareTo));
        }

        [Theory]
        [MemberDataEnumerable(nameof(Distances))]
        public void TestInequality(Distance dist)
        {
            Distance compareTo = dist;
            Distance[] allDists = Distances;
            Assert.False(dist != compareTo);

            Assert.Equal(allDists.Length - 1, allDists.Count(i => i != compareTo));
        }

        [Theory]
        [MemberDataEnumerable(nameof(Distances))]
        public void TestEqualityInqeualityOpposite(Distance compareDist)
        {
            Distance[] dists = Distances;

            foreach (Distance dist in dists)
            {
                Assert.NotEqual(dist == compareDist, dist != compareDist);
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(Distances))]
        public void TestEqualityEquivalence(Distance compareDist)
        {
            Distance[] dists = Distances;

            foreach (Distance dist in dists)
            {
                Assert.Equal(dist == compareDist, dist.Equals(compareDist));
                Assert.Equal(dist == compareDist, dist.Equals((object)compareDist));
            }
        }
        #endregion

        #region DistanceTypeToDistanceConversion
        [Theory]
        [MemberDataTuple(nameof(TypeDistanceConversion))]
        public void DistanceTypeConversion(Distance.Types type, Distance expectedDist)
        {
            Distance dir = Distance.ToDistance(type);
            Assert.Equal(expectedDist, dir);
        }
        #endregion

        #region DistanceImplicitConversions
        [Theory]
        [MemberDataTuple(nameof(AdjacencyRuleConversionValues))]
        public void AdjacencyRuleConversion(Distance dist, AdjacencyRule expected) => Assert.Equal(expected, dist);

        [Theory]
        [MemberDataTuple(nameof(RadiusConversionValues))]
        public void RadiusConversion(Distance dist, Radius expected)
        {
            Radius d = dist;
            Assert.Equal(expected, d);
        }
        #endregion

        #region DistanceCalculations
        // TODO: Test actual distance calculations
        #endregion
    }
}
