using System.Linq;
using Xunit;

namespace SadRogue.Primitives.UnitTests
{
    public class BisectionResultTests
    {
        [Fact]
        public void Constructor()
        {
            var rect1 = new Rectangle(0, 0, 3, 2);
            var rect2 = new Rectangle(3, 2, 5, 6);

            var result = new BisectionResult(rect1, rect2);
            Assert.Equal(rect1, result.Rect1);
            Assert.Equal(rect2, result.Rect2);
        }

        [Fact]
        public void Enumerable()
        {
            var rect1 = new Rectangle(0, 0, 3, 2);
            var rect2 = new Rectangle(3, 2, 5, 6);

            var result = new BisectionResult(rect1, rect2);

            var rects = result.ToArray();
            Assert.Equal(2, rects.Length);

            Assert.Equal(rect1, rects[0]);
            Assert.Equal(rect2, rects[1]);
        }

        [Fact]
        public void TupleConversion()
        {
            var rect1 = new Rectangle(0, 0, 3, 2);
            var rect2 = new Rectangle(3, 2, 5, 6);

            var result = new BisectionResult(rect1, rect2);
            (Rectangle r1, Rectangle r2) tuple1 = result;
            (Rectangle r1, Rectangle r2) tuple2 = result.ToTuple();

            Assert.Equal(rect1, tuple1.r1);
            Assert.Equal(rect2, tuple1.r2);
            Assert.Equal(tuple1, tuple2);

            var actual1 = BisectionResult.FromTuple(tuple1);
            BisectionResult actual2 = tuple1;

            Assert.Equal(rect1, actual1.Rect1);
            Assert.Equal(rect2, actual1.Rect2);

            Assert.Equal(rect1, actual2.Rect1);
            Assert.Equal(rect2, actual2.Rect2);
        }

        [Fact]
        public void Deconstruction()
        {
            var rect1 = new Rectangle(0, 0, 3, 2);
            var rect2 = new Rectangle(3, 2, 5, 6);

            var result = new BisectionResult(rect1, rect2);
            (Rectangle r1, Rectangle r2) = result;

            Assert.Equal(rect1, r1);
            Assert.Equal(rect2, r2);
        }

        #region Equality/Inequality

        [Fact]
        public void Equality()
        {
            var rect1 = new Rectangle(0, 0, 3, 2);
            var rect2 = new Rectangle(3, 2, 5, 6);

            var result1 = new BisectionResult(rect1, rect2);
            var result2 = new BisectionResult(rect1, rect2);

            Assert.True(result1.Equals(result2));
            Assert.True(result1.Matches(result2));
            Assert.True(result1 == result2);

            Assert.True(result2.Equals(result1));
            Assert.True(result2.Matches(result1));
            Assert.True(result2 == result1);

            Assert.Equal(result1.GetHashCode(), result2.GetHashCode());
        }

        [Fact]
        public void Inequality()
        {
            var rect1 = new Rectangle(0, 0, 3, 2);
            var rect2 = new Rectangle(3, 2, 5, 6);
            var rect3 = new Rectangle(3, 2, 5, 7);

            var results = new[]
            {
                new BisectionResult(rect1, rect2), new BisectionResult(rect1, rect3),
                new BisectionResult(rect3, rect2)
            };

            for (int i = 0; i < results.Length; i++)
            {
                for (int j = 0; j < results.Length; j++)
                {
                    if (j == i) continue;

                    var res1 = results[i];
                    var res2 = results[j];
                    Assert.True(res1 != res2);
                    Assert.True(res2 != res1);

                    Assert.False(res1 == res2);
                    Assert.False(res1.Matches(res2));
                    Assert.False(res1.Equals(res2));

                    Assert.False(res2 == res1);
                    Assert.False(res2.Matches(res1));
                    Assert.False(res2.Equals(res1));
                }
            }
        }
        #endregion
    }
}
