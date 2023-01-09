using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;

namespace SadRogue.Primitives.UnitTests.GridViews
{
    public class GridViewBaseTests
    {
        [Fact]
        public void TestCount()
        {
            var view = new GridViewBaseDefaultImplementationMock<int>(15, 12);
            Assert.Equal(view.Width * view.Height, view.Count);
        }

        [Fact]
        public void TestBasicSets1D()
        {
            var view = new GridView1DIndexBaseDefaultImplementationMock<bool>(15, 12);
            foreach (var pos in view.Bounds().Expand(-1, -1).Positions())
                view.View[pos] = true;

            foreach (var pos in view.Positions())
            {
                Assert.Equal(view.View[pos], view[pos]);
                Assert.Equal(view.View[pos], view[pos.X, pos.Y]);
                Assert.Equal(view.View[pos], view[pos.ToIndex(view.Width)]);
            }
        }

        [Fact]
        public void TestCount1D()
        {
            var view = new GridView1DIndexBaseDefaultImplementationMock<int>(15, 12);
            Assert.Equal(view.Width * view.Height, view.Count);
        }
    }
}
