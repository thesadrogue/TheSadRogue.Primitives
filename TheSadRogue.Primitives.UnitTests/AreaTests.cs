using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives.GridViews;
using Xunit;
using Xunit.Abstractions;

namespace SadRogue.Primitives.UnitTests
{
    /// <summary>
    /// Tests for the <see cref="Area"/> class
    /// </summary>
    public class AreaTests
    {
        private readonly ITestOutputHelper _output;

        public AreaTests(ITestOutputHelper output)
        {
            _output = output;
        }

        #region Test Data
        private static readonly Point[] s_pointsToAdd = { (1, 2), (3, 4), (5, 6) };

        #endregion
        #region Construction

        [Fact]
        public void ConstructEmpty()
        {
            var area = new Area();
            Assert.Empty(area);
            Assert.Equal(0, area.Count);
        }

        [Fact]
        public void ConstructFromEnumerable()
        {
            var points = new Point[] { (1, 2), (3, 4), (5, 6) };
            var area = new Area(points);

            Assert.Equal(points.Length, area.Count);
            Assert.Equal(points, area);
        }

        [Fact]
        public void ConstructFromParams()
        {
            var points = new Point[] { (1, 2), (3, 4), (5, 6) };
            var area = new Area(null, points[0], points[1], points[2]);

            Assert.Equal(points.Length, area.Count);
            Assert.Equal(points, area);
        }
        #endregion
        #region Add/Remove Points

        [Fact]
        public void AddRemovePoint()
        {
            var area = new Area();
            var list = new List<Point>();

            foreach (var point in s_pointsToAdd)
            {
                list.Add(point);
                area.Add(point);

                Assert.Equal(list.Count, area.Count);
                Assert.Equal(list, area);
            }
            Assert.Equal(s_pointsToAdd.Length, area.Count);
            Assert.Equal(s_pointsToAdd, area);

            foreach (var point in s_pointsToAdd)
            {
                list.Remove(point);
                area.Remove(point);

                Assert.Equal(list.Count, area.Count);
                Assert.Equal(list, area);
            }
            Assert.Empty(area);
            Assert.Equal(0, area.Count);
        }

        [Fact]
        public void AddDuplicatePoint()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            var area = new Area{(1, 2)};

            // Try to add duplicate
            area.Add((1, 2));

            Assert.Equal(1, area.Count);
            Assert.Single(area);
        }

        [Fact]
        public void AddRemoveXY()
        {
            var area = new Area();
            var list = new List<Point>();

            foreach (var point in s_pointsToAdd)
            {
                list.Add(point);
                area.Add(point.X, point.Y);

                Assert.Equal(list.Count, area.Count);
                Assert.Equal(list, area);
            }
            Assert.Equal(s_pointsToAdd.Length, area.Count);
            Assert.Equal(s_pointsToAdd, area);

            foreach (var point in s_pointsToAdd)
            {
                list.Remove(point);
                area.Remove(point.X, point.Y);

                Assert.Equal(list.Count, area.Count);
                Assert.Equal(list, area);
            }
            Assert.Empty(area);
            Assert.Equal(0, area.Count);
        }

        [Fact]
        public void AddRectangle()
        {
            var area = new Area();
            var rect = new Rectangle(1, 2, 15, 12);

            area.Add(rect);

            Assert.Equal(rect.Width * rect.Height, area.Count);
            Assert.Equal(rect.Positions().ToEnumerable(), area);
        }

        [Fact]
        public void AddReadOnlyArea()
        {
            var area = new Area();
            var area2 = new Area(s_pointsToAdd);

            area.Add(area2);

            Assert.Equal(area2.Count, area.Count);
            Assert.Equal((IEnumerable<Point>)area2, area);
        }

        [Fact]
        public void RemoveHashSet()
        {
            var area = new Area(s_pointsToAdd);
            var set = new HashSet<Point> { (1, 2), (3, 4), (432, 234) };

            area.Remove(set);

            Assert.Equal(1, area.Count);
            Assert.Equal(s_pointsToAdd[^1].Yield(), area);
        }

        [Fact]
        public void RemoveEnumerable()
        {
            var area = new Area(s_pointsToAdd);
            var set = new List<Point> { (1, 2), (3, 4), (432, 234) };

            area.Remove(set);

            Assert.Equal(1, area.Count);
            Assert.Equal(s_pointsToAdd[^1].Yield(), area);
        }

