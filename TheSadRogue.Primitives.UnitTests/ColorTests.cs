using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class ColorTests
    {
        private readonly ITestOutputHelper _output;

        #region Test Data

        private static readonly Color s_equalColor = new Color(1, 2, 3, 4);
        public static readonly Color[] NotEqualColors =
        {
            new Color(s_equalColor.R, s_equalColor.G, s_equalColor.B, s_equalColor.A),
            new Color(5, 2, 3, 4),
            new Color(1, 5, 3, 4),
            new Color(1, 2, 5, 4),
            new Color(1, 2, 3, 5)
        };


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

        // Color translation table: https://en.wikipedia.org/wiki/HSL_and_HSV#Examples
        public static readonly (Color color, (float h, float s, float v) expected)[] HSVTestCases =
        {
            (new Color(1f, 1f, 1f), (0.0f, 0f, 1f)),
            (new Color(0.5f, 0.5f, 0.5f), (0.0f, 0f, 0.5f)),
            (new Color(0f, 0f, 0f), (0.0f, 0f, 0f)),
            (new Color(1f, 0f, 0f), (0.0f, 1f, 1f)),
            (new Color(0.75f, 0.75f, 0f), (60.0f, 1f, 0.75f)),
            (new Color(0f, 0.5f, 0f), (120.0f, 1f, 0.5f)),
            (new Color(0.5f, 1f, 1f), (180.0f, 0.5f, 1f)),
            (new Color(0.5f, 0.5f, 1f), (240.0f, 0.5f, 1f)),
            (new Color(0.75f, 0.25f, 0.75f), (300.0f, 0.667f, 0.75f)),
            (new Color(0.628f, 0.643f, 0.142f), (61.8f, 0.779f, 0.643f)),
            (new Color(0.255f, 0.104f, 0.918f), (251.1f, 0.887f, 0.918f)),
            (new Color(0.116f, 0.675f, 0.255f), (134.9f, 0.828f, 0.675f)),
            (new Color(0.941f, 0.785f, 0.053f), (49.5f, 0.944f, 0.941f)),
            (new Color(0.704f, 0.187f, 0.897f), (283.7f, 0.792f, 0.897f)),
            (new Color(0.931f, 0.463f, 0.316f), (14.3f, 0.661f, 0.931f)),
            (new Color(0.998f, 0.974f, 0.532f), (56.9f, 0.467f, 0.998f)),
            (new Color(0.099f, 0.795f, 0.591f), (162.4f, 0.875f, 0.795f)),
            (new Color(0.211f, 0.149f, 0.597f), (248.3f, 0.75f, 0.597f)),
            (new Color(0.495f, 0.493f, 0.721f), (240.5f, 0.316f, 0.721f))
        };

        // Color translation table: https://en.wikipedia.org/wiki/HSL_and_HSV#Examples
        public static readonly (Color color, (float h, float s, float l) expected)[] HSLTestCases =
        {
            (new Color(1f, 1f, 1f), (0.0f, 0f, 1f)),
            (new Color(0.5f, 0.5f, 0.5f), (0.0f, 0f, 0.5f)),
            (new Color(0f, 0f, 0f), (0.0f, 0f, 0f)),
            (new Color(1f, 0f, 0f), (0.0f, 1f, 0.5f)),
            (new Color(0.75f, 0.75f, 0f), (60.0f, 1f, 0.375f)),
            (new Color(0f, 0.5f, 0f), (120.0f, 1f, 0.25f)),
            (new Color(0.5f, 1f, 1f), (180.0f, 1f, 0.75f)),
            (new Color(0.5f, 0.5f, 1f), (240.0f, 1f, 0.75f)),
            (new Color(0.75f, 0.25f, 0.75f), (300.0f, 0.5f, 0.5f)),
            (new Color(0.628f, 0.643f, 0.142f), (61.8f, 0.638f, 0.393f)),
            (new Color(0.255f, 0.104f, 0.918f), (251.1f, 0.832f, 0.511f)),
            (new Color(0.116f, 0.675f, 0.255f), (134.9f, 0.707f, 0.396f)),
            (new Color(0.941f, 0.785f, 0.053f), (49.5f, 0.893f, 0.497f)),
            (new Color(0.704f, 0.187f, 0.897f), (283.7f, 0.775f, 0.542f)),
            (new Color(0.931f, 0.463f, 0.316f), (14.3f, 0.817f, 0.624f)),
            (new Color(0.998f, 0.974f, 0.532f), (56.9f, 0.991f, 0.765f)),
            (new Color(0.099f, 0.795f, 0.591f), (162.4f, 0.779f, 0.447f)),
            (new Color(0.211f, 0.149f, 0.597f), (248.3f, 0.601f, 0.373f)),
            (new Color(0.495f, 0.493f, 0.721f), (240.5f, 0.29f, 0.607f))
        };

        #endregion
        public ColorTests(ITestOutputHelper output)
        {
            _output = output;
        }
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

        [Theory]
        [MemberDataTuple(nameof(HSLTestCases))]
        public void FromHSL(Color expected, (float h, float s, float l) input)
        {
            var actual = Color.FromHSL(input.h, input.s, input.l);

            _output.WriteLine($"Expected: {expected}");
            _output.WriteLine($"Actual  : {actual}");
            Assert.InRange(expected.R - actual.R, -1, 1);
            Assert.InRange(expected.G - actual.G, -1, 1);
            Assert.InRange(expected.B - actual.B, -1, 1);
            Assert.Equal(expected.A, actual.A);
        }

        [Fact]
        public void FromHSLInvalidH()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Color.FromHSL(360.001f, 1f, 1f));
            Assert.Throws<ArgumentOutOfRangeException>(() => Color.FromHSL(-0.0001f, 1f, 1f));
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

        #region HSV/HSL

        [Theory]
        [MemberDataTuple(nameof(HSLTestCases))]
        public void TestHSLValues(Color color, (float h, float s, float l) expected)
        {
            float h = color.GetHSLHue();
            float s = color.GetHSLSaturation();
            float l = color.GetHSLLightness();

            Assert.InRange(expected.h - h, -1f, 1f);
            Assert.InRange(expected.s - s, -.01f, .01f);
            Assert.InRange(expected.l - l, -.01f, .01f);
        }

        [Theory]
        [MemberDataTuple(nameof(HSLTestCases))]
        public void TestDeprecatedIsHSL(Color color, (float h, float s, float l) expected)
        {
            // We're intentionally testing deprecated methods to ensure backwards compatibility
#pragma warning disable CS0618
            float h = color.GetHue();
            float s = color.GetSaturation();
            float l = color.GetBrightness();
#pragma warning restore CS0618

            Assert.InRange(expected.h - h, -1f, 1f);
            Assert.InRange(expected.s - s, -.01f, .01f);
            Assert.InRange(expected.l - l, -.01f, .01f);
        }

        [Theory]
        [MemberDataTuple(nameof(HSVTestCases))]
        public void TestHSVValues(Color color, (float h, float s, float v) expected)
        {
            float h = color.GetHSVHue();
            float s = color.GetHSVSaturation();
            float v = color.GetHSVBrightness();

            Assert.InRange(expected.h - h, -1f, 1f);
            Assert.InRange(expected.s - s, -.01f, .01f);
            Assert.InRange(expected.v - v, -.01f, .01f);
        }


        #endregion

        #region Equality/Inequality

        [Fact]
        public void TestKnownEquality()
        {
            Assert.True(s_equalColor.Matches(NotEqualColors[0]));
            Assert.True(s_equalColor.Equals(NotEqualColors[0]));
            Assert.True(s_equalColor.Equals((object)NotEqualColors[0]));
            Assert.True(s_equalColor == NotEqualColors[0]);
        }
        [Theory]
        [MemberDataEnumerable(nameof(NotEqualColors))]
        public void TestEqualityInequalityRelationship(Color testColor)
        {
            Assert.Single(NotEqualColors.Where(i => i.Equals(testColor)));
            Assert.Single(NotEqualColors.Where(i => i.Equals((object)testColor)));
            Assert.Single(NotEqualColors.Where(i => i.Matches(testColor)));
            Assert.Single(NotEqualColors.Where(i => i == testColor));
        }

        [Theory]
        [MemberDataEnumerable(nameof(NotEqualColors))]
        public void TestGetHashCode(Color testColor)
        {
            foreach (var other in NotEqualColors)
                if (other.Equals(testColor))
                    Assert.Equal(testColor.GetHashCode(), other.GetHashCode());
        }
        #endregion
    }
}
