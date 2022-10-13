using System.Linq;
using SadRogue.Primitives.GridViews;
using Xunit;

namespace SadRogue.Primitives.UnitTests.GridViews
{
    public class ArrayView2DTests
    {
        private const int Width = 71;
        private const int Height = 50;

        private readonly Point _center = new Point(Width / 2, Height / 2);

        #region Construction

        [Fact]
        public void ConstructWidthHeight()
        {
            var view = new ArrayView2D<bool>(Width, Height);
            Assert.Equal(Width, view.Width);
            Assert.Equal(Height, view.Height);

            foreach (var pos in view.Positions())
                Assert.False(view[pos]);
        }

        [Fact]
        public void ConstructExistingArray()
        {
            var array = new bool[Width, Height];
            array[_center.X, _center.Y] = true;

            var view = new ArrayView2D<bool>(array);
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
            Assert.True(array[0, 0]);

            array[1, 0] = true;
            Assert.True(view[1, 0]);
        }
        #endregion

        #region Get/Set

        [Fact]
        public void TestIndexerGetAndSet()
        {
            // Create view
            var positionsToSet = new Point[] { (1, 2), (3, 4), (25, 47) };
            var view = new ArrayView2D<bool>(Width, Height);

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
            var array = new bool[Width, Height];
            array[_center.X, _center.Y] = true;
            var view = new ArrayView2D<bool>(array);

            // Clone to another view
            var view2 = (ArrayView2D<bool>)view.Clone();

            // Values should be the same
            Assert.Equal(view.Width, view2.Width);
            Assert.Equal(view.Height, view2.Height);
            foreach (var pos in view.Positions())
                Assert.Equal(view[pos], view2[pos]);

            // But new array should not be the same reference; changes in one should not affect the other
            Assert.NotSame((bool[,])view, (bool[,])view2);
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
            var array = new bool[Width, Height];
            var view = new ArrayView2D<bool>(array);
            var view2 = new ArrayView2D<bool>(array);

            // And one that does not
            var view3 = new ArrayView2D<bool>(Width, Height);

            // ArrayView2Ds match iff they reference the same array
            Assert.True(view.Matches(view2));
            Assert.False(view.Matches(view3));
        }

        #endregion

        #region Array Conversion

        [Fact]
        public void TestToAndFromArrayConversion()
        {
            // Create view with one array
            var array = new bool[Width, Height];
            var view = new ArrayView2D<bool>(array);
            var view2 = ArrayView2D<bool>.FromMultidimensionalArray(array);

            // Arrays should be the same instance as the original, not a duplicate.
            Assert.Same(array, (bool[,])view);
            Assert.Same(array, view.ToMultidimensionalArray());
            Assert.Same(array, (bool[,])view2);
            Assert.Same(array, view2.ToMultidimensionalArray());
        }
        #endregion

        #region Clear and Fill
        [Fact]
        public void TestClearAndFill()
        {
            var view = new ArrayView2D<bool>(Width, Height);
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
