using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SadRogue.Primitives.UnitTests
{
    public class PointTests
    {
        #region BearingOfLine

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestBearingOfLineKnownValues(bool yIncreasesUpwards)
        {
            Direction.SetYIncreasesUpwardsUnsafe(yIncreasesUpwards);

            // Zero degrees must point up and degrees should increment clockwise
            Point center = (1, 1);
            var positions = AdjacencyRule.EightWay.DirectionsOfNeighborsClockwise(Direction.Up).Select(i => center + i).ToArray();

            double[] bearings = positions.Select(i => Point.BearingOfLine(center, i)).ToArray();
            double[] bearings2 = positions.Select(i => Point.BearingOfLine(i - center)).ToArray();
            double[] bearings3 = positions.Select(i => Point.BearingOfLine((i - center).X, (i - center).Y)).ToArray();
            double[] bearings4 = positions.Select(i => Point.BearingOfLine(center.X, center.Y, i.X, i.Y)).ToArray();

            Assert.Equal(8, bearings.Length);

            Assert.Equal((IEnumerable<double>)bearings, bearings2);
            Assert.Equal((IEnumerable<double>)bearings, bearings3);
            Assert.Equal((IEnumerable<double>)bearings, bearings4);

            double expectedBearing = 0;
            double increment = 45;
            foreach (double bearing in bearings)
            {
                Assert.Equal(expectedBearing, bearing);
                expectedBearing = (expectedBearing + increment) % 360;
            }

            Direction.SetYIncreasesUpwardsUnsafe(false); // Ensure we reset to false for next test
        }
        #endregion
    }
}
