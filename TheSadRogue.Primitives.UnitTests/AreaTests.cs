using System.Collections.Generic;
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
        public void EnumerableCorrect()
        {
            var rect = new Rectangle(0, 0, 15, 15);
            var area = new Area(rect.Positions().ToEnumerable());
            IReadOnlyArea areaInterface = area;

            var expected = new List<Point>();
            for (int i = 0; i < area.Count; i++)
                expected.Add(area[i]);

            List<Point> l1 = area.ToEnumerable().ToList();

            var l2 = new List<Point>();
            foreach (var pos in area)
                l2.Add(pos);

            var l3 = new List<Point>();
            foreach (var pos in areaInterface)
                l3.Add(pos);
            

            Assert.Equal(expected, l1);
            Assert.Equal(expected, l2);
            Assert.Equal(expected, l3);
        }
    }
}
