using System;
using System.Collections.Generic;
using System.Linq;
using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace SadRogue.Primitives.UnitTests.GridViews
{
    public class GridViewExtensionTests
    {
        private const int Width = 71;
        private const int Height = 50;

        private readonly ITestOutputHelper _output;

        public GridViewExtensionTests(ITestOutputHelper output)
        {
            _output = output;
        }

        #region ApplyOverlay
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
        public void ApplyOverlayIncorrectSize()
        {
            var grid = MockGridViews.RectangleBooleanGrid(Width, Height);

            var duplicateGrid = new ArrayView<bool>(grid.Width + 1, grid.Height);
            Assert.Throws<ArgumentException>(() => duplicateGrid.ApplyOverlay(grid));

            duplicateGrid = new ArrayView<bool>(grid.Width, grid.Height - 1);
            Assert.Throws<ArgumentException>(() => duplicateGrid.ApplyOverlay(grid));
        }
        #endregion

        #region Contains

        [Fact]
        public void TestContainsGridView()
        {
            var view = new ArrayView<int>(Width, Height);
            var bounds = view.Bounds();

            foreach (var pos in bounds.Expand(1, 1).Positions())
            {
                Assert.Equal(bounds.Contains(pos), view.Contains(pos));
                Assert.Equal(bounds.Contains(pos), view.Contains(pos.X, pos.Y));
            }
        }


        #endregion

        #region Enumeration
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
        #endregion

        #region  Stringification
        [Fact]
        public void TestExtendToStringDefaults()
        {
            var view = new ArrayView<int?>(6, 5);
            foreach (var pos in view.Bounds().Expand(-1, -1).Positions())
                view[pos] = 1;

            foreach (var pos in view.Bounds().PerimeterPositions())
                view[pos] = 0;

            view[view.Bounds().Center] = null;

            string actual = view.ExtendToString();
            _output.WriteLine("No field size:");
            _output.WriteLine(actual);

            string expected = "0 0 0 0 0 0\n0 1 1 1 1 0\n0 1 1 null 1 0\n0 1 1 1 1 0\n0 0 0 0 0 0";
            Assert.Equal(expected, actual);

            actual = view.ExtendToString(4);
            _output.WriteLine("\nWith field size:");
            _output.WriteLine(actual);

            expected = expected.Replace("0", "   0").Replace("1", "   1");
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
