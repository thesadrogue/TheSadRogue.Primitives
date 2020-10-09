using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    // We must do these sequentially (not in parallel), as some of the tests change YIncreasesUpwards, which can very much mess with other threads using any function
    // that relies on directions.  There is a feature of XUnit that should technically allow these to be in the same assembly, but it seems to only separate the collection
    // from itself, and attempting to extrapolate which functions view directions is time consuming and the frequent switching of that flag is not something that should be done
    // in production anyway.  Thus anything that manipulates shared state will be in this assembly.
    public class DirectionTests
    {
        #region TestData

        public static Direction[] ValidDirections = AdjacencyRule.EightWay.DirectionsOfNeighborsClockwise().ToArray();
        public static Direction[] AllDirections => ValidDirections.Append(Direction.None).ToArray();
        public static Point[] TestPoints => new[] { new Point(1, 2), new Point(0, 0), new Point(-2, -5) };
        public static IEnumerable<(Point, Direction)> PointDirPairs => TestPoints.Combinate(AllDirections);

        public static IEnumerable<(Direction, int)> AddSubDirPairs
            => ValidDirections.Combinate(Enumerable.Range(0, 11));

        public static IEnumerable<(Point, Point, Direction)> GetDirectionBasePairs =>
            new (Point p1, Point p2, Direction d)[]
            {
                ((0, 0), (0, 0), Direction.None), ((0, 0), (0, 0) + Direction.Up, Direction.Up),
                ((0, 0), (0, 0) + Direction.UpRight, Direction.UpRight),
                ((0, 0), (0, 0) + Direction.Right, Direction.Right),
                ((0, 0), (0, 0) + Direction.DownRight, Direction.DownRight),
                ((0, 0), (0, 0) + Direction.Down, Direction.Down),
                ((0, 0), (0, 0) + Direction.DownLeft, Direction.DownLeft),
                ((0, 0), (0, 0) + Direction.Left, Direction.Left),
                ((0, 0), (0, 0) + Direction.UpLeft, Direction.UpLeft)
            };

        private static IEnumerable<(Point, Point, Direction)> GetCardinalDirectionBasePairs =>
            new (Point p1, Point p2, Direction d)[]
            {
                ((0, 0), (0, 0), Direction.None), ((0, 0), (0, 0) + Direction.Up, Direction.Up),
                ((0, 0), (0, 0) + Direction.UpRight, Direction.Right),
                ((0, 0), (0, 0) + Direction.Right, Direction.Right),
                ((0, 0), (0, 0) + Direction.DownRight, Direction.Down),
                ((0, 0), (0, 0) + Direction.Down, Direction.Down),
                ((0, 0), (0, 0) + Direction.DownLeft, Direction.Left),
                ((0, 0), (0, 0) + Direction.Left, Direction.Left), ((0, 0), (0, 0) + Direction.UpLeft, Direction.Up)
            };

        public static IEnumerable<(Point, Point, Direction)> GetDirectionPairs
            => GetDirectionBasePairs.Concat(GetDirectionBasePairs.Select(i => (i.Item1 + 5, i.Item2 + 5, i.Item3)));

        public static IEnumerable<(Point, Point, Direction)> GetCardinalDirectionPairs
            => GetCardinalDirectionBasePairs.Concat(
                GetCardinalDirectionBasePairs.Select(i => (i.Item1 + 5, i.Item2 + 5, i.Item3)));

        public static IEnumerable<(Direction.Types, Direction)> TypeDirectionConversion => TestUtils.Enumerable(
            (Direction.Types.None, Direction.None),
            (Direction.Types.Up, Direction.Up),
            (Direction.Types.UpRight, Direction.UpRight),
            (Direction.Types.Right, Direction.Right),
            (Direction.Types.DownRight, Direction.DownRight),
            (Direction.Types.Down, Direction.Down),
            (Direction.Types.DownLeft, Direction.DownLeft),
            (Direction.Types.Left, Direction.Left),
            (Direction.Types.UpLeft, Direction.UpLeft)
        );

        public static IEnumerable<(Direction, Point, Point)> YIncreasesDeltaValues
            => TestUtils.Enumerable<(Direction, Point, Point)>(
                (Direction.None, (0, 0), (0, 0)),
                (Direction.Up, (0, -1), (0, 1)),
                (Direction.UpRight, (1, -1), (1, 1)),
                (Direction.Right, (1, 0), (1, 0)),
                (Direction.DownRight, (1, 1), (1, -1)),
                (Direction.Down, (0, 1), (0, -1)),
                (Direction.DownLeft, (-1, 1), (-1, -1)),
                (Direction.Left, (-1, 0), (-1, 0)),
                (Direction.UpLeft, (-1, -1), (-1, 1))
            );

        public static IEnumerable<(Direction, bool)> IsCardinalPairs
            => AdjacencyRule.Cardinals.DirectionsOfNeighbors().Combinate(true.ToEnumerable())
                .Concat(AdjacencyRule.Diagonals.DirectionsOfNeighbors().Combinate(false.ToEnumerable()))
                .Append((Direction.None, false));

        #endregion


        [Fact]
        public void NoneIsDefault() =>
            // This must be the case for optional Direction parameters as currently implemented in the library to function properly.
            Assert.Equal(default, Direction.None);

        #region Equality/Inequality

        [Theory]
        [MemberDataEnumerable(nameof(ValidDirections))]
        public void TestValidEquality(Direction dir)
        {
            Direction compareTo = dir;
            Assert.True(dir == compareTo);
            Assert.False(dir == Direction.None);

            for (int i = 0; i < 7; i++)
            {
                dir++;
                Assert.False(dir == compareTo);
            }
        }

        [Fact]
        public void TestInvalidEquality()
        {
            Direction none = Direction.None;
            Assert.True(none == Direction.None);
            foreach (Direction i in ValidDirections)
                Assert.False(Direction.None == i);
        }

        [Theory]
        [MemberDataEnumerable(nameof(ValidDirections))]
        public void TestValidInequality(Direction dir)
        {
            Direction compareTo = dir;
            Assert.False(dir != compareTo);
            Assert.True(dir != Direction.None);

            for (int i = 0; i < 7; i++)
            {
                dir++;
                Assert.True(dir != compareTo);
            }
        }

        [Fact]
        public void TestInvalidInequality()
        {
            Direction none = Direction.None;
            Assert.False(none != Direction.None);
            foreach (Direction i in ValidDirections)
                Assert.True(Direction.None != i);
        }

        [Theory]
        [MemberDataEnumerable(nameof(AllDirections))]
        public void TestEqualityInequalityOpposite(Direction compareDir)
        {
            Direction[] dirs = AllDirections;

            foreach (Direction dir in dirs)
                Assert.NotEqual(dir == compareDir, dir != compareDir);
        }

        [Theory]
        [MemberDataEnumerable(nameof(AllDirections))]
        public void TestEqualityEquivalence(Direction compareDir)
        {
            Direction[] dirs = AllDirections;

            foreach (Direction dir in dirs)
            {
                Assert.Equal(dir == compareDir, dir.Equals(compareDir));
                Assert.Equal(dir == compareDir, dir.Equals((object)compareDir));
            }
        }

        #endregion

        #region Addition/Subtraction

        [Theory]
        [MemberDataTuple(nameof(PointDirPairs))]
        public void AddToPoint(Point start, Direction dir)
        {
            // Should be false to start with
            Assert.False(Direction.YIncreasesUpward);

            Point res = start + dir;
            Assert.Equal(start.X + dir.DeltaX, res.X);
            Assert.Equal(start.Y + dir.DeltaY, res.Y);

            // Set to true
            Direction.SetYIncreasesUpwardsUnsafe(true);

            // Retest
            res = start + dir;
            Assert.Equal(start.X + dir.DeltaX, res.X);
            Assert.Equal(start.Y + dir.DeltaY, res.Y);

            // Reset for next test
            Direction.SetYIncreasesUpwardsUnsafe(false);
        }

        [Theory]
        [MemberDataTuple(nameof(AddSubDirPairs))]
        public void AddToDirection(Direction dir, int value)
        {
            Direction[] dirs = ValidDirections;

            int start = Array.FindIndex(dirs, x => x == dir);
            Assert.False(start == -1, "Could not find direction in valid directions list.");

            Direction res = dir + value;
            Assert.Equal(dirs[(start + value) % dirs.Length], res);
        }

        [Theory]
        [MemberDataTuple(nameof(AddSubDirPairs))]
        public void SubFromDirection(Direction dir, int value)
        {
            Direction[] dirs = ValidDirections;

            int start = Array.FindIndex(dirs, x => x == dir);
            Assert.False(start == -1, "Could not find direction in valid directions list.");

            Direction res = dir - value;
            int index = start - Math.Abs(value) % dirs.Length;
            if (index < 0)
                index = dirs.Length + index;

            Assert.Equal(dirs[index], res);
        }

        [Theory]
        [MemberDataTuple(nameof(PointDirPairs))]
        public void SubFromPoint(Point start, Direction dir)
        {
            // Should be false to start with
            Assert.False(Direction.YIncreasesUpward);

            Point res = start - dir;
            Assert.Equal(start.X - dir.DeltaX, res.X);
            Assert.Equal(start.Y - dir.DeltaY, res.Y);

            // Set to true
            Direction.SetYIncreasesUpwardsUnsafe(true);

            // Retest
            res = start - dir;
            Assert.Equal(start.X - dir.DeltaX, res.X);
            Assert.Equal(start.Y - dir.DeltaY, res.Y);

            // Reset for next test
            Direction.SetYIncreasesUpwardsUnsafe(false);
        }

        [Theory]
        [MemberDataEnumerable(nameof(ValidDirections))]
        public void IncrementValidDirection(Direction startingDir)
        {
            Direction oldDir = startingDir;
            Direction[] dirs = ValidDirections;
            int start = Array.FindIndex(dirs, x => x == startingDir);
            Assert.False(start == -1, "Couldn't find starting direction in directions.");

            for (int i = 1; i <= 8; i++)
            {
                startingDir++;
                Assert.Equal(dirs[(start + i) % dirs.Length], startingDir);
            }

            Assert.Equal(oldDir, startingDir);
        }

        [Theory]
        [MemberDataEnumerable(nameof(ValidDirections))]
        public void DecrementValidDirection(Direction startingDir)
        {
            Direction oldDir = startingDir;
            Direction[] dirs = ValidDirections;
            int start = Array.FindIndex(dirs, x => x == startingDir);
            Assert.False(start == -1, "Couldn't find starting direction in directions.");

            for (int i = 1; i <= 8; i++)
            {
                startingDir--;
                int index = start - i;
                if (index < 0)
                    index = dirs.Length + index;

                Assert.Equal(dirs[index], startingDir);
            }

            Assert.Equal(oldDir, startingDir);
        }

        #endregion

        #region DirectionTypeToDirectionConversion

        [Theory]
        [MemberDataTuple(nameof(TypeDirectionConversion))]
        public void DirectionTypeConversion(Direction.Types type, Direction expectedDir)
            => Assert.Equal(expectedDir, (Direction)type);

        #endregion

        #region YIncreasesUpwardsToggles

        [Theory]
        [MemberDataTuple(nameof(YIncreasesDeltaValues))]
        public void YIncreasesUpwardDeltaToggle(Direction dir, Point expectedPreDeltas,
                                                Point expectedPostDeltas)
        {
            Assert.False(Direction.YIncreasesUpward, "Direction.YIncreasesUpwards is expected to be false as default.");

            Assert.Equal(expectedPreDeltas.X, dir.DeltaX);
            Assert.Equal(expectedPreDeltas.Y, dir.DeltaY);

            Direction.SetYIncreasesUpwardsUnsafe(true);
            Assert.Equal(expectedPostDeltas.X, dir.DeltaX);
            Assert.Equal(expectedPostDeltas.Y, dir.DeltaY);

            Direction.SetYIncreasesUpwardsUnsafe(false);
            Assert.Equal(expectedPreDeltas.X, dir.DeltaX);
            Assert.Equal(expectedPreDeltas.Y, dir.DeltaY);
        }

        [Theory]
        [MemberDataEnumerable(nameof(AllDirections))]
        public void YIncreaseUpwardEquality(Direction dir)
        {
            Assert.False(Direction.YIncreasesUpward, "Direction.YIncreasesUpwards is expected to be false as default.");
            Assert.True(dir == dir.Type);
            Assert.True(dir.Equals(dir.Type));
            Assert.True(dir.Equals((object)(Direction)dir.Type));

            Direction.SetYIncreasesUpwardsUnsafe(true);
            Assert.True(dir == dir.Type);
            Assert.True(dir.Equals(dir.Type));
            Assert.True(dir.Equals((object)(Direction)dir.Type));

            Direction.SetYIncreasesUpwardsUnsafe(false);
            Assert.True(dir == dir.Type);
            Assert.True(dir.Equals(dir.Type));
            Assert.True(dir.Equals((object)(Direction)dir.Type));
        }

        #endregion

        #region GetDirection/GetCardinalDirection

        [Fact]
        public void GetDirection()
        {
            Assert.False(Direction.YIncreasesUpward, "Direction.YIncreasesUpwards is expected to be false as default.");
            (Point p1, Point p2, Direction expectedDir)[] testCases = GetDirectionPairs.ToArray();

            foreach ((Point p1, Point p2, Direction expectedDir) in testCases)
            {
                Direction dir = Direction.GetDirection(p1, p2);
                Assert.Equal(expectedDir, dir);
            }

            // The test cases create the second point by adding a direction, so if we re-grab the enumerable after flipping
            // the YIncreasesUpwards flag, everything should still match up if we're accounting for the y-deltas properly.
            Direction.SetYIncreasesUpwardsUnsafe(true);
            testCases = GetDirectionPairs.ToArray();
            foreach ((Point p1, Point p2, Direction expectedDir) in testCases)
            {
                Direction dir = Direction.GetDirection(p1, p2);
                Assert.Equal(expectedDir, dir);
            }

            Direction.SetYIncreasesUpwardsUnsafe(false);
        }

        [Fact]
        public void GetDirectionDelta()
        {
            Assert.False(Direction.YIncreasesUpward, "Direction.YIncreasesUpwards is expected to be false as default.");
            (Point p1, Point p2, Direction expectedDir)[] testCases = GetDirectionPairs.ToArray();

            foreach ((Point p1, Point p2, Direction expectedDir) in testCases)
            {
                Point delta = p2 - p1;
                Direction dir = Direction.GetDirection(delta);
                Assert.Equal(expectedDir, dir);
            }
        }

        [Fact]
        public void GetCardinalDirection()
        {
            Assert.False(Direction.YIncreasesUpward, "Direction.YIncreasesUpwards is expected to be false as default.");
            (Point p1, Point p2, Direction expectedDir)[] testCases = GetCardinalDirectionPairs.ToArray();

            foreach ((Point p1, Point p2, Direction expectedDir) in testCases)
            {
                Direction dir = Direction.GetCardinalDirection(p1, p2);
                Assert.Equal(expectedDir, dir);
            }

            // The test cases create the second point by adding a direction, so if we re-grab the enumerable after flipping
            // the YIncreasesUpwards flag, everything should still match up if we're accounting for the y-deltas properly.
            Direction.SetYIncreasesUpwardsUnsafe(true);
            testCases = GetCardinalDirectionPairs.ToArray();
            foreach ((Point p1, Point p2, Direction expectedDir) in testCases)
            {
                Direction dir = Direction.GetCardinalDirection(p1, p2);
                Assert.Equal(expectedDir, dir);
            }

            Direction.SetYIncreasesUpwardsUnsafe(false);
        }

        [Fact]
        public void GetCardinalDirectionDelta()
        {
            Assert.False(Direction.YIncreasesUpward, "Direction.YIncreasesUpwards is expected to be false as default.");
            (Point p1, Point p2, Direction expectedDir)[] testCases = GetCardinalDirectionPairs.ToArray();

            foreach ((Point p1, Point p2, Direction expectedDir) in testCases)
            {
                Point delta = p2 - p1;
                Direction dir = Direction.GetCardinalDirection(delta);
                Assert.Equal(expectedDir, dir);
            }
        }

        #endregion

        [Theory]
        [MemberDataTuple(nameof(IsCardinalPairs))]
        public void IsCardinal(Direction dir, bool expected) => Assert.Equal(expected, dir.IsCardinal());
    }
}
