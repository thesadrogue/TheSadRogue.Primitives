using Xunit;
using XUnit.ValueTuples;
using System.Linq;

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

    }
}
