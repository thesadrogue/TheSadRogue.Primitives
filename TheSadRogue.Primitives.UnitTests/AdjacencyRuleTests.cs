using System;
using System.Linq;
using Xunit;
using XUnit.ValueTuples;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace TheSadRogue.Primitives.UnitTests
{
    public class AdjacencyRuleTests
    {
        #region TestData

        public static AdjacencyRule[] AdjacencyRules
            => new AdjacencyRule[] { AdjacencyRule.CARDINALS, AdjacencyRule.DIAGONALS, AdjacencyRule.EIGHT_WAY };
        
        public static Direction[] EightWayDirectionsClockwise
            => new[]{ Direction.UP, Direction.UP_RIGHT, Direction.RIGHT, Direction.DOWN_RIGHT, Direction.DOWN,
                Direction.DOWN_LEFT, Direction.LEFT, Direction.UP_LEFT };

        public static Direction[] DiagonalDirectionsClockwise
            => new[]{ Direction.UP_RIGHT,Direction.DOWN_RIGHT, Direction.DOWN_LEFT, Direction.UP_LEFT };

        public static Direction[] CardinalsDirectionsClockwise
            => new[]{ Direction.UP, Direction.RIGHT, Direction.DOWN, Direction.LEFT };

        public static IEnumerable<(AdjacencyRule, Direction)> ClockwisePairs
            => AdjacencyRule.EIGHT_WAY.ToEnumerable().Combinate(EightWayDirectionsClockwise)
            .Concat(AdjacencyRule.CARDINALS.ToEnumerable().Combinate(CardinalsDirectionsClockwise))
            .Concat(AdjacencyRule.DIAGONALS.ToEnumerable().Combinate(DiagonalDirectionsClockwise));

        public static IEnumerable<(AdjacencyRule, Direction)> AdjacencyDefaultsClockwise
            => TestUtils.Enumerable((AdjacencyRule.CARDINALS, Direction.UP), (AdjacencyRule.EIGHT_WAY, Direction.UP), (AdjacencyRule.DIAGONALS, Direction.UP_RIGHT));

        public static IEnumerable<(AdjacencyRule, Direction)> AdjacencyDefaultsCounterClockwise
            => TestUtils.Enumerable((AdjacencyRule.CARDINALS, Direction.UP), (AdjacencyRule.EIGHT_WAY, Direction.UP), (AdjacencyRule.DIAGONALS, Direction.UP_LEFT));

        public static IEnumerable<(AdjacencyRule, Direction)> RotatePairs =>
            AdjacencyRule.CARDINALS.ToEnumerable().Combinate(DiagonalDirectionsClockwise)
            .Concat(AdjacencyRule.DIAGONALS.ToEnumerable().Combinate(CardinalsDirectionsClockwise));

        public static IEnumerable<(Point, AdjacencyRule)> PointAdjacencyPairs =>
            TestUtils.Enumerable<Point>((1, 2), (-1, -2), (0, 4), (3, 0)).Combinate(AdjacencyRules);

        static public Direction[] ValidDirections = AdjacencyRule.EIGHT_WAY.DirectionsOfNeighborsClockwise().ToArray();
        static public Direction[] AllDirections => ValidDirections.Append(Direction.NONE).ToArray();

        public static IEnumerable<(Point, AdjacencyRule, Direction)> PointClockwisePairs =>
            PointAdjacencyPairs.Combinate(AllDirections);

        public static (AdjacencyRule.Types, AdjacencyRule)[] TypeAdjacencyRuleConversion
            => new (AdjacencyRule.Types, AdjacencyRule)[]
            {
                (AdjacencyRule.Types.CARDINALS, AdjacencyRule.CARDINALS),
                (AdjacencyRule.Types.DIAGONALS, AdjacencyRule.DIAGONALS),
                (AdjacencyRule.Types.EIGHT_WAY, AdjacencyRule.EIGHT_WAY)
            };
            
        #endregion

        #region ValidDirections
        [Theory]
        [MemberDataTuple(nameof(ClockwisePairs))]
        public void ValidDirectionsClockwiseOrder(AdjacencyRule rule, Direction startingDir)
        {
            Direction[] dirs = null;
            switch(rule.Type)
            {
                case AdjacencyRule.Types.CARDINALS:
                    dirs = CardinalsDirectionsClockwise.ToArray();
                    break;
                case AdjacencyRule.Types.DIAGONALS:
                    dirs = DiagonalDirectionsClockwise.ToArray();
                    break;
                case AdjacencyRule.Types.EIGHT_WAY:
                    dirs = EightWayDirectionsClockwise.ToArray();
                    break;
            }

            Direction[] clockwiseDirs = rule.DirectionsOfNeighborsClockwise(startingDir).ToArray();
            Assert.Equal(startingDir, clockwiseDirs[0]);
            Assert.Equal(clockwiseDirs.Length, dirs.Length);

            int index = Array.FindIndex(dirs, x => x == startingDir);
            Assert.False(index == -1, "Direction found in result that shouldn't be there.");

            for (int i = 0; i < clockwiseDirs.Length; i++)
                Assert.Equal(dirs[(index + i) % dirs.Length], clockwiseDirs[i]);
        }

        [Theory]
        [MemberDataTuple(nameof(ClockwisePairs))]
        public void ValidDirectionsCounterclockwiseOrder(AdjacencyRule rule, Direction startingDir)
        {
            Direction[] dirs = null;
            switch (rule.Type)
            {
                case AdjacencyRule.Types.CARDINALS:
                    dirs = CardinalsDirectionsClockwise.Reverse().ToArray();
                    break;
                case AdjacencyRule.Types.DIAGONALS:
                    dirs = DiagonalDirectionsClockwise.Reverse().ToArray();
                    break;
                case AdjacencyRule.Types.EIGHT_WAY:
                    dirs = EightWayDirectionsClockwise.Reverse().ToArray();
                    break;
            }

            Direction[] counterclockwiseDirs = rule.DirectionsOfNeighborsCounterClockwise(startingDir).ToArray();
            Assert.Equal(startingDir, counterclockwiseDirs[0]);
            Assert.Equal(counterclockwiseDirs.Length, dirs.Length);

            int index = Array.FindIndex(dirs, x => x == startingDir);
            Assert.False(index == -1, "Direction found in result that shouldn't be there.");

            for (int i = 0; i < counterclockwiseDirs.Length; i++)
            Assert.Equal(dirs[(index + i) % dirs.Length], counterclockwiseDirs[i]);

        }

        #endregion

        #region NoneDirections
        [Theory]
        [MemberDataTuple(nameof(AdjacencyDefaultsClockwise))]
        public void NoneDirectionClockwise(AdjacencyRule rule, Direction defaultDir)
        {
            Direction[] defaultDirs = rule.DirectionsOfNeighborsClockwise(defaultDir).ToArray();
            Direction[] noneDirs = rule.DirectionsOfNeighborsClockwise(Direction.NONE).ToArray();
            Direction[] noDirs = rule.DirectionsOfNeighborsClockwise().ToArray();

            TestUtils.AssertElementEquals(defaultDirs, noneDirs, noDirs);
        }

        [Theory]
        [MemberDataTuple(nameof(AdjacencyDefaultsCounterClockwise))]
        public void NoneDirectionCounterClockwise(AdjacencyRule rule, Direction defaultDir)
        {
            Direction[] defaultDirs = rule.DirectionsOfNeighborsCounterClockwise(defaultDir).ToArray();
            Direction[] noneDirs = rule.DirectionsOfNeighborsCounterClockwise(Direction.NONE).ToArray();
            Direction[] noDirs = rule.DirectionsOfNeighborsCounterClockwise().ToArray();

            TestUtils.AssertElementEquals(defaultDirs, noneDirs, noDirs);
        }
        #endregion

        #region RotatableDirections
        [Theory]
        [MemberDataTuple(nameof(RotatePairs))]
        public void RotateStartingDirectionClockwise(AdjacencyRule rule, Direction start)
        {
            Direction[] startDirs = rule.DirectionsOfNeighborsClockwise(start).ToArray();

            start++;
            Direction[] rotateDirs = rule.DirectionsOfNeighborsClockwise(start).ToArray();

            TestUtils.AssertElementEquals(startDirs, rotateDirs);
        }

        [Theory]
        [MemberDataTuple(nameof(RotatePairs))]
        public void RotateStartingDirectionCounterClockwise(AdjacencyRule rule, Direction start)
        {
            Direction[] startDirs = rule.DirectionsOfNeighborsCounterClockwise(start).ToArray();

            start--;
            Direction[] rotateDirs = rule.DirectionsOfNeighborsCounterClockwise(start).ToArray();

            TestUtils.AssertElementEquals(startDirs, rotateDirs);
        }
        #endregion

        #region CardinalFirstDirections
        [Theory]
        [MemberDataEnumerable(nameof(AdjacencyRules))]
        public void CardinalFirstDirections(AdjacencyRule rule)
        {
            Direction[] cardsFirst = rule.DirectionsOfNeighbors().ToArray();
            Direction[] expectedDirs = rule.DirectionsOfNeighborsClockwise().ToArray();
            Assert.Equal(expectedDirs.Length, cardsFirst.Length);

            int index = Array.FindIndex(cardsFirst, i => !i.IsCardinal());
            if (index == -1)
                Assert.Equal(4, cardsFirst.Length);
            else
            {
                for (int i = 0; i < cardsFirst.Length; i++)
                {
                    if (cardsFirst[i].IsCardinal())
                        Assert.True(i < index);
                    else
                        Assert.True(i >= index);
                }
            }
        }
        #endregion

        #region NeighborsClockwise
        [Theory]
        [MemberDataTuple(nameof(PointClockwisePairs))]
        public void NeighborsClockwise(Point point, AdjacencyRule rule, Direction startDir)
        {
            var result = rule.NeighborsClockwise(point, startDir).ToArray();
            var expected = rule.DirectionsOfNeighborsClockwise(startDir).Select(i => point + i).ToArray();

            TestUtils.AssertElementEquals(result, expected);
        }

        [Theory]
        [MemberDataTuple(nameof(PointAdjacencyPairs))]
        public void NeighborsClockwiseNoStart(Point point, AdjacencyRule rule)
        {
            Point[] result = rule.NeighborsClockwise(point).ToArray();
            Point[] expected = rule.NeighborsClockwise(point, default).ToArray();

            TestUtils.AssertElementEquals(expected, result);
        }
        #endregion

        #region NeighborsCounterClockwise
        [Theory]
        [MemberDataTuple(nameof(PointClockwisePairs))]
        public void NeighborsCounterClockwise(Point point, AdjacencyRule rule, Direction startDir)
        {
            var result = rule.NeighborsCounterClockwise(point, startDir).ToArray();
            var expected = rule.DirectionsOfNeighborsCounterClockwise(startDir).Select(i => point + i).ToArray();

            TestUtils.AssertElementEquals(result, expected);
        }

        [Theory]
        [MemberDataTuple(nameof(PointAdjacencyPairs))]
        public void NeighborsCounterClockwiseNoStart(Point point, AdjacencyRule rule)
        {
            Point[] result = rule.NeighborsCounterClockwise(point).ToArray();
            Point[] expected = rule.NeighborsCounterClockwise(point, default).ToArray();

            TestUtils.AssertElementEquals(expected, result);
        }
        #endregion

        #region NeighborsUnordered
        [Theory]
        [MemberDataTuple(nameof(PointAdjacencyPairs))]
        public void NeighborsUnordered(Point point, AdjacencyRule rule)
        {
            var result = rule.Neighbors(point).ToArray();
            var expected = rule.DirectionsOfNeighbors().Select(i => point + i).ToArray();

            TestUtils.AssertElementEquals(result, expected);
        }
        #endregion

        #region AdjacencyRuleTypeToAdjacencyRuleConversion
        [Theory]
        [MemberDataTuple(nameof(TypeAdjacencyRuleConversion))]
        public void AdjacencyRuleTypeConversion(AdjacencyRule.Types type, AdjacencyRule expectedRule)
        {
            AdjacencyRule dir = AdjacencyRule.ToAdjacencyRule(type);
            Assert.Equal(expectedRule, dir);
        }
        #endregion

        #region Equality/Inequality
        [Theory]
        [MemberDataEnumerable(nameof(AdjacencyRules))]
        public void TestEquality(AdjacencyRule rule)
        {
            var compareTo = rule;
            var allRules = AdjacencyRules;
            Assert.True(rule == compareTo);

            Assert.Equal(1, allRules.Count(i => i == compareTo));
        }

        [Theory]
        [MemberDataEnumerable(nameof(AdjacencyRules))]
        public void TestInequality(AdjacencyRule rule)
        {
            var compareTo = rule;
            var allRules = AdjacencyRules;
            Assert.False(rule != compareTo);

            Assert.Equal(allRules.Length - 1, allRules.Count(i => i != compareTo));
        }

        [Theory]
        [MemberDataEnumerable(nameof(AdjacencyRules))]
        public void TestEqualityInqeualityOpposite(AdjacencyRule compareRule)
        {
            var rules = AdjacencyRules;

            foreach (var rule in rules)
                Assert.NotEqual(rule == compareRule, rule != compareRule);
        }

        [Theory]
        [MemberDataEnumerable(nameof(AdjacencyRules))]
        public void TestEqualityEquivalence(AdjacencyRule compareRule)
        {
            var rules = AdjacencyRules;

            foreach (var rule in rules)
            {
                Assert.Equal(rule == compareRule, rule.Equals(compareRule));
                Assert.Equal(rule == compareRule, rule.Equals((object)compareRule));
            }
        }
        #endregion
    }
}
