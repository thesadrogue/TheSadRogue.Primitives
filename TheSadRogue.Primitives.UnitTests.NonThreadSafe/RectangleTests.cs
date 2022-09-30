﻿using System.Collections.Generic;
using System.Linq;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class RectangleTests
    {
        #region Test Data

        public static Rectangle[] TestRectangles = new Rectangle[] { (0, 0, 10, 20), (1, 2, 15, 17), (5, 2, 18, 6) };

        public static (Rectangle rect, bool yIncreaseUpwards)[] TestRectanglesWithYIncreasesUpwards =
            TestRectangles.Combinate(TestUtils.Enumerable(true, false)).ToArray();

        #endregion

        #region Test Side Points

        [Theory]
        [MemberDataTuple(nameof(TestRectanglesWithYIncreasesUpwards))]
        public void TopEdgePositions(Rectangle rect, bool yIncreaseUpwards)
        {
            Direction.SetYIncreasesUpwardsUnsafe(yIncreaseUpwards);

            // Determine "top"
            int yToCheck = yIncreaseUpwards ? rect.MaxExtentY : rect.MinExtentY;
            // Get edge points
            Point[] edgePoints = rect.PositionsOnSide(Direction.Up).ToArray();
            HashSet<Point> hashSet = new HashSet<Point>(edgePoints);

            // Verify no duplicates
            Assert.Equal(edgePoints.Length, hashSet.Count);

            // Verify correct points
            HashSet<Point> expectedHash = rect.Positions().ToEnumerable().Where(pos => pos.Y == yToCheck).ToHashSet();
            Assert.Equal(expectedHash, hashSet);

            Direction.SetYIncreasesUpwardsUnsafe(false); // Ensure we reset to false for next test
        }

        [Theory]
        [MemberDataTuple(nameof(TestRectanglesWithYIncreasesUpwards))]
        public void RightEdgePositions(Rectangle rect, bool yIncreaseUpwards)
        {
            Direction.SetYIncreasesUpwardsUnsafe(yIncreaseUpwards);

            // Get edge points
            Point[] edgePoints = rect.PositionsOnSide(Direction.Right).ToArray();
            HashSet<Point> hashSet = new HashSet<Point>(edgePoints);

            // Verify no duplicates
            Assert.Equal(edgePoints.Length, hashSet.Count);

            // Verify correct points
            HashSet<Point> expectedHash = rect.Positions().ToEnumerable().Where(pos => pos.X == rect.MaxExtentX).ToHashSet();
            Assert.Equal(expectedHash, hashSet);

            Direction.SetYIncreasesUpwardsUnsafe(false); // Ensure we reset to false for next test
        }

        [Theory]
        [MemberDataTuple(nameof(TestRectanglesWithYIncreasesUpwards))]
        public void BottomEdgePositions(Rectangle rect, bool yIncreaseUpwards)
        {
            Direction.SetYIncreasesUpwardsUnsafe(yIncreaseUpwards);

            // Determine "bottom"
            int yToCheck = yIncreaseUpwards ? rect.MinExtentY : rect.MaxExtentY;
            // Get edge points
            Point[] edgePoints = rect.PositionsOnSide(Direction.Down).ToArray();
            HashSet<Point> hashSet = new HashSet<Point>(edgePoints);

            // Verify no duplicates
            Assert.Equal(edgePoints.Length, hashSet.Count);

            // Verify correct points
            HashSet<Point> expectedHash = rect.Positions().ToEnumerable().Where(pos => pos.Y == yToCheck).ToHashSet();
            Assert.Equal(expectedHash, hashSet);

            Direction.SetYIncreasesUpwardsUnsafe(false); // Ensure we reset to false for next test
        }

        [Theory]
        [MemberDataTuple(nameof(TestRectanglesWithYIncreasesUpwards))]
        public void LeftEdgePositions(Rectangle rect, bool yIncreaseUpwards)
        {
            Direction.SetYIncreasesUpwardsUnsafe(yIncreaseUpwards);

            // Get edge points
            Point[] edgePoints = rect.PositionsOnSide(Direction.Left).ToArray();
            HashSet<Point> hashSet = new HashSet<Point>(edgePoints);

            // Verify no duplicates
            Assert.Equal(edgePoints.Length, hashSet.Count);

            // Verify correct points
            HashSet<Point> expectedHash = rect.Positions().ToEnumerable().Where(pos => pos.X == rect.MinExtentX).ToHashSet();
            Assert.Equal(expectedHash, hashSet);

            Direction.SetYIncreasesUpwardsUnsafe(false); // Ensure we reset to false for next test
        }

        #endregion

        #region Test Side Identification

        [Theory]
        [MemberDataTuple(nameof(TestRectanglesWithYIncreasesUpwards))]
        public void IsOnTopEdge(Rectangle rect, bool yIncreasesUpwards)
        {
            // Set to test's value
            Direction.SetYIncreasesUpwardsUnsafe(yIncreasesUpwards);

            // Get expected points that should return true
            var expectedPoints = rect.PositionsOnSide(Direction.Up).ToHashSet();

            // Check that all points return the proper value
            foreach (Point point in rect.Positions())
                Assert.Equal(expectedPoints.Contains(point), rect.IsOnSide(point, Direction.Up));

            // Reset to false for next test
            Direction.SetYIncreasesUpwardsUnsafe(false);
        }

        [Theory]
        [MemberDataTuple(nameof(TestRectanglesWithYIncreasesUpwards))]
        public void IsOnRightEdge(Rectangle rect, bool yIncreasesUpwards)
        {
            // Set to test's value
            Direction.SetYIncreasesUpwardsUnsafe(yIncreasesUpwards);

            // Get expected points that should return true
            var expectedPoints = rect.PositionsOnSide(Direction.Right).ToHashSet();

            // Check that all points return the proper value
            foreach (Point point in rect.Positions())
                Assert.Equal(expectedPoints.Contains(point), rect.IsOnSide(point, Direction.Right));

            // Reset to false for next test
            Direction.SetYIncreasesUpwardsUnsafe(false);
        }

        [Theory]
        [MemberDataTuple(nameof(TestRectanglesWithYIncreasesUpwards))]
        public void IsOnBottomEdge(Rectangle rect, bool yIncreasesUpwards)
        {
            // Set to test's value
            Direction.SetYIncreasesUpwardsUnsafe(yIncreasesUpwards);

            // Get expected points that should return true
            var expectedPoints = rect.PositionsOnSide(Direction.Down).ToHashSet();

            // Check that all points return the proper value
            foreach (Point point in rect.Positions())
                Assert.Equal(expectedPoints.Contains(point), rect.IsOnSide(point, Direction.Down));

            // Reset to false for next test
            Direction.SetYIncreasesUpwardsUnsafe(false);
        }

        [Theory]
        [MemberDataTuple(nameof(TestRectanglesWithYIncreasesUpwards))]
        public void IsOnLeftEdge(Rectangle rect, bool yIncreasesUpwards)
        {
            // Set to test's value
            Direction.SetYIncreasesUpwardsUnsafe(yIncreasesUpwards);

            // Get expected points that should return true
            var expectedPoints = rect.PositionsOnSide(Direction.Left).ToHashSet();

            // Check that all points return the proper value
            foreach (Point point in rect.Positions())
                Assert.Equal(expectedPoints.Contains(point), rect.IsOnSide(point, Direction.Left));

            // Reset to false for next test
            Direction.SetYIncreasesUpwardsUnsafe(false);
        }

        #endregion
    }
}
