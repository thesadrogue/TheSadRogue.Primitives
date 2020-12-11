using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace SadRogue.Primitives
{
    /// <summary>
    /// A structure that represents a standard 2D point.  Provides numerous functions and operators that enable
    /// common grid/position-related math and operations.
    /// </summary>
    /// <remarks>
    /// Point instances can be created using the standard Point c = new Point(x, y) syntax.  In addition,
    /// you may create a coord from a c# 7 tuple, like Point c = (x, y);.  As well, Point supports C#
    /// Deconstruction syntax.
    ///
    /// Point also provides operators and static helper functions that perform common grid math/operations,
    /// as well as interoperability with other grid-based classes like <see cref="Direction"/>.
    /// </remarks>
    [DataContract]
    public readonly struct Point : IEquatable<Point>, IEquatable<(int x, int y)>, IMatchable<Point>,
                                   IMatchable<(int x, int y)>
    {
        /// <summary>
        /// Point value that represents None or no position (since Point is not a nullable type).
        /// Typically you would use this constant instead of null.
        /// </summary>
        /// <remarks>
        /// This constant has (x, y) values (int.MinValue, int.MinValue), so a position with those
        /// x/y values is not considered a valid coordinate by many functions.
        /// </remarks>
        public static readonly Point None = new Point(int.MinValue, int.MinValue);

        /// <summary>
        /// X-value of the position.
        /// </summary>
        [DataMember] public readonly int X;

        /// <summary>
        /// Y-value of the position.
        /// </summary>
        [DataMember] public readonly int Y;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X-value for the coordinate.</param>
        /// <param name="y">Y-value for the coordinate.</param>
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Calculates degree bearing of the line (start =&gt; end), where 0 points in the direction <see cref="Direction.Up"/>.
        /// </summary>
        /// <param name="start">Position of line starting point.</param>
        /// <param name="end">Position of line ending point.</param>
        /// <returns>The degree bearing of the line specified by the two given points.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double BearingOfLine(Point start, Point end) => BearingOfLine(start - end);

        /// <summary>
        /// Calculates degree bearing of the line (start =&gt; end), where 0 points in the direction <see cref="Direction.Up"/>.
        /// </summary>
        /// <param name="startX">X-value of the position of line starting point.</param>
        /// <param name="startY">Y-value of the position of line starting point.</param>
        /// <param name="endX">X-value of the position of line ending point.</param>
        /// <param name="endY">X-value of the position of line ending point.</param>
        /// <returns>The degree bearing of the line specified by the two given points.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double BearingOfLine(int startX, int startY, int endX, int endY)
            => BearingOfLine(new Point(startX, startY), new Point(endX, endY));


        /// <summary>
        /// Calculates the degree bearing of a line with the given delta-x and delta-y values, where
        /// 0 degrees points in the direction <see cref="Direction.Up"/>.
        /// </summary>
        /// <param name="deltaChange">
        /// Vector, where deltaChange.X is the change in x-values across the line, and deltaChange.Y
        /// is the change in y-values across the line.
        /// </param>
        /// <returns>The degree bearing of the line with the given dx and dy values.</returns>
        [Pure]
        public static double BearingOfLine(Point deltaChange)
        {
            int dx = deltaChange.X;
            int dy = deltaChange.Y;

            dy *= Direction.s_yMult;
            double angle = Math.Atan2(dy, dx);
            double degree = MathHelpers.ToDegree(angle);
            degree += 450; // Rotate to all positive such that 0 is up
            degree %= 360; // Normalize
            return degree;
        }

        /// <summary>
        /// Calculates the degree bearing of a line with the given delta-x and delta-y values, where
        /// 0 degrees points in the direction <see cref="Direction.Up"/>.
        /// </summary>
        /// <param name="dx">Change in x-value across the line.</param>
        /// <param name="dy">Change in y-value across the line.</param>
        /// <returns>The degree bearing of the line with the given dx and dy values.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double BearingOfLine(int dx, int dy)
            => BearingOfLine(new Point(dx, dy));

        /// <summary>
        /// Returns the result of the euclidean distance formula, without the square root -- eg.,
        /// (c2.X - c1.X) * (c2.X - c1.X) + (c2.Y - c1.Y) * (c2.Y - c1.Y). Use this if you only care
        /// about the magnitude of the distance -- eg., if you're trying to compare two distances.
        /// Omitting the square root provides a speed increase.
        /// </summary>
        /// <param name="c1">The first point.</param>
        /// <param name="c2">The second point.</param>
        /// <returns>
        /// The "magnitude" of the euclidean distance between the two points -- basically the
        /// distance formula without the square root.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double EuclideanDistanceMagnitude(Point c1, Point c2) => EuclideanDistanceMagnitude(c2 - c1);

        /// <summary>
        /// Returns the result of the euclidean distance formula, without the square root -- eg.,
        /// (c2.X - c1.X) * (c2.X - c1.X) + (c2.Y - c1.Y) * (c2.Y - c1.Y). Use this if you only care
        /// about the magnitude of the distance -- eg., if you're trying to compare two distances.
        /// Omitting the square root provides a speed increase.
        /// </summary>
        /// <param name="firstX">X-value for the first point.</param>
        /// <param name="firstY">Y-value for the first point.</param>
        /// <param name="secondX">X-value for the second point.</param>
        /// <param name="secondY">Y-value for the second point.</param>
        /// <returns>
        /// The "magnitude" of the euclidean distance between the two points -- basically the
        /// distance formula without the square root.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double EuclideanDistanceMagnitude(int firstX, int firstY, int secondX, int secondY)
            => EuclideanDistanceMagnitude(new Point(firstX, firstY), new Point(secondX, secondY));

        /// <summary>
        /// Returns the result of the euclidean distance formula, without the square root, given the
        /// dx and dy values between two points -- eg., (deltaChange.X * deltaChange.X) + (deltaChange.Y
        /// * deltaChange.Y). Use this if you only care about the magnitude of the distance -- eg., if
        /// you're trying to compare two distances. Omitting the square root provides a speed increase.
        /// </summary>
        /// <param name="deltaChange">
        /// Vector, where deltaChange.X is the change in x-values between the two points, and
        /// deltaChange.Y is the change in y-values between the two points.
        /// </param>
        /// <returns>
        /// The "magnitude" of the euclidean distance of two locations with the given dx and dy
        /// values -- basically the distance formula without the square root.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double EuclideanDistanceMagnitude(Point deltaChange)
            => deltaChange.X * deltaChange.X + deltaChange.Y * deltaChange.Y;

        /// <summary>
        /// Returns the result of the euclidean distance formula, without the square root, given the
        /// dx and dy values between two points -- eg., (deltaChange.X * deltaChange.X) + (deltaChange.Y
        /// * deltaChange.Y). Use this if you only care about the magnitude of the distance -- eg., if
        /// you're trying to compare two distances. Omitting the square root provides a speed increase.
        /// </summary>
        /// <param name="dx">Change in x-values between the two points.</param>
        /// <param name="dy">Change in y-values between the two points.</param>
        /// <returns>
        /// The "magnitude" of the euclidean distance of two locations with the given dx and dy
        /// values -- basically the distance formula without the square root.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double EuclideanDistanceMagnitude(int dx, int dy)
            => EuclideanDistanceMagnitude(new Point(dx, dy));

        /// <summary>
        /// True if the given coordinate has equal x and y values to the current one.
        /// </summary>
        /// <param name="other">Position to compare.</param>
        /// <returns>True if the two positions are equal, false if not.</returns>
        [Pure]
        public bool Matches(Point other) => Equals(other);

        /// <summary>
        /// Returns the midpoint between the two points.
        /// </summary>
        /// <param name="c1">The first point.</param>
        /// <param name="c2">The second point.</param>
        /// <returns>The midpoint between <paramref name="c1"/> and <paramref name="c2"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Midpoint(Point c1, Point c2)
            => new Point((int)Math.Round((c1.X + c2.X) / 2.0f, MidpointRounding.AwayFromZero),
                (int)Math.Round((c1.Y + c2.Y) / 2.0f, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Returns the midpoint between the two points.
        /// </summary>
        /// <param name="firstX">X-value for the first point.</param>
        /// <param name="firstY">Y-value for the first point.</param>
        /// <param name="secondX">X-value for the second point.</param>
        /// <param name="secondY">Y-value for the second point.</param>
        /// <returns>The midpoint between the two points.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Midpoint(int firstX, int firstY, int secondX, int secondY)
            => Midpoint(new Point(firstX, firstY), new Point(secondX, secondY));

        /// <summary>
        /// Returns the coordinate (c1.X - c2.X, c1.Y - c2.Y).
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns>The coordinate(<paramref name="c1"/> - <paramref name="c2"/>).</returns>
        [Pure]
        public static Point operator -(Point c1, Point c2) => new Point(c1.X - c2.X, c1.Y - c2.Y);

        /// <summary>
        /// Returns the coordinate (point.X - direction.DeltaX, point.Y - direction.DeltaY).
        /// </summary>
        /// <param name="point"/>
        /// <param name="direction"/>
        /// <returns>The coordinate (point.X - direction.DeltaX, point.Y - direction.DeltaY).</returns>
        [Pure]
        public static Point operator -(Point point, Direction direction)
            => new Point(point.X - direction.DeltaX, point.Y - direction.DeltaY);

        /// <summary>
        /// Subtracts scalar <paramref name="i"/> from the x and y values of <paramref name="c"/>.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="i"></param>
        /// <returns>The coordinate (c.X - <paramref name="i"/>, c.Y - <paramref name="i"/>).</returns>
        [Pure]
        public static Point operator -(Point c, int i) => new Point(c.X - i, c.Y - i);

        /// <summary>
        /// True if either the x-values or y-values are not equal.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns>
        /// True if either the x-values or y-values are not equal, false if they are both equal.
        /// </returns>
        [Pure]
        public static bool operator !=(Point c1, Point c2) => !(c1 == c2);

        /// <summary>
        /// Multiplies the x and y values of the points together.
        /// </summary>
        /// <param name="c1"/>
        /// <param name="c2"/>
        /// <returns>Position (c1.X * c2.X, c1.Y * c2.Y)</returns>
        [Pure]
        public static Point operator *(Point c1, Point c2) => new Point(c1.X * c2.X, c1.Y * c2.Y);

        /// <summary>
        /// Multiplies the x and y of <paramref name="c"/> by <paramref name="i"/>.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="i"></param>
        /// <returns>Coordinate (c.X * <paramref name="i"/>, c.Y * <paramref name="i"/>)</returns>
        [Pure]
        public static Point operator *(Point c, int i) => new Point(c.X * i, c.Y * i);

        /// <summary>
        /// Multiplies the x and y value of <paramref name="c"/> by <paramref name="i"/>, rounding
        /// the result to the nearest integer.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="i"></param>
        /// <returns>
        /// Position (c.X * <paramref name="i"/>, c.Y * <paramref name="i"/>), with the resulting values
        /// rounded to nearest integer.
        /// </returns>
        [Pure]
        public static Point operator *(Point c, double i) =>
            new Point((int)Math.Round(c.X * i, MidpointRounding.AwayFromZero),
                (int)Math.Round(c.Y * i, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Divides the x/y components of <paramref name="c1"/> by the x/y components of <paramref name="c2"/>, rounding each resulting
        /// value to the nearest integer.
        /// </summary>
        /// <param name="c1"/>
        /// <param name="c2"/>
        /// <returns>Position (c1.X / c2.X, c1.Y / c2.Y), with each value rounded to the nearest integer.</returns>
        [Pure]
        public static Point operator /(Point c1, Point c2) =>
            new Point((int)Math.Round(c1.X / (double)c2.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(c1.Y / (double)c2.Y, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Divides the x and y of <paramref name="c"/> by <paramref name="i"/>, rounding resulting values
        /// to the nearest integer.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="i"></param>
        /// <returns>(c.X / <paramref name="i"/>, c.Y / <paramref name="i"/>), with the resulting values rounded to the nearest integer.</returns>
        [Pure]
        public static Point operator /(Point c, double i) =>
            new Point((int)Math.Round(c.X / i, MidpointRounding.AwayFromZero),
                (int)Math.Round(c.Y / i, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Returns the position (c1.X + c2.X, c1.Y + c2.Y).
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns>The position (c1.X + c2.X, c1.Y + c2.Y)</returns>
        [Pure]
        public static Point operator +(Point c1, Point c2) => new Point(c1.X + c2.X, c1.Y + c2.Y);

        /// <summary>
        /// Adds scalar i to the x and y values of <paramref name="c"/>.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="i"></param>
        /// <returns>Position (c.X + <paramref name="i"/>, c.Y + <paramref name="i"/>.</returns>
        [Pure]
        public static Point operator +(Point c, int i) => new Point(c.X + i, c.Y + i);

        /// <summary>
        /// Translates the given position by one unit in the given direction.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns>
        /// Position (c.X + d.DeltaX, c.Y + d.DeltaY)
        /// </returns>
        [Pure]
        public static Point operator +(Point c, Direction d) => new Point(c.X + d.DeltaX, c.Y + d.DeltaY);

        /// <summary>
        /// True if c1.X == c2.X and c1.Y == c2.Y.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns>True if the two positions are equal, false if not.</returns>
        [Pure]
        public static bool operator ==(Point c1, Point c2) => c1.X == c2.X && c1.Y == c2.Y;

        /// <summary>
        /// Reverses the ToIndex functions, returning the position represented by a given index.
        /// </summary>
        /// <param name="index">The index in 1D form.</param>
        /// <param name="width">The width of the 2D array.</param>
        /// <returns>The position represented by the 1D index given.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point FromIndex(int index, int width) => new Point(index % width, index / width);

        /// <summary>
        /// Returns <paramref name="y"/> * <paramref name="width"/> + <paramref name="x"/>.
        /// </summary>
        /// <param name="x">X-value of the coordinate.</param>
        /// <param name="y">Y-value of the coordinate.</param>
        /// <param name="width">The width of the 2D array, used to do the math to calculate index.</param>
        /// <returns>The 1D index of the position specified.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIndex(int x, int y, int width) => y * width + x;

        /// <summary>
        /// Reverses the ToIndex functions, returning only the X-value for the given index.
        /// </summary>
        /// <param name="index">The index in 1D form.</param>
        /// <param name="width">The width of the 2D array.</param>
        /// <returns>The X-value for the location represented by the given index.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToXValue(int index, int width) => index % width;

        /// <summary>
        /// Reverses the ToIndex functions, returning only the Y-value for the given index.
        /// </summary>
        /// <param name="index">The index in 1D form.</param>
        /// <param name="width">The width of the 2D array.</param>
        /// <returns>The Y-value for the location represented by the given index.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToYValue(int index, int width) => index / width;

        /// <summary>
        /// Same as operator == in this case; returns false if <paramref name="obj"/> is not a Point.
        /// </summary>
        /// <param name="obj">The object to compare the current Point to.</param>
        /// <returns>
        /// True if <paramref name="obj"/> is a Coord instance, and the two positions are equal, false otherwise.
        /// </returns>
        [Pure]
        public override bool Equals(object? obj) => obj is Point c && Equals(c);

        /// <summary>
        /// Returns a hash code for the Point. The important parts: it should be fairly fast and it
        /// does not collide often.
        /// </summary>
        /// <remarks>
        /// This hashing algorithm uses a separate bit-mixing algorithm for <see cref="X"/> and
        /// <see cref="Y"/>, with X and Y each multiplied by a different large integer, then xors
        /// the mixed values, does a right shift, and finally multiplies by an overflowing prime
        /// number.  This hashing algorithm should produce an exceptionally low collision rate for
        /// coordinates between (0, 0) and (255, 255), and remain relatively reasonable beyond that.
        /// </remarks>
        /// <returns>The hash-code for the Point.</returns>
        [Pure]
        public override int GetHashCode()
        {
            // Intentional overflow on both of these, part of hash-code generation
            int x2 = (int)(0x9E3779B9 * X), y2 = 0x632BE5AB * Y;
            return (int)(((uint)(x2 ^ y2) >> ((x2 & 7) + (y2 & 7))) * 0x85157AF5);
        }

        /// <summary>
        /// Returns a value that can be used to uniquely index this location 1D array.
        /// </summary>
        /// <param name="width">The width of the 2D map/array this location is referring to --
        /// used to do the math to calculate index.</param>
        /// <returns>The 1D index of this Point.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToIndex(int width) => Y * width + X;

        /// <summary>
        /// Returns representation (X, Y).
        /// </summary>
        /// <returns>String (X, Y)</returns>
        [Pure]
        public override string ToString() => $"({X},{Y})";

        /// <summary>
        /// Returns the position resulting from adding dx to the X-value of the position, and dy
        /// to the Y-value of the position.
        /// </summary>
        /// <param name="deltaChange">
        /// Vector where deltaChange.X represents the delta-x value and deltaChange.Y represents the
        /// delta-y value.
        /// </param>
        /// <returns>The position (<see cref="X"/> + deltaChange.X, <see cref="Y"/> + deltaChange.Y)</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point Translate(Point deltaChange) => new Point(X + deltaChange.X, Y + deltaChange.Y);

        /// <summary>
        /// Returns the position resulting from adding dx to the X-value of the position, and dy
        /// to the Y-value of the position.
        /// </summary>
        /// <param name="dx">Change in x-value to apply.</param>
        /// <param name="dy">Change in y-value to apply.</param>
        /// <returns>The position (<see cref="X"/> + dx, <see cref="Y"/> + dy)</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point Translate(int dx, int dy) => Translate(new Point(dx, dy));

        /// <summary>
        /// Creates a new Point with its X value moved to the given one.
        /// </summary>
        /// <param name="x">X-value for the new Point.</param>
        /// <returns>A new Point, with its X value changed to the given one.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point WithX(int x) => new Point(x, Y);

        /// <summary>
        /// Creates a new Point with its Y value moved to the given one.
        /// </summary>
        /// <param name="y">Y-value for the new Point.</param>
        /// <returns>A new Point, with its Y value changed to the given one.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point WithY(int y) => new Point(X, y);

        /// <summary>
        /// True if the given coordinate has equal x and y values to the current one.
        /// </summary>
        /// <param name="other">Position to compare.</param>
        /// <returns>True if the two positions are equal, false if not.</returns>
        [Pure]
        public bool Equals(Point other) => X == other.X && Y == other.Y;

        #region TupleCompatibility

        /// <summary>
        /// Adds support for C# Deconstruction syntax.
        /// </summary>
        /// <param name="x" />
        /// <param name="y" />
        [Pure]
        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        /// <summary>
        /// Implicitly converts a Point to an equivalent tuple of two integers.
        /// </summary>
        /// <param name="c" />
        /// <returns />
        [Pure]
        public static implicit operator (int x, int y)(Point c) => (c.X, c.Y);

        /// <summary>
        /// Implicitly converts a tuple of two integers to an equivalent Point.
        /// </summary>
        /// <param name="tuple" />
        /// <returns />
        [Pure]
        public static implicit operator Point((int x, int y) tuple) => new Point(tuple.x, tuple.y);

        /// <summary>
        /// Adds the x and y values of a Point to the corresponding values of a tuple of two integers.
        /// </summary>
        /// <param name="tuple" />
        /// <param name="c" />
        /// <returns>A tuple (tuple.x + c.X, tuple.y + c.Y).</returns>
        [Pure]
        public static (int x, int y) operator +((int x, int y) tuple, Point c) => (tuple.x + c.X, tuple.y + c.Y);

        /// <summary>
        /// Adds the x and y values of a tuple of two integers to a Point.
        /// </summary>
        /// <param name="c" />
        /// <param name="tuple" />
        /// <returns>Position (c.X + tuple.x, c.Y + tuple.y).</returns>
        [Pure]
        public static Point operator +(Point c, (int x, int y) tuple) => new Point(c.X + tuple.x, c.Y + tuple.y);

        /// <summary>
        /// Subtracts the x and y values of a Point from a tuple of two integers.
        /// </summary>
        /// <param name="tuple" />
        /// <param name="c" />
        /// <returns>A tuple (tuple.x - c.X, tuple.y - c.Y).</returns>
        [Pure]
        public static (int x, int y) operator -((int x, int y) tuple, Point c) => (tuple.x - c.X, tuple.y - c.Y);

        /// <summary>
        /// Subtracts the x and y values of a tuple of two integers from a Point.
        /// </summary>
        /// <param name="c" />
        /// <param name="tuple" />
        /// <returns>Position (c.X - tuple.x, c.Y - tuple.y).</returns>
        [Pure]
        public static Point operator -(Point c, (int x, int y) tuple) => new Point(c.X - tuple.x, c.Y - tuple.y);

        /// <summary>
        /// Multiples the x and y values of a tuple of two integers by the x and y values of a Point.
        /// </summary>
        /// <param name="tuple"/>
        /// <param name="c"/>
        /// <returns>Position (tuple.x * c.X, tuple.y * c.Y).</returns>
        [Pure]
        public static (int x, int y) operator *((int x, int y) tuple, Point c) => (tuple.x * c.X, tuple.y * c.Y);

        /// <summary>
        /// Multiples the x and y values of a Point by the x and y values of a tuple of two integers.
        /// </summary>
        /// <param name="c"/>
        /// <param name="tuple"/>
        /// <returns>Position (c.X * tuple.x, c.Y * tuple.y).</returns>
        [Pure]
        public static Point operator *(Point c, (int x, int y) tuple) => new Point(c.X * tuple.x, c.Y * tuple.y);

        /// <summary>
        /// Divides the x/y values of a tuple of two integers by the x/y values of a Point, rounding to the nearest integer.
        /// </summary>
        /// <param name="tuple"/>
        /// <param name="c"/>
        /// <returns>Position (tuple.x / c.X, tuple.y / c.Y), with each value rounded to the nearest integer.</returns>
        [Pure]
        public static (int x, int y) operator /((int x, int y) tuple, Point c)
            => new Point((int)Math.Round(tuple.x / (double)c.X, MidpointRounding.AwayFromZero),
                (int)Math.Round(tuple.y / (double)c.Y, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Divides the x/y values of a Point by the x/y values of a tuple of two integers, rounding to the nearest integer.
        /// </summary>
        /// <param name="tuple"/>
        /// <param name="c"/>
        /// <returns>Position (c.X / tuple.x, c.Y / tuple.y), with each value rounded to the nearest integer.</returns>
        [Pure]
        public static (int x, int y) operator /(Point c, (int x, int y) tuple)
            => new Point((int)Math.Round(c.X / (double)tuple.x, MidpointRounding.AwayFromZero),
                (int)Math.Round(c.Y / (double)tuple.y, MidpointRounding.AwayFromZero));

        /// <summary>
        /// True if the two point's x and y values are equal.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="tuple"></param>
        /// <returns>True if the two positions are equal, false if not.</returns>
        [Pure]
        public static bool operator ==(Point c, (int x, int y) tuple) => c.X == tuple.x && c.Y == tuple.y;

        /// <summary>
        /// True if either the x-values or y-values are not equal.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="tuple"></param>
        /// <returns>
        /// True if either the x-values or y-values are not equal, false if they are both equal.
        /// </returns>
        [Pure]
        public static bool operator !=(Point c, (int x, int y) tuple) => !(c == tuple);

        /// <summary>
        /// True if the two point's x and y values are equal.
        /// </summary>
        /// <param name="tuple"></param>
        /// <param name="c"></param>
        /// <returns>True if the two positions are equal, false if not.</returns>
        [Pure]
        public static bool operator ==((int x, int y) tuple, Point c) => tuple.x == c.X && tuple.y == c.Y;

        /// <summary>
        /// True if either the x-values or y-values are not equal.
        /// </summary>
        /// <param name="tuple"></param>
        /// <param name="c"></param>
        /// <returns>
        /// True if either the x-values or y-values are not equal, false if they are both equal.
        /// </returns>
        [Pure]
        public static bool operator !=((int x, int y) tuple, Point c) => !(tuple == c);

        /// <summary>
        /// True if the given position has equal x and y values to the current one.
        /// </summary>
        /// <param name="other">Tuple to compare.</param>
        /// <returns>True if the two positions are equal, false if not.</returns>
        [Pure]
        public bool Equals((int x, int y) other) => X == other.x && Y == other.y;

        /// <summary>
        /// True if the given position has equal x and y values to the current one.
        /// </summary>
        /// <param name="other">Point to compare.</param>
        /// <returns>True if the two positions are equal, false if not.</returns>
        [Pure]
        public bool Matches((int x, int y) other) => Equals(other);
        #endregion

        #region Circles

        /// <summary>
        /// Implicitly converts a Point to its equivalent polar coordinate.
        /// </summary>
        /// <param name="pos">Point to convert.</param>
        /// <returns>A <see cref="PolarCoordinate"/> equivalent to this cartesian point.</returns>
        public static implicit operator PolarCoordinate(Point pos) => PolarCoordinate.FromCartesian(pos);

        /// <summary>
        /// Returns a Polar Coordinate that is equivalent to this (Cartesian) Coordinate
        /// </summary>
        /// <returns>The Equivalent Polar Coordinate</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PolarCoordinate ToPolarCoordinate() => PolarCoordinate.FromCartesian(this);

        /// <summary>
        /// Rotates a single point around the origin (0, 0).
        /// </summary>
        /// <param name="degrees">The amount of Degrees to rotate this point clockwise</param>
        /// <returns>The equivalent point after a rotation</returns>
        [Pure]
        public  Point Rotate(double degrees)
        {
            double radians = MathHelpers.ToRadian(degrees);
            int x = (int)Math.Round(X * Math.Cos(radians) - Y * Math.Sin(radians));
            int y = (int)Math.Round(X * Math.Sin(radians) + Y * Math.Cos(radians));
            return new Point(x, y);
        }

        /// <summary>
        /// Rotates a single point around the origin point.
        /// </summary>
        /// <param name="degrees">The amount of Degrees to rotate this point</param>
        /// <param name="origin">The Point around which to rotate</param>
        /// <returns>The equivalent point after a rotation</returns>
        [Pure]
        public Point Rotate(double degrees, Point origin)
        {
            Point rotatingPoint = this - origin;
            double radians = MathHelpers.ToRadian(degrees);
            int x = (int)Math.Round(rotatingPoint.X * Math.Cos(radians) - rotatingPoint.Y * Math.Sin(radians));
            int y = (int)Math.Round(rotatingPoint.X * Math.Sin(radians) + rotatingPoint.Y * Math.Cos(radians));
            return origin + new Point(x, y);
        }

        /// <summary>
        /// Rotates a single point around the origin point.
        /// </summary>
        /// <param name="degrees">The amount of Degrees to rotate this point</param>
        /// <param name="originX">X-value of the location around which to rotate</param>
        /// <param name="originY">Y-value of the location around which to rotate</param>
        /// <returns>The equivalent point after a rotation</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point Rotate(double degrees, int originX, int originY)
            => Rotate(degrees, new Point(originX, originY));

        #endregion
    }
}
