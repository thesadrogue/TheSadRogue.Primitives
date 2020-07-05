using System;
using Xunit;

namespace SadRogue.Primitives.UnitTests
{
    public class PolarCoordinateTests
    {

        [Fact]
        public void PolarToCartesianTest()
        {
            PolarCoordinate origin = new PolarCoordinate(3.7416573867739413d, 0.64209261593433065d);
            Point target = PolarCoordinate.PolarToCartesian(origin);
            Assert.Equal(new Point(3, 2), target);
        }
        [Fact]
        public void CartesianToPolarTest()
        {
            Point origin = new Point(5, 5);
            PolarCoordinate target = PolarCoordinate.FromCartesian(origin);
            double expectedRadius = Math.Sqrt(50);
            double expectedTheta = Math.Atan(1);


            Assert.True(expectedTheta - 0.01 < target.Theta);
            Assert.False(expectedTheta + 0.01 < target.Theta);
            Assert.True(expectedRadius - 0.01 < target.Radius);
            Assert.False(expectedRadius + 0.01 < target.Radius);
        }
        [Fact]
        public void FromCartesianTest()
        {
            Point origin = new Point(5,5);
            PolarCoordinate target = PolarCoordinate.FromCartesian(origin);
            PolarCoordinate expected = new PolarCoordinate(7.0710678118654755f, 0.785398163397);
            Point cartTarget = target.ToCartesian();
            Point expectedPoint = expected.ToCartesian();

            Assert.Equal(expected, target);
            Assert.Equal(cartTarget, expectedPoint);
        }
        [Fact]
        public void ToCartesianTest()
        {
            PolarCoordinate o = new PolarCoordinate(7.0710678118654755f, 0.785398163397);
            Point p = new Point(5,5);
            Assert.Equal(o.ToCartesian(), p);
        }
        [Fact]
        public void ToPolarCoordinateTest()
        {
            Point p = new Point(5, 5);
            PolarCoordinate origin = new PolarCoordinate(7.0710678118654755f, 0.785398163397);
            Assert.Equal(origin, p.ToPolarCoordinate());
        }
    }
}
