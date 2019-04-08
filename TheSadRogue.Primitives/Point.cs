using System;

namespace SadRogue.Primitives
{
	/// <summary>
	/// A structure that represents a standard 2D point.  Provides numerous functions and operatorsthat enable
	/// common grid/position-related math and operations.  Other packages also may define extension methods to
	/// enable interoperability with this type.
	/// </summary>
	/// <remarks>
	/// Point instances can be created using the standard Point c = new Point(x, y) syntax.  In addition,
	/// you may create a coord from a c# 7 tuple, like Point c = (x, y);.  As well, Point supports C#
	/// Deconstrution syntax.
	///
	/// Point also provides operators and static helper functions that perform common grid math/operations,
	/// as well as interoperability with other grid-based classes like <see cref="Direction"/>.
	/// </remarks>
	public struct Point : IEquatable<Point>, IEquatable<(int x, int y)>
	{
		/// <summary>
		/// Coord value that represents None or no position (since Coord is not a nullable type).
		/// Typically you would use this constant instead of null.
		/// </summary>
		/// <remarks>
		/// This constant has (x, y) values (int.MinValue, int.MinValue), so a coordinate with those
		/// x/y values is not considered a valid coordinate by many functions.
		/// </remarks>
		public static readonly Point NONE = new Point(int.MinValue, int.MinValue);

		/// <summary>
		/// X-value of the coordinate.
		/// </summary>
		public readonly int X;

		/// <summary>
		/// Y-value of the coordinate.
		/// </summary>
		public readonly int Y;

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
		/// Calculates degree bearing of the line (start =&gt; end), where 0 points in the direction <see cref="Direction.UP"/>.
		/// </summary>
		/// <param name="start">Position of line starting point.</param>
		/// <param name="end">Position of line ending point.</param>
		/// <returns>The degree bearing of the line specified by the two given points.</returns>
		public static double BearingOfLine(Point start, Point end) => BearingOfLine(start - end);


		/// <summary>
		/// Calculates the degree bearing of a line with the given delta-x and delta-y values, where
		/// 0 degreees points in the direction <see cref="Direction.UP"/>.
		/// </summary>
		/// <param name="deltaChange">
		/// Vector, where deltaChange.X is the change in x-values across the line, and deltaChange.Y
		/// is the change in y-values across the line.
		/// </param>
		/// <returns>The degree bearing of the line with the given dx and dy values.</returns>
		public static double BearingOfLine(Point deltaChange)
		{
			int dx = deltaChange.X;
			int dy = deltaChange.Y;

			dy *= Direction.yMult;
			double angle = Math.Atan2(dy, dx);
			double degree = MathHelpers.ToDegree(angle);
			degree += 450; // Rotate to all positive such that 0 is up
			degree %= 360; // Normalize
			return degree;
		}

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
		public static double EuclideanDistanceMagnitude(Point c1, Point c2) => EuclideanDistanceMagnitude(c2 - c1);

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
		public static double EuclideanDistanceMagnitude(Point deltaChange) => deltaChange.X * deltaChange.X + deltaChange.Y * deltaChange.Y;


		/// <summary>
		/// Returns the midpoint between the two points.
		/// </summary>
		/// <param name="c1">The first point.</param>
		/// <param name="c2">The second point.</param>
		/// <returns>The midpoint between <paramref name="c1"/> and <paramref name="c2"/>.</returns>
		public static Point Midpoint(Point c1, Point c2) =>
			new Point((int)Math.Round((c1.X + c2.X) / 2.0f, MidpointRounding.AwayFromZero), (int)Math.Round((c1.Y + c2.Y) / 2.0f, MidpointRounding.AwayFromZero));

		/// <summary>
		/// Returns the coordinate (c1.X - c2.X, c1.Y - c2.Y)
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns>The coordinate(<paramref name="c1"/> - <paramref name="c2"/>).</returns>
		public static Point operator -(Point c1, Point c2) => new Point(c1.X - c2.X, c1.Y - c2.Y);

