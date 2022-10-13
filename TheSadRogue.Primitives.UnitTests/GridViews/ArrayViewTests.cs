using System;
using System.Linq;
using SadRogue.Primitives.GridViews;
using Xunit;

namespace SadRogue.Primitives.UnitTests.GridViews
{
    public class ArrayViewTests
    {
        private const int Width = 71;
        private const int Height = 50;

        private readonly Point _center = new Point(Width / 2, Height / 2);

        #region Construction

        [Fact]
        public void ConstructWidthHeight()
        {
            var view = new ArrayView<bool>(Width, Height);
            Assert.Equal(Width, view.Width);
            Assert.Equal(Height, view.Height);

            foreach (var pos in view.Positions())
                Assert.False(view[pos]);
        }

        [Fact]
        public void ConstructExistingArray()
        {
            var array = new bool[Width * Height];
            array[_center.ToIndex(Width)] = true;

            // Can't create if the array isn't an even size relative to the width (index mapping wouldn't make sense)
            Assert.Throws<ArgumentException>(() => new ArrayView<bool>(array, Width - 1));

            var view = new ArrayView<bool>(array, Width);
            Assert.Equal(Width, view.Width);
            Assert.Equal(Height, view.Height);

            // Ensure existing array was actually used
            foreach (var pos in view.Positions())
                if (pos.Matches(_center))
                    Assert.True(view[pos]);
                else
                    Assert.False(view[pos]);

            // Change either via the array or the array view should affect both
            view[0, 0] = true;
            Assert.True(array[new Point(0, 0).ToIndex(Width)]);

            array[new Point(1, 0).ToIndex(Width)] = true;
            Assert.True(view[1, 0]);
        }
        #endregion

        #region Get/Set

        [Fact]
        public void TestIndexerGetAndSet()
        {
            // Create view
            var positionsToSet = new Point[] { (1, 2), (3, 4), (25, 47) };
            var view = new ArrayView<bool>(Width, Height);

            // Set positions with indexers
            view[positionsToSet[0]] = true;
            view[positionsToSet[1].X, positionsToSet[1].Y] = true;
            view[positionsToSet[2].ToIndex(Width)] = true;

            foreach (var pos in view.Positions())
            {
                Assert.Equal(positionsToSet.Contains(pos), view[pos]);
                Assert.Equal(view[pos], view[pos.X, pos.Y]);
                Assert.Equal(view[pos], view[pos.ToIndex(Width)]);
            }
        }
        #endregion

        #region Cloning

        [Fact]
        public void TestCloneDeepCopies()
        {
            // Create view with one array
            var array = new bool[Width * Height];
            array[_center.ToIndex(Width)] = true;
            var view = new ArrayView<bool>(array, Width);

            // Clone to another view
            var view2 = (ArrayView<bool>)view.Clone();

            // Values should be the same
            Assert.Equal(view.Width, view2.Width);
            Assert.Equal(view.Height, view2.Height);
            foreach (var pos in view.Positions())
                Assert.Equal(view[pos], view2[pos]);

            // But new array should not be the same reference; changes in one should not affect the other
            Assert.NotSame((bool[])view, (bool[])view2);
            view[1, 2] = true;
            Assert.False(view2[1, 2]);
            view2[3, 4] = true;
            Assert.False(view[3, 4]);
        }
        #endregion

        #region Matches

        [Fact]
        public void TestMatches()
        {
            // Create a couple views that use the same array
            var array = new bool[Width * Height];
            var view = new ArrayView<bool>(array, Width);
            var view2 = new ArrayView<bool>(array, Width);

            // And one that does not
            var view3 = new ArrayView<bool>(Width, Height);

            // ArrayViews match iff they reference the same array
            Assert.True(view.Matches(view2));
            Assert.False(view.Matches(view3));
        }

        #endregion

        #region Array Conversion

        [Fact]
        public void TestToArrayConversion()
        {
            // Create view with one array
            var array = new bool[Width * Height];
            var view = new ArrayView<bool>(array, Width);

            // Arrays should be the same instance as the original, not a duplicate.
            Assert.Same(array, (bool[])view);
            Assert.Same(array, view.ToArray());
        }
        #endregion

        #region Clear and Fill
        [Fact]
        public void TestClearAndFill()
        {
            var view = new ArrayView<bool>(Width, Height);
            view.Fill(true);
            foreach (var pos in view.Positions())
                Assert.True(view[pos]);

            view.Clear();
            foreach (var pos in view.Positions())
                Assert.False(view[pos]);
        }
        #endregion
    }
}
