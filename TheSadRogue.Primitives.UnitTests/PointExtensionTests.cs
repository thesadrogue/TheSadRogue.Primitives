using System.Collections.Generic;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    /// <summary>
    /// Tests point extension methods to ensure they are equivalent to the equivalent operators (+, -, etc).
    /// </summary>
    /// <remarks>
    /// Direction-based operators can't be defined here since YIncreasesUpwards could change; so these are tested
    /// in the DirectionTests file in the NonThreadSafe project.
    /// </remarks>
    public class PointExtensionTests
    {
        #region TestData

        private static readonly int[] s_intSpread = { -3, 0, 1 };

        private static readonly double[] s_doubleSpread = { -3.2, 0.0, 1.0 };

        private static readonly Point[] s_testPoints = {(0, 0), (1, 2), (-3, -4)};

        public static IEnumerable<(Point p1, Point p2)> PointToPointPairs => s_testPoints.Combinate(s_testPoints);

        public static IEnumerable<(Point p1, int i)> PointToIntPairs => s_testPoints.Combinate(s_intSpread);

        public static IEnumerable<(Point p1, double d)> PointToDoublePairs => s_testPoints.Combinate(s_doubleSpread);
        #endregion

        #region Add
        [Theory]
        [MemberDataTuple(nameof(PointToPointPairs))]
        public void AddPoint(Point p1, Point p2)
        {
            var expected = p1 + p2;
            var actual = p1.Add(p2);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberDataTuple(nameof(PointToIntPairs))]
        public void AddInt(Point p1, int i)
        {
            var expected = p1 + i;
            var actual = p1.Add(i);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Subtract
        [Theory]
        [MemberDataTuple(nameof(PointToPointPairs))]
        public void SubtractPoint(Point p1, Point p2)
        {
            var expected = p1 - p2;
            var actual = p1.Subtract(p2);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberDataTuple(nameof(PointToIntPairs))]
        public void SubtractInt(Point p1, int i)
        {
            var expected = p1 - i;
            var actual = p1.Subtract(i);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Multiply
        [Theory]
        [MemberDataTuple(nameof(PointToPointPairs))]
        public void MultiplyPoint(Point p1, Point p2)
        {
            var expected = p1 * p2;
            var actual = p1.Multiply(p2);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberDataTuple(nameof(PointToIntPairs))]
        public void MultiplyInt(Point p1, int i)
        {
            var expected = p1 * i;
            var actual = p1.Multiply(i);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberDataTuple(nameof(PointToDoublePairs))]
        public void MultiplyDouble(Point p1, double d)
        {
            var expected = p1 * d;
            var actual = p1.Multiply(d);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Divide
        [Theory]
        [MemberDataTuple(nameof(PointToPointPairs))]
        public void DividePoint(Point p1, Point p2)
        {
            var expected = p1 / p2;
            var actual = p1.Divide(p2);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberDataTuple(nameof(PointToIntPairs))]
        public void DivideInt(Point p1, int i)
        {
            var expected = p1 / i;
            var actual = p1.Divide(i);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberDataTuple(nameof(PointToDoublePairs))]
        public void DivideDouble(Point p1, double d)
        {
            var expected = p1 / d;
            var actual = p1.Divide(d);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Comparison
        [Theory]
        [MemberDataTuple(nameof(PointToPointPairs))]
        public void Matches(Point p1, Point p2)
        {
            Assert.Equal(p1.Matches(p2), PointExtensions.Matches(p1, p2));
        }
        #endregion
    }
}
