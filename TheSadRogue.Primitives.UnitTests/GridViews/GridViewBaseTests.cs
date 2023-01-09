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
    }
}
