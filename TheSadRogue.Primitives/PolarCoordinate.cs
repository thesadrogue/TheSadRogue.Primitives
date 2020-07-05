using System;
using System.Collections.Generic;

namespace SadRogue.Primitives
{
    /// <summary>
    /// A Polar Coordinate.
    /// </summary>
    /// <remarks>
    /// See wikipedia: https://en.wikipedia.org/wiki/Polar_coordinate_system
    /// Polar Coordinates are very, very slow and should not be used often
    /// </remarks>
    public class PolarCoordinate
    {
        /// <summary>
        /// The distance away from the Origin (0,0) of this Polar Coord
        /// </summary>
        public readonly double Radius;

        /// <summary>
        /// The degree of rotation, clockwise
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

        public static bool operator ==(PolarCoordinate left, PolarCoordinate right)
        {
            if (left.Theta > right.Theta - 0.05 && left.Theta < right.Theta + 0.05)
                if (left.Radius > right.Radius - 0.05 && left.Radius < right.Radius + 0.05)
                    return true;

            return false;
        }

        public static bool operator !=(PolarCoordinate left, PolarCoordinate right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(PolarCoordinate))
                return false;
            else
                return this == (PolarCoordinate)obj;
        }

        /// <summary>
        /// A handly list of polar functions for you.
        /// </summary>
        /// <remarks>
        /// Each function accepts a parameter of Theta, which is the angle of rotation around the center.
        /// They return a Point that can be used on the map.
        /// Polar functions are very slow. Use them seldom, for things like spirographs
        /// As the radius gets large, there will be increased likelihood
        /// of functions producing gaps in the output.
        /// You've been warned.
        /// </remarks>
        public static List<Func<double, Point>> PolarFunctions => new List<Func<double, Point>>
        {
            //example spirograph
            (theta) =>
            {
                //a spirograph is a polar coordinate with a static radius,
                //and a subradius that generates the real point
                double masterRadius = 20.5; //arbitrary, unchanging
                double innerRadius = 5 * Math.Cos(theta); //fluctuates
                return PolarToCartesian(new PolarCoordinate(innerRadius, 5 * theta / 2));
            },

            //spiral
            (theta) =>
            {
                double radius = theta * theta;
                return PolarToCartesian(new PolarCoordinate(radius, theta));
            },

            //the butterfly curve
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
            },
        };

        public Point ToCartesian()
        {
            double x = Radius * Math.Cos(Theta);
            double y = Radius * Math.Sin(Theta);
            return new Point((int)Math.Round(x, 0), (int)Math.Round(y, 0));
        }
        public static Point PolarToCartesian(double radius, double theta)
        {
            double x = radius * Math.Cos(theta);
            double y = radius * Math.Sin(theta);
            return new Point((int)Math.Round(x, 0), (int)Math.Round(y, 0));
        }
        public static Point PolarToCartesian(PolarCoordinate pc)
        {
            return PolarToCartesian(pc.Radius, pc.Theta);
        }
        public static PolarCoordinate FromCartesian(Point c)
        {
            float radius = c.X * c.X + c.Y * c.Y;
            radius = (float)Math.Sqrt(radius);

            //tan(theta) = c.y / c.x
            //theta = tan^-1(c.y / c.x)
            float theta = c.X == 0 ? (float)Math.PI : (float)Math.Atan(c.Y / c.X);
            return new PolarCoordinate(radius, theta);
        }
    }
}
