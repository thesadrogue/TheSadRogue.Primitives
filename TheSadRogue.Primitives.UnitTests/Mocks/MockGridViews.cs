using System;
using SadRogue.Primitives.GridViews;

namespace SadRogue.Primitives.UnitTests.Mocks
{
    /// <summary>
    /// A class that creates mock grid views for use in unit tests.
    /// </summary>
    internal static class MockGridViews
    {
        public static ArrayView2D<int> RectangleArrayView2D(int width, int height)
        {
            var grid = RectangleBooleanGrid(width, height);

            var arrayGrid = new ArrayView2D<int>(grid.Width, grid.Height);
            arrayGrid.ApplyOverlay(pos => grid[pos] ? 1 : 0);

            return arrayGrid;
        }

        public static ISettableGridView<double> RandomDoubleGrid(int width, int height)
        {
            var grid = new ArrayView<double>(width, height);
            Random rng = new Random();

            foreach (var pos in grid.Positions())
                grid[pos] = rng.NextDouble();

            return grid;
        }

        public static ISettableGridView<bool> RectangleBooleanGrid(int width, int height)
        {
            ISettableGridView<bool> grid = new ArrayView<bool>(width, height);
            foreach (var pos in grid.Bounds().Expand(-1, -1).Positions())
                grid[pos] = true;

            return grid;
        }
    }
}
