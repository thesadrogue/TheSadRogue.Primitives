using System;
using System.Linq;
using Xunit;
using XUnit.ValueTuples;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace TheSadRogue.Primitives.UnitTests
{
    public class DistanceTests
    {
        #region Testdata
        public static Distance[] Distances => new Distance[] { Distance.MANHATTAN, Distance.CHEBYSHEV, Distance.EUCLIDEAN };

        public static (Distance.Types, Distance)[] TypeDistanceConversion => new (Distance.Types, Distance)[]
        { (Distance.Types.CHEBYSHEV, Distance.CHEBYSHEV), (Distance.Types.EUCLIDEAN, Distance.EUCLIDEAN), (Distance.Types.MANHATTAN, Distance.MANHATTAN) };

        public static (Distance, AdjacencyRule)[] AdjacencyRuleConversionValues => new (Distance, AdjacencyRule)[]
        { (Distance.CHEBYSHEV, AdjacencyRule.EIGHT_WAY), (Distance.EUCLIDEAN, AdjacencyRule.EIGHT_WAY), (Distance.MANHATTAN, AdjacencyRule.CARDINALS) };

        public static (Distance, Radius)[] RadiusConversionValues => new (Distance, Radius)[]
        { (Distance.CHEBYSHEV, Radius.SQUARE), (Distance.EUCLIDEAN, Radius.CIRCLE), (Distance.MANHATTAN, Radius.DIAMOND) };
        #endregion

        #region Equality/Inequality
        [Theory]
        [MemberDataEnumerable(nameof(Distances))]
        public void TestEquality(Distance dist)
        {
            var compareTo = dist;
            var allDists = Distances;
            Assert.True(dist == compareTo);

            Assert.Equal(1, allDists.Count(i => i == compareTo));
        }

        [Theory]
        [MemberDataEnumerable(nameof(Distances))]
        public void TestInequality(Distance dist)
        {
            var compareTo = dist;
            var allDists = Distances;
            Assert.False(dist != compareTo);

            Assert.Equal(allDists.Length - 1, allDists.Count(i => i != compareTo));
        }

        [Theory]
        [MemberDataEnumerable(nameof(Distances))]
        public void TestEqualityInqeualityOpposite(Distance compareDist)
        {
            var dists = Distances;

            foreach (var dist in dists)
                Assert.NotEqual(dist == compareDist, dist != compareDist);
        }

        [Theory]
        [MemberDataEnumerable(nameof(Distances))]
        public void TestEqualityEquivalence(Distance compareDist)
        {
            var dists = Distances;

            foreach (var dist in dists)
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
        public void AdjacencyRuleConversion(Distance dist, AdjacencyRule expected)
        {
            Assert.Equal(expected, dist);
        }

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
