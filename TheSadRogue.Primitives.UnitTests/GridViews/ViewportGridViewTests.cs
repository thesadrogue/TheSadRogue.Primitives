using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;
// Should disable this because the functions triggering it are just assertion methods
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace SadRogue.Primitives.UnitTests.GridViews
{
    public class ViewportGridViewTests
    {
        [Fact]
        public void ViewportBoundingRectangleTest()
        {
            const int viewportWidth = 1280 / 12;
            const int viewportHeight = 768 / 12;
            const int gridWidth = 267;
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
        public void ViewportSimpleConstructorTest()
        {
            const int gridWidth = 267;
            const int gridHeight = 250;

            var grid = MockGridViews.RectangleBooleanGrid(gridWidth, gridHeight);
            var viewport = new Viewport<bool>(grid);

            CheckViewportBounds(viewport, (0, 0), (gridWidth - 1, gridHeight - 1));
        }

        [Fact]
        public void ViewportToStringTest()
        {
            var grid = MockGridViews.RectangleBooleanGrid(56, 42);
            var viewport = new Viewport<bool>(grid, new Rectangle(0, 0, 5, 4));

            string result = viewport.ToString();
            string expected = "False False False False False\nFalse True True True True\nFalse True True True True\nFalse True True True True";

            Assert.Equal(expected, result);

            viewport.SetViewArea(viewport.ViewArea.WithPosition((1, 2)));
            expected = "True True True True True\nTrue True True True True\nTrue True True True True\nTrue True True True True";

            result = viewport.ToString();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ViewportToStringWithStringifierTest()
        {
            var grid = MockGridViews.RectangleBooleanGrid(56, 42);
            var viewport = new Viewport<bool>(grid, new Rectangle(0, 0, 5, 4));

            string Stringifier(bool i) => i ? "1" : "0";

            string result = viewport.ToString(Stringifier);
            string expected = "0 0 0 0 0\n0 1 1 1 1\n0 1 1 1 1\n0 1 1 1 1";

            Assert.Equal(expected, result);

            viewport.SetViewArea(viewport.ViewArea.WithPosition((1, 2)));
            expected = "1 1 1 1 1\n1 1 1 1 1\n1 1 1 1 1\n1 1 1 1 1";

            result = viewport.ToString(Stringifier);
            Assert.Equal(expected, result);

            result = viewport.ToString(2, Stringifier);
            expected = " 1  1  1  1  1\n 1  1  1  1  1\n 1  1  1  1  1\n 1  1  1  1  1";
            Assert.Equal(expected, result);
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

        [Fact]
        public void UnboundedViewportToStringTest()
        {
            var view = MockGridViews.RectangleBooleanGrid(71, 50);
            var viewport = new UnboundedViewport<bool>(view, new Rectangle(view.Bounds().MaxExtent, 3, 2), false);

            string expected =
                "True True True False False False False\nTrue True True False False False False\nFalse False False False False False False\nFalse False False False False False False\nFalse False False False False False False";
            string actual = viewport.ToString();
            Assert.Equal(expected, actual);

            expected = expected.Replace("True", "1").Replace("False", "0");
            actual = viewport.ToString(b => b ? "1" : "0");
            Assert.Equal(expected, actual);

            expected = expected.Replace("0", " 0").Replace("1", " 1");
            actual = viewport.ToString(2, b => b ? "1" : "0");
            Assert.Equal(expected, actual);
        }

        private static void CheckViewportBounds(Viewport<bool> viewport, Point expectedMinCorner,
                                                Point expectedMaxCorner)
        {
            Assert.Equal(expectedMaxCorner.X - expectedMinCorner.X + 1, viewport.Width);
            Assert.Equal(expectedMaxCorner.Y - expectedMinCorner.Y + 1, viewport.Height);

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
    }
}
