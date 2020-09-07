using System;
using System.Collections.Generic;
using Xunit;

namespace SadRogue.Primitives.UnitTests
{
    public class PointTests
    {
        public static readonly IEnumerable<object[]> AroundZeroZeroData = new List<object[]>()
        {
            // { Point to rotate, degrees of rotation, expected result }
            new object[] { new Point(5,0), 22.5, new Point(5,2) },
            new object[] { new Point(5,0), 45, new Point(4,4) },
            new object[] { new Point(5,0), 90, new Point(0,5) },
            new object[] { new Point(5,0), 107.3, new Point(-1,5) },
            new object[] { new Point(13,71), 16.75, new Point(-8, 72) },
            new object[] { new Point(13,71), 137.999, new Point(-57,-44) },
            new object[] { new Point(13,71), 456.2, new Point(-72,5) },
            new object[] { new Point(13,71), 0, new Point(13,71) },
        };

        public static readonly IEnumerable<object[]> AroundOtherData = new List<object[]>()
        {
            // { Point to Rotate, Point around which to rotate, degree of rotation, expected result }
            new object[] { new Point(5,0), new Point(0,5), 22.5, new Point(7,2) },
            new object[] { new Point(5,0), new Point(5,5), 45, new Point(9,1) },
            new object[] { new Point(5,0), new Point(8,16), 90, new Point(24,13) },
            new object[] { new Point(5,0), new Point(13,17), 107.3, new Point(32,14) },
            new object[] { new Point(13,71), new Point(-100,1), 16.75, new Point(-12, 101) },
            new object[] { new Point(13,71), new Point(-81,-81), 137.999, new Point(-253,-131) },
            new object[] { new Point(13,71), new Point(100,157), 456.2, new Point(195,80) },
            new object[] { new Point(13,71), new Point(13,71), 222, new Point(13,71) },
        };

        [Theory]
        [MemberData(nameof(AroundZeroZeroData))]
        public void RotateAroundZeroZeroTest(Point original, double degrees, Point expected)
        {
            Point answer = original.Rotate(degrees);
            Assert.Equal(expected, answer);
            AssertEquidistant(expected, answer, (0,0));
        }

        [Theory]
        [MemberData(nameof(AroundOtherData))]
        public void RotateAroundOtherPointTest(Point original, Point axis, double degrees, Point expected)
        {
            Point answer = original.Rotate(axis, degrees);
            Assert.Equal(expected, answer);
            AssertEquidistant(expected, answer, axis);
        }

        private void AssertEquidistant(Point expected, Point pointUnderTest, Point center)
        {
            expected -= center;
            pointUnderTest -= center;
            double expectedDistance = Math.Sqrt(expected.X * expected.X + expected.Y * expected.Y);
            double distanceUnderTest = Math.Sqrt(pointUnderTest.X * pointUnderTest.X + pointUnderTest.Y * pointUnderTest.Y);
            Assert.True(expectedDistance > distanceUnderTest - 0.001 && expectedDistance < distanceUnderTest + 0.001);
        }
    }
}
