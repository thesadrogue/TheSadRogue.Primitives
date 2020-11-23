using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;
// Should disable this because the functions triggering it are just assertion methods
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace SadRogue.Primitives.UnitTests.GridViews
{
    public class GridViewTests
    {
        private const int _width = 50;
        private const int _height = 50;

        private static void CheckGridViews(IGridView<bool> boolView, IGridView<double> doubleView)
        {
            for (var x = 0; x < boolView.Width; x++)
                for (var y = 0; y < boolView.Height; y++)
                {
                    if (boolView[x, y])
                        Assert.True(doubleView[x, y] > 0.0);
                    else
                        Assert.False(doubleView[x, y] > 0.0);
                }
        }


        private static void CheckViewportBounds(Viewport<bool> viewport, Point expectedMinCorner,
                                                Point expectedMaxCorner)
        {
            Assert.Equal(expectedMaxCorner, viewport.ViewArea.MaxExtent);
            Assert.Equal(expectedMinCorner, viewport.ViewArea.MinExtent);
            Assert.True(viewport.ViewArea.X >= 0);
            Assert.True(viewport.ViewArea.Y >= 0);
            Assert.True(viewport.ViewArea.X < viewport.GridView.Width);
            Assert.True(viewport.ViewArea.Y < viewport.GridView.Height);

            foreach (var pos in viewport.ViewArea.Positions())
            {
                Assert.True(pos.X >= viewport.ViewArea.X);
                Assert.True(pos.Y >= viewport.ViewArea.Y);
                Assert.True(pos.X <= viewport.ViewArea.MaxExtentX);
                Assert.True(pos.Y <= viewport.ViewArea.MaxExtentY);
                Assert.True(pos.X >= 0);
                Assert.True(pos.Y >= 0);
                Assert.True(pos.X < viewport.GridView.Width);
                Assert.True(pos.Y < viewport.GridView.Height);

                // Utterly stupid way to access things via viewport, but verifies that the coordinate
                // translation is working properly.
                if (pos.X == 0 || pos.Y == 0 || pos.X == viewport.GridView.Width - 1 ||
                    pos.Y == viewport.GridView.Height - 1)
                    Assert.False(viewport[pos - viewport.ViewArea.MinExtent]);
                else
                    Assert.True(viewport[pos - viewport.ViewArea.MinExtent]);
            }
        }

        [Fact]
        public void ApplyOverlayTest()
        {
            var grid = MockGridViews.RectangleBooleanGrid(_width, _height);

            var duplicateGrid = new ArrayView<bool>(grid.Width, grid.Height);

            duplicateGrid.ApplyOverlay(grid);

            foreach (var pos in grid.Positions())
                Assert.Equal(grid[pos], duplicateGrid[pos]);
        }

        [Fact]
        public void IndexerAccessGridViewTest()
        {
            var grid = new ArrayView<bool>(_width, _height);
            ISettableGridView<bool> setGridView = grid;
            IGridView<bool> gridView = grid;
            bool[] array = grid;

            // Set last entry via indexer syntax (via the ArrayView, to prove implicit implementations
            // work at all levels)
            grid[^1] = true;

            // Set second to last entry via settable grid view
            setGridView[^2] = true;

            // Both of set should be true
            Assert.True(gridView[^2]);
            Assert.True(gridView[^1]);

            // All items should be false except for the last two
            for (int i = 0; i < array.Length - 2; i++)
                Assert.False(array[i]);

            Assert.True(array[^2]);
            Assert.True(array[^1]);
        }

        [Fact]
        public void LambdaGridViewTest()
        {
            var grid = MockGridViews.RectangleBooleanGrid(_width, _height);
            IGridView<double> lambdaGridView = new LambdaGridView<double>(grid.Width, grid.Height, c => grid[c] ? 1.0 : 0.0);

            CheckGridViews(grid, lambdaGridView);
        }

        [Fact]
        public void LambdaSettableGridViewTest()
        {
            var grid = MockGridViews.RandomDoubleGrid(10, 10);
            var lambdaSettable = new LambdaSettableGridView<bool>(grid.Width, grid.Height, c => grid[c] > 0.0,
                (c, b) => grid[c] = b ? 1.0 : 0.0);
            CheckGridViews(lambdaSettable, grid);

            // Set via lambda grid view, ensuring we actually change the value
            Assert.True(lambdaSettable[1, 2]);
            lambdaSettable[1, 2] = false;

            // Make sure grid views still match
            CheckGridViews(lambdaSettable, grid);
        }

        [Fact]
        public void LambdaSettableTranslationGridViewTest()
        {
            var grid = MockGridViews.RectangleBooleanGrid(_width, _height);

            var settable = new LambdaSettableTranslationGridView<bool, double>(grid, b => b ? 1.0 : 0.0, d => d > 0.0);
            CheckGridViews(grid, settable);

            // Change the grid view via the settable, and re-check
            settable[0, 0] = 1.0;

            Assert.True(grid[0, 0]);
            CheckGridViews(grid, settable);

            // Check other constructor.  Intentionally "misusing" the position parameter, to make sure we ensure the position
            // parameter is correct without complicating our test case
            settable = new LambdaSettableTranslationGridView<bool, double>(grid, (pos, b) => grid[pos] ? 1.0 : 0.0,
                (pos, d) => d > 0.0);
            CheckGridViews(grid, settable);
        }

        [Fact]
        public void LambdaTranslationGridViewTest()
        {
            var grid = MockGridViews.RectangleBooleanGrid(_width, _height);
            var lambdaGridView = new LambdaTranslationGridView<bool, double>(grid, b => b ? 1.0 : 0.0);

            CheckGridViews(grid, lambdaGridView);

            // Check other constructor.  Intentionally "misusing" the position parameter, to make sure we ensure the position
            // parameter is correct without complicating our test case
            lambdaGridView = new LambdaTranslationGridView<bool, double>(grid, (pos, b) => grid[pos] ? 1.0 : 0.0);
            CheckGridViews(grid, lambdaGridView);
        }

        [Fact]
        public void ViewportBoundingRectangleTest()
        {
            const int viewportWidth = 1280 / 12;
            const int viewportHeight = 768 / 12;
            const int gridWidth = 250;
            const int gridHeight = 250;

            var grid = MockGridViews.RectangleBooleanGrid(gridWidth, gridHeight);

            var viewport = new Viewport<bool>(grid, new Rectangle(0, 0, viewportWidth, viewportHeight));
            CheckViewportBounds(viewport, (0, 0), (viewportWidth - 1, viewportHeight - 1));

            // Should end up being 0, 0 thanks to bounding
            viewport.SetViewArea(viewport.ViewArea.WithPosition((-1, 0)));
            CheckViewportBounds(viewport, (0, 0), (viewportWidth - 1, viewportHeight - 1));

            viewport.SetViewArea(viewport.ViewArea.WithPosition((5, 5)));
            CheckViewportBounds(viewport, (5, 5), (viewportWidth - 1 + 5, viewportHeight - 1 + 5));

            // Move outside x-bounds by 1
            Point newCenter = (gridWidth - viewportWidth / 2 + 1, gridHeight - viewportHeight / 2 + 1);
            viewport.SetViewArea(viewport.ViewArea.WithCenter(newCenter));

            Point minVal = (gridWidth - viewportWidth, gridHeight - viewportHeight);
            Point maxVal = (gridWidth - 1, gridHeight - 1);
            CheckViewportBounds(viewport, minVal, maxVal);
        }

        [Fact]
        public void UnboundedViewportTest()
        {
            const int gridWidth = 100;
            const int gridHeight = 100;
            var grid = new ArrayView<int>(gridWidth, gridHeight);
            var unboundedViewport = new UnboundedViewport<int>(grid, 1);

            foreach (var pos in grid.Positions())
                Assert.Equal(0, unboundedViewport[pos]);

            unboundedViewport.ViewArea = unboundedViewport.ViewArea.Translate((5, 5));

            foreach (var pos in unboundedViewport.Positions())
                if (pos.X < gridWidth - 5 && pos.Y < gridHeight - 5)
                    Assert.Equal(0, unboundedViewport[pos]);
                else
                    Assert.Equal(1, unboundedViewport[pos]);

            unboundedViewport.ViewArea = unboundedViewport.ViewArea.WithSize(5, 5);

            foreach (var pos in unboundedViewport.Positions())
                Assert.Equal(0, unboundedViewport[pos]);

            unboundedViewport.ViewArea = unboundedViewport.ViewArea.WithPosition((gridWidth - 1, gridHeight - 1));

            foreach (var pos in unboundedViewport.Positions())
                if (pos.X == 0 && pos.Y == 0)
                    Assert.Equal(0, unboundedViewport[pos]);
                else
                    Assert.Equal(1, unboundedViewport[pos]);
        }
    }
}
