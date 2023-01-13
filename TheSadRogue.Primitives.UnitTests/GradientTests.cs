using System;
using System.Collections.Generic;
using Xunit;
using XUnit.ValueTuples;
using System.Linq;
using System.Runtime.Loader;

namespace SadRogue.Primitives.UnitTests
{
    public class GradientStopTests
    {
        #region Test Data

        private static readonly GradientStop s_equalityStop = new GradientStop(Color.Aqua, 1.3f);

        public static readonly GradientStop[] SampleStops =
        {
            new GradientStop(s_equalityStop.Color, s_equalityStop.Stop), new GradientStop(Color.Aquamarine, 3.1f),
            new GradientStop(new Color(12, 34, 54, 46), 3.2f)
        };

        public static readonly (GradientStop stop, (Color color, float stop))[] CtorCases =
        {
            (new GradientStop(Color.Aqua, 1f), (Color.Aqua, 1f)),
            (new GradientStop(Color.Bisque, 3.4f), (Color.Bisque, 3.4f))
        };

        #endregion

        #region Constructor

        [Theory]
        [MemberDataTuple(nameof(CtorCases))]
        public void TestConstruction(GradientStop actual, (Color color, float stop) expected)
        {
            Assert.Equal(expected.color, actual.Color);
            Assert.Equal(expected.stop, actual.Stop);
        }
        #endregion

        #region Equality/Inequality

        [Fact]
        public void TestEquality()
        {
            Assert.True(s_equalityStop.Equals(SampleStops[0]));
            Assert.True(s_equalityStop.Matches(SampleStops[0]));
            Assert.True(s_equalityStop == SampleStops[0]);
            Assert.True(s_equalityStop.Equals((object)SampleStops[0]));
        }

        [Theory]
        [MemberDataEnumerable(nameof(SampleStops))]
        public void TestEqualityInequalityRelationship(GradientStop testStop)
        {
            Assert.Single(SampleStops.Where(i => i.Equals(testStop)));
            Assert.Single(SampleStops.Where(i => i.Matches(testStop)));
            Assert.Single(SampleStops.Where(i => i.Equals((object)testStop)));
            Assert.Single(SampleStops.Where(i => i == testStop));
            foreach (var other in SampleStops)
            {
                Assert.Equal(!(testStop == other), testStop != other);
                if (testStop.Equals(other))
                    Assert.Equal(testStop.GetHashCode(), other.GetHashCode());
            }
        }
        #endregion
    }

    public class GradientTests
    {
        #region Constructors

        [Fact]
        public void TwoColorConstructor()
        {
            var gradient = new Gradient(Color.Aqua, Color.Red);
            Assert.Equal(2, gradient.Stops.Length);

            Assert.Equal(Color.Aqua, gradient.Stops[0].Color);
            Assert.Equal(0f, gradient.Stops[0].Stop);

            Assert.Equal(Color.Red, gradient.Stops[1].Color);
            Assert.Equal(1f, gradient.Stops[1].Stop);
        }

        [Fact]
        public void EnumerableOfStopsConstructor()
        {
            var stops = new[]
            {
                new GradientStop(Color.Aqua, 0f),
                new GradientStop(Color.Orange, .4f),
                new GradientStop(Color.White, 1f)
            };

            var gradient = new Gradient(stops);
            Assert.Equal((IEnumerable<GradientStop>)stops, gradient.Stops);
        }

        [Fact]
        public void MultipleEnumerableConstructor()
        {
            var colors = new[] { Color.Aqua, Color.Orange, Color.White };
            float[] stops = { 0f, .4f, 1f };

            var gradient = new Gradient(colors, stops);

            Assert.Equal(colors.Length, gradient.Stops.Length);
            for (int i = 0; i < colors.Length; i++)
            {
                var expectedStop = new GradientStop(colors[i], stops[i]);
                Assert.Equal(expectedStop, gradient.Stops[i]);
            }
        }

