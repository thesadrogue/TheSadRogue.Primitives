using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SFML.System;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.SFML.UnitTests
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PointVector2uTests
    {
        #region Test Data

        public static Point[] TestPoints =
        {
            new Point(1, 2),
            new Point(1, 3),
            new Point(3, 2)
        };

        // Positive numbers only for testing math operators (no negative since Vector2i is unsigned; and no 0 since
        // C# defines unsigned divide by 0 differently than signed).
        public static readonly IEnumerable<int> IntTestCases = new[]{ 6 };

        // Positive numbers only for testing math operators (no negative since Vector2i is unsigned; and no 0 since
        // C# defines unsigned divide by 0 differently than signed).
        public static readonly IEnumerable<double> DoubleTestCases = new[]{ 6.7 };

        // Positive values only since Vector2u is unsigned
        private readonly Rectangle _testPositions = new Rectangle((40, 45), 39, 27);
        #endregion

        #region Conversion
        [Fact]
        public void ToVector2u()
        {
            var point = new Point(1, 3);
            Vector2u vector2u = point.ToVector2u();

            Assert.Equal(point.X, (int)vector2u.X);
            Assert.Equal(point.Y, (int)vector2u.Y);
        }

        [Fact]
        public void ToSadRoguePoint()
        {
            var vec2i = new Vector2u(1, 3);
            Point sadRoguePoint = vec2i.ToPoint();

            Assert.Equal((int)vec2i.X, sadRoguePoint.X);
            Assert.Equal((int)vec2i.Y, sadRoguePoint.Y);
        }
        #endregion

        #region Equality

        [Theory]
        [MemberDataEnumerable(nameof(TestPoints))]
        public void Matches(Point point)
        {
            foreach (var point2 in TestPoints)
            {
                var vector2u = point2.ToVector2u();
                Assert.Equal(point.Matches(point2), point.Matches(vector2u));
                Assert.Equal(point.Matches(point2), vector2u.Matches(point));
            }
        }
        #endregion

        #region Math Ops
        [Fact]
        public void TestAddPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var sfmlPos = pos.ToVector2u();
                var expected = new Point(_testPositions.Center.X + pos.X, _testPositions.Center.Y + pos.Y);
                var sfmlExpected = expected.ToVector2u();
                Assert.Equal(expected, _testPositions.Center.Add(sfmlPos));
                Assert.Equal(sfmlExpected, _testPositions.Center.ToVector2u().Add(pos));
            }
        }

        [Fact]
        public void TestAddDirection()
        {
            foreach (var dir in AdjacencyRule.EightWay.DirectionsOfNeighborsCache)
            {
                foreach (var pos in _testPositions.Positions())
                {
                    var sfmlPos = pos.ToVector2u();
                    var expected = pos + dir;
                    Assert.Equal(expected.ToVector2u(), sfmlPos.Add(dir));
                }
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestAddInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var sfmlPos = pos.ToVector2u();
                var expected = new Point(pos.X + i, pos.Y + i).ToVector2u();
                Assert.Equal(expected, sfmlPos.Add(i));
            }
        }

        [Fact]
        public void TestSubPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var sfmlPos = pos.ToVector2u();
                var expected = new Point(_testPositions.Center.X - pos.X, _testPositions.Center.Y - pos.Y);
                var sfmlExpected = expected.ToVector2u();
                Assert.Equal(expected, _testPositions.Center.Subtract(sfmlPos));
                Assert.Equal(sfmlExpected, _testPositions.Center.ToVector2u().Subtract(pos));
            }
        }

        [Fact]
        public void TestSubDirection()
        {
            foreach (var dir in AdjacencyRule.EightWay.DirectionsOfNeighborsCache)
            {
                foreach (var pos in _testPositions.Positions())
                {
                    var sfmlPos = pos.ToVector2u();
                    var expected = pos - dir;
                    Assert.Equal(expected.ToVector2u(), sfmlPos.Subtract(dir));
                }
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestSubInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var sfmlPos = pos.ToVector2u();
                var expected = new Point(pos.X - i, pos.Y - i).ToVector2u();
                Assert.Equal(expected, sfmlPos.Subtract(i));
            }
        }

        [Fact]
        public void TestMultiplyPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(_testPositions.Center.X * pos.X, _testPositions.Center.Y * pos.Y);
                Assert.Equal(expected.ToVector2u(), _testPositions.Center.ToVector2u().Multiply(pos));
                Assert.Equal(expected, _testPositions.Center.Multiply(pos.ToVector2u()));
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestMultiplyInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(pos.X * i, pos.Y * i);
                Assert.Equal(expected.ToVector2u(), pos.ToVector2u().Multiply(i));
            }
        }


        [Theory]
        [MemberDataEnumerable(nameof(DoubleTestCases))]
        public void TestMultiplyDouble(double d)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point((int)Math.Round(pos.X * d, MidpointRounding.AwayFromZero), (int)Math.Round(pos.Y * d, MidpointRounding.AwayFromZero));
                Assert.Equal(expected.ToVector2u(), pos.ToVector2u().Multiply(d));
            }
        }

        [Fact]
        public void TestDividePoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                // Integer rounding
                var expected = new Point((int)Math.Round((double)_testPositions.Center.X / pos.X, MidpointRounding.AwayFromZero), (int)Math.Round((double)_testPositions.Center.Y / pos.Y, MidpointRounding.AwayFromZero));
                Assert.Equal(expected.ToVector2u(), _testPositions.Center.ToVector2u().Divide(pos));
                Assert.Equal(expected, _testPositions.Center.Divide(pos.ToVector2u()));
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(DoubleTestCases))]
        public void TestDivideDouble(double d)
        {
            foreach (var pos in _testPositions.Positions())
            {
                // Integer rounding
                var expected = new Point((int)Math.Round(pos.X / d, MidpointRounding.AwayFromZero), (int)Math.Round(pos.Y / d, MidpointRounding.AwayFromZero));
                Assert.Equal(expected.ToVector2u(), pos.ToVector2u().Divide(d));
            }
        }
        #endregion
    }
}
