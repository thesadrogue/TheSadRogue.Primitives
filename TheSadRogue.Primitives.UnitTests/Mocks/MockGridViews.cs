using System;
using SadRogue.Primitives.GridViews;

namespace SadRogue.Primitives.UnitTests.Mocks
{
    /// <summary>
    /// A class that creates mock grid views for use in unit tests.
    /// </summary>
    internal static class MockGridViews
    {
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
