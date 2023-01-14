using SFML.Graphics;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.SFML.UnitTests
{
    public class ColorTests
    {
        #region Test Data

        public static Color[] TestColors =
        {
            new Color(1, 2, 3, 8),
            new Color(1, 2, 3, 0),
            new Color(4, 2, 3, 8),
            new Color(1, 4, 3, 8),
            new Color(1, 2, 4, 8)
        };
        #endregion

        #region Conversion
        [Fact]
        public void ToSFMLColor()
        {
            var color = new Color(1, 3, 6, 8);
            global::SFML.Graphics.Color sfmlColor = color.ToSFMLColor();

            Assert.Equal(color.R, sfmlColor.R);
            Assert.Equal(color.G, sfmlColor.G);
            Assert.Equal(color.B, sfmlColor.B);
            Assert.Equal(color.A, sfmlColor.A);
        }

        [Fact]
        public void ToSadRogueColor()
        {
            var color = new global::SFML.Graphics.Color(1, 3, 6, 8);
            Color sadRogueColor = color.ToSadRogueColor();

            Assert.Equal(color.R, sadRogueColor.R);
            Assert.Equal(color.G, sadRogueColor.G);
            Assert.Equal(color.B, sadRogueColor.B);
            Assert.Equal(color.A, sadRogueColor.A);
        }
        #endregion

        #region Equality

        [Theory]
        [MemberDataEnumerable(nameof(TestColors))]
        public void Matches(Color color)
        {
            foreach (var color2 in TestColors)
            {
                var sfmlColor = color2.ToSFMLColor();
                Assert.Equal(color.Matches(color2), color.Matches(sfmlColor));
                Assert.Equal(color.Matches(color2), sfmlColor.Matches(color));
            }
        }
        #endregion
    }
}
