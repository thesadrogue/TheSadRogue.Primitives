using System;
using System.Linq;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class DistanceTests
    {
        #region Testdata

        public static Distance[] Distances => new[] { (Distance)Distance.Manhattan, Distance.Chebyshev, Distance.Euclidean };

        public static (Distance.Types, Distance)[] TypeDistanceConversion => new[]
        {
            (Distance.Types.Chebyshev, (Distance)Distance.Chebyshev), (Distance.Types.Euclidean, Distance.Euclidean),
            (Distance.Types.Manhattan, Distance.Manhattan)
        };

        public static (Distance, AdjacencyRule)[] AdjacencyRuleConversionValues => new[]
        {
            ((Distance) Distance.Chebyshev, AdjacencyRule.EightWay), (Distance.Euclidean, AdjacencyRule.EightWay),
            (Distance.Manhattan, AdjacencyRule.Cardinals)
        };

        public static (Distance, Radius)[] RadiusConversionValues => new[]
        {
            ((Distance) Distance.Chebyshev, Radius.Square), (Distance.Euclidean, Radius.Circle),
            (Distance.Manhattan, Radius.Diamond)
        };

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
                Assert.NotEqual(dist == compareDist, dist != compareDist);
        }

        [Theory]
        [MemberDataEnumerable(nameof(Distances))]
        public void TestEqualityEquivalence(Distance compareDist)
        {
            Distance[] dists = Distances;

            foreach (Distance dist in dists)
            {
                Assert.Equal(dist == compareDist, dist.Equals(compareDist));
                Assert.Equal(dist == compareDist, dist.Matches(compareDist));
                // ReSharper disable once RedundantCast
                Assert.Equal(dist == compareDist, dist.Equals((object)compareDist));
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(Distances))]
        public void TestHashCode(Distance compareDist)
        {
            Distance[] dists = Distances;

            foreach (Distance dist in dists)
            {
                if (compareDist.Matches(dist))
                    Assert.Equal(compareDist.GetHashCode(), dist.GetHashCode());
            }
        }

        #endregion

        #region DistanceTypeToDistanceConversion

        [Theory]
        [MemberDataTuple(nameof(TypeDistanceConversion))]
        public void DistanceTypeConversion(Distance.Types type, Distance expectedDist)
        {
            Distance dir = type;
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

        [Theory]
        [MemberDataEnumerable(nameof(Distances))]
        public void DistanceCalculationCorrect(Distance calc)
        {
            // Negative and positive coordinates, along with 0
            var area = new Rectangle(-5, -7, 17, 12);
            var start = area.MinExtent;

            // These algorithms are so well known, we'll just do the calculation correctly with no optimizations
            Func<double, double, double> correctAlgorithm = calc.Type switch
            {
                Distance.Types.Chebyshev => Chebyshev,
                Distance.Types.Manhattan => Manhattan,
                Distance.Types.Euclidean => Euclidean,
                _ => throw new Exception("Unsupported type used.")
            };

            foreach (var pos in area.Positions())
            {
                var delta = pos - start;
                double expected = correctAlgorithm(delta.X, delta.Y);

                Assert.Equal(expected, calc.Calculate(start, pos));
                Assert.Equal(expected, calc.Calculate(pos, start));
                Assert.Equal(expected, calc.Calculate(start.X, start.Y, pos.X, pos.Y));
                Assert.Equal(expected, calc.Calculate(pos.X, pos.Y, start.X, start.Y));
                Assert.Equal(expected, calc.Calculate(delta));
                Assert.Equal(expected, calc.Calculate(delta.X, delta.Y));
            }
        }

        #endregion

        #region Distance Calculation Baselines

        public double Manhattan(double dx, double dy) => Math.Abs(dx) + Math.Abs(dy);
        public double Chebyshev(double dx, double dy) => Math.Max(Math.Abs(dx), Math.Abs(dy));

        public double Euclidean(double dx, double dy)
        {
            dx = Math.Abs(dx);
            dy = Math.Abs(dy);
            return Math.Sqrt(dx * dx + dy * dy);
        }
        #endregion
    }
}
