using System;
using System.Collections.Generic;
using SFML.System;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.SFML.UnitTests
{
    public class PointVector2iTests
    {
        #region Test Data

        public static Point[] TestPoints =
        {
            new Point(1, 2),
            new Point(1, 3),
            new Point(3, 2)
        };

        // Negative, positive, and 0 for testing math operators.
        public static readonly IEnumerable<int> IntTestCases = new[]{ -3, 0, 6 };

        // Negative, positive, and 0 for testing math operators.
        public static readonly IEnumerable<double> DoubleTestCases = new[]{ -3.1, 0, 6.7 };

        // Positive and negative values
        private readonly Rectangle _testPositions = new Rectangle((10, 20), 39, 27);
        #endregion

        #region Conversion
        [Fact]
        public void ToVector2i()
        {
            var point = new Point(1, 3);
            Vector2i vector2i = point.ToVector2i();

            Assert.Equal(point.X, vector2i.X);
            Assert.Equal(point.Y, vector2i.Y);
        }

        [Fact]
        public void ToSadRoguePoint()
        {
            var vec2i = new Vector2i(1, 3);
            Point sadRoguePoint = vec2i.ToPoint();

            Assert.Equal(vec2i.X, sadRoguePoint.X);
            Assert.Equal(vec2i.Y, sadRoguePoint.Y);
        }
        #endregion

        #region Equality

        [Theory]
        [MemberDataEnumerable(nameof(TestPoints))]
        public void Matches(Point point)
        {
            foreach (var point2 in TestPoints)
            {
                var vector2i = point2.ToVector2i();
                Assert.Equal(point.Matches(point2), point.Matches(vector2i));
                Assert.Equal(point.Matches(point2), vector2i.Matches(point));
            }
        }
        #endregion

        #region Math Ops
        [Fact]
        public void TestAddPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var sfmlPos = pos.ToVector2i();
                var expected = new Point(_testPositions.Center.X + pos.X, _testPositions.Center.Y + pos.Y);
                var sfmlExpected = expected.ToVector2i();
                Assert.Equal(expected, _testPositions.Center.Add(sfmlPos));
                Assert.Equal(sfmlExpected, _testPositions.Center.ToVector2i().Add(pos));
            }
        }

        [Fact]
        public void TestAddDirection()
        {
            foreach (var dir in AdjacencyRule.EightWay.DirectionsOfNeighborsCache)
            {
                foreach (var pos in _testPositions.Positions())
                {
                    var sfmlPos = pos.ToVector2i();
                    var expected = pos + dir;
                    Assert.Equal(expected.ToVector2i(), sfmlPos.Add(dir));
                }
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestAddInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var sfmlPos = pos.ToVector2i();
                var expected = new Point(pos.X + i, pos.Y + i).ToVector2i();
                Assert.Equal(expected, sfmlPos.Add(i));
            }
        }

        [Fact]
        public void TestSubPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var sfmlPos = pos.ToVector2i();
                var expected = new Point(_testPositions.Center.X - pos.X, _testPositions.Center.Y - pos.Y);
                var sfmlExpected = expected.ToVector2i();
                Assert.Equal(expected, _testPositions.Center.Subtract(sfmlPos));
                Assert.Equal(sfmlExpected, _testPositions.Center.ToVector2i().Subtract(pos));
            }
        }

        [Fact]
        public void TestSubDirection()
        {
            foreach (var dir in AdjacencyRule.EightWay.DirectionsOfNeighborsCache)
            {
                foreach (var pos in _testPositions.Positions())
                {
                    var sfmlPos = pos.ToVector2i();
                    var expected = pos - dir;
                    Assert.Equal(expected.ToVector2i(), sfmlPos.Subtract(dir));
                }
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestSubInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var sfmlPos = pos.ToVector2i();
                var expected = new Point(pos.X - i, pos.Y - i).ToVector2i();
                Assert.Equal(expected, sfmlPos.Subtract(i));
            }
        }

        [Fact]
        public void TestMultPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(_testPositions.Center.X * pos.X, _testPositions.Center.Y * pos.Y);
                Assert.Equal(expected.ToVector2i(), _testPositions.Center.ToVector2i().Multiply(pos));
                Assert.Equal(expected, _testPositions.Center.Multiply(pos.ToVector2i()));
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestMultInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(pos.X * i, pos.Y * i);
                Assert.Equal(expected.ToVector2i(), pos.ToVector2i().Multiply(i));
            }
        }


        [Theory]
        [MemberDataEnumerable(nameof(DoubleTestCases))]
        public void TestMultDouble(double d)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point((int)Math.Round(pos.X * d, MidpointRounding.AwayFromZero), (int)Math.Round(pos.Y * d, MidpointRounding.AwayFromZero));
                Assert.Equal(expected.ToVector2i(), pos.ToVector2i().Multiply(d));
            }
        }

        [Fact]
        public void TestDivPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                // Integer rounding
                var expected = new Point((int)Math.Round((double)_testPositions.Center.X / pos.X, MidpointRounding.AwayFromZero), (int)Math.Round((double)_testPositions.Center.Y / pos.Y, MidpointRounding.AwayFromZero));
                Assert.Equal(expected.ToVector2i(), _testPositions.Center.ToVector2i().Divide(pos));
                Assert.Equal(expected, _testPositions.Center.Divide(pos.ToVector2i()));
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(DoubleTestCases))]
        public void TestDivDouble(double d)
        {
            foreach (var pos in _testPositions.Positions())
            {
                // Integer rounding
                var expected = new Point((int)Math.Round(pos.X / d, MidpointRounding.AwayFromZero), (int)Math.Round(pos.Y / d, MidpointRounding.AwayFromZero));
                Assert.Equal(expected.ToVector2i(), pos.ToVector2i().Divide(d));
            }
        }
        #endregion
    }
}
