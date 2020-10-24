using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Structure representing methods of calculating distance on a grid. You cannot create instances of this
    /// class using a constructor -- instead this class contains static instances representing the
    /// various distance calculations.
    /// </summary>
    /// <remarks>
    /// Provides functions that calculate the distance between two points according to the distance
    /// measurement being used. Instances of Distance are also implicitly convertible to both
    /// <see cref="Radius"/> and <see cref="AdjacencyRule"/> (since both a method of determining adjacent
    /// locations and a radius shape are implied by a distance calculation).
    /// </remarks>
    [DataContract]
    public readonly struct Distance : IEquatable<Distance>, IMatchable<Distance>
    {
        /// <summary>
        /// Represents chebyshev distance (equivalent to 8-way movement with no extra cost for diagonals).
        /// </summary>
        public static Distance Chebyshev = new Distance(Types.Chebyshev);

        /// <summary>
        /// Represents euclidean distance (equivalent to 8-way movement with ~1.41 movement cost for diagonals).
        /// </summary>
        public static Distance Euclidean = new Distance(Types.Euclidean);

        /// <summary>
        /// Represents manhattan distance (equivalent to 4-way, cardinal-only movement).
        /// </summary>
        public static Distance Manhattan = new Distance(Types.Manhattan);

        /// <summary>
        /// Enum value representing the method of calcuating distance -- useful for using
        /// Distance types in switch statements.
        /// </summary>
        [DataMember] public readonly Types Type;

        private static readonly string[] s_writeVals = Enum.GetNames(typeof(Types));

        private Distance(Types type) => Type = type;

        /// <summary>
        /// Enum representing Distance types. Each Distance instance has a <see cref="Type"/> field
        /// which contains the corresponding value from this enum.  Useful for easy mapping of Distance
        /// types to a primitive type (for cases like a switch statement).
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// Enum type for <see cref="Distance.Manhattan"/>.
            /// </summary>
            Manhattan,

            /// <summary>
            /// Enum type for <see cref="Distance.Euclidean"/>.
            /// </summary>
            Euclidean,

            /// <summary>
            /// Enum type for <see cref="Distance.Chebyshev"/>.
            /// </summary>
            Chebyshev
        };

        /// <summary>
        /// Allows explicit casting to <see cref="Radius"/> type.
        /// </summary>
        /// <remarks>
        /// The 2D radius shape corresponding to the definition of a radius according to the distance calculation
        /// casted will be returned.
        /// </remarks>
        /// <param name="distance"/>
        [Pure]
        public static implicit operator Radius(Distance distance) => distance.Type switch
        {
            Types.Manhattan => Radius.Diamond,
            Types.Euclidean => Radius.Circle,
            Types.Chebyshev => Radius.Square,
            _ => throw new Exception($"Could not convert {nameof(Distance)} to {nameof(Radius)} -- this is a bug!")
        };

        /// <summary>
        /// Allows implicit casting to the <see cref="AdjacencyRule"/> type.
        /// </summary>
        /// <remarks>
        /// The adjacency rule corresponding to the definition of a adjacency according to the
        /// distance calculation casted will be returned.
        /// </remarks>
        /// <param name="distance"/>
        [Pure]
        public static implicit operator AdjacencyRule(Distance distance) => distance.Type switch
        {
            Types.Manhattan => AdjacencyRule.Cardinals,
            Types.Chebyshev => AdjacencyRule.EightWay,
            Types.Euclidean => AdjacencyRule.EightWay,
            _ => throw new Exception(
                $"Could not convert {nameof(Distance)} to {nameof(AdjacencyRule)} -- this is a bug!")
        };

        /// <summary>
        /// Implicitly converts a Distance to its corresponding <see cref="Type"/>.
        /// </summary>
        /// <param name="distance"/>
        [Pure]
        public static implicit operator Types(Distance distance) => distance.Type;

        /// <summary>
        /// Implicitly converts an <see cref="Types"/> enum value to its corresponding Distance.
        /// </summary>
        /// <param name="type"/>
        [Pure]
        public static implicit operator Distance(Types type) => type switch
        {
            Types.Manhattan => Manhattan,
            Types.Euclidean => Euclidean,
            Types.Chebyshev => Chebyshev,
            _ => throw new Exception($"Could not convert {nameof(Types)} to {nameof(Distance)} -- this is a bug!")
        };

        /// <summary>
        /// Returns the distance between the two (2D) points specified.
        /// </summary>
        /// <param name="start">Starting point.</param>
        /// <param name="end">Ending point.</param>
        /// <returns>The distance between the two points.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Calculate(Point start, Point end) => Calculate(start.X, start.Y, end.X, end.Y);

        /// <summary>
        /// Returns the distance between the two (2D) points specified.
        /// </summary>
        /// <param name="startX">X-Coordinate of the starting point.</param>
        /// <param name="startY">Y-Coordinate of the starting point.</param>
        /// <param name="endX">X-Coordinate of the ending point.</param>
        /// <param name="endY">Y-Coordinate of the ending point.</param>
        /// <returns>The distance between the two points.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Calculate(double startX, double startY, double endX, double endY)
        {
            double dx = startX - endX;
            double dy = startY - endY;
            return Calculate(dx, dy);
        }

        /// <summary>
        /// Returns the distance between two locations, given the change in X and change in Y value
        /// (specified by the X and Y values of the given vector).
        /// </summary>
        /// <param name="deltaChange">The delta-x and delta-y between the two locations.</param>
        /// <returns>The distance between two locations withe the given delta-change values.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Calculate(Point deltaChange) => Calculate(deltaChange.X, deltaChange.Y);

        /// <summary>
        /// Returns the distance between two locations, given the change in X and change in Y value.
        /// </summary>
        /// <param name="dx">The delta-x between the two locations.</param>
        /// <param name="dy">The delta-y between the two locations.</param>
        /// <returns>The distance between two locations with the given delta-change values.</returns>
        [Pure]
        public double Calculate(double dx, double dy)
        {
            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            return Type switch
            {
                Types.Chebyshev => Math.Max(dx, dy), // Radius is the longest axial distance
                Types.Manhattan => dx + dy, // Simply manhattan distance
                Types.Euclidean => Math.Sqrt(dx * dx + dy * dy), // Spherical radius
                _ => throw new NotSupportedException(
                    $"{nameof(Calculate)} does not support distance calculation {this}: this is a bug!")
            };
        }

        /// <summary>
        /// True if the given Distance has the same Type the current one.
        /// </summary>
        /// <param name="other">Distance to compare.</param>
        /// <returns>True if the two distance calculation methods are the same, false if not.</returns>
        [Pure]
        public bool Equals(Distance other) => Type == other.Type;

        /// <summary>
        /// Same as operator == in this case; returns false if <paramref name="obj"/> is not a Distance.
        /// </summary>
        /// <param name="obj">The object to compare the current Distance to.</param>
        /// <returns>
        /// True if <paramref name="obj"/> is a Distance, and the two distance calculations are equal, false otherwise.
        /// </returns>
        [Pure]
        public override bool Equals(object? obj) => obj is Distance c && Equals(c);

        /// <summary>
        /// Returns a hash-map value for the current object.
        /// </summary>
        /// <returns/>
        [Pure]
        public override int GetHashCode() => Type.GetHashCode();

        /// <summary>
        /// True if the given Distance has the same Type the current one.
        /// </summary>
        /// <param name="other">Distance to compare.</param>
        /// <returns>True if the two distance calculation methods are the same, false if not.</returns>
        [Pure]
        public bool Matches(Distance other) => Equals(other);

        /// <summary>
        /// True if the two Distances have the same Type.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs"/>
        /// <returns>True if the two distance calculations are equal, false if not.</returns>
        [Pure]
        public static bool operator ==(Distance lhs, Distance rhs) => lhs.Type == rhs.Type;

        /// <summary>
        /// True if the types are not equal.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs"/>
        /// <returns>
        /// True if the types are not equal, false if they are both equal.
        /// </returns>
        [Pure]
        public static bool operator !=(Distance lhs, Distance rhs) => !(lhs == rhs);

        /// <summary>
        /// Returns a string representation of the distance calculation method represented.
        /// </summary>
        /// <returns>A string representation of the distance method represented.</returns>
        [Pure]
        public override string ToString() => s_writeVals[(int)Type];
    }
}
