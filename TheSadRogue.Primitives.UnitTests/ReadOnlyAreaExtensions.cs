using SadRogue.Primitives.UnitTests.Mocks;
using Xunit;

namespace SadRogue.Primitives.UnitTests
{
    public class ReadOnlyAreaExtensions
    {
        [Fact]
        public void UseIndexEnumerationFalseDefault()
        {
            IReadOnlyArea area = new MockReadOnlyAreaNoUseIndex();
            Assert.False(area.UseIndexEnumeration);
        }

        [Fact]
        public void UseIndexEnumerationExtensionCompability()
        {
            var area = new MockReadOnlyAreaSetUseIndex();
            IReadOnlyArea area2 = area;
            Assert.True(area.UseIndexEnumeration);
            Assert.True(area2.UseIndexEnumeration);
        }
    }
}
