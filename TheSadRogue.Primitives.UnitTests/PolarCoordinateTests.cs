﻿using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class PolarCoordinateTests
    {
        private readonly ITestOutputHelper _output;

        #region Test Data

        private static readonly (string, Func<double, Point>)[] s_polarFuncs = PolarCoordinate.Functions.Select(pair => (pair.Key, pair.Value)).ToArray();

        public static (Point, PolarCoordinate)[] PolarCartesianConversionTestData =
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

        private static readonly PolarCoordinate s_equalPolar = new PolarCoordinate(4.1, -3.2);
        public static readonly PolarCoordinate[] TestCoordinates =
        {
            new PolarCoordinate(s_equalPolar.Radius, s_equalPolar.Theta),
            new PolarCoordinate(4.7, 5.1),
            new PolarCoordinate(1.8, -3.5),
            new PolarCoordinate(2.4, 4.5)
        };
        #endregion

        public PolarCoordinateTests(ITestOutputHelper helper)
        {
            _output = helper;
        }

        #region Manual Helper Tests
        [Fact]
        //[Theory(Skip = "For manual checking")]
        //[MemberDataTuple(nameof(PolarFuncs))]
        //public void PrintPolarFunctionsTest(string name, Func<double, Point> f)
        public void PrintPolarFunctionsTest()
        {
            int size = 500;
            double resolution = 0.01;
            foreach ((string, Func<double, Point>) polarFunc in s_polarFuncs)
            {
                bool[,] map = new bool[size, size];
                for (double x = -size / 2.0; x < size / 2.0; x += resolution)
                {
                    Point here = polarFunc.Item2(x) + size / 2;
                    if (here.X < size && here.X >= 0 && here.Y < size && here.Y >= 0)
                    {
                        map[here.X, here.Y] = true;
                    }
                }

                _output.WriteLine(polarFunc.Item1);
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
        #endregion

        #region Polar/Cartesian Conversion

        [Theory]
        [MemberDataTuple(nameof(PolarCartesianConversionTestData))]
        public void PolarToCartesianTest(Point cartesian, PolarCoordinate polar)
        {
            Assert.Equal(cartesian, polar.ToCartesian());
            Assert.Equal(cartesian, (Point)polar);
        }

        [Theory]
        [MemberDataTuple(nameof(PolarCartesianConversionTestData))]
        public void CartesianToPolarTest(Point cartesian, PolarCoordinate polar)
        {
            Assert.Equal(polar, PolarCoordinate.FromCartesian(cartesian));
            Assert.Equal(polar, (PolarCoordinate)cartesian);
            Assert.Equal(polar, cartesian.ToPolarCoordinate());
            Assert.Equal(polar, PolarCoordinate.FromCartesian(cartesian.X, cartesian.Y));
        }
        #endregion

        #region Equality
        [Fact]
        public void TestEquality()
        {
            Assert.True(s_equalPolar.Equals(TestCoordinates[0]));
            Assert.True(s_equalPolar.Matches(TestCoordinates[0]));
            Assert.True(s_equalPolar.Equals((object)TestCoordinates[0]));
            Assert.True(s_equalPolar == TestCoordinates[0]);

            // Compare to tuple
            (double radius, double delta) tuple = (s_equalPolar.Radius, s_equalPolar.Theta);
            Assert.True(s_equalPolar.Matches(tuple));
            Assert.True(s_equalPolar.Equals(tuple));
        }

        [Fact]
        public void TestEqualityWrongTypeOrNull()
        {
            // Intentional to test the equality operator
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(s_equalPolar.Equals(1));
            Assert.False(s_equalPolar.Equals(null));
        }

        [Theory]
        [MemberDataEnumerable(nameof(TestCoordinates))]
        public void TestEqualityInequalityRelationship(PolarCoordinate testCoordinate)
        {
            (double radius, double delta) tuple = (s_equalPolar.Radius, s_equalPolar.Theta);

            Assert.Single(TestCoordinates.Where(i => i.Equals(testCoordinate)));
            Assert.Single(TestCoordinates.Where(i => i.Matches(testCoordinate)));
            Assert.Single(TestCoordinates.Where(i => i.Equals((object)testCoordinate)));
            Assert.Single(TestCoordinates.Where(i => i == testCoordinate));
            Assert.Single(TestCoordinates.Where(i => i.Equals(tuple)));
            Assert.Single(TestCoordinates.Where(i => i.Matches(tuple)));
            Assert.Single(TestCoordinates.Where(i => i == tuple));

            // Test equality and inequality relationship, also across operators (lhs and rhs types)
            foreach (var other in TestCoordinates)
            {
                Assert.Equal(!(testCoordinate == other), other != testCoordinate);
                Assert.Equal(!(testCoordinate == tuple), tuple != testCoordinate);

                Assert.Equal(!(other == testCoordinate), testCoordinate != other);
                Assert.Equal(!(tuple == testCoordinate), testCoordinate != tuple);

            }
        }

        [Theory]
        [MemberDataEnumerable(nameof(TestCoordinates))]
        public void TestGetHashCode(PolarCoordinate testCoordinate)
        {
            foreach (var other in TestCoordinates)
                if (testCoordinate.Equals(other))
                    Assert.Equal(testCoordinate.GetHashCode(), other.GetHashCode());
        }
        #endregion

        #region Tuple Compatibility

        [Theory]
        [MemberDataEnumerable(nameof(TestCoordinates))]
        public void TestTupleConversions(PolarCoordinate coordinate)
        {
            // Convert to tuple
            (double radius, double theta) tuple = coordinate;

            Assert.Equal(coordinate.Radius, tuple.radius);
            Assert.Equal(coordinate.Theta, tuple.theta);

            // Convert back
            PolarCoordinate coordinate2 = tuple;
            Assert.Equal(coordinate, coordinate2);
        }

        [Theory]
        [MemberDataEnumerable(nameof(TestCoordinates))]
        public void TestDeconstruction(PolarCoordinate coordinate)
        {
            (double radius, double theta) = coordinate;

            Assert.Equal(coordinate.Radius, radius);
            Assert.Equal(coordinate.Theta, theta);
        }
        #endregion
    }
}
