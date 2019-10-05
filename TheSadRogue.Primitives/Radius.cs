using System;
using System.Collections.Generic;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Structure representing different shapes that define the concept of a radius on a grid. You cannot
    /// create instances of this class using a constructor -- instead, this class contains static instances
    /// representing the various radius shapes.
    /// </summary>
    /// <remarks>
    /// Contains utility functions to work with radius shapes.  Instances of Radius are also implicitly
    /// convertible to both <see cref="Distance"/> and <see cref="AdjacencyRule"/> (since both a method
    /// of determining adjacent locations and a method of calculating distance are implied by a radius
    /// shape).
    /// </remarks>
    [Serializable]
    public struct Radius : IEquatable<Radius>
    {
        /// <summary>
        /// Radius is a circle around the center point. CIRCLE would represent movement radius in
        /// an 8-way movement scheme with a ~1.41 cost multiplier for diagonal movement.
        /// </summary>
        [NonSerialized]
        public static readonly Radius Circle = new Radius(Types.Circle);

        /// <summary>
        /// Radius is a diamond around the center point. DIAMOND would represent movement radius
        /// in a 4-way movement scheme.
        /// </summary>
        [NonSerialized]
        public static readonly Radius Diamond = new Radius(Types.Diamond);

        /// <summary>
        /// Radius is a square around the center point. SQUARE would represent movement radius in
        /// an 8-way movement scheme, where all 8 squares around an item are considered equal distance
        /// away.
        /// </summary>
        [NonSerialized]
        public static readonly Radius Square = new Radius(Types.Square);

        /// <summary>
        /// Enum value representing the radius shape -- useful for using Radius types in switch
        /// statements.
        /// </summary>
        public readonly Types Type;

        [NonSerialized]
        private static readonly string[] s_writeVals = Enum.GetNames(typeof(Types));

        private Radius(Types type) => Type = type;

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
            Square,

            /// <summary>
            /// Type for Radius.DIAMOND.
            /// </summary>
            Diamond,

            /// <summary>
            /// Type for Radius.CIRCLE.
            /// </summary>
            Circle,
        };

        /// <summary>
        /// Returns an IEnumerable of all positions in a radius of the current shape defined by the given parameters.
        /// </summary>
        /// <remarks>
        /// If you are getting postions for a radius of the same size frequently, it may be more performant to instead
        /// construct a <see cref="RadiusLocationContext"/> to represent it, and pass that to
        /// <see cref="PositionsInRadius(RadiusLocationContext)"/>.
        /// 
        /// The positions returned are all guaranteed to be within the <paramref name="bounds"/> specified.  As well,
        /// they are guaranteed to be in order from least distance from center to most distance if either
        /// <see cref="Radius.Diamond"/> or <see cref="Radius.Square"/> is being used.
        /// </remarks>
        /// <param name="center">Center-point of the radius.</param>
        /// <param name="radius">Length of the radius.</param>
        /// <param name="bounds">Bounds to restrict the returned values by.</param>
        /// <returns>All points in the radius shape defined by the given parameters, in order from least distance to greatest
        /// if <see cref="Radius.Diamond"/> or <see cref="Radius.Square"/> is being used.</returns>
        public IEnumerable<Point> PositionsInRadius(Point center, int radius, Rectangle bounds)
            => PositionsInRadius(new RadiusLocationContext(center, radius, bounds));

        /// <summary>
        /// Returns an IEnumerable of all positions in a radius of the current shape defined by the given parameters.
        /// </summary>
        /// <remarks>
        /// If you are getting postions for a radius of the same size frequently, it may be more performant to instead
        /// construct a <see cref="RadiusLocationContext"/> to represent it, and pass that to
        /// <see cref="PositionsInRadius(RadiusLocationContext)"/>.
        /// 
        /// The positions returned are guaranteed to be in order from least distance from center to most distance if either
        /// <see cref="Radius.Diamond"/> or <see cref="Radius.Square"/> is being used.
        /// </remarks>
        /// <param name="center">Center-point of the radius.</param>
        /// <param name="radius">Length of the radius.</param>
        /// <returns>All points in the radius shape defined by the given parameters, in order from least distance to greatest
        /// if <see cref="Radius.Diamond"/> or <see cref="Radius.Square"/> is being used.</returns>
        public IEnumerable<Point> PositionsInRadius(Point center, int radius)
            => PositionsInRadius(new RadiusLocationContext(center, radius));

        /// <summary>
        /// Returns an IEnumerable of all positions in a radius of the current shape defined by the given context.  Creating
        /// a context to store, and using this function instead of another overload may be more performant when you plan to get
        /// the positions for a radius of the same size multiple times, even if the shape/position are changing.
        /// </summary>
        /// <remarks>
        /// The positions returned are all guaranteed to be within the <paramref name="bounds"/> specified in the context, unless
        /// the bounds are unspecified, in which case no bound restriction results.
        /// 
        /// As well, they are guaranteed to be in order from least distance from center to most distance if either
        /// <see cref="Radius.Diamond"/> or <see cref="Radius.Square"/> is being used.
        /// </remarks>
        /// <param name="context">Context defining radius parameters.</param>
        /// <returns>All points in the radius shape defined by the given context, in order from least distance to greatest
        /// if <see cref="Radius.Diamond"/> or <see cref="Radius.Square"/> is being used.</returns>
        public IEnumerable<Point> PositionsInRadius(RadiusLocationContext context)
        {
            if (context._newlyInitialized)
            {
                context._newlyInitialized = false;
            }
            else
            {
                Array.Clear(context._inQueue, 0, context._inQueue.Length);
            }

            int startArrayIndex = context._inQueue.GetLength(0) / 2;
            Point topLeft = context.Center - context.Radius;
            AdjacencyRule rule = this;
            Distance distCalc = this;

            var q = new Queue<Point>();
            q.Enqueue(context.Center);
            context._inQueue[startArrayIndex, startArrayIndex] = true;

            Point cur;
            Point localNeighbor;

            while (q.Count != 0)
            {
                cur = q.Dequeue();
                yield return cur;

                // Add neighbors
                foreach (Point neighbor in rule.Neighbors(cur))
                {
                    localNeighbor = neighbor - topLeft;

                    if (context._inQueue[localNeighbor.X, localNeighbor.Y] ||
                        context.Bounds != Rectangle.Empty && !context.Bounds.Contains(neighbor) ||
                        distCalc.Calculate(context.Center, neighbor) > context.Radius)
                    {
                        continue;
                    }

                    q.Enqueue(neighbor);
                    context._inQueue[localNeighbor.X, localNeighbor.Y] = true;
                }
            }
        }

        /// <summary>
        /// True if the given Radius has the same Type the current one.
        /// </summary>
        /// <param name="other">Radius to compare.</param>
        /// <returns>True if the two radius shapes are the same, false if not.</returns>
        public bool Equals(Radius other) => Type == other.Type;

        /// <summary>
        /// Same as operator == in this case; returns false if <paramref name="obj"/> is not a Radius.
        /// </summary>
        /// <param name="obj">The object to compare the current Radius to.</param>
        /// <returns>
        /// True if <paramref name="obj"/> is a Radius, and the two radius shapes are equal, false otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is Radius c && Equals(c);

        /// <summary>
        /// Returns a hash-map value for the current object.
        /// </summary>
        /// <returns/>
        public override int GetHashCode() => Type.GetHashCode();

        /// <summary>
        /// True if the two radius shapes have the same Type.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs"/>
        /// <returns>True if the two radius shapes are equal, false if not.</returns>
        public static bool operator ==(Radius lhs, Radius rhs) => lhs.Type == rhs.Type;

        /// <summary>
        /// True if the types are not equal.
        /// </summary>
        /// <param name="lhs"/>
        /// <param name="rhs"/>
        /// <returns>
        /// True if the types are not equal, false if they are both equal.
        /// </returns>
        public static bool operator !=(Radius lhs, Radius rhs) => !(lhs == rhs);

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
                case Types.Circle:
                case Types.Square:
                    return AdjacencyRule.EightWay;

                case Types.Diamond:
                    return AdjacencyRule.Cardinals;

                default:
                    throw new Exception($"Could not convert {nameof(Distance)} to {nameof(Radius)} -- this is a bug!"); // Will not occur
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
                case Types.Circle:
                    return Distance.Euclidean;

                case Types.Diamond:
                    return Distance.Manhattan;

                case Types.Square:
                    return Distance.Chebyshev;

                default:
                    throw new Exception($"Could not convert {nameof(Radius)} to {nameof(Distance)} -- this is a bug!"); // Will not occur
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
                case Types.Circle:
                    return Circle;

                case Types.Diamond:
                    return Diamond;

                case Types.Square:
                    return Square;

                default:
                    throw new Exception($"Could not convert {nameof(Types)} to {nameof(Radius)} -- this is a bug!"); // Will not occur
            }
        }

        /// <summary>
        /// Returns a string representation of the Radius.
        /// </summary>
        /// <returns>A string representation of the Radius.</returns>
        public override string ToString() => s_writeVals[(int)Type];
    }

    /// <summary>
    /// A context representing information necessary to get all the positions in a radius via functions like
    /// <see cref="Radius.PositionsInRadius(RadiusLocationContext)"/>.  Storing a context and re-using it may be
    /// more performant in cases where you're getting the positions in a radius of the same size many times,
    /// even if the location center point or shape of the radius is changing.
    /// </summary>
    public class RadiusLocationContext
    {
        internal bool[,] _inQueue;
        internal bool _newlyInitialized;

        private int _radius;

        /// <summary>
        /// The length of the radius represented.
        /// </summary>
        public int Radius
        {
            get => _radius;
            set
            {
                if (_radius != value)
                {
                    _radius = value;
                    int size = _radius * 2 + 1;
                    _inQueue = new bool[size, size];
                }
            }
        }

        /// <summary>
        /// The centr-point of the radius represented.
        /// </summary>
        public Point Center { get; set; }

        /// <summary>
        /// Represents the bounds that restrict the valid positions that are considered
        /// inside the radius.  Any positions inside the radius but outside the bounds
        /// will be ignored and considered outside the radius.Set to <see cref="Rectangle.Empty"/>
        /// to indicate no bounds.
        /// </summary>
        public Rectangle Bounds;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="center">The starting center-point of the radius.</param>
        /// <param name="radius">The starting length of the radius.</param>
        /// <param name="bounds">The bounds to restrict the radius to.  Any positions inside the radius but outside
        /// the bounds will be ignored and considered outside the radius.</param>
        public RadiusLocationContext(Point center, int radius, Rectangle bounds)
        {
            Center = center;
            _radius = radius;
            Bounds = bounds;

            int size = _radius * 2 + 1;
            _inQueue = new bool[size, size];
            _newlyInitialized = true;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="center">The starting center-point of the radius.</param>
        /// <param name="radius">The starting length of the radius.</param>
        public RadiusLocationContext(Point center, int radius)
            : this(center, radius, Rectangle.Empty) { }
    }
}
