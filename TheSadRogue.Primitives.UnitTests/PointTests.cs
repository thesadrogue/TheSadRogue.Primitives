using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class PointTests
    {
        #region Test Data
        // (Point to rotate, degrees of rotation, expected result)
        public static readonly IEnumerable<(Point original, double degrees, Point expected)> AroundZeroZeroData = new[]
        {

            (new Point(5,0), 22.5, new Point(5,2)),
            (new Point(5,0), 45, new Point(4,4)),
            (new Point(5,0), 90, new Point(0,5)),
            (new Point(5,0), 107.3, new Point(-1,5)),
            (new Point(13,71), 16.75, new Point(-8, 72)),
            (new Point(13,71), 137.999, new Point(-57,-44)),
            (new Point(13,71), 456.2, new Point(-72,5)),
            (new Point(13,71), 0, new Point(13,71)),
        };

        // (Point to Rotate, Point around which to rotate, degree of rotation, expected result)
        public static readonly IEnumerable<(Point original, Point axis, double degrees, Point expected)> AroundOtherData = new[]
        {
            (new Point(5,0), new Point(0,5), 22.5, new Point(7,2)),
            (new Point(5,0), new Point(5,5), 45, new Point(9,1)),
            (new Point(5,0), new Point(8,16), 90, new Point(24,13)),
            (new Point(5,0), new Point(13,17), 107.3, new Point(32,14)),
            (new Point(13,71), new Point(-100,1), 16.75, new Point(-12, 101)),
            (new Point(13,71), new Point(-81,-81), 137.999, new Point(-253,-131)),
            (new Point(13,71), new Point(100,157), 456.2, new Point(195,80)),
            (new Point(13,71), new Point(13,71), 222, new Point(13,71)),
        };

        // Negative, positive, and 0 for testing math operators.
        public static readonly IEnumerable<int> IntTestCases = new[]{ -3, 0, 6 };

        // Negative, positive, and 0 for testing math operators.
        public static readonly IEnumerable<double> DoubleTestCases = new[]{ -3.1, 0, 6.7 };

        public static Point[] DifferentPoints =
        {
            new Point(1, 2), new Point(3, 4), new Point(-3, -4), new Point(1, 10), new Point(6, 4)
        };

        // Positive and negative values
        private readonly Rectangle _testPositions = new Rectangle((10, 20), 39, 27);
        #endregion

        #region With Construction

        [Fact]
        public void WithXTest()
        {
            const int x = 42;
            foreach (var pos in _testPositions.Positions())
            {
                var transformed = pos.WithX(x);
                Assert.Equal(x, transformed.X);
                Assert.Equal(pos.Y, transformed.Y);
            }
        }

        [Fact]
        public void WithTTest()
        {
            const int y = 42;
            foreach (var pos in _testPositions.Positions())
            {
                var transformed = pos.WithY(y);
                Assert.Equal(pos.X, transformed.X);
                Assert.Equal(y, transformed.Y);
            }
        }
        #endregion

        #region 1D Index

        [Fact]
        public void To1DIndex()
        {
            var rect = _testPositions.WithPosition((0, 0));
            foreach (var pos in rect.Positions())
            {
                int expected = pos.Y * rect.Width + pos.X;
                Assert.Equal(expected, pos.ToIndex(rect.Width));
                Assert.Equal(expected, Point.ToIndex(pos.X, pos.Y, rect.Width));
            }
        }

        [Fact]
        public void From1DIndex()
        {
            var rect = _testPositions.WithPosition((0, 0));
            foreach (var pos in rect.Positions())
            {
                int index = pos.Y * rect.Width + pos.X;
                Assert.Equal(pos, Point.FromIndex(index, rect.Width));
                Assert.Equal(pos.X, Point.ToXValue(index, rect.Width));
                Assert.Equal(pos.Y, Point.ToYValue(index, rect.Width));
            }
        }

        [Fact]
        public void IndicesAreUnique()
        {
            var rect = _testPositions.WithPosition((0, 0));
            Assert.Equal(rect.Area, rect.Positions().ToEnumerable().Select(i => i.ToIndex(rect.Width)).ToHashSet().Count);
        }

        #endregion

        #region Rotation
        [Theory]
        [MemberDataTuple(nameof(AroundZeroZeroData))]
        public void RotateAroundZeroZeroTest(Point original, double degrees, Point expected)
        {
            Point answer = original.Rotate(degrees);
            Assert.Equal(expected, answer);
            AssertEquidistant(expected, answer, (0,0));
        }

        [Theory]
        [MemberDataTuple(nameof(AroundOtherData))]
        public void RotateAroundOtherPointTest(Point original, Point axis, double degrees, Point expected)
        {
            Point answer = original.Rotate(degrees, axis);
            Point answer2 = original.Rotate(degrees, axis.X, axis.Y);
            Assert.Equal(expected, answer);
            Assert.Equal(expected, answer2);
            AssertEquidistant(expected, answer, axis);
        }

        #endregion

        #region Euclidean Distance Magnitude

        [Fact]
        public void EuclideanDistanceMagnitudeSameAsEuclideanMagnitude()
        {
            var positionsByDistance = _testPositions.Positions().ToEnumerable()
                .OrderBy(i => Distance.Euclidean.Calculate(_testPositions.Center, i)).ToArray();
            var positionsByMagnitude = _testPositions.Positions().ToEnumerable()
                .OrderBy(i => Point.EuclideanDistanceMagnitude(_testPositions.Center, i)).ToArray();

            Assert.Equal((IEnumerable<Point>)positionsByDistance, positionsByMagnitude);
        }

        [Fact]
        public void EuclideanMagnitudeCommutative()
        {
            foreach (var pos in _testPositions.Positions())
                Assert.Equal(Point.EuclideanDistanceMagnitude(_testPositions.Center, pos), Point.EuclideanDistanceMagnitude(pos, _testPositions.Center));
        }

        [Fact]
        public void EuclideanMagnitudeOverloadsEquivalent()
        {
            foreach (var pos in _testPositions.Positions())
            {
                // Checked to be correct by other test cases
                double expected = Point.EuclideanDistanceMagnitude(_testPositions.Center, pos);

                // Other overloads should produce equivalent results
                Assert.Equal(expected, Point.EuclideanDistanceMagnitude(_testPositions.Center - pos));
                Assert.Equal(expected, Point.EuclideanDistanceMagnitude(pos - _testPositions.Center));

                var delta = pos - _testPositions.Center;
                Assert.Equal(expected, Point.EuclideanDistanceMagnitude(delta.X, delta.Y));
                delta = _testPositions.Center - pos;
                Assert.Equal(expected, Point.EuclideanDistanceMagnitude(delta.X, delta.Y));

                Assert.Equal(expected, Point.EuclideanDistanceMagnitude(_testPositions.Center.X, _testPositions.Center.Y, pos.X, pos.Y));
                Assert.Equal(expected, Point.EuclideanDistanceMagnitude(pos.X, pos.Y, _testPositions.Center.X, _testPositions.Center.Y));
            }
        }
        #endregion

        #region TranslationAndMathOps

        [Fact]
        public void TestTranslate()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(_testPositions.Center.X + pos.X, _testPositions.Center.Y + pos.Y);
                Assert.Equal(expected, _testPositions.Center.Translate(pos));
                Assert.Equal(expected, _testPositions.Center.Translate(pos.X, pos.Y));
            }
        }

        [Fact]
        public void TestAddPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(_testPositions.Center.X + pos.X, _testPositions.Center.Y + pos.Y);
                Assert.Equal(expected, _testPositions.Center + pos);
            }
        }

        [Fact]
        public void TestAddValueTuple()
        {
            foreach (var pos in _testPositions.Positions())
            {
                (int x, int y) tuple = (pos.X, pos.Y);
                var expected = new Point(_testPositions.Center.X + pos.X, _testPositions.Center.Y + pos.Y);
                (int x, int y) expected2 = expected;
                Assert.Equal(expected, _testPositions.Center + tuple);
                Assert.Equal(expected2, tuple + _testPositions.Center);
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestAddInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(_testPositions.Center.X + i, _testPositions.Center.Y + i);
                Assert.Equal(expected, _testPositions.Center + i);
            }
        }

        [Fact]
        public void TestSubPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(_testPositions.Center.X - pos.X, _testPositions.Center.Y - pos.Y);
                Assert.Equal(expected, _testPositions.Center - pos);
            }
        }

        [Fact]
        public void TestSubValueTuple()
        {
            foreach (var pos in _testPositions.Positions())
            {
                (int x, int y) tuple = (pos.X, pos.Y);
                var expected = new Point(_testPositions.Center.X - pos.X, _testPositions.Center.Y - pos.Y);
                (int x, int y) expected2 = (tuple.x - _testPositions.Center.X, tuple.y - _testPositions.Center.Y);
                Assert.Equal(expected, _testPositions.Center - tuple);

                Assert.Equal(expected2, tuple - _testPositions.Center);
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestSubInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(_testPositions.Center.X - i, _testPositions.Center.Y - i);
                Assert.Equal(expected, _testPositions.Center - i);
            }
        }

        [Fact]
        public void TestMultPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(_testPositions.Center.X * pos.X, _testPositions.Center.Y * pos.Y);
                Assert.Equal(expected, _testPositions.Center * pos);
            }
        }

        [Fact]
        public void TestMultValueTuple()
        {
            foreach (var pos in _testPositions.Positions())
            {
                (int x, int y) tuple = (pos.X, pos.Y);
                var expected = new Point(_testPositions.Center.X * pos.X, _testPositions.Center.Y * pos.Y);
                (int x, int y) expected2 = expected;
                Assert.Equal(expected, _testPositions.Center * tuple);
                Assert.Equal(expected2, tuple * _testPositions.Center);
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestMultInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(_testPositions.Center.X * i, _testPositions.Center.Y * i);
                Assert.Equal(expected, _testPositions.Center * i);
            }
        }


        [Theory]
        [MemberDataEnumerable(nameof(DoubleTestCases))]
        public void TestMultDouble(double d)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point((int)Math.Round(_testPositions.Center.X * d, MidpointRounding.AwayFromZero), (int)Math.Round(_testPositions.Center.Y * d, MidpointRounding.AwayFromZero));
                Assert.Equal(expected, _testPositions.Center * d);
            }
        }

        [Fact]
        public void TestDivPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                // Integer rounding
                var expected = new Point((int)Math.Round((double)_testPositions.Center.X / pos.X, MidpointRounding.AwayFromZero), (int)Math.Round((double)_testPositions.Center.Y / pos.Y, MidpointRounding.AwayFromZero));
                Assert.Equal(expected, _testPositions.Center / pos);
            }
        }

        [Fact]
        public void TestDivValueTuple()
        {
            foreach (var pos in _testPositions.Positions())
            {
                // Integer rounding
                (int x, int y) tuple = (pos.X, pos.Y);
                var expected = new Point((int)Math.Round((double)_testPositions.Center.X / pos.X, MidpointRounding.AwayFromZero), (int)Math.Round((double)_testPositions.Center.Y / pos.Y, MidpointRounding.AwayFromZero));
                (int x, int y) expected2 = ((int)Math.Round((double)tuple.x / _testPositions.Center.X, MidpointRounding.AwayFromZero), (int)Math.Round((double)tuple.y / _testPositions.Center.Y, MidpointRounding.AwayFromZero));
                Assert.Equal(expected, _testPositions.Center / tuple);
                Assert.Equal(expected2, tuple / _testPositions.Center);
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(DoubleTestCases))]
        public void TestDivDouble(double d)
        {
            foreach (var pos in _testPositions.Positions())
            {
                // Integer rounding
                var expected = new Point((int)Math.Round(_testPositions.Center.X / d, MidpointRounding.AwayFromZero), (int)Math.Round(_testPositions.Center.Y / d, MidpointRounding.AwayFromZero));
                Assert.Equal(expected, _testPositions.Center / d);
            }
        }

        #endregion

        #region Test Helpers

        private void AssertEquidistant(Point expected, Point pointUnderTest, Point center)
        {
            expected -= center;
            pointUnderTest -= center;
            double expectedDistance = Math.Sqrt(expected.X * expected.X + expected.Y * expected.Y);
            double distanceUnderTest = Math.Sqrt(pointUnderTest.X * pointUnderTest.X + pointUnderTest.Y * pointUnderTest.Y);
            Assert.True(expectedDistance > distanceUnderTest - 0.001 && expectedDistance < distanceUnderTest + 0.001);
        }

        #endregion

        #region Equality/Inequality

        [Theory]
        [MemberDataEnumerable(nameof(DifferentPoints))]
        public void TestEquality(Point point)
        {
            Point compareTo = point;
            Point[] allPoints = DifferentPoints;
            Assert.True(point == compareTo);
            Assert.True(point.Matches(compareTo));
            Assert.True(point.Equals(compareTo));

            Assert.True(compareTo == point);
            Assert.True(compareTo.Matches(point));
            Assert.True(compareTo.Equals(point));

            Assert.Equal(point.GetHashCode(), compareTo.GetHashCode());

            Assert.Equal(1, allPoints.Count(i => i == compareTo));
        }


        [Theory]
        [MemberDataEnumerable(nameof(DifferentPoints))]
        public void TestInequality(Point point)
        {
            Point compareTo = point;
            (int, int) compareT1 = compareTo;

            Point[] allPoints = DifferentPoints;
            Assert.False(point != compareTo);
            Assert.False(point != compareT1);
            Assert.False(compareT1 != point);

            Assert.Equal(allPoints.Length - 1, allPoints.Count(i => i != compareTo));
        }

        [Theory]
        [MemberDataEnumerable(nameof(DifferentPoints))]
        public void TestEqualityInequalityOpposite(Point comparePoint)
        {
            Point[] points = DifferentPoints;
            (int, int) compareT1 = comparePoint;

            foreach (Point point in points)
            {
                Assert.NotEqual(point == comparePoint, point != comparePoint);
                Assert.Equal(point != comparePoint, point != compareT1);
            }
        }
        #endregion

        #region Tuple Conversions

        [Theory]
        [MemberDataEnumerable(nameof(DifferentPoints))]
        public void TestTupleConversions(Point point)
        {
            (int x, int y) t1 = point;

            Point point1 = t1;
            Assert.Equal(point, point1);
        }

        #endregion

        #region Tuple Equality
        [Theory]
        [MemberDataEnumerable(nameof(DifferentPoints))]
        public void TestTupleEquality(Point point)
        {
            (int x, int y) t1 = point;

            Assert.True(point == t1);
            Assert.True(t1 == point);
            Assert.True(point.Equals(t1));
            Assert.True(point.Matches(t1));
        }
        #endregion

        #region Deconstruction

        [Theory]
        [MemberDataEnumerable(nameof(DifferentPoints))]
        public void Deconstruction(Point point)
        {
            (int x, int y) = point;

            Assert.Equal(point.X, x);
            Assert.Equal(point.Y, y);
        }
        #endregion

        #region Midpoint

        [Fact]
        public void MidpointExact()
        {
            Point p1 = (-1, -2);

            Point p2 = p1 + (4, 8);

            var expected = p1 + (2, 4);
            Assert.Equal(expected, Point.Midpoint(p1, p2));
            Assert.Equal(expected, Point.Midpoint(p2, p1));
            Assert.Equal(expected, Point.Midpoint(p1.X, p1.Y, p2.X, p2.Y));
            Assert.Equal(expected, Point.Midpoint(p2.X, p2.Y, p1.X, p1.Y));
        }

        [Fact]
        public void MidpointRound()
        {
            Point p1 = (-1, -2);

            Point p2 = p1 + (5, 7);

            var expected = p1 + (3, 4);
            Assert.Equal(expected, Point.Midpoint(p1, p2));
            Assert.Equal(expected, Point.Midpoint(p2, p1));
            Assert.Equal(expected, Point.Midpoint(p1.X, p1.Y, p2.X, p2.Y));
            Assert.Equal(expected, Point.Midpoint(p2.X, p2.Y, p1.X, p1.Y));
        }
        #endregion
    }
}
