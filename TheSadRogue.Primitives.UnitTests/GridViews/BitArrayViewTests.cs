using System;
using System.Collections;
using SadRogue.Primitives.GridViews;
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
    }
}
