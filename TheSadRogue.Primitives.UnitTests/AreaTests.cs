using System.Linq;
using Xunit;

namespace SadRogue.Primitives.UnitTests
{
    /// <summary>
    /// Tests for the <see cref="Area"/> class
    /// </summary>
    public class AreaTests
    {
        [Fact]
        public void EnumerableEquivalence()
        {
            var area = new Area(new Rectangle(0, 0, 15, 15).Positions().ToEnumerable());

            var l1 = area.ToList();
            var l2 = area.FastEnumerator().ToEnumerable().ToList();

            Assert.Equal(l1, l2);
        }
    }
}
