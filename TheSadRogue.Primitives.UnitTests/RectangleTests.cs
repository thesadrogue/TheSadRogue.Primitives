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
    }
}
