using System;
using Xunit;
using XUnit.ValueTuples;
using System.Linq;
using System.Collections.Generic;

namespace SadRogue.Primitives.UnitTests
{
    public class RectangleTests
    {
        #region Test Data

        public static Rectangle[] EqualRectangles = new Rectangle[]
        {
            new Rectangle(1, 2, 11, 17), new Rectangle(new Point(1, 2), new Point(11, 18)),
            new Rectangle(new Point(6, 10), 5, 8)
        };

        public static Rectangle[] DifferentRectangles = new Rectangle[]
        {
            new Rectangle(1, 2, 10, 16), new Rectangle(2, 3, 10, 16), new Rectangle(new Point(0, 0), 5, 6)
        };

        public static IEnumerable<(Rectangle, Rectangle)> PairwiseEqualRects =
            EqualRectangles.Combinate(EqualRectangles);

        public static Rectangle[] Dividends = new Rectangle[]
        {
            new Rectangle((0,0), (21,21)),
            new Rectangle((20,20), (45,45)),
            new Rectangle((0,15), (51, 85))
        };
        public static Rectangle[] Divisors = new Rectangle[]
        {
            new Rectangle((0,0), (3,3)),
            new Rectangle((0,0), (3, 11)),
            new Rectangle((-1,-1), (14, 6)),
        };
        public static IEnumerable<(Rectangle, Rectangle)> DivisionTestData =
            Dividends.Combinate(Divisors);

        #endregion

        #region Constructor Equivalence

        [Theory]
        [MemberDataTuple(nameof(PairwiseEqualRects))]
        public void TestConstructorEquivalence(Rectangle r1, Rectangle r2) => Assert.Equal(r1, r2);

        #endregion

        #region Equality/Inequality

        [Theory]
        [MemberDataEnumerable(nameof(DifferentRectangles))]
        public void TestEquality(Rectangle rad)
        {
            Rectangle compareTo = rad;
            Rectangle[] allRects = DifferentRectangles;
            Assert.True(rad == compareTo);

            Assert.Equal(1, allRects.Count(i => i == compareTo));
        }


        [Theory]
        [MemberDataEnumerable(nameof(DifferentRectangles))]
        public void TestInequality(Rectangle rect)
        {
            Rectangle compareTo = rect;
            Rectangle[] allRects = DifferentRectangles;
            Assert.False(rect != compareTo);

            Assert.Equal(allRects.Length - 1, allRects.Count(i => i != compareTo));
        }

        [Theory]
        [MemberDataEnumerable(nameof(DifferentRectangles))]
        public void TestEqualityInqeualityOpposite(Rectangle compareRect)
        {
            Rectangle[] rects = DifferentRectangles;

            foreach (Rectangle rect in rects)
                Assert.NotEqual(rect == compareRect, rect != compareRect);
        }

        #endregion

        #region Tuple Conversions

        [Theory]
        [MemberDataEnumerable(nameof(DifferentRectangles))]
        public void TestTupleConversions(Rectangle rect)
        {
            (int x, int y, int width, int height) t1 = rect;
            (Point minExtent, Point maxExtent) t2 = rect;

            Rectangle rect1 = t1;
            Rectangle rect2 = t2;
            Assert.Equal(rect, rect1);
            Assert.Equal(rect1, rect2);
        }

        #endregion

        #region Tuple Equality

        [Theory]
        [MemberDataEnumerable(nameof(DifferentRectangles))]
        public void TestTupleEquality(Rectangle rect)
        {
            (int x, int y, int width, int height) t1 = rect;
            (Point minExtent, Point maxExtent) t2 = rect;

            Assert.True(rect == t1);
            Assert.True(rect == t2);
            Assert.True(t1 == rect);
            Assert.True(t2 == rect);
            Assert.True(rect.Equals(t1));
            Assert.True(rect.Equals(t2));
        }

        #endregion