        [Fact]
        public void RemoveEnumerableHashSet()
        {
            var area = new Area(s_pointsToAdd);
            var set = new HashSet<Point> { (1, 2), (3, 4), (432, 234) };

            area.Remove((IEnumerable<Point>)set);

            Assert.Equal(1, area.Count);
            Assert.Equal(s_pointsToAdd[^1].Yield(), area);
        }

        [Fact]
        public void RemoveArea()
        {
            var area = new Area(s_pointsToAdd);
            var set = new Area { (1, 2), (3, 4), (432, 234) };

            area.Remove(set);

            Assert.Equal(1, area.Count);
            Assert.Equal(s_pointsToAdd[^1].Yield(), area);
        }

        [Fact]
        public void RemoveRectangle()
        {
            var rect = new Rectangle(1, 2, 15, 12);
            var area = new Area(rect.Positions().ToEnumerable());

            var bisection = rect.BisectVertically();
            _output.WriteLine($"Bisection:\n    Rect1: {bisection.Rect1}\n    Rect2: {bisection.Rect2}");
            area.Remove(bisection.Rect2.Expand(1, 1));

            var left = bisection.Rect1.WithWidth(bisection.Rect1.Width - 1);
            _output.WriteLine($"Left: {left}");
            _output.WriteLine($"Area: {area.Bounds}");
            Assert.Equal(left.Width * left.Height, area.Count);
            Assert.Equal(left.Positions().ToEnumerable().ToHashSet(), area.ToHashSet());
        }

        [Fact]
        public void RemovePredicate()
        {
            var area = new Area(s_pointsToAdd);
            var set = new HashSet<Point> { (1, 2), (3, 4), (432, 234) };

            area.Remove(p => set.Contains(p));

            Assert.Equal(1, area.Count);
            Assert.Equal(s_pointsToAdd[^1].Yield(), area);
        }

        [Fact]
        public void RemoveNonAddedPoint()
        {
            var area = new Area(s_pointsToAdd);
            area.Remove((153, 543));

            Assert.Equal(s_pointsToAdd.Length, area.Count);
            Assert.Equal(s_pointsToAdd, area);
        }
        #endregion
        #region Bounds

        [Fact]
        public void CheckBounds()
        {
            var area = new Area(s_pointsToAdd);
            var bounds = new Rectangle(s_pointsToAdd[0], s_pointsToAdd[^1]);

            Assert.Equal(bounds, area.Bounds);

            // Extend upper bound
            area.Add((bounds.X + bounds.Width / 2, bounds.Y - 1));
            bounds = bounds.Translate(Direction.Up).ChangeHeight(1);
            Assert.Equal(bounds, area.Bounds);

            // Extend right bound
            area.Add((bounds.MaxExtentX + 1, bounds.Y + bounds.Height / 2));
            bounds = bounds.ChangeWidth(1);
            Assert.Equal(bounds, area.Bounds);

            // Extend lower bound
            area.Add((bounds.X + bounds.Width / 2, bounds.MaxExtentY + 1));
            bounds = bounds.ChangeHeight(1);
            Assert.Equal(bounds, area.Bounds);

            // Extend left bound
            area.Add((bounds.X - 1, bounds.Y + bounds.Height / 2));
            bounds = bounds.Translate(Direction.Left).ChangeWidth(1);
            Assert.Equal(bounds, area.Bounds);
        }

        [Fact]
        public void CheckEmptyBounds()
        {
            var area = new Area();
            Assert.Equal(Rectangle.Empty, area.Bounds);
        }
        #endregion
        #region Contains
        [Fact]
        public void ContainsPosition()
        {
            var rect = new Rectangle(1, 2, 15, 12);
            var area = new Area(rect.Positions().ToEnumerable());

            // Contains all points inside rectangle
            foreach (var pos in rect.Positions())
            {
                Assert.True(area.Contains(pos));
                Assert.True(area.Contains(pos.X, pos.Y));
            }

            // But none outside
            foreach (var pos in rect.Expand(1, 1).PerimeterPositions())
            {
                Assert.False(area.Contains(pos));
                Assert.False(area.Contains(pos.X, pos.Y));
            }
        }

        [Fact]
        public void ContainsArea()
        {
            var rect = new Rectangle(1, 2, 15, 12);
            var area = new Area(rect.Positions().ToEnumerable());

            // Contains proper subset
            var subArea = new Area(rect.Expand(-1, -1).Positions().ToEnumerable());
            Assert.True(area.Contains(subArea));

            // Contains equivalent
            subArea = new Area(rect.Positions().ToEnumerable());
            Assert.True(area.Contains(subArea));

            // Doesn't contain anything outside
            foreach (var outsideAreaPos in rect.Expand(1, 1).PerimeterPositions())
            {
                subArea.Add(outsideAreaPos);
                Assert.False(area.Contains(subArea));
                subArea.Remove(outsideAreaPos);
            }

            // Even if bounds are contained, if points aren't the same, Contains is false.
            area.Remove(rect.Position);
            Assert.False(area.Contains(subArea));
        }
        #endregion
        #region Matches
        [Fact]
        public void TestMatchesEquivalent()
        {
            var area = new Area(s_pointsToAdd);
            var equivalentArea = new Area(s_pointsToAdd);
            Assert.True(area.Matches(area));
            Assert.True(area.Matches(equivalentArea));
        }

