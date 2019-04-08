using System;

namespace SadRogue.Primitives
{
	/// <summary>
	/// Static class consisting of mathematical "helper" functions and constants that can be useful
	/// for performing operations on a 2D grid.
	/// </summary>
	public static class MathHelpers
	{
		/// <summary>
		/// Result of 1/360; represents in decimal form a percent of a circle that a degree constitutes.
		/// </summary>
		public const double DEGREE_PCT_OF_CIRCLE = 0.002777777777777778;

		/// <summary>
		/// Converts given angle from radians to degrees.
		/// </summary>
		/// <param name="radAngle">Angle in radians.</param>
		/// <returns>The given angle in degrees.</returns>
		public static double ToDegree(double radAngle) => radAngle * (180.0 / Math.PI);

		/// <summary>
		/// Converts given angle from degrees to radians.
		/// </summary>
		/// <param name="degAngle">Angle in degrees.</param>
		/// <returns>The given angle in radians.</returns>
		public static double ToRadian(double degAngle) => Math.PI * degAngle / 180.0;
	}
}
