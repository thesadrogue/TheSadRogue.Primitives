using System;
using System.Linq;
using System.Security.Principal;
using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;

namespace SadRogue.Primitives.UnitTests.GridViews
{
    /// <summary>
    /// Tests for <see cref="TranslationGridView{T1, T2}"/> and the settable variants.
    /// </summary>
    public class TranslationViewTests
    {
        [Fact]
        public void Constructor()
        {
            var grid = MockGridViews.RectangleBooleanGrid(56, 42);
            var translated = new TranslationGridViewSimpleOverride(grid);

            Assert.Equal(grid, translated.BaseGrid);
            Assert.Equal(grid.Width, translated.Width);
            Assert.Equal(grid.Height, translated.Height);
        }

        [Fact]
        public void NoOverridesThrowsException()
        {
            var grid = MockGridViews.RectangleBooleanGrid(56, 42);
            var translated = new TranslationGridViewNoOverrides<bool, int>(grid);
            Assert.Throws<NotSupportedException>(() => translated[1, 2]);
            Assert.Throws<NotSupportedException>(() => translated[new Point(1, 2)]);
            Assert.Throws<NotSupportedException>(() => translated[1]);
        }

        [Fact]
        public void SimpleOverride()
        {
            var grid = MockGridViews.RectangleBooleanGrid(56, 42);
            var translated = new TranslationGridViewSimpleOverride(grid);

            Assert.Equal(grid.Width, translated.Width);
            Assert.Equal(grid.Height, translated.Height);
            foreach (var pos in grid.Positions())
                Assert.Equal(grid[pos] ? 1 : 0, translated[pos]);
        }

        [Fact]
        public void PositionOverride()
        {
            var grid = MockGridViews.RectangleBooleanGrid(56, 42);
            grid[0, 0] = true;
            grid[grid.Width / 2, grid.Height / 2] = false;
            var translated = new TranslationGridViewPositionOverride(grid);

            Assert.Equal(grid.Width, translated.Width);
            Assert.Equal(grid.Height, translated.Height);
            foreach (var pos in grid.Positions())
                Assert.Equal(grid.Bounds().PerimeterPositions().Contains(pos) ? 0 : grid[pos] ? 1 : 0, translated[pos]);
        }

        [Fact]
        public void TranslationToString()
        {
            var grid = MockGridViews.RectangleBooleanGrid(5, 4);
            var translated = new TranslationGridViewSimpleOverride(grid);

            string expected = "0 0 0 0 0\n0 1 1 1 0\n0 1 1 1 0\n0 0 0 0 0";
            Assert.Equal(expected, translated.ToString());

            expected = expected.Replace("0", "3").Replace("1", "2");
            Assert.Equal(expected, translated.ToString(i => i == 0 ? "3" : "2"));

            expected = " 0  0  0  0  0\n 0  1  1  1  0\n 0  1  1  1  0\n 0  0  0  0  0";
            Assert.Equal(expected, translated.ToString(2, i => i.ToString()));
        }
    }
}
