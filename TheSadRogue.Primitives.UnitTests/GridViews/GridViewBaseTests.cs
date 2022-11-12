using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;

namespace SadRogue.Primitives.UnitTests.GridViews
{
    public class GridViewBaseTests
    {
        [Fact]
        public void TestClear()
        {
            var view = new GridViewBaseDefaultImplementationMock<int>(70, 51);
            foreach (var pos in view.Positions())
                view.View[pos] = pos.ToIndex(view.Width);

            foreach (var pos in view.Positions())
                Assert.Equal(default, view[pos]);
        }
    }
}
