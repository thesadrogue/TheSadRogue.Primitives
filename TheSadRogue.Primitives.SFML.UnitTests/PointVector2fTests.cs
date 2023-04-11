using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SFML.System;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.SFML.UnitTests
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PointVector2fTests
    {
        #region Test Data

        public static Point[] TestPoints =
        {
            new Point(1, 2),
            new Point(1, 3),
            new Point(3, 2)
        };

        // Negative, positive for testing math operators.  No 0 because div by 0 is defined differently for floats
        // than it is for ints.
        public static readonly IEnumerable<int> IntTestCases = new[]{ -3, 6 };

        // Negative, positive, and 0 for testing math operators.  No 0 because div by 0 is defined differently for floats
        // than it is for ints.
        public static readonly IEnumerable<double> DoubleTestCases = new[]{ -3.1, 6.7 };

        // Positive and negative values
        private readonly Rectangle _testPositions = new Rectangle((10, 20), 39, 27);
        #endregion

        #region Conversion
        [Fact]
        public void ToVector2f()
        {
            var point = new Point(1, 3);
            Vector2f vector2f = point.ToVector2f();

            Assert.Equal(point.X, vector2f.X);
            Assert.Equal(point.Y, vector2f.Y);
        }

        [Fact]
        public void ToSadRoguePoint()
        {
            var vec2f = new Vector2f(1, 3);
            Point sadRoguePoint = vec2f.ToPoint();

            Assert.Equal(vec2f.X, sadRoguePoint.X);
            Assert.Equal(vec2f.Y, sadRoguePoint.Y);
        }
        #endregion

        #region Equality

        [Theory]
        [MemberDataEnumerable(nameof(TestPoints))]
        public void Matches(Point point)
        {
            foreach (var point2 in TestPoints)
            {
                var vector2f = point2.ToVector2f();
                Assert.Equal(point.Matches(point2), point.Matches(vector2f));
                Assert.Equal(point.Matches(point2), vector2f.Matches(point));
            }
        }
        #endregion

        #region Math Ops
        [Fact]
        public void TestAddPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var sfmlPos = pos.ToVector2f();
                var expected = new Point(_testPositions.Center.X + pos.X, _testPositions.Center.Y + pos.Y);
                var sfmlExpected = expected.ToVector2f();
                Assert.Equal(expected, _testPositions.Center.Add(sfmlPos));
                Assert.Equal(sfmlExpected, _testPositions.Center.ToVector2f().Add(pos));
            }

            // Test with a position with decimal values involved
            var vec = new Vector2f(1.9f, 2.7f);
            var point = new Point(1, 2);

            var expectedVec2f = new Vector2f(vec.X + point.X, vec.Y + point.Y);
            var expectedPoint = new Point((int)Math.Round(vec.X + point.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(vec.Y + point.Y, MidpointRounding.AwayFromZero));

            Assert.Equal(expectedVec2f, vec.Add(point));
            Assert.Equal(expectedPoint, point.Add(vec));
        }

        [Fact]
        public void TestAddDirection()
        {
            foreach (var dir in AdjacencyRule.EightWay.DirectionsOfNeighborsCache)
            {
                foreach (var pos in _testPositions.Positions())
                {
                    var sfmlPos = pos.ToVector2f();
                    var expected = pos + dir;
                    Assert.Equal(expected.ToVector2f(), sfmlPos.Add(dir));
                }

                // Test with a position with decimal values involved
                var vec = new Vector2f(1.9f, 2.7f);
                var expectedVec2f = new Vector2f(vec.X + dir.DeltaX, vec.Y + dir.DeltaY);
                Assert.Equal(expectedVec2f, vec.Add(dir));
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestAddInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var sfmlPos = pos.ToVector2f();
                var expected = new Point(pos.X + i, pos.Y + i).ToVector2f();
                Assert.Equal(expected, sfmlPos.Add(i));
            }

            // Test with a position with decimal values involved
            var vec = new Vector2f(1.9f, 2.7f);
            var expectedVec2f = new Vector2f(vec.X + i, vec.Y + i);
            Assert.Equal(expectedVec2f, vec.Add(i));
        }

        [Fact]
        public void TestSubPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var sfmlPos = pos.ToVector2f();
                var expected = new Point(_testPositions.Center.X - pos.X, _testPositions.Center.Y - pos.Y);
                var sfmlExpected = expected.ToVector2f();
                Assert.Equal(expected, _testPositions.Center.Subtract(sfmlPos));
                Assert.Equal(sfmlExpected, _testPositions.Center.ToVector2f().Subtract(pos));
            }

            // Test with a position with decimal values involved
            var vec = new Vector2f(1.9f, 2.7f);
            var point = new Point(1, 2);

            var expectedVec2f = new Vector2f(vec.X - point.X, vec.Y - point.Y);

            Assert.Equal(expectedVec2f, vec.Subtract(point));
        }

        [Fact]
        public void TestSubDirection()
        {
            foreach (var dir in AdjacencyRule.EightWay.DirectionsOfNeighborsCache)
            {
                foreach (var pos in _testPositions.Positions())
                {
                    var sfmlPos = pos.ToVector2f();
                    var expected = pos - dir;
                    Assert.Equal(expected.ToVector2f(), sfmlPos.Subtract(dir));
                }

                // Test with a position with decimal values involved
                var vec = new Vector2f(1.9f, 2.7f);
                var expectedVec2f = new Vector2f(vec.X - dir.DeltaX, vec.Y - dir.DeltaY);
                Assert.Equal(expectedVec2f, vec.Subtract(dir));
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestSubInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var sfmlPos = pos.ToVector2f();
                var expected = new Point(pos.X - i, pos.Y - i).ToVector2f();
                Assert.Equal(expected, sfmlPos.Subtract(i));
            }

            // Test with a position with decimal values involved
            var vec = new Vector2f(1.9f, 2.7f);
            var expectedVec2f = new Vector2f(vec.X - i, vec.Y - i);
            Assert.Equal(expectedVec2f, vec.Subtract(i));
        }

        [Fact]
        public void TestMultiplyPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(_testPositions.Center.X * pos.X, _testPositions.Center.Y * pos.Y);
                Assert.Equal(expected.ToVector2f(), _testPositions.Center.ToVector2f().Multiply(pos));
                Assert.Equal(expected, _testPositions.Center.Multiply(pos.ToVector2f()));
            }

            // Test with a position with decimal values involved
            var vec = new Vector2f(1.9f, 2.7f);
            var point = new Point(1, 2);

            var expectedVec2f = new Vector2f(vec.X * point.X, vec.Y * point.Y);
            var expectedPoint = new Point((int)Math.Round(vec.X * point.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(vec.Y * point.Y, MidpointRounding.AwayFromZero));

            Assert.Equal(expectedVec2f, vec.Multiply(point));
            Assert.Equal(expectedPoint, point.Multiply(vec));
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestMultiplyInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(pos.X * i, pos.Y * i);
                Assert.Equal(expected.ToVector2f(), pos.ToVector2f().Multiply(i));
            }

            // Test with a position with decimal values involved
            var vec = new Vector2f(1.9f, 2.7f);
            var expectedVec2f = new Vector2f(vec.X * i, vec.Y * i);
            Assert.Equal(expectedVec2f, vec.Multiply(i));
        }


        [Theory]
        [MemberDataEnumerable(nameof(DoubleTestCases))]
        public void TestMultiplyDouble(double d)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Vector2f(pos.X * (float)d, pos.Y * (float)d);
                var actual = pos.ToVector2f().Multiply(d);
                Assert.InRange(expected.X - actual.X, -0.00001f, 0.00001f);
                Assert.InRange(expected.Y - actual.Y, -0.00001f, 0.00001f);
            }

            // Test with a position with decimal values involved
            var vec = new Vector2f(1.9f, 2.7f);
            var expectedVec2f = new Vector2f(vec.X * (float)d, vec.Y * (float)d);
            Assert.Equal(expectedVec2f, vec.Multiply(d));
        }

        [Fact]
        public void TestDividePoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                // Integer rounding on int point type
                var expectedVec = new Vector2f((float)_testPositions.Center.X / pos.X,
                    (float)_testPositions.Center.Y / pos.Y);
                var expected = new Point((int)Math.Round((float)_testPositions.Center.X / pos.X, MidpointRounding.AwayFromZero), (int)Math.Round((float)_testPositions.Center.Y / pos.Y, MidpointRounding.AwayFromZero));
                Assert.Equal(expectedVec, _testPositions.Center.ToVector2f().Divide(pos));
                Assert.Equal(expected, _testPositions.Center.Divide(pos.ToVector2f()));
            }

            // Test with a position with decimal values involved
            var vec = new Vector2f(1.9f, 2.7f);
            var point = new Point(1, 2);

            var expectedVec2f = new Vector2f(vec.X / point.X, vec.Y / point.Y);
            var expectedPoint = new Point((int)Math.Round(point.X / vec.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(point.Y / vec.Y, MidpointRounding.AwayFromZero));

            Assert.Equal(expectedVec2f, vec.Divide(point));
            Assert.Equal(expectedPoint, point.Divide(vec));
        }

        [Theory]
        [MemberDataEnumerable(nameof(DoubleTestCases))]
        public void TestDivideDouble(double d)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Vector2f((float)(pos.X / d), (float)(pos.Y / d));
                var actual = pos.ToVector2f().Divide(d);
                Assert.InRange(expected.X - actual.X, -0.00001f, 0.00001f);
                Assert.InRange(expected.Y - actual.Y, -0.00001f, 0.00001f);
            }

            // Test with a position with decimal values involved
            var vec = new Vector2f(1.9f, 2.7f);
            var expectedVec2f = new Vector2f(vec.X / (float)d, vec.Y / (float)d);
            Assert.Equal(expectedVec2f, vec.Divide(d));
        }
        #endregion
    }
}
