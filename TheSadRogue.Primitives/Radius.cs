using System;

namespace SadRogue.Primitives
{
	/// <summary>
	/// Class representing different shapes that define the concept of a radius on a grid. You cannot
	/// create instances of this class using a constructor -- instead, this class contains static instances
	/// representing the various radius shapes.
	/// </summary>
	/// <remarks>
	/// Contains utility functions to work with radius shapes.  Instances of Radius are also implicitly
	/// convertible to both <see cref="Distance"/> and <see cref="AdjacencyRule"/> (since both a method
	/// of determining adjacent locations and a method of calculating distance are implied by a radius
	/// shape).
	/// </remarks>
	public class Radius
	{
		/// <summary>
		/// Radius is a circle around the center point. CIRCLE would represent movement radius in
		/// an 8-way movement scheme with a ~1.41 cost multiplier for diagonal movement.
		/// </summary>
		public static readonly Radius CIRCLE = new Radius(Types.CIRCLE);

		/// <summary>
		/// Radius is a diamond around the center point. DIAMOND would represent movement radius
		/// in a 4-way movement scheme.
		/// </summary>
		public static readonly Radius DIAMOND = new Radius(Types.DIAMOND);

		/// <summary>
		/// Radius is a square around the center point. SQUARE would represent movement radius in
		/// an 8-way movement scheme, where all 8 squares around an item are considered equal distance
		/// away.
		/// </summary>
		public static readonly Radius SQUARE = new Radius(Types.SQUARE);

		/// <summary>
		/// Enum value representing the radius shape -- useful for using Radius types in switch
		/// statements.
		/// </summary>
		public readonly Types Type;

		private static readonly string[] writeVals = Enum.GetNames(typeof(Types));

		private Radius(Types type)
		{
			Type = type;
		}

		/// <summary>
		/// Enum representing Radius types. Each Radius instance has a <see cref="Type"/> field
		/// which contains the corresponding value from this enum.  Useful for easy mapping of Radius
		/// types to a primitive type (for cases like a switch statement).
		/// </summary>
		public enum Types
		{
			/// <summary>
			/// Type for Radius.SQUARE.
			/// </summary>
			SQUARE,

			/// <summary>
			/// Type for Radius.DIAMOND.
			/// </summary>
			DIAMOND,

			/// <summary>
			/// Type for Radius.CIRCLE.
			/// </summary>
			CIRCLE,
		};

		/// <summary>
		/// Allows implicit casting to the <see cref="AdjacencyRule"/> type.
		/// </summary>
		/// <remarks>
		/// The rule corresponding to the proper definition of distance that creates the
		/// radius shape casted will be returned.
		/// </remarks>
		/// <param name="radius">Radius type being casted.</param>
		public static implicit operator AdjacencyRule(Radius radius)
		{
			switch (radius.Type)
			{
				case Types.CIRCLE:
				case Types.SQUARE:
					return AdjacencyRule.EIGHT_WAY;

				case Types.DIAMOND:
					return AdjacencyRule.CARDINALS;

				default:
					return null; // Will not occur
			}
		}

		/// <summary>
		/// Allows implicit casting to the <see cref="Distance"/> type.
		/// </summary>
		/// <remarks>
		/// The <see cref="Distance"/> instance corresponding to the proper definition of
		/// distance that creates the radius shape casted will be returned.
		/// </remarks>
		/// <param name="radius">Radius type being casted.</param>
		public static implicit operator Distance(Radius radius)
		{
			switch (radius.Type)
			{
				case Types.CIRCLE:
					return Distance.EUCLIDEAN;

				case Types.DIAMOND:
					return Distance.MANHATTAN;

				case Types.SQUARE:
					return Distance.CHEBYSHEV;

				default:
					return null; // Will not occur
			}
		}

		/// <summary>
		/// Gets the Radius class instance representing the radius type specified.
		/// </summary>
		/// <param name="radiusType">The enum value for the radius shape.</param>
		/// <returns>The radius class representing the given radius shape.</returns>
		public static Radius ToRadius(Types radiusType)
		{
			switch (radiusType)
			{
				case Types.CIRCLE:
					return CIRCLE;

				case Types.DIAMOND:
					return DIAMOND;

				case Types.SQUARE:
					return SQUARE;

				default:
					return null; // Will never occur
			}
		}

		/// <summary>
		/// Returns a string representation of the Radius.
		/// </summary>
		/// <returns>A string representation of the Radius.</returns>
		public override string ToString() => writeVals[(int)Type];
	}
}
