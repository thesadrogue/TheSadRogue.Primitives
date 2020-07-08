using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class AdjacencyRuleTests
    {
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
                for (int i = 0; i < cardsFirst.Length; i++)
                    if (cardsFirst[i].IsCardinal())
                        Assert.True(i < index);
                    else
                        Assert.True(i >= index);
        }

        #endregion

        #region NeighborsUnordered

        [Theory]
        [MemberDataTuple(nameof(PointAdjacencyPairs))]
        public void NeighborsUnordered(Point point, AdjacencyRule rule)
        {
            Point[] result = rule.Neighbors(point).ToArray();
            Point[] expected = rule.DirectionsOfNeighbors().Select(i => point + i).ToArray();

            TestUtils.AssertElementEquals(result, expected);
        }

        #endregion

        #region AdjacencyRuleTypeToAdjacencyRuleConversion

        [Theory]
        [MemberDataTuple(nameof(TypeAdjacencyRuleConversion))]
        public void AdjacencyRuleTypeConversion(AdjacencyRule.Types type, AdjacencyRule expectedRule)
        {
            AdjacencyRule dir = type;
            Assert.Equal(expectedRule, dir);
        }

        #endregion

        #region TestData

        public static AdjacencyRule[] AdjacencyRules
            => new[] { AdjacencyRule.Cardinals, AdjacencyRule.Diagonals, AdjacencyRule.EightWay };

        public static Direction[] EightWayDirectionsClockwise
            => new[]
            {
                Direction.Up, Direction.UpRight, Direction.Right, Direction.DownRight, Direction.Down,
                Direction.DownLeft, Direction.Left, Direction.UpLeft
            };

        public static Direction[] DiagonalDirectionsClockwise
            => new[] { Direction.UpRight, Direction.DownRight, Direction.DownLeft, Direction.UpLeft };

        public static Direction[] CardinalsDirectionsClockwise
            => new[] { Direction.Up, Direction.Right, Direction.Down, Direction.Left };

        public static IEnumerable<(AdjacencyRule, Direction)> ClockwisePairs
            => AdjacencyRule.EightWay.ToEnumerable().Combinate(EightWayDirectionsClockwise)
                .Concat(AdjacencyRule.Cardinals.ToEnumerable().Combinate(CardinalsDirectionsClockwise))
                .Concat(AdjacencyRule.Diagonals.ToEnumerable().Combinate(DiagonalDirectionsClockwise));

        public static IEnumerable<(AdjacencyRule, Direction)> AdjacencyDefaultsClockwise
            => TestUtils.Enumerable((AdjacencyRule.Cardinals, Direction.Up), (AdjacencyRule.EightWay, Direction.Up),
                (AdjacencyRule.Diagonals, Direction.UpRight));

        public static IEnumerable<(AdjacencyRule, Direction)> AdjacencyDefaultsCounterClockwise
            => TestUtils.Enumerable((AdjacencyRule.Cardinals, Direction.Up), (AdjacencyRule.EightWay, Direction.Up),
                (AdjacencyRule.Diagonals, Direction.UpLeft));

        public static IEnumerable<(AdjacencyRule, Direction)> RotatePairs =>
            AdjacencyRule.Cardinals.ToEnumerable().Combinate(DiagonalDirectionsClockwise)
                .Concat(AdjacencyRule.Diagonals.ToEnumerable().Combinate(CardinalsDirectionsClockwise));

        public static IEnumerable<(Point, AdjacencyRule)> PointAdjacencyPairs =>
            TestUtils.Enumerable<Point>((1, 2), (-1, -2), (0, 4), (3, 0)).Combinate(AdjacencyRules);

        public static Direction[] ValidDirections = AdjacencyRule.EightWay.DirectionsOfNeighborsClockwise().ToArray();
        public static Direction[] AllDirections => ValidDirections.Append(Direction.None).ToArray();

        public static IEnumerable<(Point, AdjacencyRule, Direction)> PointClockwisePairs =>
            PointAdjacencyPairs.Combinate(AllDirections);

        public static (AdjacencyRule.Types, AdjacencyRule)[] TypeAdjacencyRuleConversion
            => new[]
            {
                (AdjacencyRule.Types.Cardinals, AdjacencyRule.Cardinals),
                (AdjacencyRule.Types.Diagonals, AdjacencyRule.Diagonals),
                (AdjacencyRule.Types.EightWay, AdjacencyRule.EightWay)
            };

        #endregion

        #region ValidDirections

        [Theory]
        [MemberDataTuple(nameof(ClockwisePairs))]
        public void ValidDirectionsClockwiseOrder(AdjacencyRule rule, Direction startingDir)
        {
            Direction[] dirs;
            switch (rule.Type)
            {
                case AdjacencyRule.Types.Cardinals:
                    dirs = CardinalsDirectionsClockwise.ToArray();
                    break;
                case AdjacencyRule.Types.Diagonals:
                    dirs = DiagonalDirectionsClockwise.ToArray();
                    break;
                case AdjacencyRule.Types.EightWay:
                    dirs = EightWayDirectionsClockwise.ToArray();
                    break;
                default:
                    throw new InvalidOperationException($"Unit test does not support direction type: {rule}.");
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
            Direction[] dirs;
            switch (rule.Type)
            {
                case AdjacencyRule.Types.Cardinals:
                    dirs = CardinalsDirectionsClockwise.Reverse().ToArray();
                    break;
                case AdjacencyRule.Types.Diagonals:
                    dirs = DiagonalDirectionsClockwise.Reverse().ToArray();
                    break;
                case AdjacencyRule.Types.EightWay:
                    dirs = EightWayDirectionsClockwise.Reverse().ToArray();
                    break;
                default:
                    throw new InvalidOperationException($"Unit test does not support AdjacencyRule: {rule}.");
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
            Direction[] noneDirs = rule.DirectionsOfNeighborsClockwise(Direction.None).ToArray();
            Direction[] noDirs = rule.DirectionsOfNeighborsClockwise().ToArray();

            TestUtils.AssertElementEquals(defaultDirs, noneDirs, noDirs);
        }

        [Theory]
        [MemberDataTuple(nameof(AdjacencyDefaultsCounterClockwise))]
        public void NoneDirectionCounterClockwise(AdjacencyRule rule, Direction defaultDir)
        {
            Direction[] defaultDirs = rule.DirectionsOfNeighborsCounterClockwise(defaultDir).ToArray();
            Direction[] noneDirs = rule.DirectionsOfNeighborsCounterClockwise(Direction.None).ToArray();
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

        #region NeighborsClockwise

        [Theory]
        [MemberDataTuple(nameof(PointClockwisePairs))]
        public void NeighborsClockwise(Point point, AdjacencyRule rule, Direction startDir)
        {
            Point[] result = rule.NeighborsClockwise(point, startDir).ToArray();
            Point[] expected = rule.DirectionsOfNeighborsClockwise(startDir).Select(i => point + i).ToArray();

            TestUtils.AssertElementEquals(result, expected);
        }

        [Theory]
        [MemberDataTuple(nameof(PointAdjacencyPairs))]
        public void NeighborsClockwiseNoStart(Point point, AdjacencyRule rule)
        {
            Point[] result = rule.NeighborsClockwise(point).ToArray();
            Point[] expected = rule.NeighborsClockwise(point).ToArray();

            TestUtils.AssertElementEquals(expected, result);
        }

        #endregion

        #region NeighborsCounterClockwise

        [Theory]
        [MemberDataTuple(nameof(PointClockwisePairs))]
        public void NeighborsCounterClockwise(Point point, AdjacencyRule rule, Direction startDir)
        {
            Point[] result = rule.NeighborsCounterClockwise(point, startDir).ToArray();
            Point[] expected = rule.DirectionsOfNeighborsCounterClockwise(startDir).Select(i => point + i).ToArray();

            TestUtils.AssertElementEquals(result, expected);
        }

        [Theory]
        [MemberDataTuple(nameof(PointAdjacencyPairs))]
        public void NeighborsCounterClockwiseNoStart(Point point, AdjacencyRule rule)
        {
            Point[] result = rule.NeighborsCounterClockwise(point).ToArray();
            Point[] expected = rule.NeighborsCounterClockwise(point).ToArray();

            TestUtils.AssertElementEquals(expected, result);
        }

        #endregion

        #region Equality/Inequality

        [Theory]
        [MemberDataEnumerable(nameof(AdjacencyRules))]
        public void TestEquality(AdjacencyRule rule)
        {
            AdjacencyRule compareTo = rule;
            AdjacencyRule[] allRules = AdjacencyRules;
            Assert.True(rule == compareTo);

            Assert.Equal(1, allRules.Count(i => i == compareTo));
        }

        [Theory]
        [MemberDataEnumerable(nameof(AdjacencyRules))]
        public void TestInequality(AdjacencyRule rule)
        {
            AdjacencyRule compareTo = rule;
            AdjacencyRule[] allRules = AdjacencyRules;
            Assert.False(rule != compareTo);

            Assert.Equal(allRules.Length - 1, allRules.Count(i => i != compareTo));
        }

        [Theory]
        [MemberDataEnumerable(nameof(AdjacencyRules))]
        public void TestEqualityInqeualityOpposite(AdjacencyRule compareRule)
        {
            AdjacencyRule[] rules = AdjacencyRules;

            foreach (AdjacencyRule rule in rules)
                Assert.NotEqual(rule == compareRule, rule != compareRule);
        }

        [Theory]
        [MemberDataEnumerable(nameof(AdjacencyRules))]
        public void TestEqualityEquivalence(AdjacencyRule compareRule)
        {
            AdjacencyRule[] rules = AdjacencyRules;

            foreach (AdjacencyRule rule in rules)
            {
                Assert.Equal(rule == compareRule, rule.Equals(compareRule));
                Assert.Equal(rule == compareRule, rule.Equals((object)compareRule));
            }
        }

        #endregion
    }
}
