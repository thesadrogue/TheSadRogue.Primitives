﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    /// <summary>
    /// Tests for various shape-drawing algorithms.
    /// </summary>
    public class ShapesTests
    {
        public static int[] RadiusCases = { 1, 2, 5, 10, 25 };

        private static readonly Point s_center = (1, 2);

        [Theory]
        [MemberDataEnumerable(nameof(RadiusCases))]
        public void BoundsTest(int radius)
        {
            var bounds = new Rectangle(s_center, radius, radius);

            foreach (var point in Shapes.GetCircle(s_center, radius))
                Assert.True(bounds.Contains(point));

            // There should be at least 1 point from each perimeter side in the result
            foreach (var side in AdjacencyRule.Cardinals.DirectionsOfNeighborsCache)
            {
                var perimeterSide = bounds.PositionsOnSide(side).ToHashSet();
                Assert.Contains(Shapes.GetCircle(s_center, radius), i => perimeterSide.Contains(i));
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(RadiusCases))]
        public void IsOutlineTest(int radius)
        {
            foreach (var point in Shapes.GetCircle(s_center, radius - 1))
                Assert.DoesNotContain(Shapes.GetCircle(s_center, radius), i => i == point);
        }

        [Theory]
        [MemberDataEnumerable(nameof(RadiusCases))]
        public void EquivalentToSimpleImplementation(int radius)
        {
            var simple = SimpleCircle(s_center, radius).ToHashSet();
            var actual = Shapes.GetCircle(s_center, radius).ToHashSet();

            Assert.Equal(simple, actual);
        }

        private static IEnumerable<Point> SimpleCircle(Point center, int radius)
        {
            int xi = -radius, yi = 0, err = 2 - 2 * radius; /* II. Quadrant */
            do
            {
                yield return new Point(center.X - xi, center.Y + yi); /*   I. Quadrant */
                yield return new Point(center.X - yi, center.Y - xi); /*  II. Quadrant */
                yield return new Point(center.X + xi, center.Y - yi); /* III. Quadrant */
                yield return new Point(center.X + yi, center.Y + xi); /*  IV. Quadrant */
                radius = err;
                if (radius <= yi)
                    err += ++yi * 2 + 1;           /* e_xy+e_y < 0 */

                if (radius > xi || err > yi)
                    err += ++xi * 2 + 1; /* e_xy+e_x > 0 or no 2nd y-step */

            } while (xi < 0);
        }
    }
}
