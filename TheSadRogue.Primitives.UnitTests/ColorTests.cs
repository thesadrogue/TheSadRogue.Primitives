using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class ColorTests
    {
        #region Test Data

        public static readonly (Color color, (byte r, byte g, byte b, byte a) expected)[] CtorTestCases =
        {
            (new Color(new Color(64, 128, 192), 32), (64, 128, 192, 32)),
            (new Color(new Color(64, 128, 192), 256), (64, 128, 192, 255)),
            (new Color(new Color(64, 128, 192), 0.125f), (64, 128, 192, 32)),
            (new Color(new Color(64, 128, 192), 1.1f), (64, 128, 192, 255)),
            (new Color((byte)64, (byte)128, (byte)192, (byte)32), (64, 128, 192, 32)), (new Color(), (0, 0, 0, 0)),
            (new Color(64, 128, 192), (64, 128, 192, 255)), (new Color(256, 256, -1), (255, 255, 0, 255)),
            (new Color(64, 128, 192, 32), (64, 128, 192, 32)), (new Color(256, 256, -1, 256), (255, 255, 0, 255)),
            (new Color(0.25f, 0.5f, 0.75f), (64, 128, 192, 255)),
            (new Color(1.1f, 1.1f, -0.1f), (255, 255, 0, 255)),
            (new Color(0.25f, 0.5f, 0.75f, 0.125f), (64, 128, 192, 32)),
            (new Color(1.1f, 1.1f, -0.1f, -0.1f), (255, 255, 0, 0)),
        };


        // Color translation table: https://www.rapidtables.com/convert/color/rgb-to-hsv.html
        public static readonly (Color color, (float h, float s, float v) expected)[] HSVTestCases =
        {
            (new Color(0, 0, 0), (0, 0, 0)),
            (new Color(255, 255, 255), (0, 0, 1)),
            (new Color(255, 0, 0), (0, 1, 1)),
            (new Color(0, 255, 0), (120, 1, 1)),
            (new Color(0, 0, 255), (240, 1, 1)),
            (new Color(255, 255, 0), (60, 1, 1)),
            (new Color(0, 255, 255), (180, 1, 1)),
            (new Color(255, 0, 255), (300, 1, 1)),
            (new Color(191, 191, 191), (0, 0, .75f)),
            (new Color(128, 128, 128), (0, 0, .5f)),
            (new Color(128, 0, 0), (0, 1, .5f)),
            (new Color(128, 128, 0), (60, 1, .5f)),
            (new Color(0, 128, 0), (120, 1, .5f)),
            (new Color(128, 0, 128), (300, 1, .5f)),
            (new Color(0, 128, 128), (180, 1, .5f)),
            (new Color(0, 0, 128), (240, 1, .5f)),
        };

        #endregion

        #region Construction
        [Theory]
        [MemberDataTuple(nameof(CtorTestCases))]
        public void Ctor_Explicit(Color color, (byte r, byte g, byte b, byte a) expected)
        {
            // Account for rounding differences with float constructors
            Assert.InRange(expected.r - color.R, -1, 1);
            Assert.InRange(expected.g - color.G, -1, 1);
            Assert.InRange(expected.b - color.B, -1, 1);
            Assert.InRange(expected.a - color.A, -1, 1);
        }

        [Fact]
        public void Ctor_Packed()
        {
            var color = new Color(0x20C08040);

            Assert.Equal(64, color.R);
            Assert.Equal(128, color.G);
            Assert.Equal(192, color.B);
            Assert.Equal(32, color.A);
        }

        [Fact]
        public void FromNonPremultiplied()
        {
            var color = Color.FromNonPremultiplied(255, 128, 64, 128);
            Assert.InRange(128 - color.R, -1, 1);
            Assert.InRange(64 - color.G, -1, 1);
            Assert.InRange(32 - color.B, -1, 1);
            Assert.Equal(128, color.A);

            var overflow = Color.FromNonPremultiplied(280, 128, -10, 128);
            Assert.InRange(140 - overflow.R, -1, 1);
            Assert.InRange(64 - overflow.G, -1, 1);
            Assert.InRange(0 - overflow.B, -1, 1);
            Assert.Equal(128, overflow.A);

            var overflow2 = Color.FromNonPremultiplied(255, 128, 64, 280);
            Assert.InRange(255 - overflow2.R, -1, 1);
            Assert.InRange(140 - overflow2.G, -1, 1);
            Assert.InRange(70 - overflow2.B, -1, 1);
            Assert.Equal(255, overflow2.A);
        }
        #endregion

        #region Multiply/Lerp
        [Fact]
        public void Multiply()
        {
            var color = new Color(1, 2, 3, 4);

            // Test 1.0 scale.
            Assert.Equal(color, color * 1.0f);
            Assert.Equal(color, Color.Multiply(color, 1.0f));
            Assert.Equal(color * 1.0f, Color.Multiply(color, 1.0f));

            // Test 0.999 scale.
            var almostOne = new Color(0, 1, 2, 3);
            Assert.Equal(almostOne, color * 0.999f);
            Assert.Equal(almostOne, Color.Multiply(color, 0.999f));
            Assert.Equal(color * 0.999f, Color.Multiply(color, 0.999f));

            // Test 1.001 scale.
            Assert.Equal(color, color * 1.001f);
            Assert.Equal(color, Color.Multiply(color, 1.001f));
            Assert.Equal(color * 1.001f, Color.Multiply(color, 1.001f));

            // Test 0.0 scale.
            Assert.Equal(Color.Transparent, color * 0.0f);
            Assert.Equal(Color.Transparent, Color.Multiply(color, 0.0f));
            Assert.Equal(color * 0.0f, Color.Multiply(color, 0.0f));

            // Test 0.001 scale.
            Assert.Equal(Color.Transparent, color * 0.001f);
            Assert.Equal(Color.Transparent, Color.Multiply(color, 0.001f));
            Assert.Equal(color * 0.001f, Color.Multiply(color, 0.001f));

            // Test -0.001 scale.
            Assert.Equal(Color.Transparent, color * -0.001f);
            Assert.Equal(Color.Transparent, Color.Multiply(color, -0.001f));
            Assert.Equal(color * -0.001f, Color.Multiply(color, -0.001f));

            // Test for overflow.
            Assert.Equal(Color.White, color * 300.0f);
            Assert.Equal(Color.White, Color.Multiply(color, 300.0f));
            Assert.Equal(color * 300.0f, Color.Multiply(color, 300.0f));

            // Test for underflow.
            Assert.Equal(Color.Transparent, color * -1.0f);
            Assert.Equal(Color.Transparent, Color.Multiply(color, -1.0f));
            Assert.Equal(color * -1.0f, Color.Multiply(color, -1.0f));
        }

        [Fact]
        public void Lerp()
        {
            Color color1 = new Color(20, 40, 0, 0);
            Color color2 = new Color(41, 81, 255, 255);

            // Test zero and underflow.
            Assert.Equal(color1, Color.Lerp(color1, color2, 0.0f));
            Assert.Equal(color1, Color.Lerp(color1, color2, 0.001f));
            Assert.Equal(color1, Color.Lerp(color1, color2, -0.001f));
            Assert.Equal(color1, Color.Lerp(color1, color2, -1.0f));

            // Test one scale and overflows.
            Assert.Equal(color2, Color.Lerp(color1, color2, 1.0f));
            Assert.Equal(color2, Color.Lerp(color1, color2, 1.001f));
            Assert.Equal(new Color(254, 254, 254, 254),
                Color.Lerp(Color.Transparent, Color.White, 0.999f));
            Assert.Equal(color2, Color.Lerp(color1, color2, 2.0f));

            // Test half scale.
            var half = new Color(30, 60, 127, 127);
            Assert.Equal(half, Color.Lerp(color1, color2, 0.5f));
            Assert.Equal(half, Color.Lerp(color1, color2, 0.501f));
            Assert.Equal(half, Color.Lerp(color1, color2, 0.499f));

            // Test backwards lerp.
            Assert.Equal(color2, Color.Lerp(color2, color1, 0.0f));
            Assert.Equal(color1, Color.Lerp(color2, color1, 1.0f));
            Assert.Equal(half, Color.Lerp(color2, color1, 0.5f));
        }
        #endregion

        #region Deconstruction
        [Fact]
        public void DeconstructBytes()
        {
            Color color = new Color(255, 255, 255);

            color.Deconstruct(out byte r, out byte g, out byte b);

            Assert.Equal(color.R, r);
            Assert.Equal(color.G, g);
            Assert.Equal(color.B, b);

            Color color2 = new Color(255, 255, 255, 255);

            color2.Deconstruct(out byte r2, out byte g2, out byte b2, out byte a2);

            Assert.Equal(color2.R, r2);
            Assert.Equal(color2.G, g2);
            Assert.Equal(color2.B, b2);
            Assert.Equal(color2.A, a2);
        }

        [Fact]
        public void DeconstructInts()
        {
            Color color = new Color(255, 255, 255);

            color.Deconstruct(out int r, out int g, out int b);

            Assert.Equal(color.R, r);
            Assert.Equal(color.G, g);
            Assert.Equal(color.B, b);

            Color color2 = new Color(255, 255, 255, 255);

            color2.Deconstruct(out int r2, out int g2, out int b2, out int a2);

            Assert.Equal(color2.R, r2);
            Assert.Equal(color2.G, g2);
            Assert.Equal(color2.B, b2);
            Assert.Equal(color2.A, a2);
        }

        [Fact]
        public void DeconstructFloats()
        {
            Color color = new Color(255, 255, 255);

            color.Deconstruct(out float r, out float g, out float b);

            Assert.Equal(color.R / 255f, r);
            Assert.Equal(color.G / 255f, g);
            Assert.Equal(color.B / 255f, b);

            Color color2 = new Color(255, 255, 255, 255);

            color2.Deconstruct(out float r2, out float g2, out float b2, out float a2);

            Assert.Equal(color2.R / 255f, r2);
            Assert.Equal(color2.G / 255f, g2);
            Assert.Equal(color2.B / 255f, b2);
            Assert.Equal(color2.A / 255f, a2);
        }
        #endregion

        #region HSV

        [Theory]
        [MemberDataTuple(nameof(HSVTestCases))]
        public void TestHSVValues(Color color, (float h, float s, float v) expected)
        {
            float h = color.GetHue();
            float s = color.GetSaturation();
            float v = color.GetBrightness();

            Assert.InRange(expected.h - h, -.002f, .002f);
            Assert.InRange(expected.s - s, -.002f, .002f);
            Assert.InRange(expected.v - v, -.002f, .002f);
        }


        #endregion
    }
}
