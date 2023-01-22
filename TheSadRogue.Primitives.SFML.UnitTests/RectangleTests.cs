using SFML.Graphics;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.SFML.UnitTests
{
    public class RectangleTests
    {
        #region Test Data

        public static Rectangle[] TestRectangles =
        {
            new Rectangle(1, 2, 3, 8),
            new Rectangle(1, 2, 3, 4),
            new Rectangle(4, 2, 3, 8),
            new Rectangle(1, 4, 3, 8),
            new Rectangle(1, 2, 4, 8)
        };
        #endregion

        #region Conversion
        [Fact]
        public void ToSFMLRect()
        {
            var rect = new Rectangle(1, 3, 6, 8);
            IntRect sfmlRect = rect.ToIntRect();

            Assert.Equal(rect.Position.X, sfmlRect.Left);
            Assert.Equal(rect.Position.Y, sfmlRect.Top);
            Assert.Equal(rect.Width, sfmlRect.Width);
            Assert.Equal(rect.Height, sfmlRect.Height);
        }

        [Fact]
        public void ToSadRogueRect()
        {
            var rect = new IntRect(1, 3, 6, 8);
            Rectangle sadRogueRect = rect.ToRectangle();

            Assert.Equal(rect.Left, sadRogueRect.Position.X);
            Assert.Equal(rect.Top, sadRogueRect.Position.Y);
            Assert.Equal(rect.Width, sadRogueRect.Width);
            Assert.Equal(rect.Height, sadRogueRect.Height);
        }
        #endregion

        #region Equality

        [Theory]
        [MemberDataEnumerable(nameof(TestRectangles))]
        public void Matches(Rectangle rect)
        {
            foreach (var rect2 in TestRectangles)
            {
                var sfmlRect = rect2.ToIntRect();
                Assert.Equal(rect.Matches(rect2), rect.Matches(sfmlRect));
                Assert.Equal(rect.Matches(rect2), sfmlRect.Matches(rect));
            }
        }
        #endregion
    }
}
