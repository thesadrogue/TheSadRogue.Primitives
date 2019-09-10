using System;
using System.Linq;
using Xunit;
using XUnit.ValueTuples;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace TheSadRogue.Primitives.UnitTests
{
    // We must do these sequentially (not in paralell), as some of the tests change YIncreasesUpwards, which can very much mess with other threads using any function
    // that relies on directions.  There is a feature of XUnit that should technically allow these to be in the same assembly, but it seems to only separate the collection
    // from itself, and attempting to extrapolate which functions view directions is time consuming and the frequent switching of that flag is not something that should be done
    // in production anyway.  Thus anything that manipulates shared state will be in this assembly.
    public class DirectionTests
    {
        #region TestData
        static public Direction[] ValidDirections = AdjacencyRule.EIGHT_WAY.DirectionsOfNeighborsClockwise().ToArray();
        static public Direction[] AllDirections => ValidDirections.Append(Direction.NONE).ToArray();
        static public Point[] TestPoints => new[] { new Point(1, 2), new Point(0, 0), new Point(-2, -5) };
        static public IEnumerable<(Point, Direction)> CoordDirPairs => TestPoints.Combinate(AllDirections);

        static public IEnumerable<(Direction, int)> AddSubDirPairs => ValidDirections.Combinate(Enumerable.Range(0, 11));

        static public IEnumerable<(Point, Point, Direction)> GetDirectionBasePairs =>
            new (Point p1, Point p2, Direction d)[] {
            ((0, 0), (0, 0), Direction.NONE),
            ((0, 0), (0, 0) + Direction.UP, Direction.UP),
            ((0, 0), (0, 0) + Direction.UP_RIGHT, Direction.UP_RIGHT),
            ((0, 0), (0, 0) + Direction.RIGHT, Direction.RIGHT),
            ((0, 0), (0, 0) + Direction.DOWN_RIGHT, Direction.DOWN_RIGHT),
            ((0, 0), (0, 0) + Direction.DOWN, Direction.DOWN),
            ((0, 0), (0, 0) + Direction.DOWN_LEFT, Direction.DOWN_LEFT),
            ((0, 0), (0, 0) + Direction.LEFT, Direction.LEFT),
            ((0, 0), (0, 0) + Direction.UP_LEFT, Direction.UP_LEFT)
        };

        static public IEnumerable<(Point, Point, Direction)> GetCardinalDirectionBasePairs =>
            new (Point p1, Point p2, Direction d)[] {
            ((0, 0), (0, 0), Direction.NONE),
            ((0, 0), (0, 0) + Direction.UP, Direction.UP),
            ((0, 0), (0, 0) + Direction.UP_RIGHT, Direction.RIGHT),
            ((0, 0), (0, 0) + Direction.RIGHT, Direction.RIGHT),
            ((0, 0), (0, 0) + Direction.DOWN_RIGHT, Direction.DOWN),
            ((0, 0), (0, 0) + Direction.DOWN, Direction.DOWN),
            ((0, 0), (0, 0) + Direction.DOWN_LEFT, Direction.LEFT),
            ((0, 0), (0, 0) + Direction.LEFT, Direction.LEFT),
            ((0, 0), (0, 0) + Direction.UP_LEFT, Direction.UP)
        };

        static public IEnumerable<(Point, Point, Direction)> GetDirectionPairs
            => GetDirectionBasePairs.Concat(GetDirectionBasePairs.Select(i => (i.Item1 + 5, i.Item2 + 5, i.Item3)));

        static public IEnumerable<(Point, Point, Direction)> GetCardinalDirectionPairs
            => GetCardinalDirectionBasePairs.Concat(GetCardinalDirectionBasePairs.Select(i => (i.Item1 + 5, i.Item2 + 5, i.Item3)));

        static public IEnumerable<(Direction.Types, Direction)> TypeDirectionConversion => TestUtils.Enumerable(
            (Direction.Types.NONE, Direction.NONE),
            (Direction.Types.UP, Direction.UP),
            (Direction.Types.UP_RIGHT, Direction.UP_RIGHT),
            (Direction.Types.RIGHT, Direction.RIGHT),
            (Direction.Types.DOWN_RIGHT, Direction.DOWN_RIGHT),
            (Direction.Types.DOWN, Direction.DOWN),
            (Direction.Types.DOWN_LEFT, Direction.DOWN_LEFT),
            (Direction.Types.LEFT, Direction.LEFT),
            (Direction.Types.UP_LEFT, Direction.UP_LEFT)
        );

        static public IEnumerable<(Direction, Point, Point)> YIncreasesDeltaValues => TestUtils.Enumerable<(Direction, Point, Point)>(
            (Direction.NONE, (0, 0), (0, 0)),
            (Direction.UP, (0, -1), (0, 1)),
            (Direction.UP_RIGHT, (1, -1), (1, 1)),
            (Direction.RIGHT, (1, 0), (1, 0)),
            (Direction.DOWN_RIGHT, (1, 1), (1, -1)),
            (Direction.DOWN, (0, 1), (0, -1)),
            (Direction.DOWN_LEFT, (-1, 1), (-1, -1)),
            (Direction.LEFT, (-1, 0), (-1, 0)),
            (Direction.UP_LEFT, (-1, -1), (-1, 1))
        );

        static public IEnumerable<(Direction, bool)> IsCardinalPairs
            => AdjacencyRule.CARDINALS.DirectionsOfNeighbors().Combinate(true.ToEnumerable())
            .Concat(AdjacencyRule.DIAGONALS.DirectionsOfNeighbors().Combinate(false.ToEnumerable()))
            .Append((Direction.NONE, false));
        #endregion


        [Fact]
        public void NoneIsDefault()
        {
            // This must be the case for optional Direction parameters as currently implemented in the library to function properly.
            Assert.Equal(default, Direction.NONE);
        }

        #region Equality/Inequality
        [Theory]
        [MemberDataEnumerable(nameof(ValidDirections))]
        public void TestValidEquality(Direction dir)
        {
            Direction compareTo = dir;
            Assert.True(dir == compareTo);
            Assert.False(dir == Direction.NONE);

            for (int i = 0; i < 7; i++)
            {
                dir++;
                Assert.False(dir == compareTo);
            }
        }
        
        [Fact]
        public void TestInvalidEquality()
        {
            Direction none = Direction.NONE;
            Assert.True(none == Direction.NONE);
            foreach (var i in ValidDirections)
                Assert.False(Direction.NONE == i);
        }

        [Theory]
        [MemberDataEnumerable(nameof(ValidDirections))]
        public void TestValidInequality(Direction dir)
        {
            Direction compareTo = dir;
            Assert.False(dir != compareTo);
            Assert.True(dir != Direction.NONE);

            for (int i = 0; i < 7; i++)
            {
                dir++;
                Assert.True(dir != compareTo);
            }
        }

        [Fact]
        public void TestInvalidInequality()
        {
            Direction none = Direction.NONE;
            Assert.False(none != Direction.NONE);
            foreach (var i in ValidDirections)
                Assert.True(Direction.NONE != i);
        }

        [Theory]
        [MemberDataEnumerable(nameof(AllDirections))]
        public void TestEqualityInqeualityOpposite(Direction compareDir)
        {
            var dirs = AllDirections;

            foreach (var dir in dirs)
                Assert.NotEqual(dir == compareDir, dir != compareDir);
        }
        [Theory]
        [MemberDataEnumerable(nameof(AllDirections))]
        public void TestEqualityEquivalence(Direction compareDir)
        {
            var dirs = AllDirections;

            foreach (var dir in dirs)
            {
                Assert.Equal(dir == compareDir, dir.Equals(compareDir));
                Assert.Equal(dir == compareDir, dir.Equals((object)compareDir));
            }
        }
        #endregion

        #region Addition/Subtraction
        [Theory]
        [MemberDataTuple(nameof(CoordDirPairs))]
        public void AddToPoint(Point start, Direction dir)
        {
            Point res = start + dir;
            Assert.Equal(start.X + dir.DeltaX, res.X);
            Assert.Equal(start.Y + dir.DeltaY, res.Y);
        }

        [Theory]
        [MemberDataTuple(nameof(AddSubDirPairs))]
        public void AddToDirection(Direction dir, int value)
        {
            var dirs = ValidDirections;

            int start = Array.FindIndex(dirs, x => x == dir);
            Assert.False(start == -1, "Could not find direction in valid directions list.");

            Direction res = dir + value;
            Assert.Equal(dirs[(start + value) % dirs.Length], res);
        }

        [Theory]
        [MemberDataTuple(nameof(AddSubDirPairs))]
        public void SubFromDirection(Direction dir, int value)
        {
            var dirs = ValidDirections;

            int start = Array.FindIndex(dirs, x => x == dir);
            Assert.False(start == -1, "Could not find direction in valid directions list.");

            Direction res = dir - value;
            int index = start - (Math.Abs(value) % dirs.Length);
            if (index < 0)
                index = dirs.Length + index;

            Assert.Equal(dirs[index], res);
        }

        [Theory]
        [MemberDataEnumerable(nameof(ValidDirections))]
        public void IncrementValidDirection(Direction startingDir)
        {
            Direction oldDir = startingDir;
            var dirs = ValidDirections;
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
            var dirs = ValidDirections;
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
        {
            Direction dir = Direction.ToDirection(type);
            Assert.Equal(expectedDir, dir);
        }
        #endregion

        #region YIncreasesUpwardsToggles
        [Theory]
        [MemberDataTuple(nameof(YIncreasesDeltaValues))]
        public void YIncreasesUpwardDeltaToggle(Direction dir, (int dx, int dy) expectedPreDeltas, (int dx, int dy) expectedPostDeltas)
        {
            Assert.False(Direction.YIncreasesUpward, "Direction.YIncreasesUpwards is expected to be false as default.");

            Assert.Equal(expectedPreDeltas.dx, dir.DeltaX);
            Assert.Equal(expectedPreDeltas.dy, dir.DeltaY);

            Direction.SetYIncreasesUpwardsUnsafe(true);
            Assert.Equal(expectedPostDeltas.dx, dir.DeltaX);
            Assert.Equal(expectedPostDeltas.dy, dir.DeltaY);

            Direction.SetYIncreasesUpwardsUnsafe(false);
            Assert.Equal(expectedPreDeltas.dx, dir.DeltaX);
            Assert.Equal(expectedPreDeltas.dy, dir.DeltaY);
        }

        [Theory]
        [MemberDataEnumerable(nameof(AllDirections))]
        public void YIncreaseUpwardEquality(Direction dir)
        {
            Assert.False(Direction.YIncreasesUpward, "Direction.YIncreasesUpwards is expected to be false as default.");
            Assert.True(dir == Direction.ToDirection(dir.Type));
            Assert.True(dir.Equals(Direction.ToDirection(dir.Type)));
            Assert.True(dir.Equals((object)Direction.ToDirection(dir.Type)));

            Direction.SetYIncreasesUpwardsUnsafe(true);
            Assert.True(dir == Direction.ToDirection(dir.Type));
            Assert.True(dir.Equals(Direction.ToDirection(dir.Type)));
            Assert.True(dir.Equals((object)Direction.ToDirection(dir.Type)));

            Direction.SetYIncreasesUpwardsUnsafe(false);
            Assert.True(dir == Direction.ToDirection(dir.Type));
            Assert.True(dir.Equals(Direction.ToDirection(dir.Type)));
            Assert.True(dir.Equals((object)Direction.ToDirection(dir.Type)));
        }
        #endregion

        #region GetDirection/GetCardinalDirection
        [Fact]
        public void GetDirection()
        {
            Assert.False(Direction.YIncreasesUpward, "Direction.YIncreasesUpwards is expected to be false as default.");
            (Point p1, Point p2, Direction expectedDir)[] testCases = GetDirectionPairs.ToArray();

            foreach (var testCase in testCases)
            {
                Direction dir = Direction.GetDirection(testCase.p1, testCase.p2);
                Assert.Equal(testCase.expectedDir, dir);
            }

            // The test cases create the second point by adding a direction, so if we re-grab the enumerable after flipping
            // the YIncreasesUpwards flag, everything should still match up if we're accounting for the y-deltas properly.
            Direction.SetYIncreasesUpwardsUnsafe(true);
            testCases = GetDirectionPairs.ToArray();
            foreach (var testCase in testCases)
            {
                Direction dir = Direction.GetDirection(testCase.p1, testCase.p2);
                Assert.Equal(testCase.expectedDir, dir);
            }

            Direction.SetYIncreasesUpwardsUnsafe(false);
        }

        [Fact]
        public void GetDirectionDelta()
        {
            Assert.False(Direction.YIncreasesUpward, "Direction.YIncreasesUpwards is expected to be false as default.");
            (Point p1, Point p2, Direction expectedDir)[] testCases = GetDirectionPairs.ToArray();

            foreach (var testCase in testCases)
            {
                Point delta = testCase.p2 - testCase.p1;
                Direction dir = Direction.GetDirection(delta);
                Assert.Equal(testCase.expectedDir, dir);
            }
        }

        [Fact]
        public void GetCardinalDirection()
        {
            Assert.False(Direction.YIncreasesUpward, "Direction.YIncreasesUpwards is expected to be false as default.");
            (Point p1, Point p2, Direction expectedDir)[] testCases = GetCardinalDirectionPairs.ToArray();

            foreach (var testCase in testCases)
            {
                Direction dir = Direction.GetCardinalDirection(testCase.p1, testCase.p2);
                Assert.Equal(testCase.expectedDir, dir);
            }

            // The test cases create the second point by adding a direction, so if we re-grab the enumerable after flipping
            // the YIncreasesUpwards flag, everything should still match up if we're accounting for the y-deltas properly.
            Direction.SetYIncreasesUpwardsUnsafe(true);
            testCases = GetCardinalDirectionPairs.ToArray();
            foreach (var testCase in testCases)
            {
                Direction dir = Direction.GetCardinalDirection(testCase.p1, testCase.p2);
                Assert.Equal(testCase.expectedDir, dir);
            }

            Direction.SetYIncreasesUpwardsUnsafe(false);
        }

        [Fact]
        public void GetCardinalDirectionDelta()
        {
            Assert.False(Direction.YIncreasesUpward, "Direction.YIncreasesUpwards is expected to be false as default.");
            (Point p1, Point p2, Direction expectedDir)[] testCases = GetCardinalDirectionPairs.ToArray();

            foreach (var testCase in testCases)
            {
                Point delta = testCase.p2 - testCase.p1;
                Direction dir = Direction.GetCardinalDirection(delta);
                Assert.Equal(testCase.expectedDir, dir);
            }
        }
        #endregion

        [Theory]
        [MemberDataTuple(nameof(IsCardinalPairs))]
        public void IsCardinal(Direction dir, bool expected)
        {
            Assert.Equal(expected, dir.IsCardinal());
        }
    }
}