using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;

namespace SadRogue.Primitives.UnitTests.GridViews
{
    /// <summary>
    /// Tests for extension methods/default method implementations for <see cref="ISettableGridView{T}"/>.
    /// </summary>
    public class SettableGridViewExtensionTests
    {
        [Fact]
        public void Fill()
        {
            ISettableGridView<bool> view = new SettableGridViewDefaultImplementationMock<bool>(80, 57);
            view.Fill(true);

            foreach (var pos in view.Positions())
                Assert.True(view[pos]);
        }

        [Fact]
        public void Clear()
        {
            ISettableGridView<bool> view = new SettableGridViewDefaultImplementationMock<bool>(80, 57);
            view.ApplyOverlay(i => true);
            view.Clear();

            foreach (var pos in view.Positions())
                Assert.False(view[pos]);
        }
    }
}