		/// <summary>
		/// Subtracts scalar <paramref name="i"/> from the x and y values of <paramref name="c"/>.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="i"></param>
		/// <returns>The coordinate (c.X - <paramref name="i"/>, c.Y - <paramref name="i"/>)</returns>
		public static Point operator -(Point c, int i) => new Point(c.X - i, c.Y - i);

		/// <summary>
		/// True if either the x-values or y-values are not equal.
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns>
		/// True if either the x-values or y-values are not equal, false if they are both equal.
		/// </returns>
		public static bool operator !=(Point c1, Point c2) => !(c1 == c2);

		/// <summary>
		/// Multiplies the x and y of <paramref name="c"/> by <paramref name="i"/>.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="i"></param>
		/// <returns>Coordinate (c.X * <paramref name="i"/>, c.Y * <paramref name="i"/>)</returns>
		public static Point operator *(Point c, int i) => new Point(c.X * i, c.Y * i);

		/// <summary>
		/// Multiplies the x and y value of <paramref name="c"/> by <paramref name="i"/>, rounding
		/// the result to the nearest integer.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="i"></param>
		/// <returns>
		/// Coordinate (c.X * <paramref name="i"/>, c.Y * <paramref name="i"/>), with the resulting values
		/// rounded to nearest integer.
		/// </returns>
		public static Point operator *(Point c, double i) =>
			new Point((int)Math.Round(c.X * i, MidpointRounding.AwayFromZero), (int)Math.Round(c.Y * i, MidpointRounding.AwayFromZero));

		/// <summary>
		/// Divides the x and y of <paramref name="c"/> by <paramref name="i"/>, rounding resulting values
		/// to the nearest integer.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="i"></param>
		/// <returns>(c.X / <paramref name="i"/>, c.Y / <paramref name="i"/>), with the resulting values rounded to the nearest integer.</returns>
		public static Point operator /(Point c, int i) =>
			new Point((int)Math.Round(c.X / (double)i, MidpointRounding.AwayFromZero), (int)Math.Round(c.Y / (double)i, MidpointRounding.AwayFromZero));

		/// <summary>
		/// Divides the x and y of <paramref name="c"/> by <paramref name="i"/>, rounding resulting values
		/// to the nearest integer.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="i"></param>
		/// <returns>(c.X / <paramref name="i"/>, c.Y / <paramref name="i"/>), with the resulting values rounded to the nearest integer.</returns>
		public static Point operator /(Point c, double i) =>
			new Point((int)Math.Round(c.X / i, MidpointRounding.AwayFromZero), (int)Math.Round(c.Y / i, MidpointRounding.AwayFromZero));

		/// <summary>
		/// Returns the coordinate (c1.X + c2.X, c1.Y + c2.Y).
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns>The coordinate (c1.X + c2.X, c1.Y + c2.Y)</returns>
		public static Point operator +(Point c1, Point c2) => new Point(c1.X + c2.X, c1.Y + c2.Y);

		/// <summary>
		/// Adds scalar i to the x and y values of <paramref name="c"/>.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="i"></param>
		/// <returns>Coordinate (c.X + <paramref name="i"/>, c.Y + <paramref name="i"/>.</returns>
		public static Point operator +(Point c, int i) => new Point(c.X + i, c.Y + i);

		/// <summary>
		/// Translates the given coordinate by one unit in the given direction.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="d"></param>
		/// <returns>
		/// Coordinate (c.X + d.DeltaX, c.Y + d.DeltaY)
		/// </returns>
		public static Point operator +(Point c, Direction d) => new Point(c.X + d.DeltaX, c.Y + d.DeltaY);

		/// <summary>
		/// True if c1.X == c2.X and c1.Y == c2.Y.
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns>True if the two coordinates are equal, false if not.</returns>
		public static bool operator ==(Point c1, Point c2) => c1.X == c2.X && c1.Y == c2.Y;

