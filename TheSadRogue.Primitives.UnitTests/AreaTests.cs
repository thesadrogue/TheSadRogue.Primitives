using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SadRogue.Primitives.UnitTests
{
    /// <summary>
    /// Tests for the <see cref="Area"/> class
    /// </summary>
    public class AreaTests
    {
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
    }
}