        [Fact]
        public void TestMatchesNotEquivalent()
        {
            var area = new Area(s_pointsToAdd);
            var notEquivalentArea = new Area(s_pointsToAdd) { (10, 11) };
            var notEquivalentArea2 = new Area(s_pointsToAdd[..^1]);
            Assert.False(area.Matches(notEquivalentArea));
            Assert.False(area.Matches(notEquivalentArea2));
            Assert.False(area.Matches(null));

            // Same count, different points
            notEquivalentArea2.Add(s_pointsToAdd[^1] + 1);
            Assert.Equal(s_pointsToAdd.Length, notEquivalentArea2.Count);
            Assert.False(area.Matches(notEquivalentArea2));
        }

        [Fact]
        public void TestMatchesSameBoundsAndCount()
        {
            var area = new Area(s_pointsToAdd);
            var notEquivalentArea = new Area(null, s_pointsToAdd[0], s_pointsToAdd[2], (2, 3));
            // Required for test case to be valid
            Assert.DoesNotContain((2, 3), s_pointsToAdd);

            // Ensure this is a valid test case
            Assert.Equal(area.Count, notEquivalentArea.Count);
            Assert.Equal(area.Bounds, notEquivalentArea.Bounds);

            // Not same points, so still not equivalent
            Assert.False(area.Matches(notEquivalentArea));
        }
        #endregion
        #region Enumeration
        [Fact]
        public void EnumerableCorrect()
        {
            var rect = new Rectangle(0, 0, 15, 15);
            var area = new Area(rect.Positions().ToEnumerable());
            IReadOnlyArea areaInterface = area;

            var expected = new List<Point>();
            for (int i = 0; i < area.Count; i++)
                expected.Add(area[i]);

            List<Point> l1 = area.ToList();

            var l2 = new List<Point>();
            foreach (var pos in area)
                l2.Add(pos);

            var l3 = new List<Point>();
            foreach (var pos in areaInterface)
                l3.Add(pos);

            var l4 = new List<Point>();
            foreach (var pos in area.FastEnumerator())
                l4.Add(pos);


            Assert.Equal(expected, l1);
            Assert.Equal(expected, l2);
            Assert.Equal(expected, l3);
            Assert.Equal(expected, l4);
        }
        #endregion
        #region Perimeter Positions
        [Fact]
        public void RectangleAreaPerimeterPositions()
        {
            var rect = new Rectangle(0, 0, 15, 15);
            var area = new Area(rect.Positions().ToEnumerable());

            var expected = rect.PerimeterPositions().ToHashSet();
            var actual1 = area.PerimeterPositions(AdjacencyRule.Cardinals).ToHashSet();
            var actual2 = area.PerimeterPositions(AdjacencyRule.EightWay).ToHashSet();

            Assert.Equal(expected, actual1);
            Assert.Equal(expected, actual2);
        }

        // X _not_ considered a border, by four-way adjacency
        // . . . . . . . .
        // . . . . . . . .
        // . . . . . . . .
        // . . . . . . . .
        // . . . . . . . .
        // . . . . . . . .
        // . . . . . . X .
        // . . . . . . . #
        [Fact]
        public void IrregularAreaPerimeterPositionsCardinals()
        {
            var rect = new Rectangle(0, 0, 15, 15);
            var area = new Area(rect.Positions().ToEnumerable());
            area.Remove(rect.MaxExtent);

            var expected = rect.PerimeterPositions().ToHashSet();
            expected.Remove(rect.MaxExtent);

            var actual = area.PerimeterPositions(AdjacencyRule.Cardinals).ToHashSet();

            Assert.Equal(expected, actual);
        }

