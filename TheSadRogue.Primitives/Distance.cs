using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Class representing methods of calculating distance on a grid. You cannot create instances of this
    /// class using a constructor -- instead this class contains static instances representing the
    /// various distance calculations.
    /// </summary>
    /// <remarks>
    /// Provides functions that calculate the distance between two points according to the distance
    /// measurement being used. Instances of Distance are also implicitly convertible to both
    /// <see cref="Radius"/> and <see cref="AdjacencyRule"/> (since both a method of determining adjacent
    /// locations and a radius shape are implied by a distance calculation).
    ///
    /// Note that, although this class is abstract, you cannot practically create your own subclasses.
    /// Each subclass requires an entry in <see cref="Distance.Types"/> to function properly.  The class is only
    /// abstract in order to allow an internal implementation which will maximize performance.
    /// </remarks>
    [DataContract]
    public abstract class Distance : IMatchable<Distance>
    {
        /// <summary>
        /// Represents chebyshev distance (equivalent to 8-way movement with no extra cost for diagonals).
        /// </summary>
        public static ChebyshevDistance Chebyshev = new ChebyshevDistance();

        /// <summary>
        /// Represents euclidean distance (equivalent to 8-way movement with ~1.41 movement cost for diagonals).
        /// </summary>
        public static EuclideanDistance Euclidean = new EuclideanDistance();

        /// <summary>
        /// Represents manhattan distance (equivalent to 4-way, cardinal-only movement).
        /// </summary>
        public static ManhattanDistance Manhattan = new ManhattanDistance();

        /// <summary>
        /// Enum value representing the method of calculating distance -- useful for using
        /// Distance types in switch statements.
        /// </summary>
        [DataMember]
        public Types Type { get; }

        private static readonly string[] s_writeVals = Enum.GetNames(typeof(Types));

        /// <summary>
        /// Creates a new Distance class to represent the distance calculation specified by the type.
        /// </summary>
        /// <param name="type"/>
        protected Distance(Types type) => Type = type;

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
        /// Returns the distance between the two (2D) points specified.
        /// </summary>
        /// <param name="startX">X-Coordinate of the starting point.</param>
        /// <param name="startY">Y-Coordinate of the starting point.</param>
        /// <param name="endX">X-Coordinate of the ending point.</param>
        /// <param name="endY">Y-Coordinate of the ending point.</param>
        /// <returns>The distance between the two points.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Calculate(int startX, int startY, int endX, int endY)
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
        public abstract double Calculate(double dx, double dy);

        /// <summary>
        /// Returns the distance between two locations, given the change in X and change in Y value.
        /// </summary>
        /// <remarks>
        /// This version takes integer parameters instead of doubles; since distance calculations involving
        /// known  are common on integral grids and can often be implemented more efficiently than their
        /// floating-point counterparts.
        /// </remarks>
        /// <param name="dx">The delta-x between the two locations.</param>
        /// <param name="dy">The delta-y between the two locations.</param>
        /// <returns>The distance between two locations with the given delta-change values.</returns>
        [Pure]
        public abstract double Calculate(int dx, int dy);

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
        public bool Matches(Distance? other) => !(other is null) && Type == other.Type;

        /// <summary>
        /// Returns a string representation of the distance calculation method represented.
        /// </summary>
        /// <returns>A string representation of the distance method represented.</returns>
        [Pure]
        public override string ToString() => s_writeVals[(int)Type];
    }

    /// <summary>
    /// Represents manhattan distance (equivalent to 4-way, cardinal-only movement).
    /// </summary>
    /// <remarks>
    /// You can't create instances of this class; instead, use <see cref="Distance.Manhattan"/>.
    /// </remarks>
    [DataContract]
    public class ManhattanDistance : Distance
    {
        internal ManhattanDistance()
            : base(Types.Manhattan)
        { }

        /// <inheritdoc />
        public override double Calculate(double dx, double dy) => Math.Abs(dx) + Math.Abs(dy);

        /// <inheritdoc />
        public override double Calculate(int dx, int dy) => Math.Abs(dx) + Math.Abs(dy);
    }

    /// <summary>
    /// Represents chebyshev distance (equivalent to 8-way movement with no extra cost for diagonals).
    /// </summary>
    /// <remarks>
    /// You can't create instances of this class; instead, use <see cref="Distance.Chebyshev"/>.
    /// </remarks>
    [DataContract]
    public class ChebyshevDistance : Distance
    {
        internal ChebyshevDistance()
            : base(Types.Chebyshev)
        { }

        /// <inheritdoc />
        public override double Calculate(double dx, double dy) => Math.Max(Math.Abs(dx), Math.Abs(dy));

        /// <inheritdoc />
        public override double Calculate(int dx, int dy) => Math.Max(Math.Abs(dx), Math.Abs(dy));
    }

    /// /// <summary>
    /// Represents euclidean distance (equivalent to 8-way movement with ~1.41 movement cost for diagonals).
    /// </summary>
    /// <remarks>
    /// You can't create instances of this class; instead, use <see cref="Distance.Chebyshev"/>.
    /// </remarks>
    [DataContract]
    public class EuclideanDistance : Distance
    {
        internal EuclideanDistance()
            : base(Types.Euclidean)
        { }

        /// <inheritdoc />
        public override double Calculate(double dx, double dy) => Math.Sqrt(dx * dx + dy * dy);

        /// <inheritdoc />
        public override double Calculate(int dx, int dy) => Math.Sqrt(dx * dx + dy * dy);
    }

}
