using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    /// <summary>
    /// Tests for various shape-drawing algorithms.
    /// </summary>
    public class ShapesTests
    {
        #region Test Data
        public static int[] RadiusCases = { 1, 2, 5, 10, 25 };

        // Test cases should include:
        //    - Positive and negative x/y values
        //    - Lines covering all octants
        //    - Slopes which are not an integer value
        //    - A line such that start == end
        //    - Vertical lines
        //    - Horizontal lines
        public static readonly (Point f1, Point f2)[] EllipseCases =
        {
            // Octant 1: 0 < dx < dy
            ((-1, -2), (25, 40)),
            // Octant 0: 0 < dy <= dx
            ((-1, -2), (40, 25)),
            // Octant 7: 0 < -dy <= dx
            ((-1, -2), (40, -25)),
            // Octant 6: 0 < dx < -dy
            ((-1, -2), (25, -40)),
            // Octant 5: 0 < -dx < -dy
            ((-1, -2), (-25, -40)),
            // Octant 4: 0 < -dy <= -dx
            ((-1, -2), (-40, -25)),
            // Octant 3: 0 < dy <= -dx
            ((-1, -2), (-40, 25)),
            // Octant 2: 0 < -dx < dy
            ((-1, -2), (-25, 40)),
            // Start == end
            ((10, 11), (10, 11)),
            // Vertical lines
            ((5, 6), (5, 10)),
            ((5, 10), (5, 6)),
            // Horizontal lines
            ((6, 5), (10, 5)),
            ((10, 5), (6, 5)),
        };
        private static readonly Point s_center = (1, 2);

        public static readonly Rectangle[] BoxTestCases = { new Rectangle(1, 2, 3, 4), new Rectangle(-15, -16, 10, 9) };

        #endregion

        #region Circle Tests
        [Theory]
        [MemberDataEnumerable(nameof(RadiusCases))]
        public void CircleBoundsTest(int radius)
        {
            var circle = CircleToHashSetDirect(Shapes.GetCircle(s_center, radius));

            var bounds = new Rectangle(s_center, radius, radius);

            foreach (var point in circle)
                Assert.True(bounds.Contains(point));

            // There should be at least 1 point from each perimeter side in the result
            foreach (var side in AdjacencyRule.Cardinals.DirectionsOfNeighborsCache)
            {
                var perimeterSide = bounds.PositionsOnSide(side).ToHashSet();
                Assert.Contains(circle, i => perimeterSide.Contains(i));
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(RadiusCases))]
        public void CircleIsOutlineTest(int radius)
        {
            var outer = CircleToHashSetDirect(Shapes.GetCircle(s_center, radius));

            foreach (var point in Shapes.GetCircle(s_center, radius - 1))
                Assert.DoesNotContain(point, outer);
        }

        [Theory]
        [MemberDataEnumerable(nameof(RadiusCases))]
        public void CircleEquivalentToSimpleImplementation(int radius)
        {
            var simple = SimpleCircle(s_center, radius).ToHashSet();
            var actual = CircleToHashSetDirect(Shapes.GetCircle(s_center, radius));

            Assert.Equal(simple, actual);
        }

        [Theory]
        [MemberDataEnumerable(nameof(RadiusCases))]
        public void CircleEnumerableEquivalentToCustomIterator(int radius)
        {
            var points = new List<Point>();
            foreach (var point in Shapes.GetCircle(s_center, radius))
                points.Add(point);

            var enumerable = Shapes.GetCircle(s_center, radius);
            Assert.Equal(points, enumerable);
        }

        [Theory]
        [MemberDataEnumerable(nameof(RadiusCases))]
        public void CircleEnumerableEquivalentToDeprecatedToEnumerable(int radius)
        {
            IEnumerable<Point> expected = Shapes.GetCircle(s_center, radius);
#pragma warning disable CS0618
            IEnumerable<Point> actual = Shapes.GetCircle(s_center, radius).ToEnumerable();
#pragma warning restore CS0618

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberDataEnumerable(nameof(RadiusCases))]
        public void CircleEnumerator(int radius)
        {
            var list = new List<Point>();
            using IEnumerator<Point> enumerator = ((IEnumerable<Point>)Shapes.GetCircle(s_center, radius)).GetEnumerator();
            while (enumerator.MoveNext())
                list.Add(enumerator.Current);

            Assert.Equal(Shapes.GetCircle(s_center, radius), list);

            Assert.Throws<NotSupportedException>(() => enumerator.Reset());
        }

        [Fact]
        public void ZeroRadiusCircle()
        {
            var points = CircleToHashSetDirect(Shapes.GetCircle(s_center, 0));
            var enumerable = Shapes.GetCircle(s_center, 0).ToHashSet();

            Assert.Single(points);
            Assert.Contains(s_center, points);
            Assert.Equal(points, enumerable);
        }

        private HashSet<Point> CircleToHashSetDirect(CirclePositionsEnumerator enumerator)
        {
            // We don't use ToEnumerable or LINQ to ensure we invoke the MoveNext implementation even if GetEnumerable
            // is implemented differently
            var points = new HashSet<Point>();
            foreach (var point in enumerator)
                points.Add(point);

            return points;
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
        #endregion

        #region Ellipse Tests

        [Theory]
        [MemberDataTuple(nameof(EllipseCases))]
        public void EllipseEquivalentToSimpleImplementation(Point f1, Point f2)
        {
            var simple = SimpleEllipse(f1.X, f1.Y, f2.X, f2.Y).ToHashSet();
            var actual = EllipseToHashSetDirect(Shapes.GetEllipse(f1, f2));

            Assert.Equal(simple, actual);
        }

        [Theory]
        [MemberDataTuple(nameof(EllipseCases))]
        public void EllipseEnumerableEquivalentToCustomIterator(Point f1, Point f2)
        {
            var points = new List<Point>();
            foreach (var point in Shapes.GetEllipse(f1, f2))
                points.Add(point);

            var enumerable = Shapes.GetEllipse(f1, f2);
            Assert.Equal(points, enumerable);
        }

        [Theory]
        [MemberDataTuple(nameof(EllipseCases))]
        public void EllipseEnumerableEquivalentToDeprecatedToEnumerable(Point f1, Point f2)
        {
            IEnumerable<Point> expected = Shapes.GetEllipse(f1, f2);
#pragma warning disable CS0618
            IEnumerable<Point> actual = Shapes.GetEllipse(f1, f2).ToEnumerable();
#pragma warning restore CS0618

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberDataTuple(nameof(EllipseCases))]
        public void EllipseEnumerator(Point f1, Point f2)
        {
            var list = new List<Point>();
            using IEnumerator<Point> enumerator = ((IEnumerable<Point>)Shapes.GetEllipse(f1, f2)).GetEnumerator();
            while (enumerator.MoveNext())
                list.Add(enumerator.Current);

            Assert.Equal(Shapes.GetEllipse(f1, f2), list);

            Assert.Throws<NotSupportedException>(() => enumerator.Reset());
        }


        private HashSet<Point> EllipseToHashSetDirect(EllipsePositionsEnumerator enumerator)
        {
            // We don't use ToEnumerable or LINQ to ensure we invoke the MoveNext implementation even if GetEnumerable
            // is implemented differently
            var points = new HashSet<Point>();
            foreach (var point in enumerator)
                points.Add(point);

            return points;
        }

        public static IEnumerable<Point> SimpleEllipse(int x0, int y0, int x1, int y1)
        {
            int a = Math.Abs(x1 - x0), b = Math.Abs(y1 - y0), b1 = b & 1; /* values of diameter */
            long dx = 4 * (1 - a) * b * b, dy = 4 * (b1 + 1) * a * a; /* error increment */
            long err = dx + dy + b1 * a * a, e2; /* error of 1.step */

            if (x0 > x1) { x0 = x1; x1 += a; } /* if called with swapped points */
            if (y0 > y1)
            {
                y0 = y1; /* .. exchange them */
            }

            y0 += (b + 1) / 2; y1 = y0 - b1;   /* starting pixel */
            a *= 8 * a; b1 = 8 * b * b;

            do
            {
                yield return new Point(x1, y0); /*   I. Quadrant */
                yield return new Point(x0, y0); /*  II. Quadrant */
                yield return new Point(x0, y1); /* III. Quadrant */
                yield return new Point(x1, y1); /*  IV. Quadrant */
                e2 = 2 * err;
                if (e2 <= dy) { y0++; y1--; err += dy += a; }  /* y step */
                if (e2 >= dx || 2 * err > dy) { x0++; x1--; err += dx += b1; } /* x step */
            } while (x0 <= x1);

            while (y0 - y1 < b)
            {  /* too early stop of flat ellipses a=1 */
                yield return new Point(x0 - 1, y0); /* -> finish tip of ellipse */
                yield return new Point(x1 + 1, y0++);
                yield return new Point(x0 - 1, y1);
                yield return new Point(x1 + 1, y1--);
            }
        }

        #endregion

        #region Box Tests

        [Theory]
        [MemberDataEnumerable(nameof(BoxTestCases))]
        public void BoxEquivalentToSimpleImplementation(Rectangle rectangle)
        {
            var simple = SimpleBox(rectangle.MinExtentX, rectangle.MinExtentY, rectangle.MaxExtentX, rectangle.MaxExtentY).ToHashSet();
            var actual = BoxToHashSetDirect(Shapes.GetBox(rectangle));

            Assert.Equal(simple, actual);
        }

        [Theory]
        [MemberDataEnumerable(nameof(BoxTestCases))]
        public void BoxEnumerableEquivalentToCustomIterator(Rectangle rectangle)
        {
            var points = new List<Point>();
            foreach (var point in Shapes.GetBox(rectangle))
                points.Add(point);

            var enumerable = Shapes.GetBox(rectangle).ToList();
            Assert.Equal((IEnumerable<Point>)points, enumerable);
        }

        private HashSet<Point> BoxToHashSetDirect(RectanglePerimeterPositionsEnumerator enumerator)
        {
            // We don't use ToEnumerable or LINQ to ensure we invoke the MoveNext implementation even if GetEnumerable
            // is implemented differently
            var points = new HashSet<Point>();
            foreach (var point in enumerator)
                points.Add(point);

            return points;
        }

        public static IEnumerable<Point> SimpleBox(int x0, int y0, int x1, int y1)
        {
            for (int x = x0; x <= x1; x++)
                yield return new Point(x, y0);
            for (int y = y0; y <= y1; y++)
                yield return new Point(x1, y);
            for (int x = x1; x >= x0; x--)
                yield return new Point(x, y1);
            for (int y = y1; y >= y0; y--)
                yield return new Point(x0, y);
        }

        #endregion
    }
}
