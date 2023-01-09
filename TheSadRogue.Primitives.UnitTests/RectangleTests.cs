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

        public static Rectangle[] MiscTestRectangles = new Rectangle[] { (0, 0, 10, 20), (1, 2, 15, 17), (5, 2, 18, 6) };

        public static Rectangle[] EmptyRectangles =
            new Rectangle[] { (10, 11, 0, 20), (15, 14, 21, 0), (17, 14, 0, 0) };

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
            Assert.Throws<ArgumentOutOfRangeException>(() => rectangle.Divide(divisor).ToArray());

            divisor = new Rectangle(1, 2, 10, 0);
            Assert.Throws<ArgumentOutOfRangeException>(() => rectangle.Divide(divisor).ToArray());
        }
        #endregion

        #region CalculatedProperties

        [Theory]
        [MemberDataEnumerable(nameof(MiscTestRectangles))]
        public void AreaTest(Rectangle rect)
        {
            Assert.Equal(rect.Width * rect.Height, rect.Area);
        }

        [Theory]
        [MemberDataEnumerable(nameof(MiscTestRectangles))]
        public void SizeTest(Rectangle rect)
        {
            Assert.Equal(new Point(rect.Width, rect.Height), rect.Size);
        }

        [Theory]
        [MemberDataEnumerable(nameof(MiscTestRectangles))]
        public void NonEmptyRectIsEmptyTest(Rectangle rect)
        {
            Assert.False(rect.IsEmpty);
        }

        [Theory]
        [MemberDataEnumerable(nameof(EmptyRectangles))]
        public void EmptyRectIsEmptyTest(Rectangle rect)
        {
            Assert.True(rect.IsEmpty);
        }

        #endregion

        #region WithConstruction

        [Fact]
        public void WithExtents()
        {
            Point minExtent = (1, 2);
            Point maxExtent = (3, 4);

            var rect = Rectangle.WithExtents(minExtent, maxExtent);
            CheckRect(rect, minExtent, maxExtent.X - minExtent.X + 1, maxExtent.Y - minExtent.Y + 1);
        }

        [Fact]
        public void WithRadius()
        {
            Point center = (5, 10);
            const int hRad = 6;
            const int vRad = 7;

            var rect = Rectangle.WithRadius(center, hRad, vRad);
            CheckRect(rect, center - (hRad, vRad), hRad * 2 + 1, vRad * 2 + 1);
        }

        [Fact]
        public void WithPositionAndSize()
        {
            Point position = (1, 2);
            Point size = (3, 4);

            var rect = Rectangle.WithPositionAndSize(position, size.X, size.Y);
            var rect2 = Rectangle.WithPositionAndSize(position, size);

            CheckRect(rect, position, size.X, size.Y);
            Assert.Equal(rect, rect2);
        }

        #endregion

        #region WithEditingFunctions

        [Theory]
        [MemberDataEnumerable(nameof(MiscTestRectangles))]
        public void WithPosition(Rectangle rect)
        {
            Point newPos = (1, 2);
            var newRect = rect.WithPosition(newPos);
            var newRect2 = rect.WithX(newPos.X).WithY(newPos.Y);


            CheckRect(newRect, newPos, rect.Width, rect.Height);
            Assert.Equal(newRect, newRect2);
        }

        [Theory]
        [MemberDataEnumerable(nameof(MiscTestRectangles))]
        public void WithSize(Rectangle rect)
        {
            Point size = (15, 26);
            var newRect = rect.WithSize(size);
            var newRect2 = rect.WithWidth(size.X).WithHeight(size.Y);

            CheckRect(newRect, rect.Position, size.X, size.Y);
            Assert.Equal(newRect, newRect2);
        }

        [Theory]
        [MemberDataEnumerable(nameof(MiscTestRectangles))]
        public void WithMinExtent(Rectangle rect)
        {
            Point newExtent = (1, 4);
            var newRect = rect.WithMinExtent(newExtent);
            var newRect2 = rect.WithMinExtentX(newExtent.X).WithMinExtentY(newExtent.Y);

            CheckRect(newRect, newExtent, rect.MaxExtentX - newExtent.X + 1, rect.MaxExtentY - newExtent.Y + 1);
            Assert.Equal(newRect, newRect2);
        }

        [Theory]
        [MemberDataEnumerable(nameof(MiscTestRectangles))]
        public void WithMaxExtent(Rectangle rect)
        {
            Point newExtent = (1, 4);
            var newRect = rect.WithMaxExtent(newExtent);
            var newRect2 = rect.WithMaxExtentX(newExtent.X).WithMaxExtentY(newExtent.Y);

            CheckRect(newRect, rect.Position, newExtent.X - rect.Position.X + 1, newExtent.Y - rect.Position.Y + 1);
            Assert.Equal(newRect, newRect2);
        }
        #endregion

        #region ChangeEditingFunctions
        [Theory]
        [MemberDataEnumerable(nameof(MiscTestRectangles))]
        public void ChangePosition(Rectangle rect)
        {
            Point deltaPos = (1, 2);
            var newRect = rect.ChangePosition(deltaPos);
            var newRect2 = rect.ChangeX(deltaPos.X).ChangeY(deltaPos.Y);
            var newRect3 = rect.Translate(deltaPos);
            var newRect4 = rect.TranslateX(deltaPos.X).TranslateY(deltaPos.Y);
            var newRect5 = rect.ChangePosition(Direction.DownRight);

            CheckRect(newRect, rect.Position + deltaPos, rect.Width, rect.Height);
            Assert.Equal(newRect, newRect2);
            Assert.Equal(newRect, newRect3);
            Assert.Equal(newRect, newRect4);

            CheckRect(newRect5, rect.Position + Direction.DownRight, rect.Width, rect.Height);
        }

        [Theory]
        [MemberDataEnumerable(nameof(MiscTestRectangles))]
        public void ChangeSize(Rectangle rect)
        {
            Point deltaSize = (15, 26);
            var newRect = rect.ChangeSize(deltaSize);
            var newRect2 = rect.ChangeWidth(deltaSize.X).ChangeHeight(deltaSize.Y);

            CheckRect(newRect, rect.Position, rect.Width + deltaSize.X, rect.Height + deltaSize.Y);
            Assert.Equal(newRect, newRect2);
        }

        // [Theory]
        // [MemberDataEnumerable(nameof(MiscTestRectangles))]
        // public void ChangeMinExtent(Rectangle rect)
        // {
        //     Point deltaExtent = (1, 4);
        //     var newRect = rect.ChangeMinExtent(deltaExtent);
        //     var newRect2 = rect.ChangeMinExtentX(deltaExtent.X).ChangeMinExtentY(deltaExtent.Y);
        //
        //     CheckRect(newRect, rect.Position - deltaExtent, deltaExtent.X, deltaExtent.Y);
        //     Assert.Equal(newRect, newRect2);
        // }

        // [Theory]
        // [MemberDataEnumerable(nameof(MiscTestRectangles))]
        // public void ChangeMaxExtent(Rectangle rect)
        // {
        //     Point deltaExtent = (1, 4);
        //     var newRect = rect.ChangeMaxExtent(deltaExtent);
        //     var newRect2 = rect.ChangeMaxExtentX(deltaExtent.X).ChangeMaxExtentY(deltaExtent.Y);
        //
        //     CheckRect(newRect, deltaExtent, deltaExtent.X, deltaExtent.Y);
        //     Assert.Equal(newRect, newRect2);
        // }
        #endregion

        #region Set Operations

        [Fact]
        public void TestDifference()
        {
            var orig = new Rectangle(1, 2, 40, 42);
            var sub = new Rectangle(10, 11, 5, 6);

            var diff = Rectangle.GetDifference(orig, sub);
            foreach (var pos in orig.Positions())
            {
                bool inSet = !sub.Contains(pos);
                Assert.Equal(inSet, diff.Contains(pos));
            }
        }

        [Fact]
        public void TestUnions()
        {
            var orig = new Rectangle(1, 2, 10, 15);
            var unionOperand = new Rectangle(orig.MaxExtent - (3, 4), (13, 17));

            var union = Rectangle.GetUnion(orig, unionOperand);
            var exactUnion = Rectangle.GetExactUnion(orig, unionOperand);

            var expectedUnion = new Rectangle(orig.MinExtent, unionOperand.MaxExtent);
            Assert.Equal(expectedUnion, union);
            Assert.Equal(expectedUnion, exactUnion.Bounds);
            foreach (var pos in expectedUnion.Positions())
            {
                bool inSet = orig.Contains(pos) || unionOperand.Contains(pos);
                Assert.Equal(inSet, exactUnion.Contains(pos));
            }
        }

        [Fact]
        public void TestIntersection()
        {
            var orig = new Rectangle(1, 2, 10, 15);
            var intersectOperand = new Rectangle(orig.MaxExtent - (3, 4), (13, 17));

            var intersection = Rectangle.GetIntersection(orig, intersectOperand);
            Assert.True(orig.Contains(intersection));
            Assert.True(intersectOperand.Contains(intersection));
            foreach (var pos in new Rectangle(orig.MinExtent, intersectOperand.MaxExtent).Positions())
            {
                bool inSet = orig.Contains(pos) && intersectOperand.Contains(pos);
                Assert.Equal(inSet, intersection.Contains(pos));
            }
        }

        [Fact]
        public void GetIntersectionNoIntersect()
        {
            var orig = new Rectangle(1, 2, 3, 4);
            var intersect = new Rectangle(orig.MaxExtent + 1, (5, 6));

            Assert.Equal(Rectangle.Empty, Rectangle.GetIntersection(orig, intersect));
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

        private static void CheckRect(Rectangle rect, Point position, int width, int height)
        {
            Assert.Equal(position, rect.Position);
            Assert.Equal(position, rect.MinExtent);
            Assert.Equal(position + (width - 1, height - 1), rect.MaxExtent);
            Assert.Equal(width, rect.Width);
            Assert.Equal(height, rect.Height);
        }
    }
}
