using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;

namespace SadRogue.Primitives.UnitTests.GridViews
{
    public class GridViewExtensionTests
    {
        private const int Width = 71;
        private const int Height = 50;

        [Fact]
        public void ApplyOverlayTest()
        {
            var grid = MockGridViews.RectangleBooleanGrid(Width, Height);

            var duplicateGrid = new ArrayView<bool>(grid.Width, grid.Height);

            duplicateGrid.ApplyOverlay(grid);

            foreach (var pos in grid.Positions())
                Assert.Equal(grid[pos], duplicateGrid[pos]);
        }

        [Fact]
        public void PositionsTest()
        {
            const int gridWidth = 80;
            const int gridHeight = 61;

            IGridView<bool> view = new ArrayView<bool>(gridWidth, gridHeight);
            var set = view.Positions().ToEnumerable().ToHashSet();

            Assert.Equal(view.Count, set.Count);
            for (int i = 0; i < view.Count; i++)
                Assert.Contains(Point.FromIndex(i, view.Width), set);
        }

        [Fact]
        public void PositionsIEnumerableEquivalent()
        {
            const int gridWidth = 80;
            const int gridHeight = 61;

            IGridView<bool> view = new ArrayView<bool>(gridWidth, gridHeight);

            var l1 = new List<Point>();
            foreach (var pos in view.Positions())
                l1.Add(pos);

            var l2 = view.Positions().ToEnumerable().ToList();

            Assert.Equal((IEnumerable<Point>)l1, l2);
        }
    }
}
