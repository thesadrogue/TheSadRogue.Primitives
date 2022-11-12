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

        #region  Construction

        [Fact]
        public void LambdaSettableTranslationViewConstruction()
        {
            var sourceView = MockGridViews.RectangleBooleanGrid(Width, Height);

            int GetVal(bool b) => b ? 1 : 0;

            // Exception for nulls
            Assert.Throws<ArgumentNullException>(()
                => new LambdaSettableTranslationGridView<bool, int>(sourceView, GetVal, null!));
            Assert.Throws<ArgumentNullException>(()
                => new LambdaSettableTranslationGridView<bool, int>(sourceView, null!, i => i != 0));

            // Valid view
            var lambdaView = new LambdaSettableTranslationGridView<bool, int>(sourceView, GetVal, i => i != 0);
            CheckTranslationViewConstruction(sourceView, lambdaView, GetVal);

            // No need to clear; CheckTranslationViewConstruction reset its changes and the translation view should have
            // applied any changes on creation with this constructor.

            // Exception for nulls
            Assert.Throws<ArgumentNullException>(()
                => new LambdaSettableTranslationGridView<bool, int>(sourceView, (pos, b) => GetVal(b), null!));
            Assert.Throws<ArgumentNullException>(()
                => new LambdaSettableTranslationGridView<bool, int>(sourceView, null!, (pos, i) => i != 0));

            // Valid view
            lambdaView = new LambdaSettableTranslationGridView<bool, int>(sourceView, (pos, b) => GetVal(b), (pos, i) => i != 0);
            CheckTranslationViewConstruction(sourceView, lambdaView, GetVal);
        }

        [Fact]
        public void LambdaSettableTranslationViewConstructionWithOverlay()
        {
            var sourceView = new ArrayView<int>(Width, Height);
            var overlay = MockGridViews.RectangleBooleanGrid(sourceView.Width, sourceView.Height);

            bool GetVal(int i) => i != 0;
            var lambdaView = new LambdaSettableTranslationGridView<int, bool>(sourceView, overlay, GetVal, b => b ? 1 : 0);
            CheckTranslationViewConstruction(sourceView, lambdaView, GetVal);

            sourceView.Clear();
            lambdaView = new LambdaSettableTranslationGridView<int, bool>(sourceView, overlay, (pos, b) => GetVal(b), (pos, i) => i ? 1 : 0);
            CheckTranslationViewConstruction(sourceView, lambdaView, GetVal);
        }

        private static void CheckTranslationViewConstruction<T1, T2>(ISettableGridView<T1> source, IGridView<T2> lambdaTranslation, Func<T1, T2> translate)
        {
            Assert.Equal(source.Width, lambdaTranslation.Width);
            Assert.Equal(source.Height, lambdaTranslation.Height);
            foreach (var pos in source.Positions())
                Assert.Equal(translate(source[pos]), lambdaTranslation[pos]);

            if (!(lambdaTranslation is ISettableGridView<T2> settableLambda))
                return;

            var bounds = source.Bounds();
            Assert.NotEqual(default, source[bounds.Center]);

            T1 prevVal = source[bounds.Center];
            settableLambda[bounds.Center] = default!;
            Assert.Equal(default, source[bounds.Center]);
            source[bounds.Center] = prevVal;
        }
        #endregion

        #region Basic Equivalence Tests
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
            Assert.Throws<ArgumentNullException>(() => new LambdaTranslationGridView<bool, double>(grid, (Func<bool, double>)null!));
            Assert.Throws<ArgumentNullException>(() => new LambdaTranslationGridView<bool, double>(grid, (Func<Point, bool, double>)null!));

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


        #endregion

        #region ToString

        [Fact]
        public void LambdaSettableGridViewToString()
        {
            var arrayView = new ArrayView<int>(6, 5);
            foreach (var pos in arrayView.Bounds().Expand(-1, -1).Positions())
                arrayView[pos] = 1;

            var view = new LambdaSettableGridView<int>(arrayView.Width, arrayView.Height, pos => arrayView[pos],
                (pos, val) => arrayView[pos] = val);
            string actual = view.ToString();
            const string expected = "0 0 0 0 0 0\n0 1 1 1 1 0\n0 1 1 1 1 0\n0 1 1 1 1 0\n0 0 0 0 0 0";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LambdaSettableGridViewToStringCustomStringifier()
        {
            var arrayView = new ArrayView<int>(6, 5);
            foreach (var pos in arrayView.Bounds().Expand(-1, -1).Positions())
                arrayView[pos] = 1;

            var view = new LambdaSettableGridView<int>(arrayView.Width, arrayView.Height, pos => arrayView[pos],
                (pos, val) => arrayView[pos] = val);
            string actual = view.ToString(i => i >= 1 ? "." : "#");
            string expected = "# # # # # #\n# . . . . #\n# . . . . #\n# . . . . #\n# # # # # #";

            Assert.Equal(expected, actual);

            expected = expected.Replace("#", " #").Replace(".", " .");
            actual = view.ToString(2, i => i == 1 ? "." : "#");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LambdaGridViewToString()
        {
            var arrayView = new ArrayView<int>(6, 5);
            foreach (var pos in arrayView.Bounds().Expand(-1, -1).Positions())
                arrayView[pos] = 1;

            var view = new LambdaGridView<int>(arrayView.Width, arrayView.Height, pos => arrayView[pos]);
            string actual = view.ToString();
            const string expected = "0 0 0 0 0 0\n0 1 1 1 1 0\n0 1 1 1 1 0\n0 1 1 1 1 0\n0 0 0 0 0 0";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void LambdaGridViewToStringCustomStringifier()
        {
            var arrayView = new ArrayView<int>(6, 5);
            foreach (var pos in arrayView.Bounds().Expand(-1, -1).Positions())
                arrayView[pos] = 1;

            var view = new LambdaGridView<int>(arrayView.Width, arrayView.Height, pos => arrayView[pos]);
            string actual = view.ToString(i => i >= 1 ? "." : "#");
            string expected = "# # # # # #\n# . . . . #\n# . . . . #\n# . . . . #\n# # # # # #";

            Assert.Equal(expected, actual);

            expected = expected.Replace("#", " #").Replace(".", " .");
            actual = view.ToString(2, i => i == 1 ? "." : "#");

            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
