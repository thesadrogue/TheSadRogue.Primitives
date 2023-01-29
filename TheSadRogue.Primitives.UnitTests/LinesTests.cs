using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{

    public static class SimpleAlgorithms
    {
        private const int ModifierX = 0x7fff;
        private const int ModifierY = 0x7fff;

        public static IEnumerable<Point> Bresenham(int startX, int startY, int endX, int endY)
        {
            int w = endX - startX;
            int h = endY - startY;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1;
            else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1;
            else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1;
            else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1;
                else if (h > 0) dy2 = 1;
                dx2 = 0;
            }

            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                yield return new Point(startX, startY);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    startX += dx1;
                    startY += dy1;
                }
                else
                {
                    startX += dx2;
                    startY += dy2;
                }
            }
        }

        public static IEnumerable<Point> DDA(int startX, int startY, int endX, int endY)
        {
            int dx = endX - startX;
            int dy = endY - startY;

            int nx = Math.Abs(dx);
            int ny = Math.Abs(dy);

            // Calculate octant value
            int octant = (dy < 0 ? 4 : 0) | (dx < 0 ? 2 : 0) | (ny > nx ? 1 : 0);
            int fraction = 0;
            int mn = Math.Max(nx, ny);

            int move;

            if (mn == 0)
            {
                yield return new Point(startX, startY);
                yield break;
            }

            if (ny == 0)
            {
                if (dx > 0)
                    for (int x = startX; x <= endX; x++)
                        yield return new Point(x, startY);
                else
                    for (int x = startX; x >= endX; x--)
                        yield return new Point(x, startY);

                yield break;
            }

            if (nx == 0)
            {
                if (dy > 0)
                    for (int y = startY; y <= endY; y++)
                        yield return new Point(startX, y);
                else
                    for (int y = startY; y >= endY; y--)
                        yield return new Point(startX, y);

                yield break;
            }

            switch (octant)
            {
                case 0: // +x, +y
                    move = (ny << 16) / nx;
                    for (int primary = startX; primary <= endX; primary++, fraction += move)
                        yield return new Point(primary, startY + ((fraction + ModifierY) >> 16));
                    break;

                case 1:
                    move = (nx << 16) / ny;
                    for (int primary = startY; primary <= endY; primary++, fraction += move)
                        yield return new Point(startX + ((fraction + ModifierX) >> 16), primary);
                    break;

                case 2: // -x, +y
                    move = (ny << 16) / nx;
                    for (int primary = startX; primary >= endX; primary--, fraction += move)
                        yield return new Point(primary, startY + ((fraction + ModifierY) >> 16));
                    break;

                case 3:
                    move = (nx << 16) / ny;
                    for (int primary = startY; primary <= endY; primary++, fraction += move)
                        yield return new Point(startX - ((fraction + ModifierX) >> 16), primary);
                    break;

                case 6: // -x, -y
                    move = (ny << 16) / nx;
                    for (int primary = startX; primary >= endX; primary--, fraction += move)
                        yield return new Point(primary, startY - ((fraction + ModifierY) >> 16));
                    break;

                case 7:
                    move = (nx << 16) / ny;
                    for (int primary = startY; primary >= endY; primary--, fraction += move)
                        yield return new Point(startX - ((fraction + ModifierX) >> 16), primary);
                    break;

                case 4: // +x, -y
                    move = (ny << 16) / nx;
                    for (int primary = startX; primary <= endX; primary++, fraction += move)
                        yield return new Point(primary, startY - ((fraction + ModifierY) >> 16));
                    break;

                case 5:
                    move = (nx << 16) / ny;
                    for (int primary = startY; primary >= endY; primary--, fraction += move)
                        yield return new Point(startX + ((fraction + ModifierX) >> 16), primary);
                    break;
            }
        }

        public static IEnumerable<Point> Ortho(int startX, int startY, int endX, int endY)
        {
            int dx = endX - startX;
            int dy = endY - startY;

            int nx = Math.Abs(dx);
            int ny = Math.Abs(dy);

            int signX = dx > 0 ? 1 : -1;
            int signY = dy > 0 ? 1 : -1;

            int workX = startX;
            int workY = startY;

            yield return new Point(startX, startY);

            for (int ix = 0, iy = 0; ix < nx || iy < ny;)
            {
                // Optimized version of `if ((0.5 + ix) / nx < (0.5 + iy) / ny)`
                if ((1 + ix + ix) * ny < (1 + iy + iy) * nx)
                {
                    workX += signX;
                    ix++;
                }
                else
                {
                    workY += signY;
                    iy++;
                }

                yield return new Point(workX, workY);
            }
        }
    }


    public class LinesTests
    {
        #region TestData
        // Test cases should include:
        //    - Positive and negative x/y values
        //    - Lines covering all octants
        //    - Slopes which are not an integer value
        //    - A line such that start == end
        //    - Vertical lines
        //    - Horizontal lines
        private static readonly (Point start, Point End)[] s_testLines =
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

        // Algorithms which are guaranteed to return items in order from start to finish.
        private static readonly Lines.Algorithm[] s_orderedAlgorithms =
        {
            Lines.Algorithm.Bresenham,
            Lines.Algorithm.DDA,
            Lines.Algorithm.Orthogonal
        };

        // Each line algorithm paired with how it defines adjacency/distance between points.
        private static readonly (Lines.Algorithm, Distance distanceRule)[] s_adjacency =
        {
            (Lines.Algorithm.Bresenham, Distance.Chebyshev),
            (Lines.Algorithm.DDA, Distance.Chebyshev),
            (Lines.Algorithm.Orthogonal, Distance.Manhattan)
        };

        private static readonly Lines.Algorithm[] s_allLineAlgorithms = Enum.GetValues<Lines.Algorithm>().ToArray();

        public static IEnumerable<(Lines.Algorithm algo, (Point start, Point end) points)> OrderedTestCases =
            s_orderedAlgorithms.Combinate(s_testLines);

        public static IEnumerable<(Lines.Algorithm algo, Distance distanceRule, (Point start, Point end) points)>
            AllTestCasesWithDistance =
                s_adjacency.Combinate(s_testLines);

        public static IEnumerable<(Lines.Algorithm algo, (Point start, Point end) points)> AllTestCases =
            s_allLineAlgorithms.Combinate(s_testLines);

        #endregion

        [Theory]
        [MemberDataTuple(nameof(OrderedTestCases))]
        public void LineOrderingTests(Lines.Algorithm algo, (Point start, Point end) points)
        {
            var line = Lines.GetLine(points.start, points.end, algo).ToArray();
            Assert.Equal(points.start, line[0]);
            Assert.Equal(points.end, line[^1]);
        }

        [Theory]
        [MemberDataTuple(nameof(AllTestCasesWithDistance))]
        public void LineAdjacencyTests(Lines.Algorithm algo, Distance distanceRule, (Point start, Point end) points)
        {
            var line = Lines.GetLine(points.start, points.end, algo).ToArray();

            for (int i = 1; i < line.Length; i++)
                Assert.Equal(1, distanceRule.Calculate(line[i - 1], line[i]));
        }

        [Theory]
        [MemberDataTuple(nameof(AllTestCases))]
        public void LineBoundsTests(Lines.Algorithm algo, (Point start, Point end) points)
        {
            var min = new Point(Math.Min(points.start.X, points.end.X), Math.Min(points.start.Y, points.end.Y));
            var max = new Point(Math.Max(points.start.X, points.end.X), Math.Max(points.start.Y, points.end.Y));
            var expectedBounds = new Rectangle(min, max);

            var line = Lines.GetLine(points.start, points.end, algo).ToArray();
            foreach (var point in line)
                Assert.True(expectedBounds.Contains(point));
        }

        [Theory]
        [MemberDataTuple(nameof(AllTestCases))]
        public void LineOverloadsEquivalent(Lines.Algorithm algo, (Point start, Point end) points)
        {
            var expected = Lines.GetLine(points.start, points.end, algo);
            var actual = Lines.GetLine(points.start.X, points.start.Y, points.end.X, points.end.Y, algo);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BadAlgorithmTest()
        {
            Assert.Throws<ArgumentException>(() => Lines.GetLine((1, 2), (3, 4), (Lines.Algorithm)100));
            Assert.Throws<ArgumentException>(() => Lines.GetLine(1, 2, 3, 4, (Lines.Algorithm)100));
        }

        [Theory]
        [MemberDataTuple(nameof(AllTestCases))]
        public void EquivalentToSimpleImplementation(Lines.Algorithm algo, (Point start, Point end) points)
        {
            Func<int, int, int, int, IEnumerable<Point>> expectedFunc = algo switch
            {
                Lines.Algorithm.Bresenham => SimpleAlgorithms.Bresenham,
                Lines.Algorithm.DDA => SimpleAlgorithms.DDA,
                Lines.Algorithm.Orthogonal => SimpleAlgorithms.Ortho,
                _ => throw new Exception("Unsupported algorithm."),
            };

            var expected = expectedFunc(points.start.X, points.start.Y, points.end.X, points.end.Y);
            var actual = Lines.GetLine(points.start, points.end, algo);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberDataTuple(nameof(AllTestCases))]
        public void FastIteratorsAreEquivalent(Lines.Algorithm algo, (Point start, Point end) points)
        {
            var fastResult = new List<Point>();
            switch (algo)
            {
                case Lines.Algorithm.Bresenham:
                    foreach (var point in Lines.GetBresenhamLine(points.start, points.end))
                        fastResult.Add(point);
                    break;
                case Lines.Algorithm.DDA:
                    foreach (var point in Lines.GetDDALine(points.start, points.end))
                        fastResult.Add(point);
                    break;
                case Lines.Algorithm.Orthogonal:
                    foreach (var point in Lines.GetOrthogonalLine(points.start, points.end))
                        fastResult.Add(point);
                    break;
            }

            var enumerableResult = Lines.GetLine(points.start, points.end, algo).ToList();

            Assert.Equal(fastResult, enumerableResult);
        }
    }
}
