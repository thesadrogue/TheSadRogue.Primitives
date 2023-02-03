using System.Collections.Generic;
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

        [Fact]
        public void FastEnumeratorTriggersCorrectEnumerator()
        {
            var area = new ReadOnlyAreaCountEnumerations { UseIndexEnumeration = false };

            // Iteration should take place using the GetEnumerator function
            area.ClearCounts();
            var list = new List<Point>();
            foreach (var pos in new ReadOnlyAreaPositionsEnumerator(area))
                list.Add(pos);
            Assert.Equal(area.Points, list);
            Assert.Equal(0, area.GetIndexCount);
            Assert.Equal(1, area.GetEnumeratorCount);

            // Iteration should take place using the indexers
            list.Clear();
            area.ClearCounts();
            area.UseIndexEnumeration = true;
            foreach (var pos in new ReadOnlyAreaPositionsEnumerator(area))
                list.Add(pos);

            Assert.Equal(area.Points, list);
            Assert.Equal(area.Points.Count, area.GetIndexCount);
            Assert.Equal(0, area.GetEnumeratorCount);
        }
    }
}
