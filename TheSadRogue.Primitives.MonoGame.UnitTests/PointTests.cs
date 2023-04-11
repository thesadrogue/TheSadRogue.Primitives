using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.MonoGame.UnitTests
{
    public class PointTests
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
        public void ToMonoGamePoint()
        {
            var point = new Point(1, 3);
            Microsoft.Xna.Framework.Point monoPoint = point.ToMonoPoint();

            Assert.Equal(point.X, monoPoint.X);
            Assert.Equal(point.Y, monoPoint.Y);
        }

        [Fact]
        public void ToSadRoguePoint()
        {
            var point = new Microsoft.Xna.Framework.Point(1, 3);
            Point sadRoguePoint = point.ToPoint();

            Assert.Equal(point.X, sadRoguePoint.X);
            Assert.Equal(point.Y, sadRoguePoint.Y);
        }
        #endregion

        #region Equality

        [Theory]
        [MemberDataEnumerable(nameof(TestPoints))]
        public void Matches(Point point)
        {
            foreach (var point2 in TestPoints)
            {
                var monoPoint = point2.ToMonoPoint();
                Assert.Equal(point.Matches(point2), point.Matches(monoPoint));
                Assert.Equal(point.Matches(point2), monoPoint.Matches(point));
            }
        }
        #endregion

        #region Math Ops
        [Fact]
        public void TestAddPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var monoPos = pos.ToMonoPoint();
                var expected = new Point(_testPositions.Center.X + pos.X, _testPositions.Center.Y + pos.Y);
                var monoExpected = expected.ToMonoPoint();
                Assert.Equal(expected, _testPositions.Center.Add(monoPos));
                Assert.Equal(monoExpected, _testPositions.Center.ToMonoPoint().Add(pos));
            }
        }

        [Fact]
        public void TestAddDirection()
        {
            foreach (var dir in AdjacencyRule.EightWay.DirectionsOfNeighborsCache)
            {
                foreach (var pos in _testPositions.Positions())
                {
                    var monoPos = pos.ToMonoPoint();
                    var expected = pos + dir;
                    Assert.Equal(expected.ToMonoPoint(), monoPos.Add(dir));
                }
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestAddInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var monoPos = pos.ToMonoPoint();
                var expected = new Point(pos.X + i, pos.Y + i).ToMonoPoint();
                Assert.Equal(expected, monoPos.Add(i));
            }
        }

        [Fact]
        public void TestSubPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var monoPos = pos.ToMonoPoint();
                var expected = new Point(_testPositions.Center.X - pos.X, _testPositions.Center.Y - pos.Y);
                var monoExpected = expected.ToMonoPoint();
                Assert.Equal(expected, _testPositions.Center.Subtract(monoPos));
                Assert.Equal(monoExpected, _testPositions.Center.ToMonoPoint().Subtract(pos));
            }
        }

        [Fact]
        public void TestSubDirection()
        {
            foreach (var dir in AdjacencyRule.EightWay.DirectionsOfNeighborsCache)
            {
                foreach (var pos in _testPositions.Positions())
                {
                    var monoPos = pos.ToMonoPoint();
                    var expected = pos - dir;
                    Assert.Equal(expected.ToMonoPoint(), monoPos.Subtract(dir));
                }
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestSubInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var monoPos = pos.ToMonoPoint();
                var expected = new Point(pos.X - i, pos.Y - i).ToMonoPoint();
                Assert.Equal(expected, monoPos.Subtract(i));
            }
        }

        [Fact]
        public void TestMultiplyPoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(_testPositions.Center.X * pos.X, _testPositions.Center.Y * pos.Y);
                Assert.Equal(expected.ToMonoPoint(), _testPositions.Center.ToMonoPoint().Multiply(pos));
                Assert.Equal(expected, _testPositions.Center.Multiply(pos.ToMonoPoint()));
            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(IntTestCases))]
        public void TestMultiplyInt(int i)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point(pos.X * i, pos.Y * i);
                Assert.Equal(expected.ToMonoPoint(), pos.ToMonoPoint().Multiply(i));
            }
        }


        [Theory]
        [MemberDataEnumerable(nameof(DoubleTestCases))]
        public void TestMultiplyDouble(double d)
        {
            foreach (var pos in _testPositions.Positions())
            {
                var expected = new Point((int)Math.Round(pos.X * d, MidpointRounding.AwayFromZero), (int)Math.Round(pos.Y * d, MidpointRounding.AwayFromZero));
                Assert.Equal(expected.ToMonoPoint(), pos.ToMonoPoint().Multiply(d));
            }
        }

        [Fact]
        public void TestDividePoint()
        {
            foreach (var pos in _testPositions.Positions())
            {
                // Integer rounding
                var expected = new Point((int)Math.Round((double)_testPositions.Center.X / pos.X, MidpointRounding.AwayFromZero), (int)Math.Round((double)_testPositions.Center.Y / pos.Y, MidpointRounding.AwayFromZero));
                Assert.Equal(expected.ToMonoPoint(), _testPositions.Center.ToMonoPoint().Divide(pos));
                Assert.Equal(expected, _testPositions.Center.Divide(pos.ToMonoPoint()));
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
                Assert.Equal(expected.ToMonoPoint(), pos.ToMonoPoint().Divide(d));
            }
        }
        #endregion
    }
}