        [Fact]
        public void MultipleEnumerableConstructorMismatchedSizes()
        {
            var colors = new[] { Color.Aqua, Color.Orange, Color.White };
            float[] stops = { 0f, 1f };

            Assert.Throws<ArgumentException>(() => new Gradient(colors, stops));

            stops = new[] { 0f, .4f, .5f, 1f };
            Assert.Throws<ArgumentException>(() => new Gradient(colors, stops));
        }

        [Fact]
        public void EvenlySpaceColorsConstructor()
        {
            var initializationColors = new[] { Color.Aqua, Color.Orange, Color.White };

            // This is a params constructor, but we can call it this way too
            var gradient = new Gradient(initializationColors);

            Assert.Equal(initializationColors.Length, gradient.Stops.Length);

            float increment = 1f / (initializationColors.Length - 1);
            float expectedStop = 0f;
            for (int i = 0; i < gradient.Stops.Length; i++)
            {
                var currentStop = gradient.Stops[i];
                Assert.Equal(initializationColors[i], currentStop.Color);
                Assert.Equal(expectedStop, currentStop.Stop);

                expectedStop += increment;
            }
        }

        [Fact]
        public void EvenlySpaceColorsSingleColor()
        {
            var gradient = new Gradient(Color.Green);

            Assert.Equal(2, gradient.Stops.Length);

            Assert.Equal(Color.Green, gradient.Stops[0].Color);
            Assert.Equal(0f, gradient.Stops[0].Stop);

            Assert.Equal(Color.Green, gradient.Stops[1].Color);
            Assert.Equal(1f, gradient.Stops[1].Stop);
        }

        [Fact]
        public void EvenlySpacedColorsConstructorNoColors()
        {
            var colors = Array.Empty<Color>();
            Assert.Throws<ArgumentException>(() => new Gradient(colors));
        }
        #endregion

        #region Enumerable
        [Fact]
        public void EnumerationOfGradientStops()
        {
            var stops = new[]
            {
                new GradientStop(Color.Aqua, 0f),
                new GradientStop(Color.Orange, .4f),
                new GradientStop(Color.White, 1f)
            };

            var gradient = new Gradient(stops);

            // ReSharper disable once RedundantCast
            Assert.Equal(stops, (IEnumerable<GradientStop>)gradient);
        }

        #endregion

        #region Color Array From Gradient

        [Fact]
        public void ToColorArrayBasic()
        {
            // Define a gradient with uniform stops (every 0.25 units)
            var gradientStops = new[]
            {
                new GradientStop(new Color(0, 0, 0), 0f),
                new GradientStop(new Color(64, 32, 16), .25f),
                new GradientStop(new Color(128, 64, 32), .5f),
                new GradientStop(new Color(200, 30, 54), .75f),
                new GradientStop(new Color(150, 234, 148), 1f)
            };

            // Manually specify what a sample size of 9 would look like for this gradient

            var samplesExpected = new[]
            {
                new Color(0, 0, 0),
                new Color(32, 16, 8), // Implicit based on defined gradient
                new Color(64, 32, 16),
                new Color(96, 48, 20), // Implicit based on defined gradient
                new Color(128, 64, 32),
                new Color(164, 47, 43), // Implicit based on defined gradient
                new Color(200, 30, 54),
                new Color(175, 137, 101), // Implicit based on defined gradient
                new Color(150, 234, 148)
            };

            // Create the gradient
            var gradient = new Gradient(gradientStops);

            var samples = gradient.ToColorArray(samplesExpected.Length);

            Assert.Equal(samplesExpected.Length, samples.Length);

            for (int i = 0; i < samplesExpected.Length; i++)
            {
                var expected = samplesExpected[i];
                var actual = samples[i];
                Assert.InRange(expected.R - actual.R, -5, 5);
                Assert.InRange(expected.G - actual.G, -5, 5);
                Assert.InRange(expected.B - actual.B, -5, 5);
                Assert.InRange(expected.A - actual.A, -5, 5);
            }
        }
        #endregion
    }
}