		/// <summary>
		/// Reverses the ToIndex functions, returning the position represented by a given index.
		/// </summary>
		/// <param name="index">The index in 1D form.</param>
		/// <param name="width">The width of the 2D array.</param>
		/// <returns>The position represented by the 1D index given.</returns>
		public static Point ToCoord(int index, int width) => new Point(index % width, index / width);

		/// <summary>
		/// Returns <paramref name="y"/> * <paramref name="width"/> + <paramref name="x"/>.
		/// </summary>
		/// <param name="x">X-value of the coordinate.</param>
		/// <param name="y">Y-value of the coordinate.</param>
		/// <param name="width">The width of the 2D array, used to do the math to calculate index.</param>
		/// <returns>The 1D index of this Coord.</returns>
		public static int ToIndex(int x, int y, int width) => y * width + x;

		/// <summary>
		/// Reverses the ToIndex functions, returning only the X-value for the given index.
		/// </summary>
		/// <param name="index">The index in 1D form.</param>
		/// <param name="width">The width of the 2D array.</param>
		/// <returns>The X-value for the location represented by the given index.</returns>
		public static int ToXValue(int index, int width) => index % width;

		/// <summary>
		/// Reverses the ToIndex functions, returning only the Y-value for the given index.
		/// </summary>
		/// <param name="index">The index in 1D form.</param>
		/// <param name="width">The width of the 2D array.</param>
		/// <returns>The Y-value for the location represented by the given index.</returns>
		public static int ToYValue(int index, int width) => index / width;

		/// <summary>
		/// Same as operator == in this case; returns false if <paramref name="obj"/> is not a Coord.
		/// </summary>
		/// <param name="obj">The object to compare the current Coord to.</param>
		/// <returns>
		/// True if <paramref name="obj"/> is a Coord instance, and the two coordinates are equal, false otherwise.
		/// </returns>
		public override bool Equals(object obj) => obj is Point c && Equals(c);

		/// <summary>
		/// Returns a hash code for the Coord. The important parts: it should be fairly fast and it
		/// does not collide often.
		/// </summary>
		/// <remarks>
		/// This hashing algorithm uses a seperate bit-mixing algorithm for <see cref="X"/> and
		/// <see cref="Y"/>, with X and Y each multiplied by a differet large integer, then xors
		/// the mixed values, does a right shift, and finally multiplies by an overflowing prime
		/// number.  This hashing algorithm should produce an exceptionally low collision rate for
		/// coordinates between (0, 0) and (255, 255).
		/// </remarks>
		/// <returns>The hash-code for the Coord.</returns>
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
		/// <returns>The 1D index of this Coord.</returns>
		public int ToIndex(int width) => Y * width + X;

		/// <summary>
		/// Returns representation (X, Y).
		/// </summary>
		/// <returns>String (X, Y)</returns>
		public override string ToString() => $"({X},{Y})";

		/// <summary>
		/// Returns the coordinate resulting from adding dx to the X-value of the coordinate, and dy
		/// to the Y-value of the coordinate.
		/// </summary>
		/// <param name="deltaChange">
		/// Vector where deltaChange.X represents the delta-x value and deltaChange.Y represents the
		/// delta-y value.
		/// </param>
		/// <returns>The coordinate (<see cref="X"/> + deltaChange.X, <see cref="Y"/> + deltaChange.Y)</returns>
		public Point Translate(Point deltaChange) => new Point(X + deltaChange.X, Y + deltaChange.Y);
		
		/// <summary>
		/// True if the given coordinate has equal x and y values to the current one.
		/// </summary>
		/// <param name="other">Coordinate to compare.</param>
		/// <returns>True if the two coordinates are equal, false if not.</returns>
		public bool Equals(Point other) => X == other.X && Y == other.Y;
		
