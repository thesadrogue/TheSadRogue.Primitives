using Microsoft.Xna.Framework;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.MonoGame.UnitTests
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
        public void ToMonoGameColor()
        {
            var color = new Color(1, 3, 6, 8);
            Microsoft.Xna.Framework.Color monoColor = color.ToMonoColor();

            Assert.Equal(color.R, monoColor.R);
            Assert.Equal(color.G, monoColor.G);
            Assert.Equal(color.B, monoColor.B);
            Assert.Equal(color.A, monoColor.A);
        }

        [Fact]
        public void ToSadRogueColor()
        {
            var color = new Microsoft.Xna.Framework.Color(1, 3, 6, 8);
            Color monoColor = color.ToSadRogueColor();

            Assert.Equal(color.R, monoColor.R);
            Assert.Equal(color.G, monoColor.G);
            Assert.Equal(color.B, monoColor.B);
            Assert.Equal(color.A, monoColor.A);
        }
        #endregion

        #region Equality

        [Theory]
        [MemberDataEnumerable(nameof(TestColors))]
        public void Matches(Color color)
        {
            foreach (var color2 in TestColors)
            {
                var monoColor = color2.ToMonoColor();
                Assert.Equal(color.Matches(color2), color.Matches(monoColor));
                Assert.Equal(color.Matches(color2), monoColor.Matches(color));
            }
        }
        #endregion
    }
}
