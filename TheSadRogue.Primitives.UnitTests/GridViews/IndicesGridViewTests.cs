using SadRogue.Primitives.GridViews;
using Xunit;

namespace SadRogue.Primitives.UnitTests.GridViews
{
    /// <summary>
    /// Tests that support for Indices and Ranges works as expected for grid views.
    /// </summary>
    public class IndicesGridViewTests
    {
        private const int Width = 71;
        private const int Height = 50;

        /// <summary>
        /// Tests that indices work for both getting values on arbitrary grid views (and setting them on settable
        /// grid views).
        /// </summary>
        [Fact]
        public void IndexerAccessGridViewTest()
        {
            var grid = new ArrayView<bool>(Width, Height);
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
    }
}
