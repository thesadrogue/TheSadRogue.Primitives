using Microsoft.Xna.Framework;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.MonoGame.UnitTests
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
        public void ToMonoGameRect()
        {
            var rect = new Rectangle(1, 3, 6, 8);
            Microsoft.Xna.Framework.Rectangle monoRect = rect.ToMonoRectangle();

            Assert.Equal(rect.Position.X, monoRect.X);
            Assert.Equal(rect.Position.Y, monoRect.Y);
            Assert.Equal(rect.Width, monoRect.Width);
            Assert.Equal(rect.Height, monoRect.Height);
        }

        [Fact]
        public void ToSadRogueRect()
        {
            var rect = new Microsoft.Xna.Framework.Rectangle(1, 3, 6, 8);
            Rectangle sadRogueRect = rect.ToRectangle();

            Assert.Equal(rect.X, sadRogueRect.Position.X);
            Assert.Equal(rect.Y, sadRogueRect.Position.Y);
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
                var monoColor = rect2.ToMonoRectangle();
                Assert.Equal(rect.Matches(rect2), rect.Matches(monoColor));
                Assert.Equal(rect.Matches(rect2), monoColor.Matches(rect));
            }
        }
        #endregion
    }
}
