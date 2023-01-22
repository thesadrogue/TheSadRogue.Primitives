using System;
using System.Linq;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class MathHelpersTests
    {
        #region Test Data
        private const int ClampMin = -5;
        private const int ClampMax = 10;

        public static (float min, float max)[] MinMaxPairs =
        {
            (-3.56f, 4.23f),
            (1.2f, 5.10f),
            (-5.43f, -2.34f)
        };

        #endregion


        #region Degrees Radian Conversion

        [Fact]
        public void DegreesToRadians()
        {
            double degrees = 0.0;
            while (degrees <= 360.0)
            {
                Assert.Equal(ConvertDegreesToRadians(degrees), MathHelpers.ToRadian(degrees));
                degrees += 0.5;
            }
        }

        [Fact]
        public void RadiansToDegrees()
        {
            double radians = 0.0;
            while (radians <= 2 * Math.PI)
            {
                Assert.Equal(ConvertRadiansToDegrees(radians), MathHelpers.ToDegree(radians));
                radians += 0.1;
            }
        }

        #endregion

        #region Clamp

        [Fact]
        public void ClampInt()
        {
            for (int i = -50; i <= 100; i++)
                Assert.Equal(Math.Clamp(i, ClampMin, ClampMax), MathHelpers.Clamp(i, ClampMin, ClampMax));
        }

        [Fact]
        public void ClampFloat()
        {
            for (float i = -50.0f; i <= 100.0f; i += 0.5f)
                Assert.Equal(Math.Clamp(i, ClampMin, ClampMax), MathHelpers.Clamp(i, ClampMin, ClampMax));
        }

        [Fact]
        public void ClampDouble()
        {
            for (double i = -50.0; i <= 100.0; i += 0.5)
                Assert.Equal(Math.Clamp(i, ClampMin, ClampMax), MathHelpers.Clamp(i, ClampMin, ClampMax));
        }
        #endregion

        #region Wrap
        [Theory]
        [MemberDataTuple(nameof(MinMaxPairs))]
        public void FloatWrapOnWithinBounds(float min, float max)
        {
            for (float cur = min; cur <= max; cur += 0.01f)
                Assert.InRange(cur - MathHelpers.Wrap(cur, min, max), -0.001f, 0.001f);
        }

        [Theory]
        [MemberDataTuple(nameof(MinMaxPairs))]
        public void FloatWrapOutOfBounds(float min, float max)
        {
            float range = max - min;
            for (int i = 0; i < 3; i++)
            {
                for (float delta = 0.01f; delta <= range; delta += 0.01f)
                {
                    float cur = min - delta;
                    float expectedWrapped = max - delta;
                    float actualWrapped = MathHelpers.Wrap(cur, min, max);
                    Assert.InRange(expectedWrapped - actualWrapped, -0.001f, 0.001f);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (float delta = 0.01f; delta <= range; delta += 0.01f)
                {
                    float cur = max + delta;
                    float expectedWrapped = min + delta;
                    float actualWrapped = MathHelpers.Wrap(cur, min, max);
                    Assert.InRange(expectedWrapped - actualWrapped, -0.001f, 0.001f);
                }
            }
        }
        #endregion

        #region Lerp

        [Fact]
        public void LerpFloatTest()
        {
            Assert.Equal(2.5f, MathHelpers.Lerp(0f, 5f, .5f));
            Assert.Equal(0f, MathHelpers.Lerp(-5f, 5f, 0.5f));
            Assert.Equal(0f, MathHelpers.Lerp(0f, 0f, 0.5f));
            Assert.Equal(-3f, MathHelpers.Lerp(-5f, -1f, 0.5f));
        }

        [Fact]
        public void LerpDoubleTest()
        {
            Assert.Equal(2.5, MathHelpers.Lerp(0, 5, .5));
            Assert.Equal(0, MathHelpers.Lerp(-5, 5, 0.5));
            Assert.Equal(0, MathHelpers.Lerp(0, 0, 0.5));
            Assert.Equal(-3, MathHelpers.Lerp(-5, -1, 0.5));
        }
        #endregion

        #region Helpers
        // Converts degrees to radians the traditional way. This mathematical calculation is trivially verifiable
        // since the algorithm is so well known.
        private static double ConvertDegreesToRadians(double degrees) => (Math.PI / 180.0) * degrees;

        // Converts radians to degrees the traditional way. This mathematical calculation is trivially verifiable
        // since the algorithm is so well known.
        private static double ConvertRadiansToDegrees(double radians) => (180 / Math.PI) * radians;
        #endregion
    }
}
