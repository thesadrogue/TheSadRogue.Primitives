using System;
using System.Collections;
using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;

namespace SadRogue.Primitives.UnitTests.GridViews
{
    /// <summary>
    /// Tests for the <see cref="BitArrayView"/> class.
    /// </summary>
    public class BitArrayViewTests
    {
        [Fact]
        public void ConstructorWidthHeight()
        {
            var view = new BitArrayView(80, 56);
            Assert.Equal(80, view.Width);
            Assert.Equal(56, view.Height);
            Assert.Equal(view.Width * view.Height, view.Count);

            foreach (var pos in view.Positions())
            {
                Assert.False(view[pos]);
                Assert.False(view[pos.X, pos.Y]);
                Assert.False(view[pos.ToIndex(view.Width)]);
            }
        }

        [Fact]
        public void ConstructorExistingBitArray()
        {
            var array = new BitArray(80 * 56);
            var view = new BitArrayView(array, 80);
            Assert.Equal(80, view.Width);
            Assert.Equal(56, view.Height);
            Assert.Equal(view.Width * view.Height, view.Count);

            foreach (var pos in view.Positions())
            {
                Assert.False(view[pos]);
                Assert.False(view[pos.X, pos.Y]);
                Assert.False(view[pos.ToIndex(view.Width)]);
            }

            Assert.Throws<ArgumentException>(() => new BitArrayView(array, 81));
        }

        [Fact]
        public void Fill()
        {
            var view = new BitArrayView(80, 56);
            view.Fill(true);
            foreach (var pos in view.Positions())
            {
                Assert.True(view[pos]);
                Assert.True(view[pos.X, pos.Y]);
                Assert.True(view[pos.ToIndex(view.Width)]);
            }
        }

        [Fact]
        public void Clone()
        {
            var view = new BitArrayView(80, 56);
            view.Fill(true);

            var view2 = (BitArrayView)view.Clone();
            Assert.Equal(view.Width, view2.Width);
            Assert.Equal(view.Height, view2.Height);
            Assert.Equal(view.Count, view2.Count);

            foreach (var pos in view.Positions())
                Assert.Equal(view[pos], view2[pos]);

            view2[0, 0] = false;
            Assert.False(view2[0, 0]);
            Assert.True(view[0, 0]);
        }

        [Fact]
        public void Matches()
        {
            const int width = 80;
            const int height = 56;

            var array = new BitArray(width * height);

            // Matches = true; uses same array
            var view1 = new BitArrayView(array, width);
            var view2 = new BitArrayView(array, width);

            // Doesn't match the other two; same size and values but different array.
            var view3 = new BitArrayView(width, height);

            Assert.True(view1.Matches(view2));
            Assert.True(view2.Matches(view1));

            Assert.False(view1.Matches(view3));
            Assert.False(view3.Matches(view1));
            Assert.False(view2.Matches(view3));
            Assert.False(view3.Matches(view2));
        }

        [Fact]
        public void ImplicitBitArrayConversions()
        {
            const int width = 80;
            const int height = 56;

            var array = new BitArray(width * height);
            var view = new BitArrayView(array, width);
            BitArray viewArray = view;
            BitArray viewArray2 = view.ToBitArray();

            Assert.Equal(array, viewArray);
            Assert.Equal(array, viewArray2);

            var view2 = new BitArrayView(width, height);
            BitArray array2 = view2;
            Assert.NotNull(array2);
            Assert.NotSame(array, array2);
        }

        [Fact]
        public void BitArrayViewToString()
        {
            var grid = MockGridViews.RectangleBooleanGrid(5, 4);
            var view = new BitArrayView(grid.Width, grid.Height);
            view.ApplyOverlay(grid);

            string expected = "False False False False False\nFalse True True True False\nFalse True True True False\nFalse False False False False";
            Assert.Equal(expected, view.ToString());

            string Stringifier(bool b) => b ? "1" : "0";

            expected = expected.Replace("False", "0").Replace("True", "1");
            Assert.Equal(expected, view.ToString(Stringifier));

            expected = " 0  0  0  0  0\n 0  1  1  1  0\n 0  1  1  1  0\n 0  0  0  0  0";
            Assert.Equal(expected, view.ToString(2, Stringifier));
        }
    }
}