		#region TupleCompatibility
		/// <summary>
		/// Adds support for C# Deconstruction syntax.
		/// </summary>
		/// <param name="x" />
		/// <param name="y" />
		public void Deconstruct(out int x, out int y)
		{
			x = X;
			y = Y;
		}

		/// <summary>
		/// Implicitly converts a Coord to an equivalent tuple of two integers.
		/// </summary>
		/// <param name="c" />
		/// <returns />
		public static implicit operator (int x, int y) (Point c) => (c.X, c.Y);
		/// <summary>
		/// Implicitly converts a tuple of two integers to an equivalent Coord.
		/// </summary>
		/// <param name="tuple" />
		/// <returns />
		public static implicit operator Point((int x, int y) tuple) => new Point(tuple.x, tuple.y);
		
		/// <summary>
		/// Adds the x and y values of a Coord to the corresponding values of a tuple of two integers.
		/// </summary>
		/// <param name="tuple" />
		/// <param name="c" />
		/// <returns>A tuple (tuple.x + c.X, tuple.y + c.Y).</returns>
		public static (int x, int y) operator +((int x, int y) tuple, Point c) => (tuple.x + c.X, tuple.y + c.Y);
		/// <summary>
		/// Adds the x and y values of a tuple of two integers to a Coord.
		/// </summary>
		/// <param name="c" />
		/// <param name="tuple" />
		/// <returns>A Coord (c.X + tuple.x, c.Y + tuple.y).</returns>
		public static Point operator +(Point c, (int x, int y) tuple) => new Point(c.X + tuple.x, c.Y + tuple.y);
		
		/// <summary>
		/// Subtracts the x and y values of a Coord from a tuple of two integers.
		/// </summary>
		/// <param name="tuple" />
		/// <param name="c" />
		/// <returns>A tuple (tuple.x - c.X, tuple.y - c.Y).</returns>
		public static (int x, int y) operator -((int x, int y) tuple, Point c) => (tuple.x - c.X, tuple.y - c.Y);
		/// <summary>
		/// Subtracts the x and y values of a tuple of two integers from a Coord.
		/// </summary>
		/// <param name="c" />
		/// <param name="tuple" />
		/// <returns>A Coord (c.X - tuple.x, c.Y - tuple.y).</returns>
		public static Point operator -(Point c, (int x, int y) tuple) => new Point(c.X - tuple.x, c.Y - tuple.y);

		/// <summary>
		/// True if the two point's x and y values are equal.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="tuple"></param>
		/// <returns>True if the two positions are equal, false if not.</returns>
		public static bool operator ==(Point c, (int x, int y) tuple)
		{
			return c.X == tuple.x && c.Y == tuple.y;
		}

		/// <summary>
		/// True if either the x-values or y-values are not equal.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="tuple"></param>
		/// <returns>
		/// True if either the x-values or y-values are not equal, false if they are both equal.
		/// </returns>
		public static bool operator !=(Point c, (int x, int y) tuple) => !(c == tuple);

		/// <summary>
		/// True if the two point's x and y values are equal.
		/// </summary>
		/// <param name="tuple"></param>
		/// <param name="c"></param>
		/// <returns>True if the two positions are equal, false if not.</returns>
		public static bool operator ==((int x, int y) tuple, Point c)
		{
			return tuple.x == c.X && tuple.y == c.Y;
		}

		/// <summary>
		/// True if either the x-values or y-values are not equal.
		/// </summary>
		/// <param name="tuple"></param>
		/// <param name="c"></param>
		/// <returns>
		/// True if either the x-values or y-values are not equal, false if they are both equal.
		/// </returns>
		public static bool operator !=((int x, int y) tuple, Point c) => !(tuple == c);

		/// <summary>
		/// True if the given position has equal x and y values to the current one.
		/// </summary>
		/// <param name="other">Point to compare.</param>
		/// <returns>True if the two positions are equal, false if not.</returns>
		public bool Equals((int x, int y) other) => X == other.x && Y == other.y;
		#endregion
	}
}

