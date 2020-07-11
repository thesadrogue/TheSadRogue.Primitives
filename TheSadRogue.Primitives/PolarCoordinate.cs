using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace SadRogue.Primitives
{
    /// <summary>
    /// A Polar Coordinate.
    /// </summary>
    /// <remarks>
    /// See wikipedia: https://en.wikipedia.org/wiki/Polar_coordinate_system
    /// Polar Coordinates are very, very slow and should not be used often
    /// </remarks>
    public struct PolarCoordinate : IEquatable<PolarCoordinate>
    {
        /// <summary>
        /// The distance away from the Origin (0,0) of this Polar Coord
        /// </summary>
        public readonly double Radius;

        /// <summary>
        /// The angle of rotation, clockwise, in radians
        /// </summary>
        public readonly double Theta; //in degrees clockwise

        /// <summary>
        /// Creates a new Polaar Coordinate with the given Radius and Theta
        /// </summary>
        /// <param name="radius">Radius of the Polar Coord</param>
        /// <param name="theta">Degree of rotation (clockwise) of the Polar Coord</param>
        public PolarCoordinate(double radius, double theta)
        {
            Radius = radius;
            Theta = theta;
        }

        /// <summary>
        /// Compares two polar Coordinates
        /// </summary>
        /// <param name="left">The first polar coordinate to analyze</param>
        /// <param name="right">The Second polar coordinate to analyze</param>
        /// <returns>Whether or not these two Polar Coodinates are similar enough to be considered "Equal"</returns>
        [Pure]
        public static bool operator ==(PolarCoordinate left, PolarCoordinate right)
        {
            if (left.Theta > right.Theta - 0.05 && left.Theta < right.Theta + 0.05)
                if (left.Radius > right.Radius - 0.05 && left.Radius < right.Radius + 0.05)
                    return true;

            return false;
        }

        /// <summary>
        /// Compares two PolarCoordinates
        /// </summary>
        /// <param name="left">The first polar coordinate to analyze</param>
        /// <param name="right">The second PolarCoordinate to analyze</param>
        /// <returns>Whether these polar coordinates are dissimilar enough to not be equal</returns>
        public static bool operator !=(PolarCoordinate left, PolarCoordinate right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Performs  comparison between this PolarCoordinate and an other
        /// </summary>
        /// <param name="other">The other Polar Coordinate to analyze</param>
        /// <returns>Whether these two PolarCoordinates are equal</returns>
        [Pure]
        public bool Equals(PolarCoordinate other) => Equals((object) other);
        [Pure]
        public override bool Equals(object obj)
        {
            if (obj is PolarCoordinate polar)
                return this == polar;
            return false;
        }

        /// <summary>
        /// Returns the Cartesian Equivalent of this Polar Coordinate
        /// </summary>
        /// <returns>A Cartesian Coordinate that points at the same spot on the map as the Polar Coord</returns>
        [Pure]
        public Point ToCartesian() => PolarToCartesian(Radius, Theta);

        /// <summary>
        /// Returns the Cartesian Equivalent of this Polar Coordinate
        /// </summary>
        /// <returns>A Cartesian Coordinate that points at the same spot on the map as the Polar Coord</returns>
        [Pure]
        public static Point PolarToCartesian(double radius, double theta)
        {
            double x = radius * Math.Cos(theta);
            double y = radius * Math.Sin(theta);
            return new Point((int)Math.Round(x, 0), (int)Math.Round(y, 0));
        }


        /// <summary>
        /// Returns the Polar Equivalent of this Cartesian Coordinate
        /// </summary>
        /// <returns>A Polar Coordinate that points at the same spot on the map as this Cartesian Coord</returns>
        public static Point PolarToCartesian(PolarCoordinate pc)
        {
            return pc.ToCartesian();
        }

        /// <summary>
        /// Returns a new PolarCoordinate that is equivalent to the Cartesian point provided.
        /// </summary>
        /// <param name="c">The cartesian point to analyze</param>
        /// <returns>An Equivalent Polar Coordinate</returns>
        public static PolarCoordinate FromCartesian(Point c)
        {
            float radius = c.X * c.X + c.Y * c.Y;
            radius = (float)Math.Sqrt(radius);

            //tan(theta) = c.y / c.x
            //theta = tan^-1(c.y / c.x)
            float theta = c.X == 0 ? (float)Math.PI : (float)Math.Atan(c.Y / c.X);
            return new PolarCoordinate(radius, theta);
        }

        /// <summary>
        /// A handly list of example polar functions for you.
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
        public static Dictionary<string, Func<double, Point>> PolarFunctions => new Dictionary<string, Func<double, Point>>()
        {
            {
                "simple spirograph",
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
                "simple spiral",
                (theta) =>
                {
                    double radius = theta * theta;
                    return PolarToCartesian(new PolarCoordinate(radius, theta));
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