        // X _is_ considered a border, by eight-way adjacency
        // . . . . . . . .
        // . . . . . . . .
        // . . . . . . . .
        // . . . . . . . .
        // . . . . . . . .
        // . . . . . . . .
        // . . . . . . X .
        // . . . . . . . #
        [Fact]
        public void IrregularAreaPerimeterPositionsEightWay()
        {
            var rect = new Rectangle(0, 0, 15, 15);
            var area = new Area(rect.Positions().ToEnumerable());
            area.Remove(rect.MaxExtent);

            var expected = rect.PerimeterPositions().ToHashSet();
            expected.Remove(rect.MaxExtent);
            expected.Add(rect.MaxExtent - 1);

            var actual = area.PerimeterPositions(AdjacencyRule.EightWay).ToHashSet();

            Assert.Equal(expected, actual);
        }
        #endregion
        #region ToString

        [Fact]
        public void TestToString()
        {
            var area = new Area(s_pointsToAdd);

            string expected = "[" + string.Join(", ", s_pointsToAdd) + "]";
            Assert.Equal(expected, area.ToString());
        }
        #endregion

        #region  Set Operations

        [Fact]
        public void TestDifference()
        {
            var area1 = new Area(s_pointsToAdd);
            var set = new HashSet<Point>(s_pointsToAdd);

            var area2 = new Area(null, s_pointsToAdd[0], (845, 543), (134, 432));
            var set2 = new HashSet<Point>(area2);

            var resultArea = Area.GetDifference(area1, area2);
            set.ExceptWith(set2);

            Assert.Equal(set, resultArea.ToHashSet());
            Assert.Equal(2, resultArea.Count);
            Assert.Equal(new Rectangle(s_pointsToAdd[1], s_pointsToAdd[^1]), resultArea.Bounds);
        }

        [Fact]
        public void TestIntersection()
        {
            var area1 = new Area(s_pointsToAdd);
            var set = new HashSet<Point>(s_pointsToAdd);

            var area2 = new Area(null, s_pointsToAdd[0], (845, 543), (134, 432), (15, 15));
            var set2 = new HashSet<Point>(area2);

            var resultArea = Area.GetIntersection(area1, area2);
            var resultArea2 = Area.GetIntersection(area2, area1);
            set.IntersectWith(set2);

            Assert.Equal(set, resultArea.ToHashSet());
            Assert.Equal(1, resultArea.Count);
            Assert.Equal(new Rectangle(s_pointsToAdd[0], s_pointsToAdd[0]), resultArea.Bounds);
            Assert.True(resultArea.Matches(resultArea2));
        }

        [Fact]
        public void TestIntersectionNoIntersect()
        {
            var area1 = new Area(s_pointsToAdd);
            var area2 = new Area(null, (7, 8));

            var resultArea = Area.GetIntersection(area1, area2);
            Assert.Empty(resultArea);
            Assert.Equal(0, resultArea.Count);
        }

        [Fact]
        public void TestUnion()
        {
            var area1 = new Area(s_pointsToAdd);
            var set = new HashSet<Point>(s_pointsToAdd);

            var area2 = new Area(null, s_pointsToAdd[0], (845, 543), (134, 432));
            var set2 = new HashSet<Point>(area2);

            var resultArea = Area.GetUnion(area1, area2);
            set.UnionWith(set2);

            Assert.Equal(set, resultArea.ToHashSet());
            Assert.Equal(5, resultArea.Count);
            Assert.Equal(new Rectangle(s_pointsToAdd[0], (845, 543)), resultArea.Bounds);
        }

        [Fact]
        public void TestShiftOperator()
        {
            var delta = new Point(5, 6);
            var area = new Area(s_pointsToAdd);
            area += delta;

            Assert.Equal(s_pointsToAdd.Length, area.Count);
            for (int i = 0; i < s_pointsToAdd.Length; i++)
                Assert.Equal(s_pointsToAdd[i] + delta, area[i]);
        }

        [Fact]
        public void TestIntersects()
        {
            var area = new Area(s_pointsToAdd);
            var areaNoIntersectBounds = new Area(null, (7, 8));
            var areaIntersect = new Area(null, (3, 3), s_pointsToAdd[0]);

            // No bounds intersection at all
            Assert.False(area.Intersects(areaNoIntersectBounds));

            // Bounds intersect as do the areas both ways (also, reference check)
            Assert.True(area.Intersects(areaIntersect));
            Assert.True(areaIntersect.Intersects(area));
            Assert.True(area.Intersects(area));
        }

        [Fact]
        public void TestIntersectBoundsIntersectButNotArea()
        {
            var area = new Area(s_pointsToAdd);
            var areaNoIntersect = new Area(null, (3, 3), (1, 3));

            // Bounds intersect but no points do not
            Assert.False(area.Intersects(areaNoIntersect));
            Assert.False(areaNoIntersect.Intersects(area));
        }
        #endregion
    }
}
