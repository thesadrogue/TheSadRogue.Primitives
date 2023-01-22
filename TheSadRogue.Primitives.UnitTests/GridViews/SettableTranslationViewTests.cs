using System;
using System.Linq;
using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;

namespace SadRogue.Primitives.UnitTests.GridViews
{
    /// <summary>
    /// Tests for <see cref="SettableTranslationGridView{T1, T2}"/>.
    /// </summary>
    public class SettableTranslationViewTests
    {
        [Fact]
        public void Constructor()
        {
            var grid = MockGridViews.RectangleBooleanGrid(56, 42);
            var translated = new SettableTranslationGridViewSimpleOverride(grid);

            Assert.Equal(grid, translated.BaseGrid);
            Assert.Equal(grid.Width, translated.Width);
            Assert.Equal(grid.Height, translated.Height);

            var viewOverlay = new ArrayView<int>(grid.Width, grid.Height);
            viewOverlay[viewOverlay.Bounds().Center] = 1;

            translated = new SettableTranslationGridViewSimpleOverride(grid, viewOverlay);
            Assert.Equal(grid, translated.BaseGrid);
            Assert.Equal(grid.Width, translated.Width);
            Assert.Equal(grid.Height, translated.Height);
            foreach (var pos in translated.Positions())
            {
                Assert.Equal(viewOverlay[pos], translated[pos]);
                Assert.Equal(viewOverlay[pos] != 0, grid[pos]);
            }
        }

        [Fact]
        public void NoOverridesThrowsException()
        {
            var grid = MockGridViews.RectangleBooleanGrid(56, 42);
            var translated = new SettableTranslationGridViewNoOverrides<bool, int>(grid);
            Assert.Throws<NotSupportedException>(() => translated[1, 2]);
            Assert.Throws<NotSupportedException>(() => translated[new Point(1, 2)]);
            Assert.Throws<NotSupportedException>(() => translated[1]);

            Assert.Throws<NotSupportedException>(() => translated[1, 2] = 1);
            Assert.Throws<NotSupportedException>(() => translated[new Point(1, 2)] = 0);
            Assert.Throws<NotSupportedException>(() => translated[1] = 1);
        }

        [Fact]
        public void SimpleOverride()
        {
            var grid = MockGridViews.RectangleBooleanGrid(56, 42);
            var translated = new SettableTranslationGridViewSimpleOverride(grid);

            Assert.Equal(grid.Width, translated.Width);
            Assert.Equal(grid.Height, translated.Height);
            foreach (var pos in grid.Positions())
                Assert.Equal(grid[pos] ? 1 : 0, translated[pos]);

            translated[new Point(1, 1)] = 1;
            translated[2, 2] = 1;
            translated[new Point(3, 3).ToIndex(translated.Width)] = 1;

            foreach (var pos in translated.Positions())
            {
                bool val = pos == (1, 1) || pos == (2, 2) || pos == (3, 3) || grid[pos];
                Assert.Equal(val ? 1 : 0, translated[pos]);
                Assert.Equal(val, grid[pos]);
            }
        }

        [Fact]
        public void PositionOverride()
        {
            var grid = MockGridViews.RectangleBooleanGrid(56, 42);
            grid[0, 0] = true;
            grid[grid.Width / 2, grid.Height / 2] = false;
            var translated = new SettableTranslationGridViewPositionOverride(grid);

            Assert.Equal(grid.Width, translated.Width);
            Assert.Equal(grid.Height, translated.Height);
            foreach (var pos in grid.Positions())
                Assert.Equal(grid.Bounds().PerimeterPositions().Contains(pos) ? 0 : grid[pos] ? 1 : 0, translated[pos]);

            translated[new Point(1, 1)] = 1;
            translated[2, 2] = 1;
            translated[new Point(3, 3).ToIndex(translated.Width)] = 1;

            foreach (var pos in translated.Positions())
            {
                bool value = pos == (1, 1) || pos == (2, 2) || pos == (3, 3) || grid[pos];
                if (grid.Bounds().PerimeterPositions().Contains(pos))
                    value = false;

                Assert.Equal(value ? 1 : 0, translated[pos]);
                Assert.Equal(value, grid[pos] && !grid.Bounds().PerimeterPositions().Contains(pos));
            }
        }

        [Fact]
        public void TranslationToString()
        {
            var grid = MockGridViews.RectangleBooleanGrid(5, 4);
            var translated = new SettableTranslationGridViewSimpleOverride(grid);

            string expected = "0 0 0 0 0\n0 1 1 1 0\n0 1 1 1 0\n0 0 0 0 0";
            Assert.Equal(expected, translated.ToString());

            expected = expected.Replace("0", "3").Replace("1", "2");
            Assert.Equal(expected, translated.ToString(i => i == 0 ? "3" : "2"));

            expected = " 0  0  0  0  0\n 0  1  1  1  0\n 0  1  1  1  0\n 0  0  0  0  0";
            Assert.Equal(expected, translated.ToString(2, i => i.ToString()));
        }
    }
}
