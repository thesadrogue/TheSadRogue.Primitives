using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace SadRogue.Primitives
{
    /// <summary>
    /// A Polar Coordinate.
    /// </summary>
    /// <remarks>
    /// See wikipedia: https://en.wikipedia.org/wiki/Polar_coordinate_system
    /// Polar Coordinates are very, very slow and should not be used often
    /// </remarks>
    [DataContract]
    public readonly struct PolarCoordinate : IEquatable<PolarCoordinate>, IMatchable<PolarCoordinate>,
                                             IEquatable<(double radius, double theta)>, IMatchable<(double radius, double theta)>
    {
        /// <summary>
        /// The distance away from the Origin (0,0) of this Polar Coord
        /// </summary>
        [DataMember] public readonly double Radius;

        /// <summary>
        /// The angle of rotation, clockwise, in radians
        /// </summary>
        [DataMember] public readonly double Theta; //in degrees clockwise

        /// <summary>
        /// Creates a new Polar Coordinate with the given Radius and Theta
        /// </summary>
        /// <param name="radius">Radius of the Polar Coord</param>
        /// <param name="theta">Degree of rotation (clockwise) of the Polar Coord</param>
        public PolarCoordinate(double radius, double theta)
        {
            Radius = radius;
            Theta = theta;
        }

        /// <summary>
        /// Ovverride ToString to help you debug
        /// </summary>
        /// <returns>A short representation of the Polar Coordinate</returns>
        public override string ToString()
        {
            return "(Radius = " + Math.Round(Radius, 5) + ", Theta = " + Math.Round(Theta, 5) + ")";
        }

        /// <summary>
        /// Compares two polar Coordinates
        /// </summary>
        /// <param name="left">The first polar coordinate to analyze</param>
        /// <param name="right">The Second polar coordinate to analyze</param>
        /// <returns>Whether or not these two Polar Coordinates are similar enough to be considered "Equal"</returns>
        [Pure]
        public static bool operator ==(PolarCoordinate left, PolarCoordinate right) =>
            Math.Round(left.Theta - right.Theta, 5) == 0.0 && Math.Round(left.Radius - right.Radius, 5) == 0.0;

        /// <summary>
        /// Compares two PolarCoordinates
        /// </summary>
        /// <param name="left">The first polar coordinate to analyze</param>
        /// <param name="right">The second PolarCoordinate to analyze</param>
        /// <returns>Whether these polar coordinates are dissimilar enough to not be equal</returns>
        public static bool operator !=(PolarCoordinate left, PolarCoordinate right) => !(left == right);

        /// <summary>
        /// Gets the Object's Hash Code
        /// </summary>
        /// <returns>A Hash code.</returns>
        public override int GetHashCode() => Math.Round(Radius, 5).GetHashCode() ^ Math.Round(Theta, 5).GetHashCode();

        /// <summary>
        /// Performs comparison between this PolarCoordinate and an other
        /// </summary>
        /// <param name="other">The other Polar Coordinate to analyze</param>
        /// <returns>Whether these two PolarCoordinates are equal</returns>
        [Pure]
        public bool Equals(PolarCoordinate other) => this == other;

        /// <summary>
        /// Compares the equality of this Polar Coordinate to another object
        /// </summary>
        /// <param name="obj">The object against which to compare</param>
        /// <returns>Whether or not these objects are equal</returns>
        [Pure]
        public override bool Equals(object? obj)
        {
            if (obj is PolarCoordinate polar)
                return this == polar;
            return false;
        }

        /// <summary>
        /// Performs comparison between this PolarCoordinate and an other
        /// </summary>
        /// <param name="other">The other Polar Coordinate to analyze</param>
        /// <returns>Whether these two PolarCoordinates are equal</returns>
        [Pure]
        public bool Matches(PolarCoordinate other) => Equals(other);

        /// <summary>
        /// Implicitly converts the polar coordinate to its cartesian plane equivalent.
        /// </summary>
        /// <param name="pos">Polar coordinate to convert.</param>
        /// <returns>The equivalent cartesian coordinate.</returns>
        public static implicit operator Point(PolarCoordinate pos)
            => pos.ToCartesian();

        /// <summary>
        /// Returns the Cartesian Equivalent of this Polar Coordinate
        /// </summary>
        /// <returns>A Cartesian Coordinate that points at the same spot on the map as the Polar Coord</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point ToCartesian()
            => new Point((int)Math.Round(Radius * Math.Cos(Theta), 0), (int)Math.Round(Radius * Math.Sin(Theta), 0));

        /// <summary>
        /// Returns a new PolarCoordinate that is equivalent to the Cartesian point provided.
        /// </summary>
        /// <param name="cartesian">The cartesian point to analyze</param>
        /// <returns>An Equivalent Polar Coordinate</returns>
        [Pure]
        public static PolarCoordinate FromCartesian(Point cartesian)
        {
            double radius = Math.Sqrt(cartesian.X * cartesian.X + cartesian.Y * cartesian.Y);
            double theta = Math.Atan2(cartesian.Y, cartesian.X);
            return new PolarCoordinate(radius, theta);
        }

        /// <summary>
        /// Returns a new PolarCoordinate that is equivalent to the Cartesian point provided.
        /// </summary>
        /// <param name="cartesianX">X-value of the cartesian point to analyze.</param>
        /// <param name="cartesianY">Y-value of the cartesian point to analyze.</param>
        /// <returns>An Equivalent Polar Coordinate</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PolarCoordinate FromCartesian(int cartesianX, int cartesianY)
            => FromCartesian(new Point(cartesianX, cartesianY));

        #region Tuple Compatibility

        /// <summary>
        /// Adds support for C# Deconstruction syntax.
        /// </summary>
        /// <param name="radius" />
        /// <param name="theta" />
        [Pure]
        public void Deconstruct(out double radius, out double theta)
        {
            radius = Radius;
            theta = Theta;
        }

        /// <summary>
        /// Implicitly converts a PolarCoordinate to an equivalent tuple of two doubles.
        /// </summary>
        /// <param name="c" />
        /// <returns />
        [Pure]
        public static implicit operator (double radius, double theta)(PolarCoordinate c) => (c.Radius, c.Theta);

        /// <summary>
        /// Implicitly converts a tuple of two doubles to an equivalent PolarCoordinate.
        /// </summary>
        /// <param name="tuple" />
        /// <returns />
        [Pure]
        public static implicit operator PolarCoordinate((double radius, double theta) tuple)
            => new PolarCoordinate(tuple.radius, tuple.theta);

        /// <summary>
        /// Compares the values in this polar coordinate to the specified values.
        /// </summary>
        /// <param name="other"/>
        /// <returns/>
        public bool Equals((double radius, double theta) other)
            => Math.Round(Theta - other.theta, 5) == 0.0 && Math.Round(Radius - other.radius, 5) == 0.0;

        /// <summary>
        /// Compares the values in this polar coordinate to the specified values.
        /// </summary>
        /// <param name="other"/>
        /// <returns/>
        public bool Matches((double radius, double theta) other) => Equals(other);

        /// <summary>
        /// True if the two polar coordinates' radius and theta values are close enough to equal.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="tuple"></param>
        /// <returns>True if the two polar coordinates are equivalent, false if not.</returns>
        [Pure]
        public static bool operator ==(PolarCoordinate c, (double radius, double theta) tuple) => c.Equals(tuple);

        /// <summary>
        /// True if either the radius or theta values aren't equivalent.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="tuple"></param>
        /// <returns>
        /// True if either the radius or theta values are not equal, false if they are both equal.
        /// </returns>
        [Pure]
        public static bool operator !=(PolarCoordinate c, (double radius, double theta) tuple)
            => !(c == tuple);

        /// <summary>
        /// True if the two polar coordinate's values are equivalent.
        /// </summary>
        /// <param name="tuple"></param>
        /// <param name="c"></param>
        /// <returns>True if the two polar coordinates are equal, false if not.</returns>
        [Pure]
        public static bool operator ==((double radius, double theta) tuple, PolarCoordinate c)
            => c.Equals(tuple);

        /// <summary>
        /// True if either the radius or theta values are not equal.
        /// </summary>
        /// <param name="tuple"></param>
        /// <param name="c"></param>
        /// <returns>
        /// True if either the radius or theta values are not equal, false if they are both equal.
        /// </returns>
        [Pure]
        public static bool operator !=((double radius, double theta) tuple, PolarCoordinate c)
            => !(tuple == c);

        #endregion

        /// <summary>
        /// A handy list of example polar functions for you.
        /// </summary>
        /// <remarks>
        /// This list contains several examples of Polar Coordinates you can use.
        /// Add more by calling `PolarFunctions.Add((theta) => { Return new PolarCoordinate(radius, theta);});`
        ///
        /// Each function accepts a parameter of Theta, which is the angle of rotation around the center.
        /// They return a Point that can be used on the map.
        /// Polar functions are very slow. Use them seldom, for things like spirographs
        /// As the radius gets larger, there will be increased likelihood of functions producing gaps in output.
        /// You've been warned.
        /// </remarks>
        [Pure]
        public static Dictionary<string, Func<double, Point>> Functions => new Dictionary<string, Func<double, Point>>()
        {
            {
                "small spirograph",
                (theta) =>
                {
                    //a spirograph is a polar coordinate with an outer radius,
                    //and an inner radius whose theta grows at a different rate
                    double outerRadius = 20.5; //arbitrary
                    double innerRadius = 5; //arbitrary

                    PolarCoordinate outerPoint = new PolarCoordinate(outerRadius, theta);
                    PolarCoordinate innerPoint = new PolarCoordinate(innerRadius, 9 * theta / 4);
                    return outerPoint.ToCartesian() + innerPoint.ToCartesian();
                }
            },
            {
                "lopsided spirograph", (theta) =>
                {
                    //these two thetas will sync up occasionally, causing the spirograph to be "lopsided"
                    double outerRadius = 27 + (5 * Math.Sin(theta * 10));
                    double innerRadius = 8 + (5 * Math.Cos(theta / 10));

                    PolarCoordinate parent = new PolarCoordinate(outerRadius, theta);
                    PolarCoordinate child = new PolarCoordinate(innerRadius, theta * 10);
                    return parent.ToCartesian() + child.ToCartesian();
                }
            },
            {
                "medium spirograph", (theta) =>
                {
                    double outerRadius = 27;
                    double innerRadius = 20 * Math.Sin(0.333 * theta);
                    PolarCoordinate parent = new PolarCoordinate(outerRadius, theta);
                    PolarCoordinate child = new PolarCoordinate(innerRadius, theta * 10);
                    return parent.ToCartesian() + child.ToCartesian();
                }
            },
            {
                "large spirograph",
                (theta) =>
                {
                    double outerRadius = 35 + (8 * Math.Sin(theta / 7));
                    double innerRadius = 10 + (7 * Math.Cos(theta * 3));

                    PolarCoordinate parent = new PolarCoordinate(outerRadius, theta);
                    PolarCoordinate child = new PolarCoordinate(innerRadius, theta * 10);
                    return parent.ToCartesian() + child.ToCartesian();
                }
            },
            {
                "oblique spirograph",
                (theta) =>
                {
                    double outerRadius = 30 + (7 * Math.Sin(theta / 7));
                    double innerRadius = 7 + (3 * Math.Cos(theta * 7));

                    PolarCoordinate parent = new PolarCoordinate(outerRadius, theta);
                    PolarCoordinate child = new PolarCoordinate(innerRadius, theta * 10);
                    return parent.ToCartesian() + child.ToCartesian();
                }
            },
            {
                "simple spiral",
                (theta) =>
                {
                    double radius = theta * theta;
                    return new PolarCoordinate(radius, Math.Abs(theta)).ToCartesian();
                }
            },
            {
                "butterfly curve",
                (theta) =>
                {
                    double radius = Math.Exp(Math.Sin(theta));
                    radius -= 2 * Math.Cos(theta * 4);

                    double powerBase = 2 * theta - Math.PI;
                    powerBase /= 24;

                    radius += Math.Pow(powerBase, 5);
                    double x = radius * Math.Cos(theta);
                    double y = radius * Math.Sin(theta);
                    return new Point((int)Math.Round(x, 0), (int)Math.Round(y, 0));
                }
            },
        };
    }
}
