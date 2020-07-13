using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class PolarCoordinateTests
    {
        private readonly ITestOutputHelper _output;
        public static readonly (string, Func<double, Point>)[] PolarFuncts = PolarCoordinate.Functions.Select(pair => (pair.Key, pair.Value)).ToArray();

        public static (Point, PolarCoordinate)[] TestData =
        {
            //lovingly picked at random and calculated online
            (new Point(5,5), new PolarCoordinate(7.0710678118654755f, 0.785398163397)),
            (new Point(-5, -2), new PolarCoordinate(5.385164, -2.761086)),
            (new Point(3,4), new PolarCoordinate(5, 0.9273) ),
            (new Point(15, 49), new PolarCoordinate(51.244511, 1.273732)),
            (new Point(99, 185), new PolarCoordinate(209.82374, 1.07943726)),
            (new Point(-100, 100), new PolarCoordinate(141.42136, 2.35619)),
            (new Point(100, -10), new PolarCoordinate(100.49876, -0.09966865)),
        };

        public PolarCoordinateTests(ITestOutputHelper helper)
        {
            _output = helper;
        }

        [Fact]
        //[Theory(Skip = "For manual checking")]
        //[MemberDataTuple(nameof(PolarFuncts))]
        //public void PrintPolarFunctionsTest(string name, Func<double, Point> f)
        public void PrintPolarFunctionsTest()
        {
            int size = 500;
            double resolution = 0.01;
            foreach ((string, Func<double, Point>) pfunc in PolarFuncts)
            {
                bool[,] map = new bool[size, size];
                for (double x = -size / 2.0; x < size / 2.0; x += resolution)
                {
                    Point here = pfunc.Item2(x) + size / 2;
                    if (here.X < size && here.X >= 0 && here.Y < size && here.Y >= 0)
                    {
                        map[here.X, here.Y] = true;
                    }
                }

                _output.WriteLine(pfunc.Item1);
                for (int i = 0; i < size; i++)
                {
                    string line = "";
                    for (int j = 0; j < size; j++)
                    {
                        if (map[i, j] == false)
                            line += " ";
                        else
                            line += "*";
                    }

                    _output.WriteLine(line);
                }
            }
        }


        [Theory]
        [MemberDataTuple(nameof(TestData))]
        public void PolarToCartesianTest(Point cartesian, PolarCoordinate polar) => Assert.Equal(cartesian, polar.ToCartesian());

        [Theory]
        [MemberDataTuple(nameof(TestData))]
        public void CartesianToPolarTest(Point cartesian, PolarCoordinate polar) => Assert.Equal(polar, PolarCoordinate.FromCartesian(cartesian));
    }
}