        #region Bisection
        [Fact]
        public void RecursiveBisectionTest()
        {
            Rectangle rectangle = new Rectangle(0, 0, 30, 30);
            List<Rectangle> rectangles = rectangle.BisectRecursive(5).ToList();
            Assert.Equal(16, rectangles.Count());
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    int count = rectangles.Count(r => r.Contains(new Point(i, j)));
                    Assert.Equal(1, count);
                }
            }
        }

        [Fact]
        public void BisectInHalfTest()
        {
            Rectangle rectangle = new Rectangle(0, 0, 5, 13);
            List<Rectangle> rectangles = rectangle.Bisect().ToEnumerable().ToList();
            foreach (Point c in rectangles[0].Positions())
            {
                Assert.True(rectangle.Contains(c));
                Assert.False(rectangles[1].Contains(c));
            }

            foreach (Point c in rectangles[1].Positions())
            {
                Assert.True(rectangle.Contains(c));
                Assert.False(rectangles[0].Contains(c));
            }

            Assert.True(rectangles[0].Height >= 3);
            Assert.True(rectangles[0].Height <= 11);
            Assert.True(rectangles[1].Height >= 3);
            Assert.True(rectangles[1].Height <= 11);
            Assert.Equal(5, rectangles[0].Width);
            Assert.Equal(5, rectangles[1].Width);

            rectangle = new Rectangle(0, 0, 13, 5);
            rectangles.AddRange(rectangle.Bisect().ToEnumerable());
            foreach (Point c in rectangles[2].Positions())
            {
                Assert.True(rectangle.Contains(c));
                Assert.False(rectangles[3].Contains(c));
            }

            foreach (Point c in rectangles[3].Positions())
            {
                Assert.True(rectangle.Contains(c));
                Assert.False(rectangles[2].Contains(c));
            }

            Assert.True(rectangles[2].Width >= 3);
            Assert.True(rectangles[2].Width <= 10);
            Assert.True(rectangles[3].Width >= 3);
            Assert.True(rectangles[3].Width <= 10);
            Assert.Equal(5, rectangles[2].Height);
            Assert.Equal(5, rectangles[3].Height);
        }

        [Fact]
        public void BisectHorizontallyTest()
        {
            Rectangle rectangle = new Rectangle(0, 0, 5, 13);
            List<Rectangle> rectangles = rectangle.BisectHorizontally().ToEnumerable().ToList();
            foreach (Point c in rectangles[0].Positions())
            {
                Assert.True(rectangle.Contains(c));
                Assert.False(rectangles[1].Contains(c));
            }

            Assert.True(rectangles[0].Height >= 3);
            Assert.True(rectangles[0].Height <= 10);
            Assert.True(rectangles[1].Height >= 3);
            Assert.True(rectangles[1].Height <= 10);
            Assert.Equal(5, rectangles[0].Width);
            Assert.Equal(5, rectangles[1].Width);

        }

        [Fact]
        public void BisectVerticallyTest()
        {
            Rectangle rectangle = new Rectangle(0, 0, 13, 5);
            List<Rectangle> rectangles = rectangle.BisectVertically().ToEnumerable().ToList();
            foreach (Point c in rectangles[0].Positions())
            {
                Assert.True(rectangle.Contains(c));
                Assert.False(rectangles[1].Contains(c));
            }

            Assert.True(rectangles[0].Width >= 3);
            Assert.True(rectangles[0].Width <= 10);
            Assert.True(rectangles[1].Width >= 3);
            Assert.True(rectangles[1].Width <= 10);
            Assert.Equal(5, rectangles[0].Height);
            Assert.Equal(5, rectangles[1].Height);
        }

        #endregion

        #region Division
        [Theory]
        [MemberDataTuple(nameof(DivisionTestData))]
        public void DivisionTest(Rectangle dividend, Rectangle divisor)
        {
            int expectedColumns = dividend.Width / divisor.Width;
            int expectedRows = dividend.Height / divisor.Height;
            int expectedRectangles = expectedRows * expectedColumns;

            var answer = dividend.Divide(divisor).ToList();

            Assert.Equal(expectedRectangles, answer.Count);

            List<int> xLocations = new List<int>();
            List<int> yLocations = new List<int>();
            foreach (Rectangle actual in answer)
            {
                Assert.Equal(divisor.Width, actual.Width);
                Assert.Equal(divisor.Height, actual.Height);
                xLocations.Add(actual.MinExtentX);
                xLocations.Add(actual.MaxExtentX);
                yLocations.Add(actual.MinExtentY);
                yLocations.Add(actual.MaxExtentY);
            }

            Assert.Empty(xLocations.Where(location => location < dividend.MinExtentX || location > dividend.MaxExtentX ));
            Assert.Empty(yLocations.Where(location => location < dividend.MinExtentY || location > dividend.MaxExtentY ));
        }

        [Fact]
        public void DivisionByZeroDivisorTest()
        {
            var rectangle = new Rectangle(1, 2, 50, 60);

            var divisor = new Rectangle(1, 2, 0, 10);
            Assert.Throws<ArgumentOutOfRangeException>(() => rectangle.Divide(divisor));

            divisor = new Rectangle(1, 2, 10, 0);
            Assert.Throws<ArgumentOutOfRangeException>(() => rectangle.Divide(divisor));
        }
        #endregion

        [Fact]
        public void PositionsEmptyRectTest()
        {
            var rect1 = new Rectangle(1, 2, 1, 0);
            var rect2 = new Rectangle(1, 2, 0, 1);
            var rect3 = new Rectangle(1, 2, 0, 0);

            var set1 = rect1.Positions().ToEnumerable().ToHashSet();
            Assert.Empty(set1);

            var set2 = rect2.Positions().ToEnumerable().ToHashSet();
            Assert.Empty(set2);

            var set3 = rect3.Positions().ToEnumerable().ToHashSet();
            Assert.Empty(set3);
        }
    }
}
