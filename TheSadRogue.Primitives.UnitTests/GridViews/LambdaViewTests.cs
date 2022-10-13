using System;
using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;

namespace SadRogue.Primitives.UnitTests.GridViews
{
    /// <summary>
    /// Tests for <see cref="LambdaGridView{T}"/> and the settable/translation variants.
    /// </summary>
    public class LambdaViewTests
    {
        private const int Width = 71;
        private const int Height = 50;

        [Fact]
        public void LambdaGridViewTest()
        {
            var grid = MockGridViews.RectangleBooleanGrid(Width, Height);
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
            var grid = MockGridViews.RectangleBooleanGrid(Width, Height);

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
            var grid = MockGridViews.RectangleBooleanGrid(Width, Height);
            var lambdaGridView = new LambdaTranslationGridView<bool, double>(grid, b => b ? 1.0 : 0.0);

            CheckGridViews(grid, lambdaGridView);

            // Check other constructor.  Intentionally "misusing" the position parameter, to make sure we ensure the position
            // parameter is correct without complicating our test case
            lambdaGridView = new LambdaTranslationGridView<bool, double>(grid, (pos, b) => grid[pos] ? 1.0 : 0.0);
            CheckGridViews(grid, lambdaGridView);
        }

        /// <summary>
        /// Checks that the given <see cref="LambdaGridView{T}"/> of doubles has a value greater than 0.0 for every
        /// location in the boolean value with a value of true, and a value of 0.0 for every location with a value
        /// of false.
        /// </summary>
        /// <param name="boolView">Boolean view to be considered the "expected" value</param>
        /// <param name="doubleView">Double grid view to check.</param>
        private static void CheckGridViews(IGridView<bool> boolView, IGridView<double> doubleView)
        {
            foreach (var pos in boolView.Positions())
            {
                if (boolView[pos])
                {
                    Assert.True(doubleView[pos] > 0.0);
                    Assert.True(doubleView[pos.X, pos.Y] > 0.0);
                    Assert.True(doubleView[pos.ToIndex(boolView.Width)] > 0.0);
                }
                else
                {
                    Assert.Equal(0.0, doubleView[pos]);
                    Assert.Equal(0.0, doubleView[pos.X, pos.Y]);
                    Assert.Equal(0.0, doubleView[pos.ToIndex(boolView.Width)]);
                }
            }
        }
    }
}
